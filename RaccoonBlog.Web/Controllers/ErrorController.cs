/*using System.Web.Mvc;

using NLog;

namespace RaccoonBlog.Web.Controllers
{
    public class ErrorController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("error")]
        public virtual ActionResult Error()
        {
            HttpContext.Response.StatusCode = ViewBag.ErrorCode = 500;
            HttpContext.Response.TrySkipIisCustomErrors = true;
            ViewBag.ErrorMessage = "error";

            return View(MVC.Shared.Views.Error);
        }

        [HttpGet]
        [Route("error/404")]
        public virtual ActionResult Error404(string aspxerrorpath)
        {
            if (string.IsNullOrEmpty(aspxerrorpath) == false)
            {
                Log.Warn("Could not find path: " + aspxerrorpath);
            }

            HttpContext.Response.StatusCode = ViewBag.ErrorCode = 404;
            HttpContext.Response.TrySkipIisCustomErrors = true;
            ViewBag.ErrorMessage = "not found";

            return View(MVC.Shared.Views.Error);
        }
    }
}*/