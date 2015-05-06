using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.ViewModels
{
	public class ContactMeViewModel
	{
		public ContactMeViewModel(User user)
		{
			if (user == null)
				return;

			FullName = user.FullName;
			Email = user.Email;
			Phone = user.Phone;
		}

		public string FullName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
	}
}