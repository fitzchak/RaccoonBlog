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
            var user = GetUserByEmail(input.Email);

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

        private User GetUserByEmail(string email)
        {
            return Session.Query<User>()
                .Where(u => u.Email == email)
                .FirstOrDefault();
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
            var user = GetCurrentUser();
            var vm = new CurrentUserViewModel();

            if (user != null)
            {
                vm.FullName = user.FullName;
            }
            return View(vm);
        }

        private User GetCurrentUser()
        {
            if (Request.IsAuthenticated == false)
                return null;
	
            var email = HttpContext.User.Identity.Name;
            var user = GetUserByEmail(email);
            return user;
        }
    }
}