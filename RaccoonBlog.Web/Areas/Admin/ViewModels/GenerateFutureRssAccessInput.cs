using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.Areas.Admin.ViewModels
{
	public class GenerateFutureRssAccessInput
	{
		[Display(Name = "User Name", Description = "Name of the user whom should get access")]
		public string User { get; set; }

		[Display(Name = "Token", Description = "The access token")]
		public string Token { get; set; }
	}
}