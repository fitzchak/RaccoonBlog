using System.Web.Mvc;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
{
	public class WelcomeController : RaccoonController
	{
		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// don't load the non existant Blog/Config

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
			Session.Store(config);

			// Create default sections
			Session.Store(new Section { Title = "Archive", IsActive = true, Position = 1, ControllerName = "Section", ActionName = "ArchivesList" });
			Session.Store(new Section { Title = "Tags", IsActive = true, Position = 2, ControllerName = "Section", ActionName = "TagsList" });
			Session.Store(new Section { Title = "Statistics", IsActive = true, Position = 3, ControllerName = "Section", ActionName = "PostsStatistics" });
			var user = new User
			{
				FullName = "Default User",
				Email = "user@example.org",
				Enabled = true,
			};
			user.SetPassword("raccoon");
			Session.Store(user);

			return RedirectToAction("Success");
		}

		public ActionResult Success()
		{
			var config = Session.Load<BlogConfig>("Blog/Config");

			return config == null ? View("Index") : View(config);
		}

		private ActionResult AssertConfigurationIsNeeded()
		{
			if (Session.Load<BlogConfig>("Blog/Config") != null)
			{
				return Redirect("/");
			}
			return null;
		}
	}
}
