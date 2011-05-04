using System;
using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;

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


		[ChildActionOnly]
		public ActionResult TagsList()
		{
			var mostRecentTag = new DateTimeOffset(DateTimeOffset.Now.Year - 2,
												   DateTimeOffset.Now.Month,
												   1, 0, 0, 0,
												   DateTimeOffset.Now.Offset);

			var tagCounts = Session.Query<TagCount, Tags_Count>()
				.Where(x => x.Count > 20 && x.LastSeenAt > mostRecentTag)
				.OrderBy(x => x.Name)
				.As<TempTagCount>();
			var tags = tagCounts
				.ToList();

			return View(tags.MapTo<TagsListViewModel>());
		}

		[ChildActionOnly]
		public ActionResult ArchivesList()
		{
			var dates = Session.Query<PostCountByMonth, Posts_ByMonthPublished_Count>()
				.OrderByDescending(x => x.Year)
				.ThenByDescending(x => x.Month)
				.ToList();

			return View(dates);
		}
    }
}