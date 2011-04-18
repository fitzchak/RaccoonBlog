using System.Web.Mvc;
using System.Web.Routing;

namespace RavenDbBlog
{
    public static class RouteConfigurator
    {
        private const string MatchPositiveInteger = @"\d{1,10}";

        public static void Configure(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("CommentOnPost",
                "{id}/comment",
                new { controller = "Post", action = "Comment" },
                new { id = MatchPositiveInteger }
                );

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

            routes.MapRoute("AllPosts",
                "",
                new { controller = "Post", action = "List" }
                );

            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(routes);
        }
    }
}