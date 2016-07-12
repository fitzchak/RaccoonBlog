using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaccoonBlog.Web.Services.Reddit
{
    public class LinkPost
    {
        public string Subreddit { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}