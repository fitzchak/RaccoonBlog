using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaccoonBlog.Web.Controllers.Results;
using RaccoonBlog.Web.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace RaccoonBlog.Web.Controllers
{
    public abstract class RaccoonController : Controller
    {
        public static IDocumentStore DocumentStore { get; set; }

        public IAsyncDocumentSession RavenSession { get; set; }

        protected NotModifiedResult HttpNotModified()
        {
            return new NotModifiedResult();
        }

        protected ActionResult Xml(XDocument xml, string etag)
        {
            return new XmlResult(xml, etag);
        }

        public const int DefaultPage = 1;

        public BlogConfig BlogConfig { get; private set; }
        public List<Section> Sections { get; private set; }

        /*private OutputCacheManager outputCacheManager;
        protected OutputCacheManager OutputCacheManager => outputCacheManager ?? (outputCacheManager = new OutputCacheManager());*/

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.IsHomePage = false;
            using (RavenSession = DocumentStore.OpenAsyncSession())
            {
                using (RavenSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5)))
                {
                    BlogConfig = await RavenSession.LoadAsync<BlogConfig>(BlogConfig.Key);
                    Sections =  await RavenSession.Query<Section>().ToListAsync();
                }

                if (BlogConfig == null && 
                    "welcome".Equals((string) RouteData.Values["controller"], StringComparison.OrdinalIgnoreCase) == false) // first launch
                {
                    HttpContext.Response.Redirect("~/welcome", true);
                }

                await next();

                ViewBag.BlogConfig = BlogConfig;
                ViewBag.Sections = Sections;


                /*if (Server.GetLastError() != null)
                    return;*/

                await RavenSession.SaveChangesAsync();
            }

            // TaskExecutor.StartExecuting();
        }

        public int PageSize => BlogConfig.PostsOnPage;

        protected int CurrentPage
        {
            get
            {
               /* var s = Request.QueryString["page"];
                int result;
                if (int.TryParse(s, out result))
                    return Math.Max(DefaultPage, result);*/
                return DefaultPage;
            }
        }
    }
}
