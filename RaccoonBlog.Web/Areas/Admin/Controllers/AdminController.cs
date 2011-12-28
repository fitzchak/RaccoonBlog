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

		private IList<AdminMenu> GetTopMenu()
		{
			return new List<AdminMenu>
			       	{
						new AdminMenu
						    {
						        Title = "Back To Blog",
						        SubMenus = null,
						        Url = Url.RouteUrl("Default"),
								
						    },
			       		new AdminMenu
			       			{
			       				Title = "Posts",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "All posts", Url = Url.Action("List", "Posts")},
			       				           		new AdminMenu{Title = "Create a new post", Url=Url.Action("Add", "Posts")}
			       				           	}
                                      
			       			},
			       		new AdminMenu
			       			{
			       				Title = "Sections",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "Sections list", Url = Url.Action("List", "Sections")},
			       				           		new AdminMenu {Title = "Add a new section", Url = Url.Action("Add", "Sections")}
			       				           	}
			       			},
			       		new AdminMenu
			       			{
			       				Title = "Users",
								Url = Url.Action("Index", "Users"),
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "All users", Url = Url.Action("List", "Users")},
			       				           		new AdminMenu {Title = "Add a new user", Url = Url.Action("Add", "Users")}
			       				           	}
			       			},
						new AdminMenu
			       			{
			       				Title = "Tools",
			       				SubMenus = new List<AdminMenu>
			       				           	{
												new AdminMenu {Title = "Settings", Url = Url.Action("Index", "Settings")},
			       				           	}
			       			},
			       	};
		}
	}
}