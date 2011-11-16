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

        [Display(Name = "Twitter Nick")]
        public string TwitterNick { get; set; }

        [Display(Name = "Related Twitter Nick", Prompt = "This is a nick of someone that you would want to recommend the user to follow.")]
        public string RelatedTwitterNick { get; set; }
    }
}
