using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.Models.SocialNetwork;

namespace RaccoonBlog.Web.ViewModels
{
    public class RedditSubmissionFailedViewModel
    {
        public Post Post { get; set; }

        public Reddit.PostSubmission Submission { get; set; } 
    }
}