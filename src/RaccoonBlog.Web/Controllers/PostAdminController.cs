using System;
using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
    public class PostAdminController : AdminController
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
				
				Comments = comments
					.Comments
					.Concat(comments.Spam)
					.OrderBy(comment => comment.CreatedAt)
					.MapTo<AdminPostDetailsViewModel.Comment>(),

				NextPost = Session.GetPostReference(x => x.PublishAt > post.PublishAt),
				PreviousPost = Session.GetPostReference(x => x.PublishAt < post.PublishAt),
				AreCommentsClosed = comments.AreCommentsClosed(post),
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

        	var author = Session.GetCurrentUser().MapTo<Post.AuthorReference>();
        	if (post.Author == null || string.IsNullOrEmpty(post.Author.FullName))
        	{
        		post.Author = author;
        	}
        	else
        	{
        		post.LastEditedBy = author;
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
            var post = Session.Load<Post>(id);
            if (post == null)
                return Json(new {success = false});

            post.PublishAt = post.PublishAt.WithDate(DateTimeOffsetUtil.ConvertFromJsTimestamp(date));
            Session.Store(post);
            Session.SaveChanges();

            return Json(new { success = true });
        }
        
        [HttpPost]
        public ActionResult CommentsAdmin(int id, string command, int[] commentIds)
        {
            if (commentIds.Length < 1)
                ModelState.AddModelError("CommentIdsAreEmpty", "Not comments was selected.");
            var commands = new[] {"Delete", "Mark Spam", "Mark Ham"};
            if (commands.Any(c => c == command) == false)
                ModelState.AddModelError("CommentIsNotRecognized", command + " command is not recognized.");
            var post = Session.Load<Post>(id);
            if (post == null)
                return HttpNotFound();
            var slug =  SlugConverter.TitleToSlag(post.Title);

            if (ModelState.IsValid == false)
            {
                if (Request.IsAjaxRequest())
                    return Json(new {Success = false, message = ModelState.GetFirstErrorMessage()});

                return Details(id, slug);
            }

            var comments = Session.Load<PostComments>(id);
            var requestValues = Request.MapTo<RequestValues>();
            switch (command)
            {
                case "Delete":
                    comments.Comments.RemoveAll(c => commentIds.Contains(c.Id));
                    comments.Spam.RemoveAll(c => commentIds.Contains(c.Id));
                    break;
                case "Mark Spam": 
                    var spams = comments.Comments
                        .Where(c => commentIds.Contains(c.Id))
                        .ToArray();

                    comments.Comments.RemoveAll(spams.Contains);
                    comments.Spam.RemoveAll(spams.Contains);
                    foreach (var comment in spams)
                    {
                        new AskimetService(requestValues).MarkHum(comment);
                    }
                    break;
                case "Mark Ham":
                    var ham = comments.Spam
                        .Where(c => commentIds.Contains(c.Id))
                        .ToArray();

                    comments.Spam.RemoveAll(ham.Contains);
                    foreach (var comment in ham)
                    {
                        comment.IsSpam = false;
                        new AskimetService(requestValues).MarkHum(comment);
                    }
                    comments.Comments.AddRange(ham);
                    break;
                default:
                    throw new InvalidOperationException(command + " command is not recognized.");
            }

            if (Request.IsAjaxRequest())
            {
                return Json(new {Success = true});
            }
            return RedirectToAction("Details", new { id, slug });
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

    public class DateTimeOffsetUtil
    {
        public static DateTimeOffset ConvertFromUnixTimestamp(long timestamp)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            return origin.AddSeconds(timestamp);
        }

        public static DateTimeOffset ConvertFromJsTimestamp(long timestamp)
        {
            var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            return origin.AddMilliseconds(timestamp);
        }
    }
}