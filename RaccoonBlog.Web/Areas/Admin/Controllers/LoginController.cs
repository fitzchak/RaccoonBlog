using System.Web;
using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Models;

using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.Helpers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public partial class LoginController : RaccoonController
	{
		private readonly SignInHelper signInHelper;

		public LoginController()
		{
			signInHelper = new SignInHelper(System.Web.HttpContext.Current.GetOwinContext().Authentication);
		}

		[HttpGet]
		public virtual ActionResult Index(string returnUrl)
		{
			if (Request.IsAuthenticated)
			{
				return RedirectFromLoginPage();
			}

			return View(new LogOnModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public virtual ActionResult Index(LogOnModel input)
		{
			var user = RavenSession.GetUserByEmail(input.Login);

			if (user == null || user.ValidatePassword(input.Password) == false)
			{
				ModelState.AddModelError("UserNotExistOrPasswordNotMatch",
										 "Email and password do not match to any known user.");
			}
			else if (user.Enabled == false)
			{
				ModelState.AddModelError("NotEnabled", "The user is not enabled");
			}

			if (ModelState.IsValid)
			{
				signInHelper.SignIn(input, true);
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
		public virtual ActionResult LogOut(string returnurl)
		{
			signInHelper.SignOut();
			return RedirectFromLoginPage(returnurl);
		}

		[ChildActionOnly]
		public virtual ActionResult CurrentUser()
		{
			if (Request.IsAuthenticated == false)
				return View(new CurrentUserViewModel());

			var user = RavenSession.GetUserByEmail(HttpContext.User.Identity.Name);
			return View(new CurrentUserViewModel { FullName = user.FullName }); // TODO: we don't really need a VM here
		}
	}
}