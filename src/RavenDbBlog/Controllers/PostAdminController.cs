using System;
using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.DataServices;
using RavenDbBlog.Helpers;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using RavenDbBlog.Services;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class PostAdminController : AdminController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Details(int id, string slug)
        {
            var post = Session
                .Include<Post>(x => x.CommentsId)
                .Load(id);

            var vm = new AdminPostDetailsViewModel
            {
                Post = post.MapTo<AdminPostDetailsViewModel.PostDetails>(),
            };

            if (vm.Post.Slug != slug)
                return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            var comments = Session.Load<PostComments>(post.CommentsId);
            var allComments = comments.Comments
                .Concat(comments.Spam)
                .OrderBy(comment => comment.CreatedAt);
            vm.Comments = allComments.MapTo<AdminPostDetailsViewModel.Comment>();
            vm.NextPost = new PostService(Session).GetPostReference(x => x.PublishAt > post.PublishAt);
            vm.PreviousPost = new PostService(Session).GetPostReference(x => x.PublishAt < post.PublishAt);
            // vm.IsCommentClosed = TODO: set this value.
            
            return View(vm);
        }

        public ActionResult ListFeed(long start, long end)
        {
            var posts = Session.Query<Post>()
                .Where(post => post.PublishAt >= DateTimeOffsetUtil.ConvertFromUnixTimestamp(start) &&
                    post.PublishAt <= DateTimeOffsetUtil.ConvertFromUnixTimestamp(end))
                .OrderBy(post => post.PublishAt)
                .Take(1000)
                .ToList();

            return Json(posts.MapTo<PostSummaryJson>());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var post = Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound("Post does not exist.");
            return View(post.MapTo<PostInput>());
        }

        [HttpPost]
        public ActionResult Update(PostInput input)
        {
            if (ModelState.IsValid)
            {
                var post = Session.Load<Post>(input.Id) ?? new Post();
                post.Title = input.Title;
                post.Body = input.Body.ToHtmlString();
                Session.Store(post);
                return RedirectToAction("List");
            }
            return View("Edit", input);
        }
        
        [HttpPost]
        [AjaxOnly]
        public ActionResult SetPostDate(int id, long date)
        {
            var post = Session.Load<Post>(id);
            if (post == null)
                return Json(new {success = false});

            post.PublishAt = post.PublishAt.WithDate(DateTimeOffsetUtil.ConvertFromJsTimestamp(date));
            Session.Store(post);
            Session.SaveChanges();

            return Json(new { success = true });
        }
        
        [HttpPost]
        public ActionResult CommentsAdmin(int id, string operation, int[] commentId)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new {Success = false});
            }

            var post = Session.Load<Post>(id);
            return Details(id, post == null ? null : SlugConverter.TitleToSlag(post.Title) );
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            return View();
        }
    }

    public class DateTimeOffsetUtil
    {
        public static DateTimeOffset ConvertFromUnixTimestamp(long timestamp)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            return origin.AddSeconds(timestamp);
        }

        public static DateTimeOffset ConvertFromJsTimestamp(long timestamp)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            return origin.AddMilliseconds(timestamp);
        }
    }
}