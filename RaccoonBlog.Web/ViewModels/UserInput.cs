using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace RaccoonBlog.Web.ViewModels
{
	public class UserInput
	{
		[HiddenInput]
		public string Id { get; set; }

		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Required]
		[Display(Name = "Email")]
		[Email]
		public string Email { get; set; }

        [Display(Name = "Phone")]
	    public string Phone { get; set; }

		[Display(Name = "Is Enabled")]
		public bool Enabled { get; set; }

		[Display(Name = "Twitter Nick")]
		public string TwitterNick { get; set; }

		[Display(Name = "Related Twitter Nick", Prompt = "This is a nick of someone that you want to recommend the user to follow.")]
		public string RelatedTwitterNick { get; set; }

		[Display(Name = "Related  Description")]
		public string RelatedTwitNickDes { get; set; }

		public bool IsNewUser()
		{
			return string.IsNullOrEmpty(Id);
		}
	}
}
