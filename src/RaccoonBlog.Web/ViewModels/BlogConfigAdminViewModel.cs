using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.ViewModels
{
	public class BlogConfigAdminViewModel
	{
		[Display(Name = "Title")]
		public string Title { get; set; }

		[Display(Name = "Subtitle")]
		public string Subtitle { get; set; }

		[Display(Name = "CustomCss")]
		public string CustomCss { get; set; }

		[Display(Name = "Copyright")]
		public string Copyright { get; set; }

		[Display(Name = "AkismetKey")]
		public string AkismetKey { get; set; }

		[Display(Name = "GoogleAnalyticsKey")]
		public string GoogleAnalyticsKey { get; set; }

		[Display(Name = "RssFuturePostsKey")]
		public string RssFuturePostsKey { get; set; }

		[Display(Name = "RssFutureDaysAllowed")]
		public int RssFutureDaysAllowed { get; set; }

		[Display(Name = "MetaDescription")]
		public string MetaDescription { get; set; }

		[Display(Name = "MetaKeywords")]
		public string MetaKeywords { get; set; }

		[Display(Name = "MinNumberOfPostForSignificantTag")]
		public int MinNumberOfPostForSignificantTag { get; set; }

		[Display(Name = "NumberOfDayToCloseComments")]
		public int NumberOfDayToCloseComments { get; set; }
	}
}