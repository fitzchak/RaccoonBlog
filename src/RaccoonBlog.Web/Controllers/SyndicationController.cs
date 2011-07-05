using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Linq;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Infrastructure.Common;

namespace RaccoonBlog.Web.Controllers
{
	public class SyndicationController : AbstractController
	{
		public ActionResult Rsd()
		{
			return XmlView();
		}

		public ActionResult Rss(string tag, Guid key)
		{
			RavenQueryStatistics stats;
			var postsQuery = Session.Query<Post>()
				.Statistics(out stats);

			if (key != Guid.Empty && key == BlogConfig.RssFuturePostsKey)
			{
				postsQuery = postsQuery.Where(x => x.PublishAt < DateTimeOffset.Now.AddDays(BlogConfig.RssFutureDaysAllowed).AsMinutes());
			}
			else
			{
				postsQuery = postsQuery.Where(x => x.PublishAt < DateTimeOffset.Now.AsMinutes());
			}

			if (string.IsNullOrWhiteSpace(tag) == false)
				postsQuery = postsQuery.Where(x => x.TagsAsSlugs.Any(postTag => postTag == tag));

			var posts = postsQuery.OrderByDescending(x => x.PublishAt)
				.Take(20)
				.ToList();

			string requestETagHeader = Request.Headers["If-None-Match"] ?? string.Empty;
			var responseETagHeader = stats.Timestamp.ToString("o");
			if (requestETagHeader == responseETagHeader)
				return HttpNotModified();

			return XmlView(posts.MapTo<PostRssFeedViewModel>(), responseETagHeader);
		}

		public ActionResult CommentsRss(Guid key)
		{
			var commentsIdentifiersQuery = Session.Query<PostCommentsIdentifier, PostComments_CreationDate>()
				.Include(comment => comment.PostCollectionId)
				.Include(comment => comment.PostId);

			var commentsIdentifiers = commentsIdentifiersQuery.OrderByDescending(x => x.CreatedAt)
				.Take(30)
				.AsProjection<PostCommentsIdentifier>()
				.ToList();
			
			var results = new List<CommentRssFeedViewModel>();
			foreach (var commentIdentifier in commentsIdentifiers)
			{
				var comments = Session.Load<PostComments>(commentIdentifier.PostId);
				var post = Session.Load<Post>(commentIdentifier.PostId);
				if (comments != null && post != null)
				{
					var comment = comments.Comments.FirstOrDefault(x => x.Id == commentIdentifier.CommentId);
					if (comment != null)
					{
						var model = comment.MapTo<CommentRssFeedViewModel>();
						post.MapPropertiesToInstance(model);
						results.Add(model);
					}
				}
			}

			return View(results);
		}

		private string GetPostLink(Post post)
		{
			var postReference = post.MapTo<PostReference>();
			return Url.RelativeToAbsolute(Url.Action("Details", "PostDetails", new { Id = postReference.DomainId, postReference.Slug }));
		}

		private string GetBlogTitle()
		{
			return BlogConfig.Title;
		}

		public ActionResult LegacyRss()
		{
			return RedirectToActionPermanent("Rss", "Syndication");
		}
	}
}
