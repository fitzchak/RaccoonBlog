using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Infrastructure.Indexes;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class DocumentSessionExtensions
	{
		public static async Task<IList<Tuple<PostComments.Comment, Post>>> QueryForRecentComments(
			this IAsyncDocumentSession documentSession,
			Func<IRavenQueryable<PostComments_CreationDate.Result>, IQueryable<PostComments_CreationDate.Result>> processQuery)
		{
			var query = documentSession
				.Query<PostComments_CreationDate.Result, PostComments_CreationDate>()
				.Include(comment => comment.PostCommentsId)
				.Include(comment => comment.PostId)
				.OrderByDescending(x => x.PostPublishAt)
				.ThenByDescending(x => x.CreatedAt)
				.Where(x => x.PostPublishAt < DateTimeOffset.Now.AsMinutes())
				.ProjectInto<PostComments_CreationDate.Result>();

			var commentsIdentifiers = await processQuery(query)
				.ToListAsync();

		    var list = new List<Tuple<PostComments.Comment, Post>>();
		    foreach (var commentIdentifier in commentsIdentifiers)
		    {
		        var comments = await documentSession.LoadAsync<PostComments>(commentIdentifier.PostCommentsId);
		        var post = await documentSession.LoadAsync<Post>(commentIdentifier.PostId);
		        var comment = comments.Comments.FirstOrDefault(x => x.Id == commentIdentifier.CommentId);
		        if (comment != null) 
		            list.Add(Tuple.Create(comment, post));
		    }
		    return list;
		}

		public static async Task<PostReference> GetNextPrevPost(this IAsyncDocumentSession session, Post compareTo, bool isNext)
		{
			var queryable = session.Query<Post>()
				.WhereIsPublicPost();

			if (isNext)
			{
				queryable = queryable
					.Where(post => post.PublishAt >= compareTo.PublishAt && post.Id != compareTo.Id)
					.OrderBy(post => post.PublishAt);
			}
			else
			{
				queryable = queryable
					.Where(post => post.PublishAt <= compareTo.PublishAt && post.Id != compareTo.Id)
					.OrderByDescending(post => post.PublishAt);
			}

			var postReference = await queryable
				.Select(p => new Post {Id = p.Id, Title = p.Title})
				.FirstOrDefaultAsync();

			if (postReference == null)
				return null;

			return postReference.MapTo<PostReference>();
		}

		public static User GetCurrentUser(this IAsyncDocumentSession session)
		{
			//if (HttpContext.Current.Request.IsAuthenticated == false)
				return null;

			/*var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
			if (claimsIdentity == null) 
				return null;

			var email = claimsIdentity.FindFirst(ClaimTypes.Email).Value;
			var user = session.GetUserByEmail(email);
			return user;*/
		}

		public static Task<User> GetUserByEmail(this IAsyncDocumentSession session, string email)
		{
			return session.Query<User>().FirstOrDefaultAsync(u => u.Email == email);
		}

		public static Commenter GetCommenter(this IAsyncDocumentSession session, string commenterKey)
		{
			Guid guid;
			if (Guid.TryParse(commenterKey, out guid) == false)
				return null;
			return GetCommenter(session, guid);
		}

		public static Commenter GetCommenter(this IAsyncDocumentSession session, Guid? commenterKey)
		{
			if (commenterKey == null)
				return null;
			return GetCommenter(session, commenterKey.Value);
		}

		public static Commenter GetCommenter(this IAsyncDocumentSession session, Guid commenterKey)
		{
			return session.Query<Commenter>().FirstOrDefault(x => x.Key == commenterKey);
		}
	}
}