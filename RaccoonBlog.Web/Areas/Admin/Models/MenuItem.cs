using System.Collections.Generic;

namespace RaccoonBlog.Web.Areas.Admin.Models
{
	public class MenuItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public bool IsCurrent { get; set; }
		public IList<MenuItem> SubMenus { get; set; }
	}
}