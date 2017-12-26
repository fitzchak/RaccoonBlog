/*using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using RaccoonBlog.Web.Areas.Admin.Models;

namespace RaccoonBlog.Web.Areas.Admin.Helpers
{
    using RaccoonBlog.Web.Areas.Admin.Enums;

    public static class MenuUtil
	{
		public static IList<MenuItem> GetTopMenu(UrlHelper url)
		{
			var items = new List<MenuItem>
			{
				new MenuItem {Title = "Back To Blog", Url = url.RouteUrl("homepage"), Type = MenuButtonType.Plain},
				new MenuItem {Title = "Posts", Url = url.Action("Index", "Posts"), Type = MenuButtonType.Plain},
                new MenuItem {Title = "Add new post", Url = url.Action("Add", "Posts"), Type = MenuButtonType.Add},
				new MenuItem {Title = "Sections", Url = url.Action("Index", "Sections"), Type = MenuButtonType.Plain},
                new MenuItem {Title = "Add new section", Url = url.Action("Add", "Sections"), Type = MenuButtonType.Add},
				new MenuItem {Title = "Users", Url = url.Action("Index", "Users"), Type = MenuButtonType.Plain},
                new MenuItem {Title = "Add new user", Url = url.Action("Add", "Users"), Type = MenuButtonType.Add},
				new MenuItem {Title = "Tools", Type = MenuButtonType.Toggle,
                    SubMenus = new List<MenuItem>
                    {
                        new MenuItem {Title = "Settings", Url = url.Action("Index", "Settings"), Type = MenuButtonType.Plain},
                        new MenuItem {Title = "RSS Future Access", Url = url.Action("RssFutureAccess", "Settings"), Type = MenuButtonType.Plain},
                        new MenuItem {Title = "Reddit submission", Url = url.Action("RedditSubmission", "Settings"), Type = MenuButtonType.Plain},
                    }
				},
			};

			AnalyzeMenuItems(items, url.RequestContext.HttpContext.Request.Url ?? new Uri("/"));
			

			return items;
		}

		private static void AnalyzeMenuItems(IEnumerable<MenuItem> items, Uri currentUri)
		{
			foreach (var menu in items)
			{
				if (menu.SubMenus != null)
				{
					if (menu.Url == null)
					{
						menu.Url = (menu.SubMenus.FirstOrDefault() ?? new MenuItem()).Url;
					}
					AnalyzeMenuItems(menu.SubMenus, currentUri);
				}
				menu.IsCurrent = currentUri.PathAndQuery == menu.Url;
			}
		}
	}
}*/