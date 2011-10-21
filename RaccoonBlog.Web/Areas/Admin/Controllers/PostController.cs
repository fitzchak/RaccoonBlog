using System;
using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
    public class PostController : AdminController
    {
        public ActionResult List()
        {
			// the actual UI is handled via JavaScript
            return View();
        }

        public ActionResult Details(int id, string slug)
        {
			var post = Session
                .Include<Post>(x => x.CommentsId)
                .Load(id);

			if (post == null)
				return HttpNotFound();

			var comments = Session.Load<PostComments>(post.CommentsId);

        	var vm = new AdminPostDetailsViewModel
			{
				Post = post.MapTo<AdminPostDetailsViewModel.PostDetails>(),
				
				Comments = comments.Comments
					.Concat(comments.Spam)
					.OrderBy(comment => comment.CreatedAt)
					.MapTo<AdminPostDetailsViewModel.Comment>(),

				NextPost = Session.GetNextPrevPost(post, true),
				PreviousPost = Session.GetNextPrevPost(post, false),
				AreCommentsClosed = comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments),
			};


        	if (vm.Post.Slug != slug)
				return RedirectToActionPermanent("Details", new { id, vm.Post.Slug });

            return View("Details", vm);
        }

        public ActionResult ListFeed(long start, long end)
        {
        	var startAsDateTimeOffset = DateTimeOffsetUtil.ConvertFromUnixTimestamp(start);
        	var endAsDateTimeOffset = DateTimeOffsetUtil.ConvertFromUnixTimestamp(end);

        	var posts = Session.Query<Post>()
				.Where(post => post.IsDeleted == false)
				.Where
				(
					post => post.PublishAt >= startAsDateTimeOffset &&
							post.PublishAt <= endAsDateTimeOffset
				)
                .OrderBy(post => post.PublishAt)
                .Take(256)
                .ToList();

            return Json(posts.MapTo<PostSummaryJson>());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var post = Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound("Post does not exist.");
            return View(new EditPostViewModel {Input = post.MapTo<PostInput>()});
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(PostInput input)
        {
        	if (!ModelState.IsValid)
        		return View("Edit", new EditPostViewModel {Input = input});

        	var post = Session.Load<Post>(input.Id) ?? new Post();
        	input.MapPropertiesToInstance(post);

        	var user = Session.GetCurrentUser();
            if (string.IsNullOrEmpty(post.AuthorId))
        	{
                post.AuthorId = user.Id;
        	}
        	else
        	{
        		post.LastEditedByUserId = user.Id;
        		post.LastEditedAt = DateTimeOffset.Now;
        	}

        	Session.Store(post);

        	var postReference = post.MapTo<PostReference>();
        	return RedirectToAction("Details", new { Id = postReference.DomainId, postReference.Slug});
        }
        
        [HttpPost]
        [AjaxOnly]
        public ActionResult SetPostDate(int id, long date)
        {
            var post = Session
				.Include<Post>(x=>x.CommentsId)
				.Load(id);
            if (post == null)
                return Json(new {success = false});

            post.PublishAt = post.PublishAt.WithDate(DateTimeOffsetUtil.ConvertFromJsTimestamp(date));
        	Session.Load<PostComments>(post.CommentsId).Post.PublishAt = post.PublishAt;
			
        	return Json(new { success = true });
        }
        
        [HttpPost]
        public ActionResult CommentsAdmin(int id, CommentCommandOptions command, int[] commentIds)
        {
            if (commentIds.Length < 1)
                ModelState.AddModelError("CommentIdsAreEmpty", "Not comments was selected.");
        	
            var post = Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();

			var postReference = post.MapTo<PostReference>();

            if (ModelState.IsValid == false)
            {
                if (Request.IsAjaxRequest())
                    return Json(new {Success = false, message = ModelState.GetFirstErrorMessage()});

				return Details(id, postReference.Slug);
            }

            var comments = Session.Load<PostComments>(id);
            switch (command)
            {
                case CommentCommandOptions.Delete:
                    comments.Comments.RemoveAll(c => commentIds.Contains(c.Id));
                    comments.Spam.RemoveAll(c => commentIds.Contains(c.Id));
                    break;

                case CommentCommandOptions.MarkSpam: 
                    var spams = comments.Comments.Concat(comments.Spam)
                        .Where(c => commentIds.Contains(c.Id))
                        .ToArray();

                    comments.Comments.RemoveAll(spams.Contains);
                    comments.Spam.RemoveAll(spams.Contains);
                    foreach (var comment in spams)
                    {
                        AkismetService.MarkSpam(comment);
                    }
                    break;

                case CommentCommandOptions.MarkHam:
                    var ham = comments.Spam
                        .Where(c => commentIds.Contains(c.Id))
                        .ToArray();

                    comments.Spam.RemoveAll(ham.Contains);
                    foreach (var comment in ham)
                    {
                        comment.IsSpam = false;
                        AkismetService.MarkHam(comment);
                    }
                    comments.Comments.AddRange(ham);
                    break;
                default:
                    throw new InvalidOperationException(command + " command is not recognized.");
            }

            post.CommentsCount = comments.Comments.Count;

            if (Request.IsAjaxRequest())
            {
                return Json(new {Success = true});
            }
			return RedirectToAction("Details", new { id, postReference.Slug });
        }

    	[HttpPost]
        public ActionResult Delete(int id)
        {
            var post = Session.Load<Post>(id);
            post.IsDeleted = true;

            if (Request.IsAjaxRequest())
            {
                return Json(new { Success = true });
            }
            return RedirectToAction("List");
        }
    }
}
