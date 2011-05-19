using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.Results;

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
