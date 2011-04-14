using System.Web.Mvc;

namespace RavenDbBlog.Controllers
{
    public class AbstractController : Controller
    {
        public AbstractController()
        {
            ViewBag.MetaDescription = "";
            ViewBag.MetaKeywords = "";
        }

        protected new HttpNotFoundWithViewResult HttpNotFound(string statusDescription = null)
        {
            return new HttpNotFoundWithViewResult(statusDescription);
        }

        protected HttpUnauthorizedResult HttpUnauthorized(string statusDescription = null)
        {
            return new HttpUnauthorizedResult(statusDescription);
        }
    }
}