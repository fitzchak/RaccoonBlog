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

            routes.MapRouteLowerCase("LoginController",
               "users/{action}",
               new { controller = "Login" },
               new { action = "Login|LogOut|CurrentUser" }
               );

    		ConfigureAdmin();

            routes.MapRouteLowerCase("AllPosts",
                "",
                new { controller = "Post", action = "List" }
                );

            routes.MapRouteLowerCase("Default",
                "",
                new { controller = "Post", action = "List" }
                );
        }

    	private void ConfigureAdmin()
    	{
			routes.MapRouteLowerCase("UserAdminController-ActionWithId",
			  "admin/users/{id}/{action}",
			  new { controller = "UserAdmin" },
			  new { action = "Edit|Details|Delete", id = MatchPositiveInteger }
			  );

			routes.MapRouteLowerCase("UserAdminController-Action",
			   "admin/users/{action}",
			   new { controller = "UserAdmin" },
			   new { action = "Add|Update" }
			   );

            routes.MapRouteLowerCase("UserAdminController-UsersList",
               "admin/users",
               new { controller = "UserAdmin", action = "List" }
               );
    	}

    	private void ConfigureArchive()
    	{

			routes.MapRouteLowerCase("RedirectLegacyPostUrl",
				"archive/{year}/{month}/{day}/{slug}.aspx",
				new { controller = "Post", action = "RedirectItem" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
				);

			routes.MapRouteLowerCase("PostsByYearMonthDay",
				"archive/{year}/{month}/{day}",
				new { controller = "Post", action = "ArchiveYearMonthDay" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
				);

			routes.MapRouteLowerCase("PostsByYearMonth",
				"archive/{year}/{month}",
				new { controller = "Post", action = "ArchiveYearMonth" },
				new { Year = MatchPositiveInteger, Month = MatchPositiveInteger }
				);

			routes.MapRouteLowerCase("PostsByYear",
				"archive/{year}",
				new { controller = "Post", action = "ArchiveYear" },
				new { Year = MatchPositiveInteger }
				);
    	}

    	private void ConfigurePosts()
    	{
			routes.MapRouteLowerCase("PostById",
				"{id}/{slug}",
				new { controller = "Post", action = "Item", slug = UrlParameter.Optional },
				new { id = MatchPositiveInteger }
				);

			routes.MapRouteLowerCase("PostController",
				"{controller}/{action}",
				new { controller = "Post", action = "List" },
				new { controller = "Post", action = "TagsList|ArchivesList" }
				);

			routes.MapRouteLowerCase("PostsByTag",
				"tags/{name}",
				new { controller = "Post", action = "Tag" }
				);
    	}

    	private void ConfigureComments()
    	{
			routes.MapRouteLowerCase("CommentOnPost",
			 "{id}/comment",
			 new { controller = "Post", action = "Comment" },
			 new { id = MatchPositiveInteger }
			 );
    	}

    	private void ConfigureRss()
    	{

			routes.MapRouteLowerCase("RssFeed",
			  "rss",
			  new { controller = "Syndication", action = "Rss" }
			  );

			routes.MapRouteLowerCase("RssFeedByTag",
			  "rss/{name}",
			  new { controller = "Syndication", action = "Tag" }
			  );


			routes.MapRouteLowerCase("RsdFeed",
			  "rsd",
			  new { controller = "Syndication", action = "Rsd" }
			  );
    	}
    }
}