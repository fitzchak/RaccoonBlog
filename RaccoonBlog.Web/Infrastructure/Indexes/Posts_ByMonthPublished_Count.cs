using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class Posts_ByMonthPublished_Count : AbstractIndexCreationTask<Post, Posts_ByMonthPublished_Count.ReduceResult>
	{
		public class ReduceResult
		{
			public int Year { get; set; }
			public int Month { get; set; }
			public int Count { get; set; }
		}

		public Posts_ByMonthPublished_Count()
		{
			Map = posts => from post in posts
						   select new {post.PublishAt.Year, post.PublishAt.Month, Count = 1};
			Reduce = results => from result in results
								group result by new {result.Year, result.Month}
								into g
								select new {g.Key.Year, g.Key.Month, Count = g.Sum(x => x.Count)};

			Sort(x=>x.Month, global::Raven.Abstractions.Indexing.SortOptions.Int);
			Sort(x => x.Year, global::Raven.Abstractions.Indexing.SortOptions.Int);
		}
	}
}
