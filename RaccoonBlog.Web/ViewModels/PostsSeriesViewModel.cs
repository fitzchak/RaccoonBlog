namespace RaccoonBlog.Web.ViewModels
{
    using RaccoonBlog.Web.Infrastructure.Indexes;

    public class RecentSeriesViewModel
    {
        public int SeriesId { get; set; }
        public string SeriesSlug { get; set; }
		public string SeriesTitle { get; set; }
        public int PostsCount { get; set; }
        public Posts_Series.PostInformation PostInformation { get; set; }
    }
}