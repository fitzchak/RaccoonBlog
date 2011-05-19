using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Infrastructure.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Binders;
using RaccoonBlog.Web.Infrastructure.Controllers;

namespace RaccoonBlog.Web
{
    public class MvcApplication : HttpApplication
    {
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
            ModelBinders.Binders.Add(typeof(CommentCommandOptions), new RemoveSpacesEnumBinder());

            AutoMapperConfiguration.Configure();
        }
    }
}
