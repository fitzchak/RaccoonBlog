using System.Linq;
using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Models;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminController
    {
		public ActionResult Index()
		{
			return List();
		}

    	public ActionResult List()
        {
			var users = RavenSession.Query<User>()
                .OrderBy(u => u.FullName)
                .ToList();

            var vm = users.MapTo<UserSummeryViewModel>();
            return View("List", vm);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View("Edit", new UserInput());
        }

		[HttpPost]
		public ActionResult Add(UserInput input)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			var user = new User();
			input.MapPropertiesToInstance(user);
			RavenSession.Store(user);
			return RedirectToAction("Index");
		}

        [HttpGet]
        public ActionResult Edit(int id)
        {
			var user = RavenSession.Load<User>(id);
            if (user == null)
                return HttpNotFound("User does not exist.");
        	return View(user.MapTo<UserInput>());
        }

        [HttpPost]
        public ActionResult Edit(UserInput input, int id)
        {
        	if (!ModelState.IsValid)
        		return View("Edit", input);

			var user = RavenSession.Load<User>(id) ?? new User();
        	input.MapPropertiesToInstance(user);
			RavenSession.Store(user);
        	return RedirectToAction("Index");
        }

        [HttpGet]
		public ActionResult ChangePassword(int id)
        {
			var user = RavenSession.Load<User>(id);
            if (user == null)
                return HttpNotFound("User does not exist.");

        	return View(new ChangePasswordModel());
        }

		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel input, int id)
		{
			if (!ModelState.IsValid)
				return View("ChangePassword", input);

			var user = RavenSession.Load<User>(id);
			if (user == null)
				return HttpNotFound("User does not exist.");

		    if (user.ValidatePassword(input.OldPassword) == false)
		    {
		        ModelState.AddModelError("OldPassword", "Old password did not match existing password");
		    }

			if (ModelState.IsValid == false)
				return View(input);

            user.SetPassword(input.NewPassword);
		    return RedirectToAction("Index");
		}

        [HttpPost]
		public ActionResult SetActivation(int id, bool isActive)
		{
			var user = RavenSession.Load<User>(id);
			if (user == null)
				return HttpNotFound("User does not exist.");

			user.Enabled = isActive;

			return RedirectToAction("Index");
		}
    }
}
