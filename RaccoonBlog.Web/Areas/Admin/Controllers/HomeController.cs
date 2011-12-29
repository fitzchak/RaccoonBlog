using System.Web.Mvc;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class HomeController : AdminController
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}