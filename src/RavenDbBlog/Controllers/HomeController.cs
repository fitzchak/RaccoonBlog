using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new PostsViewModel
                            {
                                Posts = new List<PostsViewModel.Post>
                                            {
                                                new PostsViewModel.Post
                                                    {
                                                        Body = "abc",
                                                        Title = "abc",
                                                        Tags = new[]{"ab","cb"},
                                                        CommentsCount = 2,
                                                        PostedAt = DateTimeOffset.Now,
                                                        PublishedAt = DateTimeOffset.Now,
                                                        Slug = "abc"
                                                    }
                                            }
                            });
        }

        public ActionResult Post(int id)
        {
            return View(new PostViewModel
            {
                Body = "abc",
                Title = "abc",
                Tags = new[] { "ab", "cb" },
                PublishedAt = DateTimeOffset.Now,
            });
        }
    }
}
