using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace RaccoonBlog.Web
{
	public class RouteConfigurator
	{
		private const string MatchPositiveInteger = @"\d{1,10}";

		private readonly IRouteBuilder _routes;

		public RouteConfigurator(IRouteBuilder routes)
		{
			_routes = routes;
		}

		public void Configure()
		{
			/*Syndication();
		    Series();

			Posts();
			LegacyPost();
			PostDetails();

			Search();
			Css();
		    Error();
*/

		    _routes.MapRoute(
		        name: "areas",
		        template: "{area:exists}/{controller=Posts}/{action=Index}/{id?}"
		    );
            
/*routes.MapAttributeRoutes(config =>
			{
				config.UseLowercaseRoutes = true;
				config.PreserveCaseForUrlParameters = true;
			});*/
		    _routes.MapRoute(
		        name: "default",
		        template: "{controller=Posts}/{action=Index}");

			_routes.MapRoute(
		        name: "homepage",
		        template: "",
			    defaults: new { controller = "Posts", action = "Index" });
		}

       /* private void Error()
        {
            _routes.MapRouteLowerCase("Error",
                "error",
                new { controller = MVC.Error.Name, action = MVC.Error.ActionNames.Error },
                new[] { "RaccoonBlog.Web.Controllers" }
                );

            _routes.MapRouteLowerCase("Error404",
                "error/404",
                new { controller = MVC.Error.Name, action = MVC.Error.ActionNames.Error404 },
                new[] { "RaccoonBlog.Web.Controllers" }
                );
        }

        private void Css()
		{
			_routes.MapRouteLowerCase("CssController",
				"css",
				new { controller = "Css", action = "Merge" },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void Search()
		{
			_routes.MapRouteLowerCase("SearchController-GoogleCse",
			   "search/google_cse.xml",
			   new { controller = "Search", action = "GoogleCse" },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );

			_routes.MapRouteLowerCase("SearchController",
			   "search/{action}",
			   new { controller = "Search", action = "SearchResult" },
			   new { action = "SearchResult" },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );
		}

		private void Posts()
		{
			_routes.MapRouteLowerCase("PostsByTag",
				"tags/{slug}",
				new { controller = "Posts", action = "Tag" },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			_routes.MapRouteLowerCase("PostsByYearMonthDay",
				"archive/{year}/{month}/{day}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			_routes.MapRouteLowerCase("PostsByYearMonth",
				"archive/{year}/{month}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			_routes.MapRouteLowerCase("PostsByYear",
				"archive/{year}",
				new { controller = "Posts", action = "Archive" },
				new { Year = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void LegacyPost()
		{
			_routes.MapRouteLowerCase("RedirectLegacyPostUrl",
				"archive/{year}/{month}/{day}/{slug}.aspx",
				new { controller = "LegacyPost", action = "RedirectLegacyPost" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			_routes.MapRouteLowerCase("RedirectLegacyArchive",
			   "archive/{year}/{month}/{day}.aspx",
			   new { controller = "LegacyPost", action = "RedirectLegacyArchive" },
			   new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger },
			   new[] { "RaccoonBlog.Web.Controllers" }
			   );
		}

		private void PostDetails()
		{
			_routes.MapRouteLowerCase("PostDetailsController-Comment",
				"{id}/comment",
				new { controller = "PostDetails", action = "Comment" },
				new { httpMethod = new HttpMethodConstraint("POST"), id = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);

			_routes.MapRouteLowerCase("PostDetailsController-Details",
				"{id}/{slug}",
				new { controller = "PostDetails", action = "Details", slug = UrlParameter.Optional },
				new { id = MatchPositiveInteger },
				new[] { "RaccoonBlog.Web.Controllers" }
				);
		}

		private void Syndication()
		{
			_routes.MapRouteLowerCase("CommentsRssFeed",
			  "rss/comments",
			  new { controller = "Syndication", action = "CommentsRss"},
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			_routes.MapRouteLowerCase("RssFeed",
			  "rss/{tag}",
			  new { controller = "Syndication", action = "Rss", tag = UrlParameter.Optional },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			_routes.MapRouteLowerCase("RsdFeed",
			  "rsd",
			  new { controller = "Syndication", action = "Rsd" },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );

			_routes.MapRouteLowerCase("RssFeed-LegacyUrl",
			  "rss.aspx",
			  new { controller = "Syndication", action = "LegacyRss" },
			  new[] { "RaccoonBlog.Web.Controllers" }
			  );
		}

	    private void Series()
	    {
	        _routes.MapRouteLowerCase("SeriesController-PostsSeries",
	            "posts/series",
	            new {controller = "Series", action = "PostsSeries"},
	            new[] {"RaccoonBlog.Web.Controllers"}
	            );

            _routes.MapRouteLowerCase("PostsController-Series",
                "posts/series/{seriesId}/{seriesSlug}",
                new { controller = "Posts", action = "Series" },
                new[] { "RaccoonBlog.Web.Controllers" }
                );
	    }*/
	}
}