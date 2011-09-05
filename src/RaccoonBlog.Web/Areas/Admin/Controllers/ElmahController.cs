using System.Web.Mvc;
using RaccoonBlog.Web.Helpers.Results;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
    public class ElmahController : AdminController
    {
        public ActionResult Index(string type)
        {
            return new ElmahResult(type);
        }
    }
}
