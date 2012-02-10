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

		public string AkismetKey { get; set; }
		public string GoogleAnalyticsKey { get; set; }

		public string FuturePostsEncryptionKey { get; set; }
		public string FuturePostsEncryptionIV { get; set; }

		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }

		public int MinNumberOfPostForSignificantTag { get; set; }
		public int NumberOfDayToCloseComments { get; set; }

		public static BlogConfig New()
		{
			return new BlogConfig
			       	{
						Id = "Blog/Config",
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
