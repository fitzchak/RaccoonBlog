using System;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaccoonBlog.Web.Helpers.Results;
using RaccoonBlog.Web.Infrastructure.ActionResults;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Raven;
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

		private BlogConfig blogConfig;
		public BlogConfig BlogConfig
		{
			get
			{
				if (blogConfig == null)
				{
					using (Session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
					{
						blogConfig = Session.Load<BlogConfig>("Blog/Config");
					}

					if (blogConfig == null) // first launch
					{
						Session.Advanced.DocumentStore.DisableAggressiveCaching();
						Response.Redirect("/welcome");
						Response.End();
					}
				}
				return blogConfig;
			}
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			if (filterContext.IsChildAction)
				return;

			ViewBag.BlogConfig = BlogConfig.MapTo<BlogConfigViewModel>();

			using (Session)
			{
				if (filterContext.Exception == null)
					Session.SaveChanges();
			}
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
		
		protected ActionResult Xml(XDocument xml, string etag)
		{
			return new XmlResult(xml, etag);
		}

		protected new JsonNetResult Json(object data)
		{
			var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			return new JsonNetResult(data, settings);
		}
	}
}
