using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
    public class PostViewModel
    {
        public PostReference PreviousPost { get; set; }
        public PostReference NextPost { get; set; }

        public PostDetails Post { get; set; }
        public IList<Comment> Comments { get; set; }
        public CommentInput Input { get; set; }

        public bool AreCommentsClosed { get; set; }
        public bool IsTrustedCommenter { get; set; }
        public bool IsLoggedInCommenter { get; set; }

        public class Comment
        {
            public int Id { get; set; }
            public MvcHtmlString Body { get; set; }
            public string Author { get; set; }
            public string Tooltip { get; set; }
            public string Url { get; set; }    // Look for HTML injection.
            public string EmailHash { get; set; }
            public string CreatedAt { get; set; }
            public bool IsImportant { get; set; }
        }

        public class PostDetails
        {
            public int Id { get; set; }
			public Guid Key { get; set; }
            public MvcHtmlString Title { get; set; }
            public string Slug { get; set; }
            public MvcHtmlString Body { get; set; }

            public DateTimeOffset CreatedAt { get; set; }
            public DateTimeOffset PublishedAt { get; set; }
            public bool IsCommentAllowed { get; set; }

            public ICollection<TagDetails> Tags { get; set; }

            public UserDetails Author { get; set; }
        }

        public class UserDetails
        {
            public string FullName { get; set; }

            public string TwitterNick { get; set; }
            public string RelatedTwitterNick { get; set; }
			public string RelatedTwitNickDes { get; set; }
        }
    }
}
