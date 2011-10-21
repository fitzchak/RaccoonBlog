using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
	public class SearchController : RaccoonController
	{
		public ActionResult SearchResult(string q)
		{
			object model = q;
			return View(model);
		} 
		
		public ActionResult GoogleCse()
		{
			return View();
		}
	}
}
