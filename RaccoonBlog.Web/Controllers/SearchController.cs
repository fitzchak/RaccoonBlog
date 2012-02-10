using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
	public class SearchController : RaccoonController
	{
		public ActionResult SearchResult(string q)
		{
			return View((object)q);
		} 
		
		public ActionResult GoogleCse()
		{
			return View();
		}
	}
}