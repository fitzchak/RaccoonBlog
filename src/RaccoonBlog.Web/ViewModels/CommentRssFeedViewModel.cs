using System;

namespace RaccoonBlog.Web.ViewModels
{
	public class CommentRssFeedViewModel
	{
		public string CommentId { get; set; }
		public string Body { get; set; }
		public string Author { get; set; }
		public string CreatedAt { get; set; }

		public int PostId { get; set; }
		public string PostTitle { get; set; }
		public string PostSlug { get; set; }

		public string Title { get { return string.Format("{1} commented on {0}", PostTitle, Author); }}
	}
}