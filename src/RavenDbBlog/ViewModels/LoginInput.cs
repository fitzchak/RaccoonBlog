using System.ComponentModel.DataAnnotations;
using System.Security;

namespace RavenDbBlog.ViewModels
{
    public class LoginInput
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }


        [Required]
        [Display(Name = "Password")]
        public SecureString Password { get; set; }
    }
}