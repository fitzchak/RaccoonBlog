using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class PostComments_CreationDate : AbstractIndexCreationTask<PostComments, PostCommentsIdentifier>
	{
		public PostComments_CreationDate()
		{
			Map = postComments => from postComment in postComments
								  from comment in postComment.Comments
								  select new { comment.CreatedAt, CommentId = comment.Id, PostCollectionId = postComment.Id, postComment.PostId };

			Store(x =>x.CreatedAt, FieldStorage.Yes);
			Store(x =>x.CommentId, FieldStorage.Yes);
			Store(x =>x.PostId, FieldStorage.Yes);
		}
	}
}