using System.Collections.Generic;

namespace RaccoonBlog.Web.Areas.Admin.Models
{
    using RaccoonBlog.Web.Areas.Admin.Enums;

    public class MenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public bool IsCurrent { get; set; }
	    public MenuButtonType Type { get; set; }
		public IList<MenuItem> SubMenus { get; set; }
	}
}