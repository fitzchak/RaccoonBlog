using System;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class PostCommentsIdentifier
	{
		public DateTimeOffset CreatedAt { get; set; }
		public int CommentId { get; set; }
		public string PostCollectionId { get; set; }
		public string PostId { get; set; }
	}
}