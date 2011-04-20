using System.ComponentModel.DataAnnotations;

namespace RavenDbBlog.ViewModels
{
    public class LoginInput
    {
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}