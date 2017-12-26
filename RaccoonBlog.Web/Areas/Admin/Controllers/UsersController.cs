using System.Linq;
using System.Threading.Tasks;
using HibernatingRhinos.Loci.Common.Models;
using Microsoft.AspNetCore.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class UsersController : AdminController
	{
		public ActionResult Index()
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
		public async Task<ActionResult> Add(UserInput input)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			var user = new User();
			input.MapPropertiesToInstance(user);
			await RavenSession.StoreAsync(user);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<ActionResult> Edit(int id)
		{
			var user = await RavenSession.LoadAsync<User>("users/" + id);
			if (user == null)
				return NotFound("User does not exist.");
			return View(user.MapTo<UserInput>());
		}

		[HttpPost]
		public async Task<ActionResult> Update(UserInput input)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			var user = await RavenSession.LoadAsync<User>("users/" + input.Id) ?? new User();
			input.MapPropertiesToInstance(user);
			await RavenSession.StoreAsync(user);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<ActionResult> ChangePassword(int id)
		{
			var user = await RavenSession.LoadAsync<User>("users/" + id);
			if (user == null)
				return NotFound("User does not exist.");

			return View(new ChangePasswordModel());
		}

		[HttpPost]
		public async Task<ActionResult> ChangePassword(ChangePasswordModel input)
		{
			if (!ModelState.IsValid)
				return View("ChangePassword", input);

			var user = await RavenSession.LoadAsync<User>("users/" + input.Id);
			if (user == null)
				return NotFound("User does not exist.");

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
		public async Task<ActionResult> SetActivation(int id, bool isActive)
		{
			var user = await RavenSession.LoadAsync<User>("users/" + id);
			if (user == null)
				return NotFound("User does not exist.");

			user.Enabled = isActive;

			return RedirectToAction("Index");
		}
	}
}
