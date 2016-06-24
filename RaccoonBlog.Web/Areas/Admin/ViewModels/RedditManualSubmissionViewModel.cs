using System.Collections.Generic;
using RaccoonBlog.Web.Models;

namespace RaccoonBlog.Web.Areas.Admin.ViewModels
{
    public class RedditManualSubmissionViewModel
    {
        public IList<Post> NotSubmittedPosts { get; set; }

        public IList<string> SubredditsToSubmitTo { get; set; }
    }
}