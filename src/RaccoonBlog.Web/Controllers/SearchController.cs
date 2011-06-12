using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
	public class SearchController : AbstractController
    {
        public ActionResult SearchResult()
        {
        	return View();
        } 
		
		public ActionResult GoogleCse()
        {
        	return View();
        }
    }
}
