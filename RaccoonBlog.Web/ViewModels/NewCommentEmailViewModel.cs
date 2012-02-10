using System;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
	public class NewCommentEmailViewModel
	{
		public int Id { get; set; }
		public string Author { get; set; }
		public string Url { get; set; }
		public string Email { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public MvcHtmlString Body { get; set; }
		public bool IsSpam { get; set; }

		public int PostId { get; set; }
		public string PostTitle { get; set; }
		public string PostSlug { get; set; }
		public string BlogName { get; set; }
		public string Key { get; set; }
		public string CommenterId { get; set; }
	}
}