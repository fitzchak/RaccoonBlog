using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RaccoonBlog.Web.Controllers;

namespace RaccoonBlog.Web.ViewModels
{
	public class PostsViewModel
	{
		public bool HasNextPage
		{
			get { return CurrentPage*PageSize < PostsCount; }
		}

		public bool HasPrevPage
		{
			get { return CurrentPage*PageSize > PageSize*RaccoonController.DefaultPage; }
		}

		public int CurrentPage { get; set; }
		public int PostsCount { get; set; }
	    public int PageSize { get; set; }

		public IList<PostSummary> Posts { get; set; }

		public class PostSummary
		{
			public string Id { get; set; }
			public string Title { get; set; }
			public string Slug { get; set; }
			public MvcHtmlString Body { get; set; }
			public ICollection<TagDetails> Tags { get; set; }
			public DateTimeOffset CreatedAt { get; set; }
			public DateTimeOffset PublishedAt { get; set; }
			public int CommentsCount { get; set; }
			public UserDetails Author { get; set; }

			public bool IsSerie { get; set; }

			public class UserDetails
			{
				public string TwitterNick { get; set; }
				public string RelatedTwitterNick { get; set; }
				public string RelatedTwitNickDes { get; set; }
			}
		}
	}
}