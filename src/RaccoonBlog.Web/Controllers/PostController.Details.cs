using System;
using System.Linq;
using RaccoonBlog.Web.Commands;
using RaccoonBlog.Web.DataServices;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Validation;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Commands;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using System.Web.Mvc;
using System.Web;

namespace RaccoonBlog.Web.Controllers
{
    public partial class PostController
    {
        public const string CommenterCookieName = "commenter";

        public ActionResult Details(int id, string slug)
        {
            var post = Session
                .Include<Post>(x => x.CommentsId)
                .Load(id);

            if (post == null)
                return HttpNotFound();

            if (post.IsPublicPost() == false)
            {
                Guid guid;
                if (Guid.TryParse(Request.QueryString["key"], out guid) == false || guid != post.ShowPostEvenIfPrivate)
                    return HttpNotFound();
            }

            var vm = new PostViewModel
            {
                Post = post.MapTo<PostViewModel.PostDetails>(),
            };

            if (vm.Post.Slug != slug)
                return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            var comments = Session.Load<PostComments>(post.CommentsId);
            vm.Comments = comments.Comments
                .OrderBy(comment => comment.CreatedAt)
                .MapTo<PostViewModel.Comment>();
            vm.NextPost = new PostService(Session).GetPostReference(x => x.PublishAt > post.PublishAt);
            vm.PreviousPost = new PostService(Session).GetPostReference(x => x.PublishAt < post.PublishAt);
            vm.AreCommentsClosed = comments.AreCommentsClosed(post);

            var cookie = Request.Cookies[CommenterCookieName];
            if (Request.IsAuthenticated)
            {
                var user = new UserService(Session).GetCurrentUser();
                vm.Input = user.MapTo<CommentInput>();
                vm.IsTrustedCommenter = true;
            }
            else if (cookie != null)
            {
                var commenter = GetCommenter(cookie.Value);
                if (commenter != null)
                {
                    vm.Input = commenter.MapTo<CommentInput>();
                    vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
                }
            }
            
            return View("Details", vm);
        }

        [HttpPost]
        public ActionResult Comment(CommentInput input, int id)
        {
			var post = Session
				.Include<Post>(x => x.CommentsId)
				.Load(id);

			if (post == null || post.IsPublicPost() == false)
				return HttpNotFound();

        	var comments = Session.Load<PostComments>(post.CommentsId);
			if (comments == null)
				return HttpNotFound();

        	bool areCommentsClosed = comments.AreCommentsClosed(post);
            if (areCommentsClosed)
            {
                ModelState.AddModelError("CommentsClosed", "This post is closed for new comments.");
            }

            var commenter = GetCommenter(input.CommenterKey) ?? new Commenter {Key = Guid.NewGuid()};
            bool isCaptchaRequired = commenter.IsTrustedCommenter != true && Request.IsAuthenticated == false;
            if (isCaptchaRequired)
            {
                var isCaptchaValid = RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext);
                if (isCaptchaValid == false)
                {
                    ModelState.AddModelError("CaptchaNotValid", "You did not type the verification word correctly. Please try again.");
                }
            }

            if (post.AllowComments == false)
            {
                ModelState.AddModelError("CommentNotAllowed", "This post is closed for comments.");
            }

            PostReference postReference;
            if (ModelState.IsValid == false)
            {
                if (Request.IsAjaxRequest())
                    return Json(new {Success = false, message = ModelState.GetFirstErrorMessage()});

                postReference = post.MapTo<PostReference>();
                var result = Details(postReference.DomainId, postReference.Slug);
                var model = result as ViewResult;
                if (model != null)
                {
                    var viewModel = model.Model as PostViewModel;
                    if (viewModel != null)
                        viewModel.Input = input;
                }
                return result;
            }

            CommandExcucator.ExcuteLater(new AddCommentCommand(input, Request.MapTo<RequestValues>(), id));

            var cookie = new HttpCookie(CommenterCookieName, commenter.Key.ToString())
                {Expires = DateTime.Now.AddYears(1)};
            Response.Cookies.Add(cookie);

            const string successMessage = "You feedback will be posted soon. Thanks!";
            if (Request.IsAjaxRequest())
                return Json(new { Success = true, message = successMessage });

            TempData["message"] = successMessage;
            postReference = post.MapTo<PostReference>();
            return RedirectToAction("Details", new { Id = postReference.DomainId, postReference.Slug });
        }

        private Commenter GetCommenter(string commenterKey)
        {
            Guid guid;
            if (Guid.TryParse(commenterKey, out guid) == false)
                return null;
            return Session.Query<Commenter>()
                        .Where(x => x.Key == guid)
                        .FirstOrDefault();
        }
    }
}