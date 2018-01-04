using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Helpers;

namespace RaccoonBlog.Web
{
	public class RouteConfigurator
	{
		private const string MatchPositiveInteger = @"\d{1,10}";
		private const string MatchRavenDocumentId = @"\d{1,10}(-[A-Za-z]{1,2})?";

		private readonly RouteCollection routes;

		public RouteConfigurator(RouteCollection routes)
		{
			this.routes = routes;
		}

		public void Configure()
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

			Syndication();
		    Series();

			Posts();
			LegacyPost();
			PostDetails();

			Search();
			Css();
		    Error();

			routes.MapRouteLowerCase("Default",
				"{controller}/{action}",
				new { controller = "Posts", action = "Index" },
				new [] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("homepage",
			   "",
			   new { controller = "Posts", action = "Index" },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );
		}

        private void Error()
        {
            routes.MapRouteLowerCase("Error",
                "error",
                new { controller = MVC.Error.Name, action = MVC.Error.ActionNames.Error },
                new[] { "RaccoonBlog.Web.Controllers" }
                );

            routes.MapRouteLowerCase("Error404",
                "error/404",
                new { controller = MVC.Error.Name, action = MVC.Error.ActionNames.Error404 },
                new[] { "RaccoonBlog.Web.Controllers" }
                );
        }

        private void Css()
		{
			routes.MapRouteLowerCase("CssController",
				"css",
				new { controller = "Css", action = "Merge" },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void Search()
		{
			routes.MapRouteLowerCase("SearchController-GoogleCse",
			   "search/google_cse.xml",
			   new { controller = "Search", action = "GoogleCse" },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );

			routes.MapRouteLowerCase("SearchController",
			   "search/{action}",
			   new { controller = "Search", action = "SearchResult" },
			   new { action = "SearchResult" },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );
		}

		private void Posts()
		{
			routes.MapRouteLowerCase("PostsByTag",
				"tags/{slug}",
				new { controller = "Posts", action = "Tag" },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("PostsByYearMonthDay",
				"archive/{year}/{month}/{day}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("PostsByYearMonth",
				"archive/{year}/{month}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("PostsByYear",
				"archive/{year}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void LegacyPost()
		{
			routes.MapRouteLowerCase("RedirectLegacyPostUrl",
				"archive/{year}/{month}/{day}/{slug}.aspx",
				new { controller = "LegacyPost", action = "RedirectLegacyPost" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("RedirectLegacyArchive",
			   "archive/{year}/{month}/{day}.aspx",
			   new { controller = "LegacyPost", action = "RedirectLegacyArchive" },
			   new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );
		}

		private void PostDetails()
		{
			routes.MapRouteLowerCase("PostDetailsController-Comment",
				"{id}/comment",
				new { controller = "PostDetails", action = "Comment" },
				new { httpMethod = new HttpMethodConstraint("POST"), id = MatchRavenDocumentId },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			routes.MapRouteLowerCase("PostDetailsController-Details",
				"{id}/{slug}",
				new { controller = "PostDetails", action = "Details", slug = UrlParameter.Optional },
			    new { id = MatchRavenDocumentId },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void Syndication()
		{
			routes.MapRouteLowerCase("CommentsRssFeed",
			  "rss/comments",
			  new { controller = "Syndication", action = "CommentsRss"},
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			routes.MapRouteLowerCase("RssFeed",
			  "rss/{tag}",
			  new { controller = "Syndication", action = "Rss", tag = UrlParameter.Optional },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			routes.MapRouteLowerCase("RsdFeed",
			  "rsd",
			  new { controller = "Syndication", action = "Rsd" },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			routes.MapRouteLowerCase("RssFeed-LegacyUrl",
			  "rss.aspx",
			  new { controller = "Syndication", action = "LegacyRss" },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );
		}

	    private void Series()
	    {
	        routes.MapRouteLowerCase("SeriesController-PostsSeries",
	            "posts/series",
	            new {controller = "Series", action = "PostsSeries"},
	            new[] {"RaccoonBlog.Web.Controllers"}
	            );

            routes.MapRouteLowerCase("PostsController-Series",
                "posts/series/{seriesId}/{seriesSlug}",
                new { controller = "Posts", action = "Series" },
                new[] { "RaccoonBlog.Web.Controllers" }
                );
	    }
	}
}