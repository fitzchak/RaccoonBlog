using System.Web.Mvc;
using System.Web.Routing;
using RavenDbBlog.Infrastructure;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.Controllers;

namespace RavenDbBlog
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RavenActionFilterAttribute());
        }

        public const string MatchPositiveInteger = @"\d{1,10}";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("CommentOnPost",
                "{id}/comment",
                new { controller = "Post", action = "Comment"},
                new { id = MatchPositiveInteger }
                );

            routes.MapRoute("PostById",
                "{id}/{slug}",
                new { controller = "Post", action = "Item", slug = UrlParameter.Optional },
                new { id = MatchPositiveInteger }
                );
            
            routes.MapRoute("PostController",
                "{controller}/{action}",
                new { controller = "Post", action = "List"},
                new { controller = "Post", action = "TagsList|ArchivesList" }
                );

            routes.MapRoute("PostsByTag",
                "tags/{name}",
                new { controller = "Post", action = "Tag" }
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
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            AutoMapperConfiguration.Configure();
        }
    }
}