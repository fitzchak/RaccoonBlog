using System;
using System.Collections.Generic;
using System.Linq;

using RaccoonBlog.Web.Models;

using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
    using RaccoonBlog.Web.Helpers;

    public class Posts_Series : AbstractIndexCreationTask<Post, Posts_Series.Result>
	{
		public class Result
		{
			public Result()
			{
				Posts = new List<PostInformation>();
			}

			public int SerieId { get; set; }

		    public string Series { get; set; }

		    public List<PostInformation> Posts { get; set; }

			public int Count { get; set; }

			public DateTimeOffset MaxDate { get; set; }

			public DateTimeOffset MinDate { get; set; }

			public int Days { get; set; }
		}

		public class PostInformation
		{
			public string Id { get; set; }

		    public string Title { get; set; }

		    public DateTimeOffset PublishAt { get; set; }
		}

		public Posts_Series()
		{
			Map = posts => from p in posts
						   let parts = p.Title.Split(':')
						   where parts.Length > 1
						   let series = parts[0].Trim().ToLower()
						   select new
						   {
							   Series = series,
							   SerieId = p.Id.Split('/')[1],
							   Posts = new[] { new { p.Id, p.Title, p.PublishAt } },
							   Count = 1,
							   MaxDate = (DateTimeOffset)p.PublishAt,
							   MinDate = (DateTimeOffset)p.PublishAt,
							   Days = 1
						   };

			Reduce = results => from r in results
								group r by r.Series into g
								let maxDate = g.Select(x => x.MaxDate).Max()
								let minDate = g.Select(x => x.MinDate).Min()
								select new
								{
									Series = g.Key,
									SerieId = g.Select(x => x.SerieId).OrderBy(x => x).First(),
									Posts = g.SelectMany(x => x.Posts),
									Count = g.Sum(x => x.Count),
									MaxDate = maxDate,
									MinDate = minDate,
									Days = (int)(maxDate - minDate).TotalDays
								};
		}
	}
}