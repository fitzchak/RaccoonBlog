using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Controllers
{
	public class WelcomeController : RaccoonController
	{
		public async Task<ActionResult> Index()
		{
			return await AssertConfigurationIsNeeded() ?? View(BlogConfig.New());
		}

		[HttpPost]
		public async Task<ActionResult> CreateBlog(BlogConfig config)
		{
			var result = await AssertConfigurationIsNeeded();
			if (result != null)
				return  result;

			if (!ModelState.IsValid)
				return View("Index");

			// Create the blog by storing the config
			config.Id = BlogConfig.Key;
			await RavenSession.StoreAsync(config);

			// Create default sections
		    await RavenSession.StoreAsync(new Section { Title = "Archive", IsActive = true, Position = 1, ControllerName = "Section", ActionName = "ArchivesList" });
		    await RavenSession.StoreAsync(new Section { Title = "Tags", IsActive = true, Position = 2, ControllerName = "Section", ActionName = "TagsList" });
		    await RavenSession.StoreAsync(new Section { Title = "Statistics", IsActive = true, Position = 3, ControllerName = "Section", ActionName = "PostsStatistics" });
			
			var user = new User
			{
				FullName = "Default User",
				Email = config.OwnerEmail,
				Enabled = true,
			}.SetPassword("raccoon");
		    await RavenSession.StoreAsync(user);

			return RedirectToAction("Success", config);
		}

		public async Task<ActionResult> Success()
		{
			BlogConfig bc;

			// Bypass the aggressive caching to force loading the BlogConfig object,
			// otherwise we might get a null BlogConfig even though a valid one exists
			using (RavenSession.Advanced.DocumentStore.DisableAggressiveCaching())
			{
				bc = await RavenSession.LoadAsync<BlogConfig>(BlogConfig.Key);
			}

			return bc == null ? View("Index") : View(bc);
		}

		private async Task<ActionResult> AssertConfigurationIsNeeded()
		{
			BlogConfig bc;

			// Bypass the aggressive caching to force loading the BlogConfig object,
			// otherwise we might get a null BlogConfig even though a valid one exists
			using (RavenSession.Advanced.DocumentStore.DisableAggressiveCaching())
			{
				bc = await RavenSession.LoadAsync<BlogConfig>("Blog/Config");
			}

			if (bc != null)
			{
				return Redirect("/");
			}
			return null;
		}
	}
}
