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
        public ActionResult List(int page = DefaultPage)
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
    }
}