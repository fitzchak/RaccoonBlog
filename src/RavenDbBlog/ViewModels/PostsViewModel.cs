using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
    public class PostsViewModel
    {
        public IList<PostInternalViewModel> Posts { get; set; }

        public class PostInternalViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public MvcHtmlString Body { get; set; }
            public string Slug { get; set; }
            public ICollection<string> Tags { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public DateTimeOffset PublishedAt { get; set; }
            public int CommentsCount { get; set; }
        }
    }
}