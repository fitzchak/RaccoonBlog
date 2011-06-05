using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.Web.Infrastructure
{
	public static class DocumentSessionExtensions
	{
		public static PostReference GetNextPrevPost(this IDocumentSession session, Post compareTo, bool isNext)
		{
			var queryable = session.Query<Post>()
				.WhereIsPublicPost();

			if (isNext)
			{
				queryable = queryable
					.Where(post => post.PublishAt > compareTo.PublishAt)
					.OrderBy(post => post.PublishAt);
			}
			else
			{
				queryable = queryable
					.Where(post => post.PublishAt < compareTo.PublishAt)
					.OrderByDescending(post => post.PublishAt);
			}
			
			var postReference = queryable
			  .Select(p => new PostReference{ Id = p.Id, Title = p.Title })
			  .FirstOrDefault();

			if (postReference == null)
				return null;

			return postReference;
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
