using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
    public class ElmahController : AdminController
    {
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }

        public ActionResult Detail(string type)
        {
            return new ElmahResult(type);
        }
    }
}