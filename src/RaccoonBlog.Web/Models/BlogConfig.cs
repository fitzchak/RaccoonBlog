using System;
using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.Models
{
	public class BlogConfig
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "Blog title")]
		public string Title { get; set; }

		[Display(Name = "Slogan")]
		public string Subtitle { get; set; }

		[Required]
		[Display(Name = "Custom CSS")]
		public string CustomCss { get; set; }

		[Display(Name = "Copyright string")]
		public string Copyright { get; set; }

		public string AkismetKey { get; set; }
		public string GoogleAnalyticsKey { get; set; }
		public Guid RssFuturePostsKey { get; set; }
		public int RssFutureDaysAllowed { get; set; }

		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }

		public int MinNumberOfPostForSignificantTag { get; set; }
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
