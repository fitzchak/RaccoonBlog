using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace RavenDbBlog
{
    public class RouteConfigurator
    {
        private const string MatchPositiveInteger = @"\d{1,10}";

    	private readonly RouteCollection routes;

    	public RouteConfigurator(RouteCollection routes)
    	{
    		this.routes = routes;
    	}

    	public void Configure()
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

    		ConfigureComments();

        	ConfigureRss();

    		ConfigurePosts();

    		ConfigureArchive();

            routes.MapRoute("LoginController",
               "users/{action}",
               new { controller = "Login" },
               new { action = "Login|LogOut|CurrentUser" }
               );

    		ConfigureAdmin();

            routes.MapRoute("AllPosts",
                "",
                new { controller = "Post", action = "List" }
                );

            routes.MapRoute("Default",
                "",
                new { controller = "Post", action = "List" }
                );
        }

    	private void ConfigureAdmin()
    	{
			routes.MapRoute("UserAdminController1",
			  "admin/users/{id}/{action}",
			  new { controller = "UserAdmin" },
			  new { action = "Edit|Details|Delete", id = MatchPositiveInteger }
			  );

			routes.MapRoute("UserAdminController2",
			   "admin/users/{action}",
			   new { controller = "UserAdmin" },
			   new { action = "Update|List" }
			   );
    	}

    	private void ConfigureArchive()
    	{

			routes.MapRoute("RedirectLegacyPostUrl",
				"archive/{year}/{month}/{day}/{slug}.aspx",
				new { controller = "Post", action = "RedirectItem" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
				);

			routes.MapRoute("PostsByYearMonthDay",
				"archive/{year}/{month}/{day}",
				new { controller = "Post", action = "ArchiveYearMonthDay" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
				);

			routes.MapRoute("PostsByYearMonth",
				"archive/{year}/{month}",
				new { controller = "Post", action = "ArchiveYearMonth" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger }
				);

			routes.MapRoute("PostsByYear",
				"archive/{year}",
				new { controller = "Post", action = "ArchiveYear" },
				new { Year = MatchPositiveInteger }
				);
    	}

    	private void ConfigurePosts()
    	{
			routes.MapRoute("PostById",
				"{id}/{slug}",
				new { controller = "Post", action = "Item", slug = UrlParameter.Optional },
				new { id = MatchPositiveInteger }
				);

			routes.MapRoute("PostController",
				"{controller}/{action}",
				new { controller = "Post", action = "List" },
				new { controller = "Post", action = "TagsList|ArchivesList" }
				);

			routes.MapRoute("PostsByTag",
				"tags/{name}",
				new { controller = "Post", action = "Tag" }
				);
    	}

    	private void ConfigureComments()
    	{
			routes.MapRoute("CommentOnPost",
			 "{id}/comment",
			 new { controller = "Post", action = "Comment" },
			 new { id = MatchPositiveInteger }
			 );
    	}

    	private void ConfigureRss()
    	{

			routes.MapRoute("RssFeed",
			  "rss",
			  new { controller = "Syndication", action = "Rss" }
			  );

			routes.MapRoute("RssFeedByTag",
			  "rss/{name}",
			  new { controller = "Syndication", action = "Tag" }
			  );


			routes.MapRoute("RsdFeed",
			  "rsd",
			  new { controller = "Syndication", action = "Rsd" }
			  );
    	}
    }
}