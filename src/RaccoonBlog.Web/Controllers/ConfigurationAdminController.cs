using System.Web.Mvc;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
	public class ConfigurationAdminController : AdminController
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View(BlogConfig.MapTo<BlogConfigurationInput>());
		}

		[HttpPost]
		public ActionResult Index(BlogConfigurationInput input)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.Message = ModelState.GetFirstErrorMessage();
				if (Request.IsAjaxRequest())
					return Json(new { Success = false, ViewBag.Message });
				return View(BlogConfig.MapTo<BlogConfigurationInput>());
			}

			var config = input.MapPropertiesToInstance(BlogConfig);
			Session.Store(config);

			ViewBag.Message = "Configuration successfully saved!";
			if (Request.IsAjaxRequest())
				return Json(new { Success = true, ViewBag.Message });
			return View(config.MapTo<BlogConfigurationInput>());
		}
	}
}