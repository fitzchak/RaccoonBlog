using System;
using System.Collections.Generic;

namespace RavenDbBlog.ViewModels
{
    public class PostsAdminViewModel
    {
        public IList<PostSummaryJson> PostsJson { get; set; }

        public class PostSummaryJson
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public DateTimeOffset PublishAt { get; set; }
            public bool IsPublished { get; set; }
        }
    }
}