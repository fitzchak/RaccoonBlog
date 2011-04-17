using System;
using System.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
using RavenDbBlog.Core.Models;
using System.Linq;
using Raven.Client.Linq;

namespace RavenDbBlog.Controllers
{
	public class SyndicationController : AbstractController
	{
		public ActionResult Rsd()
		{
			var ns = XNamespace.Get("http://archipelago.phrasewise.com/rsd");

			return Xml(new XDocument(
			           	new XElement(ns + "service",
			           	             new XElement(ns + "engineName", "Raccoon Blog"),
			           	             new XElement(ns + "engineLink", "http://hibernatingrhinos.com"),
			           	             new XElement(ns + "homePageLink", Url.Action("List", "Post")),
			           	             new XElement(ns + "apis",
			           	                          new XElement(ns + "api",
			           	                                       new XAttribute("name", "MetaWeblog"),
			           	                                       new XAttribute("preferred", "true"),
			           	                                       new XAttribute("blogID", "0"),
			           	                                       new XAttribute("apiLink",Url.Content("~/Services/MetaWeblogAPI.ashx"))
			           	                          	)
			           	             	)
			           		)
			           	), typeof (SyndicationController).FullName);
		}

		public ActionResult Rss()
		{
			RavenQueryStatistics stats;
			var posts = Session.Query<Post>()
				.Statistics(out stats)
				.Where(x => x.PublishAt < DateTimeOffset.Now)
				.OrderByDescending(x => x.PublishAt)
				.Take(20)
				.ToList();

			string requestETagHeader = Request.Headers["If-None-Match"] ?? string.Empty;
			var responseETagHeader = stats.Timestamp.ToString("o");
			if (requestETagHeader == responseETagHeader)
				return HttpNotModified();

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