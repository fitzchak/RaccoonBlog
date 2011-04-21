using System.Linq;
using Raven.Client;
using RavenDbBlog.Core.Models;

namespace RavenDbBlog.Controllers
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