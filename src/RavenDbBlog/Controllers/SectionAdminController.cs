using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class SectionAdminController : AdminController
    {
        public ActionResult List()
        {
            var sections = Session.Query<Section>()
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
            if (ModelState.IsValid)
            {
                var section = Session.Load<Section>(input.Id) ?? new Section();
                input.MapPropertiestoInstance(section);
                Session.Store(section);
                return RedirectToAction("List");
            }
            return View("Edit", input);
        }
    }
}