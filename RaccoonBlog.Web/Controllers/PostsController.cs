using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.Web.Controllers
{
    public class PostsController : AggresivelyCachingRacconController
	{
		public ActionResult Index()
		{
			RavenQueryStatistics stats;
			var posts = RavenSession.Query<Post>()
				.Include(x => x.AuthorId)
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
			var posts = RavenSession.Query<Post>()
				.Include(x => x.AuthorId)
				.Statistics(out stats)
				.WhereIsPublicPost()
				.Where(post => post.TagsAsSlugs.Any(postTag => postTag == slug))
				.OrderByDescending(post => post.PublishAt)
				.Paging(CurrentPage, DefaultPage, PageSize)
				.ToList();

			return ListView(stats.TotalResults, posts);
		}

	    public ActionResult Series(int seriesId, string seriesSlug)
	    {
            var post = RavenSession
                .Include<Post>(x => x.CommentsId)
                .Include(x => x.AuthorId)
                .Load(seriesId);

            if (post == null)
                return HttpNotFound();

		    var seriesTitle = TitleConverter.ToSeriesTitle(post.Title);

            RavenQueryStatistics stats;
            var posts = RavenSession.Query<Post>()
                .Include(x => x.AuthorId)
				.Statistics(out stats)
				.WhereIsPublicPost()
				.Where(p => p.Title.StartsWith(seriesTitle))
                .OrderByDescending(p => p.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToList();

            return ListView(stats.TotalResults, posts);
	    }

		public ActionResult Archive(int year, int? month, int? day)
		{
			RavenQueryStatistics stats;
			var postsQuery = RavenSession.Query<Post>()
				.Include(x => x.AuthorId)
				.Statistics(out stats)
				.WhereIsPublicPost()
				.Where(post => post.PublishAt.Year == year);
			
			if (month != null)
				postsQuery = postsQuery.Where(post => post.PublishAt.Month == month.Value);

			if (day != null)
				postsQuery = postsQuery.Where(post => post.PublishAt.Day == day.Value);

			var posts = 
				postsQuery.OrderByDescending(post => post.PublishAt)
				.Paging(CurrentPage, DefaultPage, PageSize)
				.ToList();

			return ListView(stats.TotalResults, posts);
		}

		private ActionResult ListView(int count, IList<Post> posts)
		{
		    ViewBag.ChangeViewStyle = true;

			var summaries = posts.MapTo<PostsViewModel.PostSummary>();

			var serieTitles = summaries
				.Select(x => TitleConverter.ToSeriesTitle(x.Title))
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Distinct()
				.ToList();

			var series = RavenSession
				.Query<Posts_Series.Result, Posts_Series>()
				.Where(x => x.Series.In(serieTitles) && x.Count > 1)
				.ToList();

			foreach (var post in posts)
			{
				var postSummary = summaries.First(x => x.Id == RavenIdResolver.Resolve(post.Id));
				postSummary.IsSerie = series.Any(x => string.Equals(x.Series, TitleConverter.ToSeriesTitle(postSummary.Title), StringComparison.OrdinalIgnoreCase));

				if (string.IsNullOrWhiteSpace(post.AuthorId))
					continue;

				var author = RavenSession.Load<User>(post.AuthorId);
				if (author == null)
					continue;

				
				postSummary.Author = author.MapTo<PostsViewModel.PostSummary.UserDetails>();
			}

			

			return View("List", new PostsViewModel
			{
                PageSize = BlogConfig.PostsOnPage,
				CurrentPage = CurrentPage,
				PostsCount = count,
				Posts = summaries
			});
		}

		protected override TimeSpan CacheDuration
		{
			get { return TimeSpan.FromMinutes(3); }
		}
	}
}