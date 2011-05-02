using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    public class SectionController : AbstractController
    {
        [ChildActionOnly]
        public ActionResult List()
        {
            return View();
        }
    }
}