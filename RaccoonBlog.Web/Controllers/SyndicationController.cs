using System;
using System.IO;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Linq;
using System.Xml.Linq;
using HibernatingRhinos.Loci.Common.Models;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
using Raven.Client;
using Raven.Client.Linq;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Helpers;

namespace RaccoonBlog.Web.Controllers
{
	public class SyndicationController : RaccoonController
	{
		private static readonly string EtagInitValue = Guid.NewGuid().ToString();

		public ActionResult Rsd()
		{
			var ns = XNamespace.Get("http://archipelago.phrasewise.com/rsd");

			return Xml(new XDocument(
						new XElement(ns + "service",
									 new XElement(ns + "engineName", "Raccoon Blog"),
									 new XElement(ns + "engineLink", "http://hibernatingrhinos.com"),
									 new XElement(ns + "homePageLink", Url.RelativeToAbsolute(Url.RouteUrl("homepage"))),
									 new XElement(ns + "apis",
												  new XElement(ns + "api",
															   new XAttribute("name", "MetaWeblog"),
															   new XAttribute("preferred", "true"),
															   new XAttribute("blogID", "0"),
															   new XAttribute("apiLink", Url.RelativeToAbsolute(Url.Content("~/services/metaweblogapi.ashx")))
													)
										)
							)
						), typeof(SyndicationController).FullName);

		}

		private class QueryBehavior
		{
			public string Title;
			public int Take;
			public bool AddExpiredNote;
			public IRavenQueryable<Post> PostsQuery;
		}

		public ActionResult Rss(string tag, string token)
		{
			RavenQueryStatistics stats;

			var queryBehavior = SetQueryLimitsBasedOnToken(token, RavenSession.Query<Post>().Statistics(out stats));


			var postsQuery = queryBehavior.PostsQuery;

			if (string.IsNullOrWhiteSpace(tag) == false)
				postsQuery = postsQuery.Where(x => x.TagsAsSlugs.Any(postTag => postTag == tag));

			var posts = postsQuery.OrderByDescending(x => x.PublishAt)
				.Take(queryBehavior.Take)
				.ToList();

			if (queryBehavior.AddExpiredNote)
			{
				posts.Insert(0, new Post
				{
					Title = "Feed token expired, you are no longer able to read future posts",
					Id = null,
					ContentType = DynamicContentType.Html,
					Body = "<p>This feed token has expired, and will no longer show any future posts.</p><p>You can still read current posts.</p>"
				});
			}

			string responseETagHeader;
			if (CheckEtag(stats, out responseETagHeader))
				return HttpNotModified();

			var rss = new XDocument(
				new XElement("rss",
							 new XAttribute("version", "2.0"),
							 new XElement("channel",
										  new XElement("title", queryBehavior.Title),
										  new XElement("link", Url.RelativeToAbsolute(Url.RouteUrl("homepage"))),
										  new XElement("description", BlogConfig.MetaDescription ?? queryBehavior.Title),
										  new XElement("copyright", String.Format("{0} (c) {1}", BlogConfig.Copyright, DateTime.Now.Year)),
										  new XElement("ttl", "60"),
										  from post in posts
										  let postLink = GetPostLink(post)
										  select new XElement("item",
															  new XElement("title", Server.HtmlDecode(post.Title)),
															  new XElement("description", post.CompiledContent(true)),
															  new XElement("link", postLink),
																new XElement("guid", postLink),
															  new XElement("pubDate", post.PublishAt.ToString("R"))
											)
								)
					)
				);

			return Xml(rss, responseETagHeader);
		}

		private QueryBehavior SetQueryLimitsBasedOnToken(string token, IRavenQueryable<Post> postsQuery)
		{
			var behavior = new QueryBehavior
			{
				AddExpiredNote = false,
				Title = BlogConfig.Title,
				Take = 20
			};
			if (string.IsNullOrEmpty(token) != false)
			{
				behavior.PostsQuery = postsQuery.Where(x => x.PublishAt < DateTimeOffset.Now.AsMinutes());
				return behavior;
			}

			int numberOfDays;
			string user;
			if (GetNumberOfDays(token, out numberOfDays, out user))
			{
				behavior.Take = Math.Max(numberOfDays, behavior.Take);
				behavior.Title = behavior.Title + " for " + user;
				behavior.PostsQuery = postsQuery.Where(x => x.PublishAt < DateTimeOffset.Now.AddDays(numberOfDays).AsMinutes());
			}
			else
			{
				behavior.Title = behavior.Title + " for " + user + " EXPIRED TOKEN";
				behavior.PostsQuery = postsQuery.Where(x => x.PublishAt < DateTimeOffset.Now.AsMinutes());
				behavior.AddExpiredNote = true;
			}
			return behavior;
		}

		private string GetPostLink(Post post)
		{
			if (post.Id == null) // invalid feed
				return Url.AbsoluteAction("Index", "Posts");
			return Url.AbsoluteAction("Details", "PostDetails", new { Id = RavenIdResolver.Resolve(post.Id), Slug = SlugConverter.TitleToSlug(post.Title), Key = post.ShowPostEvenIfPrivate });
		}


		private bool GetNumberOfDays(string token, out int numberOfDays, out string user)
		{
			using (var rijndael = Rijndael.Create())
			{
				rijndael.Key = Convert.FromBase64String(BlogConfig.FuturePostsEncryptionKey);
				rijndael.IV = Convert.FromBase64String(BlogConfig.FuturePostsEncryptionIV);

				using(var memoryStream = new MemoryStream(Convert.FromBase64String(token)))
				using (var cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
				using (var reader = new BinaryReader(cryptoStream))
				{
					var expiry = DateTime.FromBinary(reader.ReadInt64());
					numberOfDays =  reader.ReadInt32();
					user = reader.ReadString();
					if (DateTime.UtcNow > expiry)
						return false;

					return true;
				}
			}
		}

		public ActionResult CommentsRss(int? id)
		{
			RavenQueryStatistics stats = null;
			var commentsTuples = RavenSession.QueryForRecentComments(q =>
			{
				if (id != null)
				{
					var postId = RavenSession.Advanced.GetDocumentId(id);
					q = q.Where(x => x.PostId == postId);
				}
				return q.Statistics(out stats).Take(30);
			});

			string responseETagHeader;
			if (CheckEtag(stats, out responseETagHeader))
				return HttpNotModified();

			var rss = new XDocument(
			new XElement("rss",
						 new XAttribute("version", "2.0"),
						 new XElement("channel",
									  new XElement("title", BlogConfig.Title),
									  new XElement("link", Url.RelativeToAbsolute(Url.RouteUrl("homepage"))),
									  new XElement("description", BlogConfig.MetaDescription ?? BlogConfig.Title),
									  new XElement("copyright", String.Format("{0} (c) {1}", BlogConfig.Copyright, DateTime.Now.Year)),
									  new XElement("ttl", "60"),
									  from commentsTuple in commentsTuples
									  let comment = commentsTuple.Item1
									  let post = commentsTuple.Item2
									  let link = Url.AbsoluteAction("Details", "PostDetails", new { Id = RavenIdResolver.Resolve(post.Id), Slug = SlugConverter.TitleToSlug(post.Title) }) + "#comment" + comment.Id
									  select new XElement("item",
														  new XElement("title", comment.Author +" commented on " + post.Title),
														  new XElement("description", comment.Body),
														  new XElement("link", link),
															new XElement("guid", link),
														  new XElement("pubDate", comment.CreatedAt.ToString("R"))
										)
							)
				)
			);

			return Xml(rss, responseETagHeader);

		}

		private bool CheckEtag(RavenQueryStatistics stats, out string responseETagHeader)
		{
			string requestETagHeader = Request.Headers["If-None-Match"] ?? string.Empty;
			responseETagHeader = stats.Timestamp.ToString("o") + EtagInitValue;
			return requestETagHeader == responseETagHeader;
		}

		public ActionResult LegacyRss()
		{
			return RedirectToActionPermanent("Rss", "Syndication");
		}
	}
}
