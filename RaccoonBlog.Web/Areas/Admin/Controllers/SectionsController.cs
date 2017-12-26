/*
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Areas.Admin.Controllers
{
	public class SectionsController : AdminController
	{
		public ActionResult Index()
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
		public ActionResult Edit(string id)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return NotFound("Section does not exist.");

			return View(section);
		}

		[HttpPost]
		public ActionResult Activate(string id, bool activate)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return NotFound("Section does not exist.");

			section.IsActive = activate;

			OutputCacheManager.RemoveItems(MVC.Section.Name);

		    return Ok();
		}

		[HttpPost]
		public ActionResult Update(Section section)
		{
			if (!ModelState.IsValid)
				return View("Edit", section);

			if (section.Position == 0)
			{
				section.Position = RavenSession.Query<Section>()
					.OrderByDescending(sec => sec.Position)
					.Select(sec => sec.Position)
					.FirstOrDefault() + 1;
			}
			RavenSession.Store(section);

			OutputCacheManager.RemoveItems(MVC.Section.Name);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult Delete(string id)
		{
			var section = RavenSession.Load<Section>(id);
			if (section == null)
				return NotFound("Section does not exist.");

			RavenSession.Delete(section);

			OutputCacheManager.RemoveItems(MVC.Section.Name);

			if (Request.IsAjaxRequest())
			{
				return Json(new { Success = true });
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult SetPosition(string id, int newPosition)
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

			OutputCacheManager.RemoveItems(MVC.Section.Name);

			return Json(new { success = true });
		}
	}
}
*/
