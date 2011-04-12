using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Raven.Client;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure;
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
                                //Posts = posts.Select(post => new PostsViewModel.PostInternalViewModel
                                //                                 {
                                //                                     Body = MvcHtmlString.Create(post.Body),
                                //                                     CommentsCount = post.CommentsCount,
                                //                                     CreatedAt = post.CreatedAt,
                                //                                     PublishedAt = post.PublishAt,
                                //                                     Slug = post.Slug,
                                //                                     Tags = new string[0],
                                //                                     Title = post.Title
                                //                                 }).ToList(), 
                                Posts = posts.MapTo<PostsViewModel.PostSummary>()
                            });
        }

        public ActionResult Show(string id, string slug)
        {
            var results = Session.Load<object>("posts/" + id, "posts/" + id + "/comments");

            if (results.Length == 0)
                return HttpNotFound();

            var post = (Post) results[0];
            var comments = new CommentsCollection();
            if(results.Length > 1)
                comments = (CommentsCollection) results[1];

            if(post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound();

            if (post.Slug != slug)
                return RedirectPermanent("/posts/" + id + "/" + post.Slug); // TODO: do this properly

            var vm = new PostViewModel
                         {
                             Post = post.MapTo<PostViewModel.PostDetails>(),
                             Comments = comments.Comments.MapTo<PostViewModel.Comment>()
                         };
            return View(vm);
        }
    }
}
