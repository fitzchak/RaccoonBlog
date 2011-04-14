using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            ViewBag.MetaDescription = "";
            ViewBag.MetaKeywords = "";
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        protected new HttpNotFoundResult HttpNotFound(string statusDescription = null)
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            return new HttpNotFoundResult(statusDescription);
        }

        protected HttpUnauthorizedResult HttpUnauthorized(string statusDescription = null)
        {
            return new HttpUnauthorizedResult(statusDescription);
        }
    }
}