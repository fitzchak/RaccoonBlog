/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Helpers
{
    public static class PostUrlHelperExtensions
    {
	    public static string Series(this UrlHelper helper, int seriesId, string seriesSlug)
	    {
	        return helper.Action(
                MVC.Posts.ActionNames.Series,
	            MVC.Posts.Name,
	            new { seriesId, seriesSlug });
	    }

        public static string Post(this UrlHelper helper, int id, string slug)
        {
            return helper.Action(MVC.PostDetails.ActionNames.Details, MVC.PostDetails.Name, new
            {
                id,
                slug
            });
        }
    }
}*/