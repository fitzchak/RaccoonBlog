using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
    public class ConfigurationAdminController : AdminController
    {
		public ActionResult Index()
		{
			return View(BlogConfig);
		}
    }
}