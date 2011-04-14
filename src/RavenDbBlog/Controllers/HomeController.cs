using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MvcReCaptcha;
using Newtonsoft.Json;
using Raven.Client;
using RavenDbBlog.Commands;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.Commands;
using RavenDbBlog.ViewModels;
using System.Linq;

namespace RavenDbBlog.Controllers
{
    public class HomeController : BaseController
    {
        private const string CommentCookieName = "commenter";
        private const int DefaultPageSize = 25;

        public new IDocumentSession Session { get; set; }

        public ActionResult Index(string tag, int page, int pagesize = DefaultPageSize)
        {
            if (pagesize < 1 || pagesize > 50)
                pagesize = DefaultPageSize;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                             select post;

            if (!string.IsNullOrEmpty(tag))
            {
                postsQuery = from post in postsQuery
                             where post.Tags.Any(postTag => postTag == tag)
                             select post;
            }

            var posts = postsQuery
                .Where(x => x.PublishAt < DateTimeOffset.Now)
                .OrderByDescending(x => x.PublishAt)
                .Take(pagesize)
                .ToList();

            return View(new PostsViewModel
                            {
                                Posts = posts.MapTo<PostsViewModel.PostSummary>()
                            });
        }

        public ActionResult Show(int id, string slug)
        {
            Session.Load<object>("posts/" + id, "posts/" + id + "/comments");

            var post = Session.Load<Post>("posts/" + id);
            var comments = Session.Load<CommentsCollection>("posts/" + id + "/comments");

            if (post == null)
                return HttpNotFound();

            if(post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound("The post you looked for does not exist.");

            if (post.Slug != slug)
                return RedirectPermanent("/posts/" + id + "/" + post.Slug); // TODO: do this properly

            var vm = new PostViewModel
                         {
                             Post = post.MapTo<PostViewModel.PostDetails>(),
                             Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
                         };

            var cookie = Request.Cookies[CommentCookieName];
            if (cookie != null)
            {
                var commentCookie = JsonConvert.DeserializeObject<CommentCookie>(cookie.Value);
                vm.NewComment = commentCookie.MapTo<CommentInput>();
                // vm.IsTrustedUser = true;
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult NewComment(int id)
        {
            return RedirectToAction("Show", new { Id = id });
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult NewComment(CommentInput newComment, int id, bool captchaValid)
        {
            var isCaptchaRequired = true;
            //GetCommenter(newComment.CommenterKey);
            if (!captchaValid)
                ModelState.AddModelError("_FORM", "You did not type the verification word correctly. Please try again.");

            Session.Load<object>("posts/" + id, "posts/" + id + "/comments");
            var post = Session.Load<Post>("posts/" + id);
            var comments = Session.Load<CommentsCollection>("posts/" + id + "/comments");

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
                return View("Show", vm);
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand
                                             {
                                                 CommentInput = newComment,
                                                 PostId = id
                                             });

            // if (!commenter.IsSpamer) // && newComment.RememberMe == true )
            {
                var commentCookie = newComment.MapTo<CommentCookie>();
                Response.Cookies.Add(new HttpCookie(CommentCookieName, JsonConvert.SerializeObject(commentCookie)));
            }  

            TempData["message"] = "You feedback will be posted soon. Thanks for the feedback.";
            return RedirectToAction("Show", new { post.Slug });
        }

        

        [ChildActionOnly]
        public ActionResult TagsList()
        {
            // Session.Advanced.LuceneQuery<Post>().GroupBy(AggregationOperation.Count, );

            //var tags = Session.Query<Post>()
            //    .SelectMany(post => post.Tags)
            //    .GroupBy(tag => tag)
            //    .OrderBy(tag => tag)
            //    .Take(1000)
            //    .ToList();

            var tags = new List<string>();
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