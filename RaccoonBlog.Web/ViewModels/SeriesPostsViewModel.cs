namespace RaccoonBlog.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime.Misc;

    public class SeriesPostsViewModel
    {
        public SeriesPostsViewModel()
        {
            PostsInSeries = new ListStack<PostInSeries>();
        }
        public string Series { get; set; }
        public IList<PostInSeries> PostsInSeries { get; set; }
    }

    public class PostInSeries
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTimeOffset PublishAt { get; set; }
    }
}