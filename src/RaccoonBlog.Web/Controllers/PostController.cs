using System;
using System.Linq;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
    public partial class PostController : AbstractController
    {
        public ActionResult Tag(string slug)
        {
            RavenQueryStatistics stats;
            var posts = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
                .Where(post => post.Tags.Any(postTag => postTag == slug))
                .OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }

        public ActionResult RedirectLegacyPost(int year, int month, int day, string slug)
        {
            var postQuery = Session.Query<Post>()
                .WhereIsPublicPost()
                .Where(post1 => post1.LegacySlug == slug &&
                        (post1.PublishAt.Year == year && post1.PublishAt.Month == month && post1.PublishAt.Day == day));

            var post = postQuery.FirstOrDefault();
            if (post == null)
                return HttpNotFound();

            var postReference = post.MapTo<PostReference>();
            return RedirectToActionPermanent("Details", new { Id = postReference.DomainId, postReference.Slug });
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