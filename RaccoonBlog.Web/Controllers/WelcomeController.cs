using System.Web.Mvc;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Controllers
{
	public class WelcomeController : RaccoonController
	{
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// don't load the non existent Blog/Config

			CompleteSessionHandler(filterContext);
		}

		//
		// GET: /Welcome/
		public ActionResult Index()
		{
			return AssertConfigurationIsNeeded() ?? View(BlogConfig.New());
		}

		[HttpPost]
		public ActionResult CreateBlog(BlogConfig config)
		{
			var result = AssertConfigurationIsNeeded();
			if (result != null)
				return result;

			if (!ModelState.IsValid)
				return View("Index");

			// Create the blog by storing the config
			config.Id = "Blog/Config";
			RavenSession.Store(config);

			// Create default sections
			RavenSession.Store(new Section { Title = "Archive", IsActive = true, Position = 1, ControllerName = "Section", ActionName = "ArchivesList" });
			RavenSession.Store(new Section { Title = "Tags", IsActive = true, Position = 2, ControllerName = "Section", ActionName = "TagsList" });
			RavenSession.Store(new Section { Title = "Statistics", IsActive = true, Position = 3, ControllerName = "Section", ActionName = "PostsStatistics" });
			
			var user = new User
			{
				FullName = "Default User",
				Email = config.OwnerEmail,
				Enabled = true,
			}.SetPassword("raccoon");
			RavenSession.Store(user);

			return RedirectToAction("Success", config);
		}

		public ActionResult Success()
		{
			BlogConfig bc;

			// Bypass the aggressive caching to force loading the BlogConfig object,
			// otherwise we might get a null BlogConfig even though a valid one exists
			using (RavenSession.Advanced.DocumentStore.DisableAggressiveCaching())
			{
				bc = RavenSession.Load<BlogConfig>("Blog/Config");
			}

			return bc == null ? View("Index") : View(bc);
		}

		private ActionResult AssertConfigurationIsNeeded()
		{
			BlogConfig bc;

			// Bypass the aggressive caching to force loading the BlogConfig object,
			// otherwise we might get a null BlogConfig even though a valid one exists
			using (RavenSession.Advanced.DocumentStore.DisableAggressiveCaching())
			{
				bc = RavenSession.Load<BlogConfig>("Blog/Config");
			}

			if (bc != null)
			{
				return Redirect("/");
			}
			return null;
		}
	}
}
