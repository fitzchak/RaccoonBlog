namespace RaccoonBlog.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime.Misc;
    using RaccoonBlog.Web.Helpers;
    using RaccoonBlog.Web.Infrastructure.Common;

    public class SeriesPostsViewModel
    {
        public SeriesPostsViewModel()
        {
            SeriesInfo = new List<SeriesInfo>();
        }
        public int CurrentPage { get; set; }
        public int PostsCount { get; set; }
        public int PageSize { get; set; }
        public IList<SeriesInfo> SeriesInfo { get; set; }
    }

    public class SeriesInfo
    {
        public SeriesInfo()
        {
            PostsInSeries = new ListStack<PostInSeries>();
        }

        public int SeriesId { get; set; }
        public string SeriesTitle { get; set; }
        public IList<PostInSeries> PostsInSeries { get; set; }

        private string seriesSlug;
        public string SeriesSlug
        {
            get { return seriesSlug ?? (seriesSlug = SlugConverter.TitleToSlug(SeriesTitle)); }
            set { seriesSlug = value; }
        }
    }

    public class PostInSeries
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public DateTimeOffset PublishAt { get; set; }
        public string Title { get; set; }
    }
}