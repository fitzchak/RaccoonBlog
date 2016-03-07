using System.Web;
using System.Web.Mvc;

using RaccoonBlog.Web.Helpers.Attributes;

namespace RaccoonBlog.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
            filters.Add(new CustomHandleErrorAttribute());
        }
	}
}