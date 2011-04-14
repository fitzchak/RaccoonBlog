using System;
using Raven.Client;
using RavenDbBlog.Core.Models;

namespace RavenDbBlog.Domain
{
    public class PostReader
    {
        private readonly IDocumentSession _sessoin;

        public PostReader(IDocumentSession sessoin)
        {
            _sessoin = sessoin;
        }

        protected IDocumentSession Session
        {
            get { return _sessoin; }
        }

        public Tuple<Post, CommentsCollection> GetPostAndComments(int postId)
        {
            var results = Session.Load<object>("posts/" + postId, "posts/" + postId + "/comments");

            Post post = null;
            if (results.Length > 0)
                post = (Post)results[0];

            var comments = new CommentsCollection();
            if (results.Length > 1)
                comments = (CommentsCollection)results[1];

            return new Tuple<Post, CommentsCollection>(post, comments);
        }
    }
}