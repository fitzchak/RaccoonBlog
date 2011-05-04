using System.Linq;
using RaccoonBlog.Web.Models;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
{
	public static class Queries
	{
		public static User GetUserByEmail(this IDocumentSession session,  string email)
		{
			return session.Query<User>()
				.Where(u => u.Email == email)
				.FirstOrDefault();
		}
	}
}