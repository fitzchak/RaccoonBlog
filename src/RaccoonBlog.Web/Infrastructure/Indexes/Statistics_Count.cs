using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class Posts_Statistics : AbstractIndexCreationTask<PostComments, PostsStatistics>
	{
		public Posts_Statistics()
		{
			Map = postComments => from postComment in postComments
								  select new { PostCount = 1, CommentsCount = postComment.Comments.Count };

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