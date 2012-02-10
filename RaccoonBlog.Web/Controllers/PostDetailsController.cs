using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Tasks;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Validation;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Infrastructure.Tasks;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
	public class PostDetailsController : RaccoonController
	{
		public ActionResult Details(int id, string slug, Guid key)
		{
			var post = RavenSession
				.Include<Post>(x => x.CommentsId)
				.Include(x => x.AuthorId)
				.Load(id);

			if (post == null)
				return HttpNotFound();

			if (post.IsPublicPost(key) == false)
				return HttpNotFound();

			var comments = RavenSession.Load<PostComments>(post.CommentsId) ?? new PostComments();
			var vm = new PostViewModel
			         {
			         	Post = post.MapTo<PostViewModel.PostDetails>(),
			         	Comments = comments.Comments
			         		.OrderBy(comment => comment.CreatedAt)
			         		.MapTo<PostViewModel.Comment>(),
			         	NextPost = RavenSession.GetNextPrevPost(post, true),
			         	PreviousPost = RavenSession.GetNextPrevPost(post, false),
			         	AreCommentsClosed = comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments),
			         };

			vm.Post.Author = RavenSession.Load<User>(post.AuthorId).MapTo<PostViewModel.UserDetails>();

			if (vm.Post.Slug != slug)
				return RedirectToActionPermanent("Details", new {id, vm.Post.Slug});

			SetWhateverUserIsTrustedCommenter(vm);

			return View("Details", vm);
		}

		[ValidateInput(false)]
		[HttpPost]
		public ActionResult Comment(CommentInput input, int id, Guid showPostEvenIfPrivate)
		{
			var post = RavenSession
				.Include<Post>(x => x.CommentsId)
				.Load(id);

			if (post == null || post.IsPublicPost(showPostEvenIfPrivate) == false)
				return HttpNotFound();

			var comments = RavenSession.Load<PostComments>(post.CommentsId);
			if (comments == null)
				return HttpNotFound();

			var commenter = RavenSession.GetCommenter(input.CommenterKey);
			if (commenter == null)
			{
				input.CommenterKey = Guid.NewGuid();
			}

			ValidateCommentsAllowed(post, comments);
			ValidateCaptcha(input, commenter);

			if (ModelState.IsValid == false)
				return PostingCommentFailed(post, input, showPostEvenIfPrivate);

			TaskExecutor.ExcuteLater(new AddCommentTask(input, Request.MapTo<AddCommentTask.RequestValues>(), id));

			CommenterUtil.SetCommenterCookie(Response, input.CommenterKey.MapTo<string>());

			return PostingCommentSucceeded(post);
		}

		private ActionResult PostingCommentSucceeded(Post post)
		{
			const string successMessage = "Your comment will be posted soon. Thanks!";
			if (Request.IsAjaxRequest())
				return Json(new {Success = true, message = successMessage});

			TempData["message"] = successMessage;
			var postReference = post.MapTo<PostReference>();
			return RedirectToAction("Details", new {Id = postReference.DomainId, postReference.Slug});
		}

		private void ValidateCommentsAllowed(Post post, PostComments comments)
		{
			if (comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments))
				ModelState.AddModelError("CommentsClosed", "This post is closed for new comments.");
			if (post.AllowComments == false)
				ModelState.AddModelError("CommentsClosed", "This post does not allow comments.");
		}

		private void ValidateCaptcha(CommentInput input, Commenter commenter)
		{
			if (Request.IsAuthenticated ||
			    (commenter != null && commenter.IsTrustedCommenter == true))
				return;

			if (RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext))
				return;

			ModelState.AddModelError("CaptchaNotValid",
			                         "You did not type the verification word correctly. Please try again.");
		}

		private ActionResult PostingCommentFailed(Post post, CommentInput input, Guid key)
		{
			if (Request.IsAjaxRequest())
				return Json(new {Success = false, message = ModelState.FirstErrorMessage()});

			var postReference = post.MapTo<PostReference>();
			var result = Details(postReference.DomainId, postReference.Slug, key);
			var model = result as ViewResult;
			if (model != null)
			{
				var viewModel = model.Model as PostViewModel;
				if (viewModel != null)
					viewModel.Input = input;
			}
			return result;
		}

		private void SetWhateverUserIsTrustedCommenter(PostViewModel vm)
		{
			if (Request.IsAuthenticated)
			{
				var user = RavenSession.GetCurrentUser();
				vm.Input = user.MapTo<CommentInput>();
				vm.IsTrustedCommenter = true;
				vm.IsLoggedInCommenter = true;
				return;
			}

			var cookie = Request.Cookies[CommenterUtil.CommenterCookieName];
			if (cookie == null) return;

			var commenter = RavenSession.GetCommenter(cookie.Value);
			if (commenter == null)
			{
				vm.IsLoggedInCommenter = false;
				Response.Cookies.Set(new HttpCookie(CommenterUtil.CommenterCookieName) {Expires = DateTime.Now.AddYears(-1)});
				return;
			}

			vm.IsLoggedInCommenter = string.IsNullOrWhiteSpace(commenter.OpenId) == false;
			vm.Input = commenter.MapTo<CommentInput>();
			vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
		}
	}
}