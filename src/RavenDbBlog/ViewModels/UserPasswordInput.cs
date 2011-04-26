using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
	public class UserPasswordInput
	{
		[HiddenInput]
		public int Id { get; set; }

		public string FullName { get; set; }

		[Required]
		[Display(Name = "Old Password")]
		[DataType(DataType.Password)]
		public string OldPass { get; set; }

		[Required]
		[Display(Name = "New Password")]
		[DataType(DataType.Password)]
		public string NewPass { get; set; }

		[Required]
		[Display(Name = "New Password Confirmation")]
		[DataType(DataType.Password)]
		public string NewPassConfirmation { get; set; }
	}
}