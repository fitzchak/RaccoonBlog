using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class DocumentSessionExtensions
	{
		public static IList<Tuple<PostComments.Comment, Post>> QueryForRecentComments(this IDocumentSession documentSession, Guid key, int pageSize, out RavenQueryStatistics stats)
		{
			var commentsIdentifiersQuery = documentSession.Query<PostCommentsIdentifier, PostComments_CreationDate>()
				.Statistics(out stats)
				.Include(comment => comment.PostCollectionId)
				.Include(comment => comment.PostId);

			var commentsIdentifiers = commentsIdentifiersQuery.OrderByDescending(x => x.CreatedAt)
				.Take(pageSize)
				.AsProjection<PostCommentsIdentifier>()
				.ToList();

			return (from commentIdentifier in commentsIdentifiers
			        let comments = documentSession.Load<PostComments>(commentIdentifier.PostId)
			        let post = documentSession.Load<Post>(commentIdentifier.PostId)
			        where comments != null && post != null && post.IsPublicPost(key)
			        let comment = comments.Comments.FirstOrDefault(x => x.Id == commentIdentifier.CommentId)
			        where comment != null
			        select Tuple.Create(comment, post))
				.ToList();
		}

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


		public static Commenter GetCommenter(this IDocumentSession session, string commenterKey)
		{
			Guid guid;
			if (Guid.TryParse(commenterKey, out guid) == false)
				return null;
			return GetCommenter(session, guid);
		}

		public static Commenter GetCommenter(this IDocumentSession session, Guid commenterKey)
		{
			return session.Query<Commenter>()
						.Where(x => x.Key == commenterKey)
						.FirstOrDefault();
		}
	}
}