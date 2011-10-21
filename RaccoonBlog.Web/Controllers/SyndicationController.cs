using System;
using System.Web.Mvc;
using System.Linq;
using System.Xml.Linq;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Models;
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
									 new XElement(ns + "homePageLink", Url.RelativeToAbsolute(Url.RouteUrl("Default"))),
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

		public ActionResult Rss(string tag, Guid key)
		{
			RavenQueryStatistics stats;
			var postsQuery = RavenSession.Query<Post>()
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

			string responseETagHeader;
			if (CheckEtag(stats, out responseETagHeader))
				return HttpNotModified();

			var rss = new XDocument(
				new XElement("rss",
							 new XAttribute("version", "2.0"),
							 new XElement("channel",
										  new XElement("title", BlogConfig.Title),
										  new XElement("link", Url.RelativeToAbsolute(Url.RouteUrl("Default"))),
										  new XElement("description", BlogConfig.MetaDescription ?? BlogConfig.Title),
										  new XElement("copyright", String.Format("{0} (c) {1}", BlogConfig.Copyright, DateTime.Now.Year)),
										  new XElement("ttl", "60"),
										  from post in posts
										  let postLink = Url.AbsoluteAction("Details", "PostDetails", new { Id = RavenIdResolver.Resolve(post.Id), Slug = SlugConverter.TitleToSlug(post.Title) })
										  select new XElement("item",
															  new XElement("title", post.Title),
															  new XElement("description", post.Body),
															  new XElement("link", postLink),
																new XElement("guid", postLink),
															  new XElement("pubDate", post.PublishAt.ToString("R"))
											)
								)
					)
				);

			return Xml(rss, responseETagHeader);
		}


		public ActionResult CommentsRss(int? id)
		{
			RavenQueryStatistics stats = null;
			var commentsTuples = RavenSession.QueryForRecentComments(q =>
			{
				if (id != null)
				{
					var postId = "posts/" + id;
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
									  new XElement("link", Url.RelativeToAbsolute(Url.RouteUrl("Default"))),
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
