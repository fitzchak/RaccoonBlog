using System;
using System.Collections.Generic;

namespace RavenDbBlog.ViewModels
{
    public class PostsViewModel
    {
        public IList<Post> Posts { get; set; }

        public class Post
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string Slug { get; set; }
            public ICollection<string> Tags { get; set; }
            public DateTimeOffset PostedAt { get; set; }
            public DateTimeOffset PublishedAt { get; set; }
            public int CommentsCount { get; set; }
        }
    }
}