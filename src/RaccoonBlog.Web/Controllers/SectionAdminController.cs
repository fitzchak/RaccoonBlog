using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.Attributes;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
{
    public class SectionAdminController : AdminController
    {
        public ActionResult List()
        {
            var sections = Session.Query<Section>()
                .OrderBy(x => x.Position)
                .ToList();

            return View(sections.MapTo<SectionSummery>());
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View("Edit", new SectionInput());
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var section = Session.Load<Section>(id);
            if (section == null)
                return HttpNotFound("Section does not exist.");
            return View(section.MapTo<SectionInput>());
        }

        [HttpPost]
        public ActionResult Update(SectionInput input)
        {
        	if (!ModelState.IsValid)
        		return View("Edit", input);

        	var section = Session.Load<Section>(input.Id) ?? new Section();
        	input.MapPropertiesToInstance(section);
        	Session.Store(section);
        	return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var section = Session.Load<Section>(id);
            if (section == null)
                return HttpNotFound("Section does not exist.");
          
            Session.Delete(section);
            
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
            var section = Session.Load<Section>(id);
            if (section == null)
                return Json(new {success = false, message = string.Format("There is no post with id {0}", id)});

            if (section.Position == newPosition)
                return Json(new {success = false, message = string.Format("The {0} section has already this position", section.Title)});

            if (section.Position > newPosition)
            {
                var sections = Session.Query<Section>()
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
                var sections = Session.Query<Section>()
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
