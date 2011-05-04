using System.Web;
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

    public class ElmahResult : ActionResult
    {
        private string _resouceType;

        public ElmahResult(string resouceType)
        {
            _resouceType = resouceType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var factory = new Elmah.ErrorLogPageFactory();
            if (!string.IsNullOrEmpty(_resouceType))
            {
                var pathInfo = "." + _resouceType;
                HttpContext.Current.RewritePath("/admin/elmah", pathInfo, HttpContext.Current.Request.QueryString.ToString());
            }

            var handler = factory.GetHandler(HttpContext.Current, null, null, null);

            handler.ProcessRequest(HttpContext.Current);
        }
    }
}