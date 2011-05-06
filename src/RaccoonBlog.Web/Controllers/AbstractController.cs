using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaccoonBlog.Web.Helpers.Results;
using RaccoonBlog.Web.Infrastructure.ActionResults;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        public const int DefaultPage = 1;
        public const int PageSize = 25;

        public new IDocumentSession Session { get; set; }

    	protected AbstractController()
        {
            ViewBag.MetaDescription = "";
            ViewBag.MetaKeywords = "";
            ViewBag.CustomCss = ConfigurationManager.AppSettings["MainUrl"] ?? Request.Url.Authority.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
        
        protected int CurrentPage
        {
            get
            {
                var s = Request.QueryString["page"];
                int result;
                if (int.TryParse(s, out result))
                    return Math.Max(DefaultPage, result);
                return DefaultPage;
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