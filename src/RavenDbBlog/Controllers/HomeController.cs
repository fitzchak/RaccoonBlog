using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MvcReCaptcha;
using Newtonsoft.Json;
using Raven.Client;
using RavenDbBlog.Commands;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Domain;
using RavenDbBlog.Infrastructure;
using RavenDbBlog.ViewModels;
using System.Linq;

namespace RavenDbBlog.Controllers
{
    public class HomeController : BaseController
    {
        private const string CommentCookieName = "commenter";

        public new IDocumentSession Session { get; set; }

        public ActionResult Index(string tag)
        {
            var postsQuery = from post in Session.Query<Post>()
                             where post.PublishAt < DateTimeOffset.Now
                             select post;

            if (!string.IsNullOrEmpty(tag))
            {
                postsQuery = from post in postsQuery
                             where post.Tags.Any(postTag => postTag == tag)
                             select post;
            }

            var posts = postsQuery
                .Where(x => x.PublishAt < DateTimeOffset.Now)
                .OrderByDescending(x => x.PublishAt)
                .Take(30)
                .ToList();

            return View(new PostsViewModel
                            {
                                Posts = posts.MapTo<PostsViewModel.PostSummary>()
                            });
        }

        public ActionResult Show(int id, string slug)
        {
            var result = new PostReader(Session).GetPostAndComments(id);
            var post = result.Item1;
            var comments = result.Item2;

            if (post == null)
                return HttpNotFound();

            if(post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound("The post you looked for does not exist.");

            if (post.Slug != slug)
                return RedirectPermanent("/posts/" + id + "/" + post.Slug); // TODO: do this properly

            var vm = new PostViewModel
                         {
                             Post = post.MapTo<PostViewModel.PostDetails>(),
                             Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
                         };

            var cookie = Request.Cookies[CommentCookieName];
            if (cookie != null)
            {
                var commentCookie = JsonConvert.DeserializeObject<CommentCookie>(cookie.Value);
                vm.NewComment = commentCookie.MapTo<CommentInput>();
                // vm.IsTrustedUser = true;
            }
            return View(vm);
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
            if (!captchaValid)
                ModelState.AddModelError("_FORM", "You did not type the verification word correctly. Please try again.");

            var result = new PostReader(Session).GetPostAndComments(id);
            var post = result.Item1;
            var comments = result.Item2;

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

            CommandExcucator.ExcuteLater(new AddCommentCommand(newComment, id));

            // if (!commenter.IsSpamer) // && newComment.RememberMe == true )
            {
                var commentCookie = newComment.MapTo<CommentCookie>();
                Response.Cookies.Add(new HttpCookie(CommentCookieName, JsonConvert.SerializeObject(commentCookie)));
            }  

            TempData["message"] = "You feedback will be posted soon. Thanks for the feedback.";
            return RedirectToAction("Show", new { post.Slug });
        }

        [ChildActionOnly]
        public ActionResult TagsList(int id)
        {
            // Session.Query<>()
            return View("TagsList");
        }
    }
}