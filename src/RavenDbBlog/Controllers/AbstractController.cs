using System.Web.Mvc;
using Raven.Client;

namespace RavenDbBlog.Controllers
{
    public class AbstractController : Controller
    {
        public new IDocumentSession Session { get; set; }

        public AbstractController()
        {
            ViewBag.MetaDescription = "";
            ViewBag.MetaKeywords = "";
        }

        protected new HttpNotFoundWithViewResult HttpNotFound(string statusDescription = null)
        {
            return new HttpNotFoundWithViewResult(statusDescription);
        }

        protected HttpUnauthorizedWithViewResult HttpUnauthorized(string statusDescription = null)
        {
            return new HttpUnauthorizedWithViewResult(statusDescription);
        }
    }
}