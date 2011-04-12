using System;
using System.Collections.Generic;

namespace RavenDbBlog.ViewModels
{
    public class PostViewModel
    {
        public PostReference PreviousPost { get; set; }
        public PostReference NextPost { get; set; }

        public IList<Post> Posts { get; set; }
        public IList<Comment> Comments { get; set; }

        public bool IsCommentClosed { get; set; }

        public class Comment
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string PostedBy { get; set; }
            public string OwnerWebsite { get; set; }    // Look for HTML injection.
            public DateTimeOffset PostedAt { get; set; }
        }

        public class Post
        {
            public string Title { get; set; }
            public string Body { get; set; }

            public DateTimeOffset PostedAt { get; set; }
            public DateTimeOffset PublishedAt { get; set; }

            public ICollection<string> Tags { get; set; }
        }

        public class PostReference
        {
            public string Slug { get; set; }
            public string Title { get; set; }
        }
    }
}
