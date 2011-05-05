using System;
using System.Web.Mvc;

namespace RaccoonBlog.Web.ViewModels
{
    public class NewCommentEmailViewModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }    // TODO: Look for HTML injection.
        public string Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public MvcHtmlString Body { get; set; }

        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public string BlogName { get; set; }
    }
}