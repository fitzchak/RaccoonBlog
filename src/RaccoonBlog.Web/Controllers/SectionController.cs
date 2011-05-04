using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Controllers
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