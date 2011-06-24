using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using RaccoonBlog.Web.Infrastructure.Common;

namespace RaccoonBlog.Web.Controllers
{
	public class SectionController : AbstractController
	{
		[ChildActionOnly]
		public ActionResult FuturePosts()
		{
			RavenQueryStatistics stats;
			var futurePosts = Session.Query<Post>()
				.Statistics(out stats)
				.Where(x => x.PublishAt > DateTimeOffset.Now.AsMinutes() && x.IsDeleted == false)
				.Select(x => new Post {Title = x.Title, PublishAt = x.PublishAt})
				.OrderBy(x => x.PublishAt)
				.Take(5)
				.ToList();

			return View(
				new FuturePostsViewModel
				{
					TotalCount = stats.TotalResults,
					Posts = futurePosts.MapTo<FuturePostViewModel>()
				});
		}

		[ChildActionOnly]
		public ActionResult List()
		{
			if (true.Equals(HttpContext.Items["CurrentlyProcessingException"]))
				return View(new SectionDetails[0]);

			var sections = Session.Query<Section>()
				.Where(s => s.IsActive)
				.OrderBy(x => x.Position)
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

			var blogConfig = Session.Load<BlogConfig>("Blog/Config");

			var tagCounts = Session.Query<TagCount, Tags_Count>()
				.Where(x => x.Count > blogConfig.MinNumberOfPostForSignificantTag && x.LastSeenAt > mostRecentTag)
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
