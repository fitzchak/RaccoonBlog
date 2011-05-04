using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.Web.Common
{
	public static class DocumentSessionExtensions
	{
		public static PostReference GetPostReference(this IDocumentSession session, Expression<Func<Post, bool>> expression)
		{
			var postReference = session.Query<Post>()
			  .Where(expression)
			  .OrderByDescending(post => post.PublishAt)
			  .Select(p => new { p.Id, p.Title })
			  .FirstOrDefault();

			if (postReference == null)
				return null;

			return postReference.DynamicMapTo<PostReference>();
		}

		public static User GetCurrentUser(this IDocumentSession session)
		{
			if (HttpContext.Current.Request.IsAuthenticated == false)
				return null;

			var email = HttpContext.Current.User.Identity.Name;
			var user = session.GetUserByEmail(email);
			return user;
		}

		public static User GetUserByEmail(this IDocumentSession session, string email)
		{
			return session.Query<User>()
				.Where(u => u.Email == email)
				.FirstOrDefault();
		}
	}
}