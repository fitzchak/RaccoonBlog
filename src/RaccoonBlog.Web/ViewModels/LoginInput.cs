using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
    public class LoginInput
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Password")]
		[DataType(DataType.Password)]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }
}
