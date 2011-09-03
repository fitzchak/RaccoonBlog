using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.Raven;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Controllers
{
    public class WelcomeController : Controller
    {
        //
        // GET: /Welcome/

        public ActionResult Index()
        {
        	AssertConfigurationIsNeeded();

            return View(BlogConfig.New());
        }
		
		[HttpPost]
		public ActionResult CreateBlog(BlogConfig config)
		{
			AssertConfigurationIsNeeded();

			using (var session = DocumentStoreHolder.DocumentStore.OpenSession())
			{
				session.Store(config);
				session.SaveChanges();
			}

			return RedirectToAction("Success");
		}

		public ActionResult Success()
		{
			BlogConfig config;
			using (var session = DocumentStoreHolder.DocumentStore.OpenSession())
			{
				config = session.Load<BlogConfig>("Blog/Config");
			}

			return config == null ? View("Index") : View(config);
		}

		private void AssertConfigurationIsNeeded()
		{
			bool canContinue = true;
			using (var session = DocumentStoreHolder.DocumentStore.OpenSession())
			{
				if (session.Load<BlogConfig>("Blog/Config") != null)
					canContinue = false;
			}

			if (!canContinue)
			{
				Response.Redirect("/");
				Response.End();
			}
		}
    }
}
