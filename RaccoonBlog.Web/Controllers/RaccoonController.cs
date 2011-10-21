using System;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaccoonBlog.Web.Helpers.Results;
using RaccoonBlog.Web.Infrastructure.ActionResults;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Commands;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
{
	public abstract class RaccoonController : Controller
	{
		public const int DefaultPage = 1;
		public const int PageSize = 25;

		public new IDocumentSession Session { get; private set; }

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
						HttpContext.Response.Redirect("/welcome", true);
						//throw new HttpException(302, "Found")
						//{
						//    Data =
						//        {
						//            {"Location", "/welcome"}
						//        }
						//};
					}
				}
				return blogConfig;
			}
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			Session = MvcApplication.DocumentStore.OpenSession();
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			//if (HandleRedirection(filterContext))
			//    return;

			if (filterContext.IsChildAction)
				return;

			ViewBag.BlogConfig = BlogConfig.MapTo<BlogConfigViewModel>();

			CompleteSessionHandler(filterContext);
		}

		//private bool HandleRedirection(ActionExecutedContext filterContext)
		//{
		//    var httpException = filterContext.Exception as HttpException;
		//    if (httpException == null || httpException.GetHttpCode() != 302)
		//    {
		//        return false;
		//    }
		//    filterContext.ExceptionHandled = true;
		//    filterContext.HttpContext.Response.Redirect((string) httpException.Data["Location"]);
		//    return true;
		//}

		protected void CompleteSessionHandler(ActionExecutedContext filterContext)
		{
			using (Session)
			{
				if (filterContext.Exception != null)
					return;

				if (Session != null)
					Session.SaveChanges();
			}

			TaskExecutor.StartExecuting();
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
