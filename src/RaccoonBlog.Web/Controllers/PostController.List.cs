using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;
using System.Web.Mvc;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.Controllers
{
    public partial class PostController
    {
        public ActionResult List()
        {
            RavenQueryStatistics stats;
            var posts = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
                .OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }

        private ActionResult ListView(int count, ICollection<Post> posts)
        {
            return View("List", new PostsViewModel
            {
                CurrentPage = CurrentPage,
                PostsCount = count,
                Posts = posts.MapTo<PostsViewModel.PostSummary>()
            });
        }

        public ActionResult ArchiveYear(int year)
        {
            RavenQueryStatistics stats;
            var posts = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
                .Where(post => post.PublishAt.Year == year)
                .OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }

        public ActionResult ArchiveYearMonth(int year, int month)
        {
            RavenQueryStatistics stats;
            var posts = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
                .Where(post => post.PublishAt.Year == year && post.PublishAt.Month == month)
                .OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }

        public ActionResult ArchiveYearMonthDay(int year, int month, int day)
        {
            RavenQueryStatistics stats;
            var posts = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
                .Where(post => post.PublishAt.Year == year && post.PublishAt.Month == month && post.PublishAt.Day == day)
                .OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }
    }
}