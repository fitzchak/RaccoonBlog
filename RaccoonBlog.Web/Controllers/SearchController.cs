/*using System.Configuration;

namespace RaccoonBlog.Web.Controllers
{
	public class SearchController : RaccoonController
	{
	    private static string GoogleCustomSearchId => ConfigurationManager.AppSettings["Raccoon/GoogleCustomSearch/Id"];

	    public virtual ActionResult SearchResult(string q)
	    {
	        ViewBag.GoogleCustomSearchId = GoogleCustomSearchId;
		    ViewBag.SearchTerm = q;
			return View((object)q);
		}
	}
}*/