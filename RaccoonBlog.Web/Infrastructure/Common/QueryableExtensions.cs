using System;
using System.Linq;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class QueryableExtensions
	{
		public static IQueryable<T> Paging<T>(this IQueryable<T> query, int currentPage, int defaultPage, int pageSize)
		{
			return query
				.Skip((currentPage - defaultPage)*pageSize)
				.Take(pageSize);
		}

		public static IQueryable<Post> WhereIsPublicPost(this IQueryable<Post> query)
		{
			return query
				.Where(post => post.PublishAt < DateTimeOffset.Now.AsMinutes() && post.IsDeleted == false);
		}
	}
}