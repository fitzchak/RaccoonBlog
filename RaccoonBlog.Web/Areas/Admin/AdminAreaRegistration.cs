using System.Web.Mvc;

namespace RaccoonBlog.Web.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Admin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Admin_default",
				"admin/{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				new[] { "RaccoonBlog.Web.Areas.Admin.Controllers" }
			);
		}
	}
}
