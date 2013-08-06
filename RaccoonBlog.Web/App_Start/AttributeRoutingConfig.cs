using System.Web.Routing;
using AttributeRouting.Web.Mvc;

namespace RaccoonBlog.Web 
{
    public static class AttributeRoutingConfig
	{
		public static void RegisterRoutes(RouteCollection routes) 
		{    
			// See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			// To debug routes locally using the built in ASP.NET development server, go to /routes.axd
            
			routes.MapAttributeRoutes(config =>
			{
				config.UseLowercaseRoutes = true;
				config.PreserveCaseForUrlParameters = true;
			});
		}
    }
}