using System;

namespace RavenDbBlog.ViewModels
{
    public class PostSummaryJson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public DateTimeOffset PublishAt { get; set; }
        public bool IsPublished { get; set; }
    }
}