using System.Web.Mvc;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class SettingsController : AdminController
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View(BlogConfig);
		}

		[HttpPost]
		public ActionResult Index(BlogConfig config)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.Message = ModelState.FirstErrorMessage();
				if (Request.IsAjaxRequest())
					return Json(new { Success = false, ViewBag.Message });
				return View(BlogConfig);
			}

			RavenSession.Store(config);

			ViewBag.Message = "Configurations successfully saved!";
			if (Request.IsAjaxRequest())
				return Json(new { Success = true, ViewBag.Message });
			return View(config);
		}
	}
}
