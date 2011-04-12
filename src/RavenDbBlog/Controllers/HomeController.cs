using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Raven.Client;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;
using System.Linq;

namespace RavenDbBlog.Controllers
{
    public class HomeController : Controller
    {

        public new IDocumentSession Session { get; set; }

        public ActionResult Index()
        {
            var posts = Session.Query<Post>()
                .Where(x => x.PublishAt < DateTimeOffset.Now)
                .OrderByDescending(x => x.PublishAt)
                .Take(30)
                .ToList();


            return View(new PostsViewModel
                            {
                                Posts = posts.Select(post => new PostsViewModel.Post
                                                                 {
                                                                     Body = MvcHtmlString.Create(post.Body),
                                                                     CommentsCount = post.CommentsCount,
                                                                     CreatedAt = post.CreatedAt,
                                                                     PublishedAt = post.PublishAt,
                                                                     Slug = post.Slug,
                                                                     Tags = new string[0],
                                                                     Title = post.Title
                                                                 }).ToList()
                            });
        }

        public ActionResult Post(int id)
        {
            var vm = new PostViewModel();
            return View(vm);
        }
    }
}
