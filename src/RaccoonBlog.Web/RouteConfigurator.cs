using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Helpers.Routes;

namespace RaccoonBlog.Web
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
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            ConfigureSyndication();

            ConfigureWelcome();

            ConfigurePost();
            ConfigureLegacyPost();
            ConfigurePostDetails();
            ConfigurePostAdmin();
            ConfigureSocialLogin();
            
    	    ConfigureSection();
    	    ConfigureSectionAdmin();

    	    ConfigureSearch();

    	    ConfigureLogin();

    	    ConfigureCss();

            ConfigureUserAdmin();

			ConfigureConfigurationAdmin();

            ConfigureElmah();

            #region "Default"

            routes.MapRouteLowerCase("Default",
                "",
                new { controller = "Post", action = "List" }
                );

            #endregion
        }

    	private void ConfigureWelcome()
    	{
    		routes.MapRouteLowerCase("WelcomeScreen",
    		    "welcome/{action}",
    		    new {controller = "Welcome", action = "Index"}
    			);
    	}

    	private void ConfigureConfigurationAdmin()
    	{
			routes.MapRouteLowerCase("ConfigureConfigurationAdmin",
				"admin/configuration",
				new { controller = "ConfigurationAdmin", action = "Index" }
				);
    	}

    	private void ConfigureElmah()
        {
            routes.MapRouteLowerCase("ElmahController-internal",
                "admin/elmah/{type}",
                new { controller = "Elmah", action = "Index", type = UrlParameter.Optional }
                );
        }

        private void ConfigureCss()
        {
            routes.MapRouteLowerCase("CssController",
                "css",
                new { controller = "Css", action = "Merge" }
                );
        }

		private void ConfigureSocialLogin()
        {
			routes.MapRouteLowerCase("SocialLoginController",
				"users/authenticate",
				new { controller = "SocialLogin", action = "Authenticate" }
                );
        }

        private void ConfigureSection()
        {
            routes.MapRouteLowerCase("SectionController-internal",
                "{controller}/{action}",
                new { },
                new { controller = "Section", action = "List|TagsList|ArchivesList|FuturePosts|PostsStatistics|RecentComments" }
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
               new { action = "Add|Update|Delete" }
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

        private void ConfigureSearch()
        {
			routes.MapRouteLowerCase("SearchController-GoogleCse",
			   "search/google_cse.xml",
			   new { controller = "Search", action = "GoogleCse" }
			   );

            routes.MapRouteLowerCase("SearchController",
               "search/{action}",
			   new { controller = "Search", action = "SearchResult" },
			   new { action = "SearchResult" }
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
            routes.MapRouteLowerCase("PostController-PostsByTag",
                "tags/{slug}",
                new { controller = "Post", action = "Tag" }
                );

            #region "Archive"

            routes.MapRouteLowerCase("PostControllerPostsByYearMonthDay",
                "archive/{year}/{month}/{day}",
                new { controller = "Post", action = "Archive" },
                new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostsByYearMonth",
                "archive/{year}/{month}",
                new { controller = "Post", action = "Archive" },
                new { Year = MatchPositiveInteger, Month = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostsByYear",
                "archive/{year}",
                new { controller = "Post", action = "Archive" },
                new { Year = MatchPositiveInteger }
                );

            #endregion
        }

        private void ConfigureLegacyPost()
        {
            routes.MapRouteLowerCase("LegacyPostController-RedirectLegacyPostUrl",
                "archive/{year}/{month}/{day}/{slug}.aspx",
                new { controller = "LegacyPost", action = "RedirectLegacyPost" },
                new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("LegacyPostController-RedirectLegacyArchive",
               "archive/{year}/{month}/{day}.aspx",
               new { controller = "LegacyPost", action = "RedirectLegacyArchive" },
               new { Year = MatchPositiveInteger, Month = MatchPositiveInteger, Day = MatchPositiveInteger }
               );
        }

        private void ConfigurePostDetails()
        {
            routes.MapRouteLowerCase("PostDetailsController-Comment",
                "{id}/comment",
                new { controller = "PostDetails", action = "Comment" },
                new { httpMethod = new HttpMethodConstraint("POST"), id = MatchPositiveInteger }
                );

            routes.MapRouteLowerCase("PostDetailsController-Details",
                "{id}/{slug}",
                new { controller = "PostDetails", action = "Details", slug = UrlParameter.Optional },
                new { id = MatchPositiveInteger }
                );
        }

    	private void ConfigureSyndication()
    	{
			routes.MapRouteLowerCase("CommentsRssFeed",
			  "rss/comments",
			  new { controller = "Syndication", action = "CommentsRss"}
			  );

			routes.MapRouteLowerCase("RssFeed",
			  "rss/{tag}",
			  new { controller = "Syndication", action = "Rss", tag = UrlParameter.Optional }
			  );

			

			routes.MapRouteLowerCase("RsdFeed",
			  "rsd",
			  new { controller = "Syndication", action = "Rsd" }
			  );

            routes.MapRouteLowerCase("RssFeed-LegacyUrl",
              "rss.aspx",
              new { controller = "Syndication", action = "LegacyRss" }
              );
    	}
    }
}
