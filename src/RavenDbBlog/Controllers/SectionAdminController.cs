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
    }
}