using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Helpers;
using RavenDbBlog.Infrastructure.AutoMapper;
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
            return View();
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