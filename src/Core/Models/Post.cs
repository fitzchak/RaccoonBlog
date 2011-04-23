using System;

namespace RavenDbBlog.Core.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string LegacySlug { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        
        public string Author { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
		public bool SkipAutoReschedule { get; set; }
        public DateTimeOffset PublishAt { get; set; }

        public int CommentsCount { get; set; }

        public string CommentsId { get; set; }
    }
}