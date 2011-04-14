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

            routes.MapRoute(
                "post-new-comment",
                "posts/comment/{id}",
                new {controller = "Home", action = "NewComment"},
                new { id = MatchPositiveInteger }
                );

            routes.MapRoute(
                "post-details",
                "posts/{id}/{slug}",
                new { controller = "Home", action = "Show", slug = UrlParameter.Optional },
                new {id = MatchPositiveInteger}
                );

            routes.MapRoute(
                "post-list",
                "posts",
                new {controller = "Home", action = "Index"}
                );

            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new {controller = "Home", action = "Index"}
                );

            // RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
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