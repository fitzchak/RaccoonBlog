using System;

namespace RaccoonBlog.Web.Infrastructure.Indexes
{
	public class TagCount
	{
		public string Name { get; set; }
		public int Count { get; set; }
		public DateTimeOffset LastSeenAt { get; set; }
	}
}
