using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class Posts_Statistics : AbstractIndexCreationTask<PostComments, Posts_Statistics.ReduceResult>
	{
		public class ReduceResult
		{
			public int PostsCount { get; set; }
			public int CommentsCount { get; set; }
		}

		public Posts_Statistics()
		{
			Map = postComments => from postComment in postComments
								  select new { PostsCount = 1, CommentsCount = postComment.Comments.Count };

			Reduce = results => from result in results
								group result by "constant" into g
								select new
								{
									PostsCount = g.Sum(x => x.PostsCount),
									CommentsCount = g.Sum(x => x.CommentsCount)
								};
		}
	}
}
