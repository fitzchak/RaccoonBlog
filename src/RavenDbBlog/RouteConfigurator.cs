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

            ConfigureSyndication();

            ConfigurePost();
            ConfigurePostAdmin();
            
    	    ConfigureSection();
    	    ConfigureSectionAdmin();

    	    ConfigureLogin();

            ConfigureUserAdmin();

            #region "Default"

            routes.MapRouteLowerCase("Default",
                "",
                new { controller = "Post", action = "List" }
                );

            #endregion
        }

        private void ConfigureSection()
        {
            routes.MapRouteLowerCase("SectionController-internal",
                "{controller}/{action}",
                new { },
                new { controller = "Section", action = "List" }
                );
        }

        private void ConfigureSectionAdmin()
        {
            routes.MapRouteLowerCase("SectionAdminController-ActionWithId",
                "admin/sections/{id}/{action}",
                new { controller = "SectionAdmin" },
                new { action = "Edit|SetPosition", id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("SectionAdminController-Action",
               "admin/sections/{action}",
               new { controller = "SectionAdmin" },
               new { action = "Add|Update" }
               );

            routes.MapRouteLowerCase("SectionAdminController-List",
                "admin/sections",
                new { controller = "SectionAdmin", action = "List" }
                );
        }

        private void ConfigureUserAdmin()
        {
            routes.MapRouteLowerCase("UserAdminController-ActionWithId",
              "admin/users/{id}/{action}",
              new { controller = "UserAdmin" },
              new { id = MatchPositiveInteger, action = "Edit|SetActivation|ChangePass" }
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

        private void ConfigurePostAdmin()
        {
            routes.MapRouteLowerCase("PostAdminController-PostActionWithId",
                "admin/posts/{id}/{action}",
                new {controller = "PostAdmin"},
                new {httpMethod = new HttpMethodConstraint("POST"), action = "SetPostDate|CommentsAdmin", id = MatchPositiveInteger}
                );

            routes.MapRouteLowerCase("PostAdminController-ActionWithId",
                "admin/posts/{id}/{action}",
                new { controller = "PostAdmin" },
                new { action = "Edit", id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostAdminController-Details",
                "admin/posts/{id}/{slug}",
                new { controller = "PostAdmin", action = "Details", slug = UrlParameter.Optional },
                new { id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostAdminController-Action",
                "admin/posts/{action}",
                new { controller = "PostAdmin" },
                new { action = "Update|Delete" }
                );

            routes.MapRouteLowerCase("PostAdminController-ListFeed",
                "admin/posts/feed",
                new { controller = "PostAdmin", action = "ListFeed" }
                );

            routes.MapRouteLowerCase("PostAdminController-List",
               "admin/posts",
               new { controller = "PostAdmin", action = "list" }
               );
        }

        private void ConfigureLogin()
        {
            routes.MapRouteLowerCase("LoginController",
               "users/{action}",
               new { controller = "Login" },
               new { action = "Login|LogOut|CurrentUser|AdministrationPanel" }
               );
        }

        private void ConfigurePost()
        {
            routes.MapRouteLowerCase("PostController-Comment",
                "{id}/comment",
                new { controller = "Post", action = "Comment" },
                new { httpMethod = new HttpMethodConstraint("POST"), id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostController-Details",
                "{id}/{slug}",
                new { controller = "Post", action = "Details", slug = UrlParameter.Optional },
                new { id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostController-internal",
                "{controller}/{action}",
                new { controller = "Post" },
                new { controller = "Post", action = "TagsList|ArchivesList" }
                );

            routes.MapRouteLowerCase("PostController-PostsByTag",
                "tags/{slug}",
                new { controller = "Post", action = "Tag" }
                );

            #region "Archive"

            routes.MapRouteLowerCase("RedirectLegacyPostUrl",
                "archive/{year}/{month}/{day}/{slug}.aspx",
                new { controller = "Post", action = "RedirectLegacyPost" },
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

            #endregion
        }

    	private void ConfigureSyndication()
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