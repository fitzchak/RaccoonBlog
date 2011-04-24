using System.IO;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Raven.Client;
using RavenDbBlog.Infrastructure.ActionResults;

namespace RavenDbBlog.Controllers
{
    public class AbstractController : Controller
    {
        protected const int DefaultPage = 1;
        protected const int PageSize = 25;

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

		protected HttpStatusCodeResult HttpNotModified()
		{
			return new HttpStatusCodeResult(304);
		}

		protected ActionResult Xml(XDocument document, string etag)
		{
			return new XmlResult(document, etag);
		}
    }
}