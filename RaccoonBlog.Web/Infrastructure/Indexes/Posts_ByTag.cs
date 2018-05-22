using System;
using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client.Documents.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class Posts_ByTag : AbstractIndexCreationTask<Post, Posts_ByTag.Query>
	{
		public class Query
		{
			public string Id { get; set; }

			public string Title { get; set; }

			public string[] Tags { get; set; }

			public DateTimeOffset PublishAt { get; set; }
		}

		public Posts_ByTag()
		{
			Map = posts => from post in posts 
			               where post.PublishAt != null 
			               select new
			               {
				               Tags = post.Tags, 
				               PublishAt = post.PublishAt,
			               };
		}
	}
}