using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace RaccoonBlog.Web.Areas.Admin.ViewModels
{
	public class LoginInput
	{
		[Required]
		[Display(Name = "Email")]
		[Email]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[HiddenInput]
		public string ReturnUrl { get; set; }
	}
}
