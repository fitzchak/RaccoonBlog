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

        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View("Edit", new UserInput());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = Session.Load<User>(id);
            var vm = user.MapTo<UserInput>();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Update(UserInput input)
        {
            if (ModelState.IsValid)
            {
                var user = input.MapTo<User>();
                Session.Store(user);
                RedirectToAction("List");
            }
            return View("Edit", input);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}