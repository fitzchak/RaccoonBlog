using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.Infrastructure.Controllers;

namespace RavenDbBlog
{
    public class MvcApplication : HttpApplication
    {
        public override void Init()
        {
            base.Init();
            RegisterHttpModules(this);
        }

        public static void RegisterHttpModules(HttpApplication context)
        {
            new EnsureLowerCaseRouteModule().Init(context);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RavenActionFilterAttribute());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
			new RouteConfigurator(RouteTable.Routes).Configure();

            AutoMapperConfiguration.Configure();
        }
    }
}