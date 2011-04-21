using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RavenDbBlog.Core.Models;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class LoginController : AbstractController
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectFromLoginPage();
            }

            return View(new LoginInput { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public ActionResult Login(LoginInput input)
        {
            var user = Session.GetUserByEmail(input.Email);

            const string loginFailMessage = "Email and password are not match.";
            if (user == null || user.ValidatePassword(input.Password) == false)
            {
                ModelState.AddModelError("UserNotExistOrPasswordNotMatch", loginFailMessage);
            }

            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(input.Email, false);
                return RedirectFromLoginPage(input.ReturnUrl);
            }

            var vm = new LoginInput {Email = input.Email, ReturnUrl = input.ReturnUrl};
            return View(vm);
        }

        private ActionResult RedirectFromLoginPage(string retrunUrl = null)
        {
            if (string.IsNullOrEmpty(retrunUrl))
                return RedirectToRoute("Default");
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

        	var user = Session.GetUserByEmail(HttpContext.User.Identity.Name);
        	return View(new CurrentUserViewModel {FullName = user.FullName});

        }

        private User GetCurrentUser()
        {
                return null;
	
           
        }
    }
}