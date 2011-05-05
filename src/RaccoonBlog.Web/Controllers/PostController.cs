using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using RaccoonBlog.Web.Common;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Linq;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Controllers
{
	public class PostController : AbstractController
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


        public ActionResult Archive(int year, int? month, int? day)
        {
            RavenQueryStatistics stats;
            var postsQuery = Session.Query<Post>()
                .Statistics(out stats)
                .WhereIsPublicPost()
				.Where(post => post.PublishAt.Year == year);
			
			if(month != null)
				postsQuery = postsQuery.Where(post => post.PublishAt.Month == month.Value);

			if(day != null)
				postsQuery = postsQuery.Where(post => post.PublishAt.Day == day.Value);

            var posts = 
				postsQuery.OrderByDescending(post => post.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
        }

		private ActionResult ListView(int count, IEnumerable<Post> posts)
		{
			return View("List", new PostsViewModel
			{
				CurrentPage = CurrentPage,
				PostsCount = count,
				Posts = posts.MapTo<PostsViewModel.PostSummary>()
			});
		}
    }
}