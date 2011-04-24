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
        public ActionResult List(int page)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             orderby post.PublishAt descending
                             select post;

            var posts = postsQuery
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToList();

            return View(new PostsAdminViewModel {Posts = posts.MapTo<PostsAdminViewModel.PostSummary>()});
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