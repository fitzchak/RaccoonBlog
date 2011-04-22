using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class UserAdminController : AdminController
    {
    	private const int PageSize = 25;

    	public ActionResult List(int page = 0)
        {
            var users = Session.Query<User>()
                .OrderBy(u => u.FullName)
				.Skip(page * PageSize)
                .ToList();

            var vm = users.MapTo<UserSummeryViewModel>();
            return View(vm);
        }

        public ActionResult Pass(int id)
        {
			var user = Session.Load<User>(id);
			if (user == null)
				return HttpNotFound("User does not exist.");
			return View(user.MapTo<UserPasswordInput>());
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
            if (user == null)
                return HttpNotFound("User does not exist.");
        	return View(user.MapTo<UserInput>());
        }

        [HttpPost]
        public ActionResult Update(UserInput input)
        {
            if (ModelState.IsValid)
            {
                var user = Session.GetUserByEmail(input.Email) ?? new User();

            	input.MapTo(user);
                Session.Store(user);

                return RedirectToAction("List");
            }
            return View("Edit", input);
        }


		[HttpPost]
		public ActionResult ChangePass(UserPasswordInput input)
		{
			if (input.NewPass != input.NewPassConfirmation)
			{
				ModelState.AddModelError("NewPass", "New password does not match password confirmation");

			}
			var user = Session.Load<User>(input.Id);

			if (user.ValidatePassword(input.OldPass) == false)
			{
				ModelState.AddModelError("OldPass", "Old password did not match existing password");
			}

			if (ModelState.IsValid)
			{
				user.SetPassword(input.NewPass);
				return RedirectToAction("List");
			}

			return View("Pass", input);
		}
    }
}