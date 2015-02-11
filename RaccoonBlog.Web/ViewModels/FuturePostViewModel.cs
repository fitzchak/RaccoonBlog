using System;
using System.Collections.Generic;

using RaccoonBlog.Web.Infrastructure.Common;

namespace RaccoonBlog.Web.ViewModels
{
	public class FuturePostsViewModel
	{
		public int TotalCount { get; set; }
		public List<FuturePostViewModel> Posts { get; set; }

		public DateTimeOffset? LastPostDate { get; set; }
	}

	public class FuturePostViewModel
	{
		public string Title { get; set; }

		public DateTimeOffset PublishAt { get; set; }

		public string Time
		{
			get
			{
				var totalMinutes = (PublishAt - DateTimeOffset.Now).TotalMinutes;

				if (totalMinutes < 0)
					throw new InvalidOperationException(string.Format("Future post error: the post is already published. Post Id: {0}, PublishAt: {1}, Now: {2}", Title, PublishAt, DateTimeOffset.Now));

				return TimeConverter.DistanceOfTimeInWords(totalMinutes) + " from now";
			}
		}
	}
}
