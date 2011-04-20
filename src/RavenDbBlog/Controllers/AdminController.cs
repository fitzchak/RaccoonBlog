using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    [Authorize]
    public class AdminController : AbstractController
    {
        public ActionResult Comments()
        {
            //var comments = GetComments
            return View();
        }
    }
}