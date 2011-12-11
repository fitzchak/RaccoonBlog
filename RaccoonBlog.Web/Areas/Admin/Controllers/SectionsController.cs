using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Helpers.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class SectionsController : AdminController
	{
		public ActionResult Index()
		{
			return List();
		}

		public ActionResult List()
		{
			var sections = RavenSession.Query<Section>()
				.OrderBy(x => x.Position)
				.ToList();

			return View("List", sections);
		}

		[HttpGet]
		public ActionResult Add()
		{
			return View("Edit", new Section());
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return HttpNotFound("Section does not exist.");
			return View(section);
		}

		[HttpPost]
		public ActionResult Update(Section input)
		{
			if (!ModelState.IsValid)
				return View("Edit", input);

			var section = RavenSession.Load<Section>(input.Id) ?? new Section();
			input.MapPropertiesToInstance(section);
			if (section.Position == 0)
			{
				section.Position = RavenSession.Query<Section>()
					.Select(sec => sec.Position)
					.OrderByDescending(position => position)
					.FirstOrDefault() + 1;
			}
			RavenSession.Store(section);
			return RedirectToAction("List");
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return HttpNotFound("Section does not exist.");

			RavenSession.Delete(section);
			
			if (Request.IsAjaxRequest())
			{
				return Json(new { Success = true });
			}
			return RedirectToAction("List");
		}

		[AjaxOnly]
		[HttpPost]
		public ActionResult SetPosition(int id, int newPosition)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return Json(new {success = false, message = string.Format("There is no post with id {0}", id)});

			if (section.Position == newPosition)
				return Json(new {success = false, message = string.Format("The {0} section has already this position", section.Title)});

			if (section.Position > newPosition)
			{
				var sections = RavenSession.Query<Section>()
					.Where(s => s.Position >= newPosition && s.Position < section.Position)
					.OrderBy(s => s.Position)
					.ToList();

				foreach (var s in sections)
				{
					s.Position++;
				}
			}
			else if (section.Position < newPosition)
			{
				var sections = RavenSession.Query<Section>()
					.Where(s => s.Position < newPosition && s.Position >= section.Position)
					.OrderBy(s => s.Position)
					.ToList();

				foreach (var s in sections)
				{
					s.Position--;
				}
			}

			section.Position = newPosition;
			return Json(new { success = true });
		}
	}
}
