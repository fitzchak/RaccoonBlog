using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raven.Client;
using RavenDbBlog.Infrastructure.ActionResults;

namespace RavenDbBlog.Controllers
{
    public abstract class AbstractController : Controller
    {
        protected const int DefaultPage = 1;
        protected const int PageSize = 25;

        public new IDocumentSession Session { get; set; }

        public AbstractController()
        {
            ViewBag.MetaDescription = "";
            ViewBag.MetaKeywords = "";
        }
        
        protected int CurrentPage
        {
            get
            {
                var s = Request.QueryString["page"];

                int result;
                if (int.TryParse(s, out result))
                    return result;
                return 0;
            }
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

        protected new JsonNetResult Json(object data)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return new JsonNetResult(data, settings);
        }
    }
}