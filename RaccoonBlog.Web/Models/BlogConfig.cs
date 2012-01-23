using System;
using System.ComponentModel.DataAnnotations;
using RaccoonBlog.Web.Helpers.Validation;

namespace RaccoonBlog.Web.Models
{
	public class BlogConfig : Model
	{
		[Required]
		[Display(Name = "Blog title")]
		public string Title { get; set; }

		[Required]
		[Display(Name = "Owner Email")]
		public string OwnerEmail { get; set; }

		[Display(Name = "Slogan")]
		public string Subtitle { get; set; }

		[Required]
		[Display(Name = "Custom CSS")]
		public string CustomCss { get; set; }

		[Display(Name = "Copyright string")]
		public string Copyright { get; set; }

		[Display(Name = "Akismet Key")]
		public string AkismetKey { get; set; }

		[Display(Name = "Google-Analytics Key")]
		public string GoogleAnalyticsKey { get; set; }

		[Required]
		[NonEmptyGuid]
		[Display(Name = "RssFuturePostsKey")]
		public Guid RssFuturePostsKey { get; set; }

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

		public static BlogConfig New()
		{
			return new BlogConfig
			       	{
						Id = "Blog/Config",
			       		RssFuturePostsKey = Guid.NewGuid(),
						RssFutureDaysAllowed = 0,
						CustomCss = "hibernatingrhinos"
			       	};
		}

		public static BlogConfig NewDummy()
		{
			return new BlogConfig
			{
				Id = "Blog/Config/Dummy",
			};
		}
	}
}