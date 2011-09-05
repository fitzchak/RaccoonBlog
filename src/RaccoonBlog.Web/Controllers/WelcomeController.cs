using System.Web.Mvc;
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

			if (!ModelState.IsValid)
				return View("Index");

			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				// Create the blog by storing the config
				config.Id = "Blog/Config";
				session.Store(config);

				// Create default sections
				session.Store(new Section{Title="Archive", IsActive = true, Position = 1, ControllerName = "Section", ActionName = "ArchivesList"});
				session.Store(new Section{Title="Tags", IsActive=true, Position = 2, ControllerName = "Section", ActionName = "TagsList"});
				session.Store(new Section{Title="Statistics", IsActive = true, Position=3, ControllerName="Section", ActionName="PostsStatistics"});

				session.SaveChanges();
			}

			return RedirectToAction("Success");
		}

		public ActionResult Success()
		{
			BlogConfig config;
			using (var session = MvcApplication.DocumentStore.OpenSession())
			{
				config = session.Load<BlogConfig>("Blog/Config");
			}

			return config == null ? View("Index") : View(config);
		}

		private void AssertConfigurationIsNeeded()
		{
			bool canContinue = true;
			using (var session = MvcApplication.DocumentStore.OpenSession())
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
