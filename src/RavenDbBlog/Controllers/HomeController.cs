using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MvcReCaptcha;
using Newtonsoft.Json;
using Raven.Client;
using RavenDbBlog.Commands;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.Commands;
using RavenDbBlog.ViewModels;
using System.Linq;

namespace RavenDbBlog.Controllers
{
    public class HomeController : AbstractController
    {
        private const int DefaultPage = 1;
        private const int PageSize = 25;
       
        public ActionResult Index(int page = DefaultPage)
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

        public ActionResult Tag(string tag, int page = DefaultPage)
        {
            page = Math.Max(DefaultPage, page) - 1;

            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now && 
                                   post.Tags.Any(postTag => postTag == tag)
                             orderby post.PublishAt descending
                             select post;

            var posts = postsQuery
                .Skip(page * PageSize)
                .Take(PageSize)
                .ToList();

            return View("Index",new PostsViewModel
                            {
                                Posts = posts.MapTo<PostsViewModel.PostSummary>()
                            });
        }


        [HttpGet]
        public ActionResult NewComment(int id)
        {
            return RedirectToAction("Show", new { Id = id });
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult NewComment(CommentInput newComment, int id, bool captchaValid)
        {
            var isCaptchaRequired = true;
            //GetCommenter(newComment.CommenterKey);
            if (!captchaValid)
                ModelState.AddModelError("_FORM", "You did not type the verification word correctly. Please try again.");

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
                             Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
                             NewComment = newComment,
                         };
                return View("Show", vm);
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand
                                             {
                                                 CommentInput = newComment,
                                                 PostId = id
                                             });

            // if (!commenter.IsSpamer) // && newComment.RememberMe == true )
            {
                var commentCookie = newComment.MapTo<CommentCookie>();
                Response.Cookies.Add(new HttpCookie(Constants.CommentCookieName, JsonConvert.SerializeObject(commentCookie)));
            }  

            TempData["message"] = "You feedback will be posted soon. Thanks for the feedback.";
            return RedirectToAction("Show", new { post.Slug });
        }

        

        [ChildActionOnly]
        public ActionResult TagsList()
        {
            // Session.Advanced.LuceneQuery<Post>().GroupBy(AggregationOperation.Count, );

            //var tags = Session.Query<Post>()
            //    .SelectMany(post => post.Tags)
            //    .GroupBy(tag => tag)
            //    .OrderBy(tag => tag)
            //    .Take(1000)
            //    .ToList();

            var tags = new List<string>();
            return View("UnsortedList", tags);
        }

        //[ChildActionOnly]
        //public ActionResult ArchivesList()
        //{
        //    var datesQuery = from post in Session.Query<Post>()
        //                     group post by new { post.PublishAt.Month, post.PublishAt.Year }
        //                         into date
        //                         select ", " + date.Count();

        //    var dates = datesQuery
        //        .Take(1000)
        //        .ToList();

        //    return View("UnsortedList", dates);
        //}
    }
}