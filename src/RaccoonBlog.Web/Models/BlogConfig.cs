using System;

namespace RaccoonBlog.Web.Models
{
	public class BlogConfig
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string CustomCss { get; set; }
		public string Copyright { get; set; }
		public string AkismetKey { get; set; }
		public int MinNumberOfPostForSignificantTag { get; set; }

        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }

		public int NumberOfDayToCloseComments { get; set; }
	}
}
