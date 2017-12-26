using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using RaccoonBlog.Web.Infrastructure.Common;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace RaccoonBlog.Web.Controllers
{
	public class PostsController : AggresivelyCachingRacconController
	{
		public async Task<ActionResult> Index()
		{
			ViewBag.IsHomePage = CurrentPage == DefaultPage;

		    var posts = await RavenSession.Query<Post>()
				.Include(x => x.AuthorId)
				.Statistics(out var stats)
				.WhereIsPublicPost()
				.OrderByDescending(post => post.PublishAt)
				.Paging(CurrentPage, DefaultPage, PageSize)
				.ToListAsync();

			return await ListView(stats.TotalResults, posts);
		}

		public async Task<ActionResult> Tag(string slug)
		{
			var posts = await RavenSession.Query<Post>()
				.Include(x => x.AuthorId)
				.Statistics(out var stats)
				.WhereIsPublicPost()
				.Where(post => post.TagsAsSlugs.Any(postTag => postTag == slug))
				.OrderByDescending(post => post.PublishAt)
				.Paging(CurrentPage, DefaultPage, PageSize)
				.ToListAsync();

			return await ListView(stats.TotalResults, posts);
		}

		public async Task<ActionResult> Series(int seriesId, string seriesSlug)
	    {
			var serie = RavenSession
				.Query<Posts_Series.Result, Posts_Series>()
				.FirstOrDefault(x => x.SerieId == seriesId);

			if (serie == null)
                return NotFound();

            var posts = await RavenSession.Query<Post>()
                .Include(x => x.AuthorId)
				.Statistics(out var stats)
				.WhereIsPublicPost()
				.Where(p => p.Id.In(serie.Posts.Select(x => x.Id)))
                .OrderByDescending(p => p.PublishAt)
                .Paging(CurrentPage, DefaultPage, PageSize)
                .ToListAsync();

            return await ListView(stats.TotalResults, posts);
	    }

	    public async Task<ActionResult> Archive(int year, int? month, int? day)
	    {
	        var postsQuery = RavenSession.Query<Post>()
	            .Include(x => x.AuthorId)
	            .Statistics(out var stats)
	            .WhereIsPublicPost()
	            .Where(post => post.PublishAt.Year == year);

	        if (month != null)
	            postsQuery = postsQuery.Where(post => post.PublishAt.Month == month.Value);

	        if (day != null)
	            postsQuery = postsQuery.Where(post => post.PublishAt.Day == day.Value);

	        var posts = await postsQuery.OrderByDescending(post => post.PublishAt)
	            .Paging(CurrentPage, DefaultPage, PageSize)
	            .ToListAsync();

	        return await ListView(stats.TotalResults, posts);
	    }

	    private async Task<ActionResult> ListView(int count, IList<Post> posts)
		{
		    ViewBag.ChangeViewStyle = true;

			var summaries = posts.MapTo<PostsViewModel.PostSummary>();

			var serieTitles = summaries
				.Select(x => TitleConverter.ToSeriesTitle(x.Title))
				.Where(x => string.IsNullOrEmpty(x) == false)
				.Distinct()
				.ToList();

			var series = await RavenSession
				.Query<Posts_Series.Result, Posts_Series>()
				.Where(x => x.Series.In(serieTitles) && x.Count > 1)
				.ToListAsync();

			foreach (var post in posts)
			{
				var postSummary = summaries.First(x => x.Id == RavenIdResolver.Resolve(post.Id));
				postSummary.IsSerie = series.Any(x => string.Equals(x.Series, TitleConverter.ToSeriesTitle(postSummary.Title), StringComparison.OrdinalIgnoreCase));

				if (string.IsNullOrWhiteSpace(post.AuthorId))
					continue;

				var author = await RavenSession.LoadAsync<User>(post.AuthorId);
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