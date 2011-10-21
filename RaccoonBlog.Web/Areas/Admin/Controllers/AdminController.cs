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
			       				           	}
			       			},
			       		new AdminMenu
			       			{
			       				Title = "Sections",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "Sections list", Url = Url.Action("List", "Section")},
			       				           		new AdminMenu {Title = "Add a new section", Url = Url.Action("Add", "Section")}
			       				           	}
			       			},
			       		new AdminMenu
			       			{
			       				Title = "Posts",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "All posts", Url = Url.Action("List", "Post")},
			       				           		//new AdminMenu{Title = "Add a new section", Url=Url.Action("Add", "Post")}
			       				           	}
			       			},
			       		new AdminMenu
			       			{
			       				Title = "Users",
			       				SubMenus = new List<AdminMenu>
			       				           	{
			       				           		new AdminMenu {Title = "All users", Url = Url.Action("List", "Users")},
			       				           		new AdminMenu {Title = "Add a new user", Url = Url.Action("Add", "Users")}
			       				           	}
			       			},
			       	};
		}
	}
}
