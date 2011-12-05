using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Extensions;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Services;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class PostsController : AdminController
	{
		public ActionResult Index()
		{
			return List();
		}

		public ActionResult List()
		{
			// the actual UI is handled via JavaScript
			return View("List");
		}

		[HttpGet]
		public ActionResult Add()
		{
			return View("Edit", new PostInput());
		}

		[HttpPost]
		public ActionResult Add(PostInput input)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			// Be able to record the user making the actual post
			var user = RavenSession.GetCurrentUser();

			// Create the post comments object and link between it and the post
			var comments = new PostComments
			{
				Comments = new List<PostComments.Comment>(),
				Spam = new List<PostComments.Comment>()
			};
			RavenSession.Store(comments);

			// Create new post object
			var post = new Post
						{
							Tags = TagsResolver.ResolveTagsInput(input.Tags),
							PublishAt = input.PublishAt,
							AllowComments = input.AllowComments,
							AuthorId = user.Id,
							LastEditedByUserId = user.Id,
							LastEditedAt = DateTimeOffset.Now,
							CommentsId = comments.Id,
							ContentType = input.ContentType,
							Body = input.Body,
							CreatedAt = DateTimeOffset.Now,
							Title = input.Title,
						};

			if (post.PublishAt == DateTimeOffset.MinValue)
			{
				var postScheduleringStrategy = new PostSchedulingStrategy(RavenSession, DateTimeOffset.Now);
				post.PublishAt = postScheduleringStrategy.Schedule();
			}

			// Actually save the post now
			RavenSession.Store(post);
			comments.Post = new PostComments.PostReference
			{
				Id = post.Id,
				PublishAt = post.PublishAt,
			};

			return RedirectToAction("Details", new { id = post.Id.ToIntId() });
		}

		public ActionResult Details(int id)
		{
			var post = RavenSession
				.Include<Post>(x => x.CommentsId)
				.Load(id);

			if (post == null)
				return HttpNotFound();

			var comments = RavenSession.Load<PostComments>(post.CommentsId);

			var vm = new AdminPostDetailsViewModel
			{
				Post = post.MapTo<AdminPostDetailsViewModel.PostDetails>(),
				
				Comments = comments.Comments
					.Concat(comments.Spam)
					.OrderBy(comment => comment.CreatedAt)
					.MapTo<AdminPostDetailsViewModel.Comment>(),

				NextPost = RavenSession.GetNextPrevPost(post, true),
				PreviousPost = RavenSession.GetNextPrevPost(post, false),
				AreCommentsClosed = comments.AreCommentsClosed(post, BlogConfig.NumberOfDayToCloseComments),
			};

			return View("Details", vm);
		}

		public ActionResult ListFeed(long start, long end)
		{
			var startAsDateTimeOffset = DateTimeOffsetUtil.ConvertFromUnixTimestamp(start);
			var endAsDateTimeOffset = DateTimeOffsetUtil.ConvertFromUnixTimestamp(end);

			var posts = RavenSession.Query<Post>()
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
			var post = RavenSession.Load<Post>(id);
			if (post == null)
				return HttpNotFound("Post does not exist.");
			return View(post.MapTo<PostInput>());
		}
		
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(PostInput input, int id)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			var post = RavenSession.Load<Post>(id) ?? new Post();
			input.MapPropertiesToInstance(post);

			var user = RavenSession.GetCurrentUser();
			if (string.IsNullOrEmpty(post.AuthorId))
			{
				post.AuthorId = user.Id;
			}
			else
			{
				post.LastEditedByUserId = user.Id;
				post.LastEditedAt = DateTimeOffset.Now;
			}

			RavenSession.Store(post);

			var postReference = post.MapTo<PostReference>();
			return RedirectToAction("Details", new { Id = postReference.DomainId, postReference.Slug});
		}
		
		[HttpPost]
		[AjaxOnly]
		public ActionResult SetPostDate(int id, long date)
		{
			var post = RavenSession
				.Include<Post>(x=>x.CommentsId)
				.Load(id);
			if (post == null)
				return Json(new {success = false});

			post.PublishAt = post.PublishAt.WithDate(DateTimeOffsetUtil.ConvertFromJsTimestamp(date));
			RavenSession.Load<PostComments>(post.CommentsId).Post.PublishAt = post.PublishAt;
			
			return Json(new { success = true });
		}
		
		[HttpPost]
		public ActionResult CommentsAdmin(int id, CommentCommandOptions command, int[] commentIds)
		{
			if (commentIds == null || commentIds.Length == 0)
				ModelState.AddModelError("CommentIdsAreEmpty", "Not comments was selected.");

			var post = RavenSession.Load<Post>(id);
			if (post == null)
				return HttpNotFound();

			if (ModelState.IsValid == false)
			{
				if (Request.IsAjaxRequest())
					return Json(new {Success = false, message = ModelState.GetFirstErrorMessage()});

				return Details(id);
			}

			var comments = RavenSession.Load<PostComments>(id);
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
			return RedirectToAction("Details", new { id });
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			var post = RavenSession.Load<Post>(id);
			post.IsDeleted = true;

			if (Request.IsAjaxRequest())
			{
				return Json(new { Success = true });
			}
			return RedirectToAction("List");
		}
	}
}
