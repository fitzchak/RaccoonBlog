using System;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client;
using Raven.Client.Linq;
using RavenDbBlog.Core.Models;
using RavenDbBlog.Infrastructure.AutoMapper;
using RavenDbBlog.ViewModels;

namespace RavenDbBlog.DataServices
{
    public class PostService
    {
        protected IDocumentSession Session { get; set; }

        public PostService(IDocumentSession session)
        {
            Session = session;
        }

        public PostReference GetPostReference(Expression<Func<Post, bool>> expression)
        {
            var postReference = Session.Query<Post>()
                .Where(expression)
                .Select(p => new { p.Id, p.Title })
                .FirstOrDefault();

            if (postReference == null)
                return null;

            return postReference.DynamicMapTo<PostReference>();
        }

        public DateTimeOffset? GetLastCommentDateForPost(int postId)
        {
            var comments = Session.Load<PostComments>(postId);
            var comment = comments.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new {c.CreatedAt})
                .FirstOrDefault();
           
            if (comment == null)
                return null;

            return comment.CreatedAt;
        }
    }
}