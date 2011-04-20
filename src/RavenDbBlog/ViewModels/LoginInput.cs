using System.ComponentModel.DataAnnotations;
using System.Security;

namespace RavenDbBlog.ViewModels
{
    public class LoginInput
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Password")]
        public SecureString Password { get; set; }
    }
}