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
                return RedirectToRoute("Default");
            }

            return View(new LoginInput());
        }

        [HttpPost]
        // [RequireHttps(Order = 1)]
        public ActionResult Login(LoginInput input)
        {
            var user = GetUserByName(input.Username);

            const string loginFailMessage = "Username and password are not valid.";
            if (user == null || user.ValidatePassword(input.Password) == false)
            {
                ModelState.AddModelError("UserNotExistOrPasswordNotMatch", loginFailMessage);
            }

            if (ModelState.IsValid)
            {
                FormsAuthentication.RedirectFromLoginPage(input.Username, false);
            }

            var vm = new LoginInput {Username = input.Username};
            return View(vm);
        }

        private User GetUserByName(string username)
        {
            return Session.Query<User>()
                .Where(u => u.Username == username)
                .FirstOrDefault();
        }

        [HttpGet]
        public ActionResult LogOut(string returnurl)
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("Default");
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
	
            var username = HttpContext.User.Identity.Name;
            var user = GetUserByName(username);
            return user;
        }
    }
}