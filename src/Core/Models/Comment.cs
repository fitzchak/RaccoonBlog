using System;
using System.Collections.Generic;

namespace RavenDbBlog.Core.Models
{
    public class CommentsCollection
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public List<Comment> Comments { get; set; }

        public class Comment
        {
            public string Body { get; set; }
            public string Author { get; set; }
            public string Email { get; set; }
            public string Url { get; set; }

            public bool Important { get; set; }
            public bool IsSpam { get; set; }

            public DateTimeOffset CreatedAt { get; set; }
        }
    }
}