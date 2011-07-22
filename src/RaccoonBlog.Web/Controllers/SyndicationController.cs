using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;
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

		public ActionResult CommentsRss()
		{
			RavenQueryStatistics stats;
			var commentsIdentifiers = Session.QueryForRecentComments(30, out stats);

			string requestETagHeader = Request.Headers["If-None-Match"] ?? string.Empty;
			var responseETagHeader = stats.Timestamp.ToString("o");
			if (requestETagHeader == responseETagHeader)
				return HttpNotModified();

			var results = new List<CommentRssFeedViewModel>();
			foreach (var commentIdentifier in commentsIdentifiers)
			{
				var comments = Session.Load<PostComments>(commentIdentifier.PostId);
				var post = Session.Load<Post>(commentIdentifier.PostId);
				if (comments == null || post == null)  // the post / comments got deleted in the meantime?
					continue;

				var comment = comments.Comments.FirstOrDefault(x => x.Id == commentIdentifier.CommentId);

				if (comment == null) // we couldn't get the comment, maybe it was removed
					continue;

				var model = comment.MapTo<CommentRssFeedViewModel>();
				post.MapPropertiesToInstance(model);
				results.Add(model);
			}

			return XmlView(results, responseETagHeader);
		}

		public ActionResult LegacyRss()
		{
			return RedirectToActionPermanent("Rss", "Syndication");
		}
	}
}