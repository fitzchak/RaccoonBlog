using System.Collections.Generic;
using System.Web.Mvc;
using RaccoonBlog.Web.Areas.Admin.Models;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	[Authorize]
	public abstract class AdminController : RaccoonController
	{
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
			ViewBag.TopMenus = GetTopMenu();
		}

		private IList<MenuItem> GetTopMenu()
		{
			return new List<MenuItem>
					{
						new MenuItem
							{
								Title = "Back To Blog",
								SubMenus = null,
								Url = Url.RouteUrl("homepage"),
								
							},
						new MenuItem
							{
								Title = "Posts",
								SubMenus = new List<MenuItem>
											{
												new MenuItem {Title = "All posts", Url = Url.Action("List", "Posts")},
												new MenuItem{Title = "Create a new post", Url=Url.Action("Add", "Posts")}
											}
									  
							},
						new MenuItem
							{
								Title = "Sections",
								SubMenus = new List<MenuItem>
											{
												new MenuItem {Title = "Sections list", Url = Url.Action("List", "Sections")},
												new MenuItem {Title = "Add a new section", Url = Url.Action("Add", "Sections")}
											}
							},
						new MenuItem
							{
								Title = "Users",
								Url = Url.Action("Index", "Users"),
								SubMenus = new List<MenuItem>
											{
												new MenuItem {Title = "All users", Url = Url.Action("List", "Users")},
												new MenuItem {Title = "Add a new user", Url = Url.Action("Add", "Users")}
											}
							},
						new MenuItem
							{
								Title = "Tools",
								SubMenus = new List<MenuItem>
											{
												new MenuItem {Title = "Settings", Url = Url.Action("Index", "Settings")},
											}
							},
					};
		}
	}
}