namespace RaccoonBlog.Web.ViewModels
{
	public class BlogConfigViewModel
	{
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string CustomCss { get; set; }
		public string Copyright { get; set; }
		public string AkismetKey { get; set; }
		public string GoogleAnalyticsKey { get; set; }

		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
	}
}