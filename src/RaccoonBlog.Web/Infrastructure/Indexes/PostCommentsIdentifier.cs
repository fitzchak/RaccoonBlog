using System;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class PostCommentsIdentifier
	{
		public DateTimeOffset CreatedAt { get; set; }
		public int CommentId { get; set; }
		public string PostCommentsId { get; set; }
		public string PostId { get; set; }
		public DateTimeOffset PostPublishAt { get; set; }
	}
}
