using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class PostController : AbstractController
    {
        public ActionResult List(int id, string slug)
        {
            var post = Session
                .Include<Post>(x=>x.CommentsId)
                .Load("posts/" + id);

            if (post == null || post.PublishAt > DateTimeOffset.Now)
                return HttpNotFound();
            
            var comments = Session.Load<PostComments>(post.CommentsId);

            if (post.Slug != slug)
                return RedirectToActionPermanent("Index", new {id, post.Slug});

            var vm = new PostViewModel
            {
                Post = post.MapTo<PostViewModel.PostDetails>(),
                Comments = comments.Comments.MapTo<PostViewModel.Comment>(),
            };

            var cookie = Request.Cookies[Constants.CommentCookieName];
            if (cookie != null)
            {
                var commentCookie = JsonConvert.DeserializeObject<CommentCookie>(cookie.Value);
                vm.NewComment = commentCookie.MapTo<CommentInput>();
                // vm.IsTrustedUser = true;
            }
            return View(vm);
        }
    }
}