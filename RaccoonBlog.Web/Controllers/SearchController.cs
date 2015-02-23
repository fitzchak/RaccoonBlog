using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
	public partial class SearchController : RaccoonController
	{
		public virtual ActionResult SearchResult(string q)
		{
			return View((object)q);
		}

		public virtual ActionResult GoogleCse()
		{
			return View();
		}
	}
}