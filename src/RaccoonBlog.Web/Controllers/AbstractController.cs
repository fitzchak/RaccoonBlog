using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaccoonBlog.Web.Helpers.Results;
using RaccoonBlog.Web.Infrastructure.ActionResults;
using RaccoonBlog.Web.Models;
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
        }

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var blogConfig = Session.Load<BlogConfig>("Blog/Config");

			ViewBag.CustomCss = blogConfig.CustomCss;
			ViewBag.BlogTitle = blogConfig.Title;
			ViewBag.BlogSubtitle = blogConfig.Subtitle;
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