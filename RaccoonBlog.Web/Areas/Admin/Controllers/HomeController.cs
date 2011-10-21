using System.Web.Mvc;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
