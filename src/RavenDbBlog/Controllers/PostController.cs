using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Helpers.Validation;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;
using System.Web;
using Raven.Abstractions.Data;
using RavenDbBlog.Commands;
using RavenDbBlog.Infrastructure.Commands;
using Constants = RavenDbBlog.Infrastructure.Constants;

namespace RavenDbBlog.Controllers
{
    public class PostController : AbstractController
    {
        public ActionResult Item(int id, string slug)
        {
            var post = Session
                .Include<Post>(x=>x.CommentsId)
                .Load("posts/" + id);

            if (post == null || post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound();
            
            var comments = Session.Load<PostComments>(post.CommentsId);

            if (post.Slug != slug)
                return RedirectToActionPermanent("Index", new {id, post.Slug});

            var vm = new PostViewModel
            {
                Post = post.MapTo<PostViewModel.PostDetails>(),
                Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
            };

            var cookie = Request.Cookies[Constants.CommenterKeyCookieName];
            if (cookie != null)
            {
                var commenter =  GetCommenter(cookie.Value);
                vm.NewComment = commenter.MapTo<CommentInput>();
                vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
            }
            return View(vm);
        }

        private const int DefaultPage = 1;
        private const int PageSize = 25;

        public ActionResult List(int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                             orderby post.PublishAt descending
                             select post;

            var posts = postsQuery
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToList();

            return View(new PostsViewModel
            {
                Posts = posts.MapTo<PostsViewModel.PostSummary>()
            });
        }

        public ActionResult Tag(string tag, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now &&
                                   post.Tags.Any(postTag => postTag == tag)
                             orderby post.PublishAt descending
                             select post;

            var posts = postsQuery
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToList();

            return View("List", new PostsViewModel
            {
                Posts = posts.MapTo<PostsViewModel.PostSummary>()
            });
        }

        [HttpGet]
        public ActionResult Comment(int id)
        {
            return RedirectToAction("Item", new { Id = id });
        }

        [HttpPost]
        public ActionResult Comment(CommentInput newComment, int id)
        {
            var commenter = GetCommenter(newComment.CommenterKey) ?? new Commenter();
            bool isCaptchaRequired = commenter.IsTrustedCommenter == false;
            if (isCaptchaRequired)
            {
                var isCaptchaValid = RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext);
                if (isCaptchaValid)
                {
                    ModelState.AddModelError("_FORM", "You did not type the verification word correctly. Please try again.");
                }
            }

            Session.Load<object>("posts/" + id, "posts/" + id + "/comments");
            var post = Session.Load<Post>("posts/" + id);
            var comments = Session.Load<PostComments>("posts/" + id + "/comments");

            if (post == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                var vm = new PostViewModel
                {
                    Post = post.MapTo<PostViewModel.PostDetails>(),
                    Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
                    NewComment = newComment,
                };
                return View("Item", vm);
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand
            {
                CommentInput = newComment,
                PostId = id
            });

            if (newComment.RememberMe == true)
            {
                Response.Cookies.Add(new HttpCookie(Constants.CommenterKeyCookieName, commenter.Key.ToString()));
            }

            TempData["message"] = "You feedback will be posted soon. Thanks for the feedback.";
            return RedirectToAction("Item", new { post.Slug });
        }


        private Commenter GetCommenter(string commenterKey)
        {
            Guid guid;
            if (Guid.TryParse(commenterKey, out guid))
                GetCommenter(guid);
            return null;
        }

        private Commenter GetCommenter(Guid commenterKey)
        {
            return Session.Query<Commenter>()
                .Where(x => x.Key == commenterKey)
                .FirstOrDefault();
        }


        [ChildActionOnly]
        public ActionResult TagsList()
        {
            var tags = Session.Advanced.LuceneQuery<Post>()
                .GroupBy(AggregationOperation.Count, "Tags,")
                .ToList();
            //posts.Contains()
            //var tags = Session.Query<Post>()
            //    .SelectMany(post => post.Tags)
            //    .GroupBy(tag => tag)
            //    .OrderBy(tag => tag)
            //    .Take(1000)
            //    .ToList();

            return View("UnsortedList", tags);
        }

        //[ChildActionOnly]
        //public ActionResult ArchivesList()
        //{
        //    var datesQuery = from post in Session.Query<Post>()
        //                     group post by new { post.PublishAt.Month, post.PublishAt.Year }
        //                         into date
        //                         select ", " + date.Count();

        //    var dates = datesQuery
        //        .Take(1000)
        //        .ToList();

        //    return View("UnsortedList", dates);
        //}
    }
}