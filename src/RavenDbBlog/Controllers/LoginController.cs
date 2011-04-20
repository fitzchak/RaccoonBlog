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
            var user = Session.Query<User>()
                .Where(u => u.FullName == input.Username)
                .FirstOrDefault();

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

        [HttpGet]
        public ActionResult LogOut(string returnurl)
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("Default");
        }
    }
}