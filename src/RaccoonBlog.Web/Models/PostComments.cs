using System;
using System.Collections.Generic;
using System.Linq;

namespace RaccoonBlog.Web.Models
{
    public class PostComments
    {
        public string Id { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Comment> Spam { get; set; }

        public int LastCommentId { get; set; }

        public int GenerateNewCommentId()
        {
            return ++LastCommentId;
        }

    	public bool AreCommentsClosed(Post post)
    	{
    		DateTimeOffset lastCommentDate = Comments.Count == 0 ? post.CreatedAt : Comments.Max(x => x.CreatedAt);
    		return DateTimeOffset.Now - lastCommentDate > TimeSpan.FromDays(30);
    	}

    	public class Comment
        {
            public int Id { get; set; }
            public string Body { get; set; }
            public string Author { get; set; }
            public string Email { get; set; }
            public string Url { get; set; }

            public bool Important { get; set; }
            public bool IsSpam { get; set; }

            public DateTimeOffset CreatedAt { get; set; }
        }
    }
}