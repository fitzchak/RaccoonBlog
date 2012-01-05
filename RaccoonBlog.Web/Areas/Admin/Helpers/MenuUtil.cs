using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RaccoonBlog.Web.Areas.Admin.Models;

namespace RaccoonBlog.Web.Areas.Admin.Helpers
{
	public static class MenuUtil
	{
		public static IList<MenuItem> GetTopMenu(UrlHelper url)
		{
			var items = new List<MenuItem>
			            	{
			            		new MenuItem {Title = "Back To Blog", Url = url.RouteUrl("homepage")},
			            		new MenuItem
			            			{
			            				Title = "Posts",
			            				SubMenus = new List<MenuItem>
			            				           	{
			            				           		new MenuItem {Title = "All posts", Url = url.Action("Index", "Posts")},
			            				           		new MenuItem {Title = "Create a new post", Url = url.Action("Add", "Posts")}
			            				           	}
			            			},
			            		new MenuItem
			            			{
			            				Title = "Sections",
			            				SubMenus = new List<MenuItem>
			            				           	{
			            				           		new MenuItem {Title = "Sections list", Url = url.Action("Index", "Sections")},
			            				           		new MenuItem {Title = "Add a new section", Url = url.Action("Add", "Sections")}
			            				           	}
			            			},
			            		new MenuItem
			            			{
			            				Title = "Users",
			            				SubMenus = new List<MenuItem>
			            				           	{
			            				           		new MenuItem {Title = "All users", Url = url.Action("Index", "Users")},
			            				           		new MenuItem {Title = "Add a new user", Url = url.Action("Add", "Users")}
			            				           	}
			            			},
			            		new MenuItem
			            			{
			            				Title = "Tools",
			            				SubMenus = new List<MenuItem>
			            				           	{
			            				           		new MenuItem {Title = "Settings", Url = url.Action("Index", "Settings")},
			            				           	}
			            			},
			            	};

			foreach (var menu in items)
			{
				var currentUri = url.RequestContext.HttpContext.Request.Url ?? new Uri("/");
				menu.IsCurrent = currentUri.PathAndQuery == menu.Url;
			}

			return items;
		}
	}
}