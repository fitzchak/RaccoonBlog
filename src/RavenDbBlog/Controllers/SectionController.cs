using System.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public class SectionController : AbstractController
    {
        [ChildActionOnly]
        public ActionResult List()
        {
            var sections = Session.Query<Section>()
                .Where(s => s.IsActive)
                .ToList();

            return View(sections.MapTo<SectionDetails>());
        }
    }
}