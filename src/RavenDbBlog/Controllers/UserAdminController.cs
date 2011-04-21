using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class UserAdminController : AdminController
    {
        public ActionResult List()
        {
            var users = Session.Query<User>()
                .OrderBy(u => u.FullName)
                .ToList();

            var vm = users.MapTo<UserSummeryViewModel>();
            return View(vm);
        }

        public ActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View(new UserInput());
        }

        [HttpPost]
        public ActionResult Add(UserInput input)
        {
            if (ModelState.IsValid)
            {
                var user = input.MapTo<User>();
                Session.Store(user);
                RedirectToAction("List");
            }
            return View(input);
        }

        public ActionResult Edit(UserInput input)
        {
            return View();
        }

        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}