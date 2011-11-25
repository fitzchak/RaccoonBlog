using System.Web.Mvc;
using System.Web.Security;
using HibernatingRhinos.Loci.Common.Models;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class LoginController : RaccoonController
	{
		[HttpGet]
		public ActionResult Index(string returnUrl)
		{
			if (Request.IsAuthenticated)
			{
				return RedirectFromLoginPage();
			}

			return View(new LogOnModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public ActionResult Index(LogOnModel input)
		{
			var user = RavenSession.GetUserByEmail(input.Login);

			if (user == null || user.ValidatePassword(input.Password) == false)
			{
				ModelState.AddModelError("UserNotExistOrPasswordNotMatch", 
					"Email and password do not match to any known user.");
			}
			else if(user.Enabled == false)
			{
				ModelState.AddModelError("NotEnabled", "The user is not enabled");
			}

			if (ModelState.IsValid)
			{
				FormsAuthentication.SetAuthCookie(input.Login, true);
				return RedirectFromLoginPage(input.ReturnUrl);
			}

			return View(new LogOnModel { Login = input.Login, ReturnUrl = input.ReturnUrl });
		}

		private ActionResult RedirectFromLoginPage(string retrunUrl = null)
		{
			if (string.IsNullOrEmpty(retrunUrl))
				return RedirectToRoute("homepage");
			return Redirect(retrunUrl);
		}

		[HttpGet]
		public ActionResult LogOut(string returnurl)
		{
			FormsAuthentication.SignOut();
			return RedirectFromLoginPage(returnurl);
		}

		[ChildActionOnly]
		public ActionResult CurrentUser()
		{
			if (Request.IsAuthenticated == false)
				return View(new CurrentUserViewModel());

			var user = RavenSession.GetUserByEmail(HttpContext.User.Identity.Name);
			return View(new CurrentUserViewModel {FullName = user.FullName}); // TODO: we don't really need a VM here
		}
	}
}