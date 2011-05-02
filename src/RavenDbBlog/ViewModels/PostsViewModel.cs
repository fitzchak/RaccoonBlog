using System;
using System.Collections.Generic;
using System.Web.Mvc;
using RavenDbBlog.Controllers;

namespace RavenDbBlog.ViewModels
{
    public class PostsViewModel
    {
        public bool HasNextPage { get { return CurrentPage*AbstractController.PageSize < PostsCount; } }
        public bool HasPrevPage { get { return CurrentPage*AbstractController.PageSize > AbstractController.PageSize*AbstractController.DefaultPage; } }

        public int CurrentPage { get; set; }
        public int PostsCount { get; set; }

        public IList<PostSummary> Posts { get; set; }

        public class PostSummary
        {
            public int Id { get; set; }
            public MvcHtmlString Title { get; set; }
            public string Slug { get; set; }
            public MvcHtmlString Body { get; set; }
            public ICollection<string> Tags { get; set; }
            public DateTimeOffset CreatedAt { get; set; }
            public DateTimeOffset PublishedAt { get; set; }
            public int CommentsCount { get; set; }
        }
    }
}