using System;
using System.Linq;
using RaccoonBlog.Web.Commands;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Validation;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Commands;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using System.Web.Mvc;
using System.Web;

namespace RaccoonBlog.Web.Controllers
{
    public class PostDetailsController : RaccoonController
    {

		public ActionResult Details(int id, string slug, Guid key)
        {
        	var post = Session
        		.Include<Post>(x => x.CommentsId)
        		.Include(x => x.AuthorId)
        		.Load(id);

            if (post == null)
                return HttpNotFound();

			if (post.IsPublicPost(key) == false)
				return HttpNotFound();  

			var comments = Session.Load<PostComments>(post.CommentsId);
			var vm = new PostViewModel
			{
				Post = post.MapTo<PostViewModel.PostDetails>(),
				Comments = comments.Comments
					.OrderBy(comment => comment.CreatedAt)
					.MapTo<PostViewModel.Comment>(),
				NextPost = Session.GetNextPrevPost(post, true),
				PreviousPost = Session.GetNextPrevPost(post, false),
				AreCommentsClosed = comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments),
			};

			vm.Post.Author = Session.Load<User>(post.AuthorId).MapTo<PostViewModel.UserDetails>();

			vm.Post.Key = key; // Save the key too, to allow operations on privately accessible posts too. Redirection (below) isn't of concern.

        	if (vm.Post.Slug != slug)
				return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            SetWhateverUserIsTrustedCommenter(vm);

        	return View("Details", vm);
        }

		[ValidateInput(false)]
    	[HttpPost]
        public ActionResult Comment(CommentInput input, int id, Guid key)
        {
			var post = Session
				.Include<Post>(x => x.CommentsId)
				.Load(id);

			if (post == null || post.IsPublicPost(key) == false)
				return HttpNotFound();

        	var comments = Session.Load<PostComments>(post.CommentsId);
			if (comments == null)
				return HttpNotFound();
			
			var commenter = Session.GetCommenter(input.CommenterKey);
			if (commenter == null)
			{
				input.CommenterKey = Guid.NewGuid();
			}
			
    		ValidateCommentsAllowed(post, comments);
			ValidateCaptcha(input, commenter);

    		if (ModelState.IsValid == false)
    			return PostingCommentFailed(post, input, key);

			CommandExecutor.ExcuteLater(new AddCommentCommand(input, Request.MapTo<RequestValues>(), id));

			CommenterUtil.SetCommenterCookie(Response, input.CommenterKey.MapTo<string>());

    		return PostingCommentSucceeded(post);
        }

    	private ActionResult PostingCommentSucceeded(Post post)
    	{
    		const string successMessage = "Your comment will be posted soon. Thanks!";
    		if (Request.IsAjaxRequest())
    			return Json(new { Success = true, message = successMessage });

    		TempData["message"] = successMessage;
    		var postReference = post.MapTo<PostReference>();
    		return RedirectToAction("Details", new { Id = postReference.DomainId, postReference.Slug });
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
				return Json(new { Success = false, message = ModelState.GetFirstErrorMessage() });

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
				var user = Session.GetCurrentUser();
				vm.Input = user.MapTo<CommentInput>();
				vm.IsTrustedCommenter = true;
				vm.IsLoggedInCommenter = true;
				return;
			}

			var cookie = Request.Cookies[CommenterUtil.CommenterCookieName];
    		if (cookie == null) return;

			var commenter = Session.GetCommenter(cookie.Value);
			if (commenter == null)
			{
				vm.IsLoggedInCommenter = false;
    			Response.Cookies.Set(new HttpCookie(CommenterUtil.CommenterCookieName) { Expires = DateTime.Now.AddYears(-1) });
    			return;
    		}

    		vm.IsLoggedInCommenter = string.IsNullOrWhiteSpace(commenter.OpenId) == false;
    		vm.Input = commenter.MapTo<CommentInput>();
    		vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
		}
    }
}
