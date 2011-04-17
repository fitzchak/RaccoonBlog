using System;
using System.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
using RavenDbBlog.Core.Models;
using System.Linq;
using Raven.Client.Linq;

namespace RavenDbBlog.Controllers
{
	public class RssController : AbstractController
	{
		public ActionResult Feed()
		{
			RavenQueryStatistics stats;
			var posts = Session.Query<Post>()
				.Statistics(out stats)
				.Where(x => x.PublishAt < DateTimeOffset.Now)
				.OrderByDescending(x => x.PublishAt)
				.Take(20)
				.ToList();

			string requestETagHeader = Request.Headers["If-None-Match"] ?? string.Empty;
			var responseETagHeader = stats.Timestamp.ToString("r");
			if (requestETagHeader == responseETagHeader)
				return HttpNotModified();

			Response.AddHeader("ETag", responseETagHeader);

			var rss = new XDocument(
				new XElement("rss",
				             new XAttribute("version", "2.0"),
				             new XElement("channel",
				                          new XElement("title", GetBlogTitle()),
				                          new XElement("link", Request.ApplicationPath),
				                          new XElement("description", GetBlogTitle()),
				                          new XElement("copyright", String.Format("{0} (c) {1}", GetBlogCopyright(), DateTime.Now.Year)),
				                          new XElement("ttl", "60"),
				                          from post in posts
				                          select new XElement("item",
				                                              new XElement("title", post.Title),
				                                              new XElement("description", post.Body),
				                                              new XElement("link", post.Slug),
				                                              new XElement("pubDate", post.PublishAt.ToString("R"))
				                          	)
				             	)
					)
				);

			return Xml(rss, responseETagHeader);
		}

		private static string GetBlogCopyright()
		{
			return ConfigurationManager.AppSettings["Copyright"];
		}

		private static string GetBlogTitle()
		{
			return ConfigurationManager.AppSettings["BlogName"];
		}
	}
}