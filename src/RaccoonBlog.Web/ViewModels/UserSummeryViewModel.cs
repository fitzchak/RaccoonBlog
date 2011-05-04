using System.ComponentModel.DataAnnotations;

namespace RaccoonBlog.Web.ViewModels
{
    public class UserSummeryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Enabled?")]
		public bool Enabled { get; set; }
    }
}