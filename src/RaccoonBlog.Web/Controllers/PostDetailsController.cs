using System;
using System.Linq;
using RaccoonBlog.Web.Commands;
using RaccoonBlog.Web.Common;
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
    public class PostDetailsController : AbstractController
    {
        public const string CommenterCookieName = "commenter";

        public ActionResult Details(int id, string slug)
        {
            var post = Session
                .Include<Post>(x => x.CommentsId)
                .Load(id);

            if (post == null)
                return HttpNotFound();

			if (post.IsPublicPost(Request.QueryString["key"]) == false)
				return HttpNotFound();  

			var comments = Session.Load<PostComments>(post.CommentsId);
			var vm = new PostViewModel
			{
				Post = post.MapTo<PostViewModel.PostDetails>(),
				Comments = comments.Comments
					.OrderBy(comment => comment.CreatedAt)
					.MapTo<PostViewModel.Comment>(),
				NextPost = Session.GetPostReference(x => x.PublishAt > post.PublishAt),
				PreviousPost = Session.GetPostReference(x => x.PublishAt < post.PublishAt),
				AreCommentsClosed = comments.AreCommentsClosed(post),
			};

        	if (vm.Post.Slug != slug)
				return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            SetWhateverUserIsTrustedCommenter(vm);

        	return View("Details", vm);
        }

        [ValidateInput(false)]
    	[HttpPost]
        public ActionResult Comment(CommentInput input, int id)
        {
			var post = Session
				.Include<Post>(x => x.CommentsId)
				.Load(id);

			if (post == null || post.IsPublicPost(Request.QueryString["key"]) == false)
				return HttpNotFound();

        	var comments = Session.Load<PostComments>(post.CommentsId);
			if (comments == null)
				return HttpNotFound();
			
			var commenter = GetCommenter(input.CommenterKey) ?? new Commenter { Key = Guid.NewGuid() };
			
    		ValidateCommentsAllowed(post, comments);
			ValidateCaptcha(input, commenter);

    		if (ModelState.IsValid == false)
    			return PostingCommentFailed(post, input);

    		new AddCommentCommand(input, Request.MapTo<RequestValues>(), id).Execute(Session);

            SetCommenterCookie(commenter);

    		return PostingCommentSucceeded(post);
        }

    	private ActionResult PostingCommentSucceeded(Post post)
    	{
    		const string successMessage = "You feedback will be posted soon. Thanks!";
    		if (Request.IsAjaxRequest())
    			return Json(new { Success = true, message = successMessage });

    		TempData["message"] = successMessage;
    		var postReference = post.MapTo<PostReference>();
    		return RedirectToAction("Details", new { Id = postReference.DomainId, postReference.Slug });
    	}

    	private void SetCommenterCookie(Commenter commenter)
    	{
    		var cookie = new HttpCookie(CommenterCookieName, commenter.Key.ToString())
    		{Expires = DateTime.Now.AddYears(1)};
    		Response.Cookies.Add(cookie);
    	}

    	private void ValidateCommentsAllowed(Post post, PostComments comments)
    	{
    		if (comments.AreCommentsClosed(post))
    			ModelState.AddModelError("CommentsClosed", "This post is closed for new comments.");
    		if (post.AllowComments == false)
    			ModelState.AddModelError("CommentsClosed", "This post does not allow comments.");
    	}

    	private void ValidateCaptcha(CommentInput input, Commenter commenter)
    	{
    		if (Request.IsAuthenticated || 
				commenter.IsTrustedCommenter == true)
				return;

    		if (RecaptchaValidatorWrapper.Validate(ControllerContext.HttpContext)) 
				return;

    		ModelState.AddModelError("CaptchaNotValid",
    		                         "You did not type the verification word correctly. Please try again.");
    	}

    	private ActionResult PostingCommentFailed(Post post, CommentInput input)
    	{
			if (Request.IsAjaxRequest())
				return Json(new { Success = false, message = ModelState.GetFirstErrorMessage() });

			var postReference = post.MapTo<PostReference>();
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

    	private void SetWhateverUserIsTrustedCommenter(PostViewModel vm)
		{
			if (Request.IsAuthenticated)
			{
				var user = Session.GetCurrentUser();
				vm.Input = user.MapTo<CommentInput>();
				vm.IsTrustedCommenter = true;
				return;
			}

			var cookie = Request.Cookies[CommenterCookieName];
			if (cookie == null)
				return;

			var commenter = GetCommenter(cookie.Value);
			if (commenter == null)
				return;

			vm.Input = commenter.MapTo<CommentInput>();
			vm.IsTrustedCommenter = commenter.IsTrustedCommenter == true;
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