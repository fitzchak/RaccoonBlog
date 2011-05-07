using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Controllers;

namespace RaccoonBlog.Web
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
            filters.Add(new ElmahHandleErrorAttribute());
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