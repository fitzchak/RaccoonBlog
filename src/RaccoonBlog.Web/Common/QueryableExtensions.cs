using System;
using System.Linq;
using System.Linq.Expressions;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;

namespace RaccoonBlog.Web.Controllers
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
                .Where(post => post.PublishAt < DateTimeOffset.Now && post.IsDeleted == false);
        }

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
    }
}