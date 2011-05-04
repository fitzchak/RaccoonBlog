using System;
using System.Web.Mvc;

namespace RavenDbBlog.ViewModels
{
    public class NewCommentEmailViewModel
    {
        public MvcHtmlString Body { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }    // Look for HTML injection.
        public string Email { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}