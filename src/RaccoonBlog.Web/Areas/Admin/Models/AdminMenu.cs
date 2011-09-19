using System.Collections.Generic;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Areas.Admin.Models
{
	public class AdminMenu
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public bool IsCurrent { get; set; }
		public IList<AdminMenu> SubMenus { get; set; }
	}
}