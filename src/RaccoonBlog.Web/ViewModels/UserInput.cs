using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
    public class UserInput
    {
        [HiddenInput]
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Is Enabled?")]
		public bool Enabled { get; set; }

        public bool IsNewUser()
        {
            return Id == 0;
        }
    }
}