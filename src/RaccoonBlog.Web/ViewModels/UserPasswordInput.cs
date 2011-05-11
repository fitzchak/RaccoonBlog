using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RaccoonBlog.Web.Helpers.Attributes;

namespace RaccoonBlog.Web.ViewModels
{
	public class UserPasswordInput
	{
		[HiddenInput]
		public int Id { get; set; }

		[Required]
		[Display(Name = "Old Password")]
		[DataType(DataType.Password)]
		public string OldPass { get; set; }

		[Required]
		[Display(Name = "New Password")]
		[DataType(DataType.Password)]
        [ValidatePasswordLength]
		public string NewPass { get; set; }

		[Required]
		[Display(Name = "New Password Confirmation")]
		[DataType(DataType.Password)]
        [Compare("NewPass", ErrorMessage = "The password and confirmation password do not match.")]
		public string NewPassConfirmation { get; set; }
	}
}
