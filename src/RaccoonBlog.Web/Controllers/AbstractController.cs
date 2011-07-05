using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaccoonBlog.Web.Helpers.Results;
using RaccoonBlog.Web.Infrastructure.ActionResults;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        public const int DefaultPage = 1;
        public const int PageSize = 25;

        public new IDocumentSession Session { get; set; }

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
            if (filterContext.IsChildAction)
                return;

			ViewBag.BlogConfig = BlogConfig.MapTo<BlogConfigViewModel>();
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

		
		protected ActionResult XmlView(object model = null, string etag = null)
		{
			if (model != null)
				ViewData.Model = model;

			return new XmlViewResult
			{
				ETag = etag,
				ViewName = null,
				MasterName = null,
				ViewData = ViewData,
				TempData = TempData
			};
		}

        protected new JsonNetResult Json(object data)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return new JsonNetResult(data, settings);
        }

    	private BlogConfig blogConfig;
    	public BlogConfig BlogConfig
    	{
    		get
    		{
				if (blogConfig == null)
				{
					blogConfig = Session.Load<BlogConfig>("Blog/Config");
					if (blogConfig == null)
					{
						Session.Store(blogConfig = BlogConfig.New());
					}
				}
				return blogConfig;
    		}
    	}
    }
}
