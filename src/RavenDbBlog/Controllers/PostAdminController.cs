using System;
using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class PostAdminController : AdminController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult ListFeed(double start, double end)
        {
            var posts = Session.Query<Post>()
                .Where(post => post.PublishAt >= ConvertFromUnixTimestamp(start) &&
                    post.PublishAt <= ConvertFromUnixTimestamp(end))
                .OrderBy(post => post.PublishAt)
                .Take(1000)
                .ToList();

            return Json(posts.MapTo<PostSummaryJson>());
        }

        private static DateTimeOffset ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            return origin.AddSeconds(timestamp);
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
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}