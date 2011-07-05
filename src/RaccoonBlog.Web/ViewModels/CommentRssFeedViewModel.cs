using System;

namespace RaccoonBlog.Web.ViewModels
{
	public class CommentRssFeedViewModel
	{
		public string Body { get; set; }
		public string Author { get; set; }
		public DateTimeOffset CreatedAt { get; set; }

		public PostSummary Post { get; set; }

		public class PostSummary
		{
			public int Id { get; set; }
			public string Title { get; set; }
		}
	}
}