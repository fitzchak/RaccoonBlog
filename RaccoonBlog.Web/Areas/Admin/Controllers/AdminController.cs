using System.Collections.Generic;
using System.Web.Mvc;
using RaccoonBlog.Web.Areas.Admin.Models;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	[Authorize]
	public abstract class AdminController : RaccoonController
	{
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.IsChildAction)
				return;

			ViewBag.BlogConfig = BlogConfig.MapTo<BlogConfigViewModel>();
			ViewBag.TopMenus = GetTopMenu();

			CompleteSessionHandler(filterContext);
		}

		private IList<AdminMenu> GetTopMenu()
		{
			return new List<AdminMenu>
			       	{
			       		new AdminMenu
			       			{
			       				Title = "Dashboard",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           	},
								Url = Url.Action("Index", "Home"),
								
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
			       				           		new AdminMenu {Title = "Elmah", Url = Url.Action("Index", "Elmah")},
												new AdminMenu {Title = "Settings", Url = Url.Action("Index", "Settings")},
			       				           	}
			       			},
			       	};
		}
	}
}
