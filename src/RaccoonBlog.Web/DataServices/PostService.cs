using System;
using System.Linq;
using System.Linq.Expressions;
using RaccoonBlog.Web.Infrastructure.AutoMapper;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;
using Raven.Client;
using Raven.Client.Linq;

namespace RaccoonBlog.Web.DataServices
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
                .OrderByDescending(post => post.PublishAt)
                .Select(p => new {p.Id, p.Title})
                .FirstOrDefault();

            if (postReference == null)
                return null;

            return postReference.DynamicMapTo<PostReference>();
        }
    }
}