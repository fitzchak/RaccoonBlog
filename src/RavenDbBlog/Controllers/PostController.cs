using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Helpers.Validation;
using RavenDbBlog.Indexes;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.ViewModels;
using System.Web;
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
                .Include<Post>(x => x.CommentsId)
                .Load("posts/" + id);

            if (post == null || post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound();
            
            var comments = Session.Load<PostComments>(post.CommentsId);

            if (post.Slug != slug)
                return RedirectToActionPermanent("Item", new {id, post.Slug});

            var vm = new PostViewModel
            {
                Post = post.MapTo<PostViewModel.PostDetails>(),
                Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
                NextPost = GetPostReference(x => x.PublishAt > post.PublishAt),
                PreviousPost = GetPostReference(x => x.PublishAt < post.PublishAt),
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

        private PostViewModel.PostReference GetPostReference(Expression<Func<Post, bool>> expression)
        {
            return Session.Query<Post>()
                .Where(expression)
                .Select(p => new PostViewModel.PostReference
                    {
                        Id = p.Id,
                        Slug = p.Slug,
                        Title = p.Title
                    })
                .FirstOrDefault();
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

        public ActionResult Tag(string name, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now &&
                                   post.Tags.Any(postTag => postTag == name)
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

        public ActionResult ArchiveYear(int year, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                                   && (post.PublishAt.Year == year)
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

        public ActionResult ArchiveYearMonth(int year, int month, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                                   && (post.PublishAt.Year == year && post.PublishAt.Month == month)
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

        public ActionResult ArchiveYearMonthDay(int year, int month, int day, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                                   && (post.PublishAt.Year == year && post.PublishAt.Month == month && post.PublishAt.Day == day)
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

        public ActionResult RedirectItem(int year, int month, int day, string slug)
        {
            var postQuery = from post1 in Session.Query<Post>()
                      where post1.Slug == slug &&
                            (post1.PublishAt.Year == year && post1.PublishAt.Month == month && post1.PublishAt.Day == day)
                      select post1;

            var post = postQuery.FirstOrDefault();
            if (post == null)
                return HttpNotFound();

            return RedirectToActionPermanent("Item", new { Id = RavenIdResolver.Resolve(post.Id), post.Slug });
        }

        [HttpGet]
        public ActionResult Comment(int id)
        {
            return RedirectToAction("Item", new { id });
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
                    Comments = comments != null ? comments.Comments.MapTo<PostViewModel.Comment>() : new List<PostViewModel.Comment>(),
                    NewComment = newComment,
                };
                return View("Item", vm);
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand(newComment, Request.MapTo<RequestValues>(), id));

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
            var mostRecentTag = new DateTimeOffset(DateTimeOffset.Now.Year - 2,
                                                   DateTimeOffset.Now.Month, 
                                                   1, 0, 0, 0,
                                                   DateTimeOffset.Now.Offset);

            var tagCounts = Session.Query<TagCount, Tags_Count>()
                .Where(x => x.Count > 20 && x.LastSeenAt > mostRecentTag)
                .OrderBy(x=>x.Name)
                .As<TempTagCount>();
            var tags = tagCounts
                .ToList();
        
            return View(tags);
        }

        [ChildActionOnly]
        public ActionResult ArchivesList()
        {
            var dates = Session.Query<PostCountByMonth, Posts_ByMonthPublished_Count>()
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToList();

            return View(dates);
        }
    }
}