using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.DataServices;
using RavenDbBlog.Helpers.Validation;
using RavenDbBlog.Indexes;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;
using System.Web;
using RavenDbBlog.Commands;
using RavenDbBlog.Infrastructure.Commands;
using Constants = RavenDbBlog.Infrastructure.Constants;

namespace RavenDbBlog.Controllers
{
    public class PostController : AbstractController
    {
        public ActionResult Details(int id, string slug)
        {
            var post = Session
                .Include<Post>(x => x.CommentsId)
                .Load(id);

            if (post == null || post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound();
            

            var vm = new PostViewModel
                         {
                             Post = post.MapTo<PostViewModel.PostDetails>(),
                         };

            if (vm.Post.Slug != slug)
                return RedirectToActionPermanent("Details", new {id, vm.Post.Slug});

            var comments = Session.Load<PostComments>(post.CommentsId);
            vm.Comments = comments.Comments
                .OrderBy(comment => comment.CreatedAt)
                .MapTo<PostViewModel.Comment>();
            vm.NextPost = new PostService(Session).GetPostReference(x => x.PublishAt > post.PublishAt);
            vm.PreviousPost = new PostService(Session).GetPostReference(x => x.PublishAt < post.PublishAt);
            vm.IsCommentClosed = DateTimeOffset.Now - new PostService(Session).GetLastCommentDateForPost(id) > TimeSpan.FromDays(30D);

            var cookie = Request.Cookies[Constants.CommenterKeyCookieName];
            if (cookie != null)
            {
                var commenter =  GetCommenter(cookie.Value);
                vm.Input = commenter.MapTo<CommentInput>();
                vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
            }
            return View(vm);
        }

        public ActionResult List(int page)
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

        public ActionResult Tag(string name, int page)
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

        public ActionResult ArchiveYear(int year, int page)
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

        public ActionResult ArchiveYearMonth(int year, int month, int page)
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

        public ActionResult ArchiveYearMonthDay(int year, int month, int day, int page)
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

        public ActionResult RedirectLegacyPost(int year, int month, int day, string slug)
        {
            var postQuery = from post1 in Session.Query<Post>()
                      where post1.LegacySlug == slug &&
                            (post1.PublishAt.Year == year && post1.PublishAt.Month == month && post1.PublishAt.Day == day)
                      select post1;

            var post = postQuery.FirstOrDefault();
            if (post == null)
                return HttpNotFound();

            var postReference = post.MapTo<PostReference>();
            return RedirectToActionPermanent("Details", new { postReference.Id, postReference.Slug });
        }

        [HttpPost]
        public ActionResult Comment(CommentInput input, int id)
        {
            bool isCommentClosed = DateTimeOffset.Now - new PostService(Session).GetLastCommentDateForPost(id) > TimeSpan.FromDays(30D);
            if (isCommentClosed)
            {
                ModelState.AddModelError("CommentClosed", "This post is closed for comments.");
            }

            var commenter = GetCommenter(input.CommenterKey) ?? new Commenter();
            bool isCaptchaRequired = commenter.IsTrustedCommenter != true;
            if (isCaptchaRequired)
            {
                var isCaptchaValid = RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext);
                if (isCaptchaValid == false)
                {
                    ModelState.AddModelError("CaptchaNotValid", "You did not type the verification word correctly. Please try again.");
                }
            }

            var post = Session.Load<Post>(id);
            var comments = Session.Load<PostComments>(id);

            if (post == null)
                return HttpNotFound();

            if (ModelState.IsValid == false)
            {
                var vm = new PostViewModel
                {
                    Post = post.MapTo<PostViewModel.PostDetails>(),
                    Comments = comments != null ? comments.Comments.MapTo<PostViewModel.Comment>() : new List<PostViewModel.Comment>(),
                    Input = input,
                };
                return View("Details", vm);
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand(input, Request.MapTo<RequestValues>(), id));

            if (input.RememberMe == true)
            {
                Response.Cookies.Add(new HttpCookie(Constants.CommenterKeyCookieName, commenter.Key.ToString()));
            }

            TempData["message"] = "You feedback will be posted soon. Thanks for the feedback.";
            var postReference = post.MapTo<PostReference>();
            return RedirectToAction("Details", new { postReference.Id, postReference.Slug });
        }


        private Commenter GetCommenter(string commenterKey)
        {
            Guid guid;
        	if (Guid.TryParse(commenterKey, out guid) == false)
        		return null;
        	return Session.Query<Commenter>()
        				.Where(x => x.Key == guid)
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