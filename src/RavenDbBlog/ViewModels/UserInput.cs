using System.ComponentModel.DataAnnotations;

namespace RavenDbBlog.ViewModels
{
    public class UserInput
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}