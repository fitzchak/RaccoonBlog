using System;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, object routeValues)
        {
            return AbsoluteAction(url, url.Action(action, routeValues));
        }

        public static string AbsoluteAction(this UrlHelper url, string action, string controller)
        {
            return AbsoluteAction(url, url.Action(action, controller));
        }

		public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues)
		{
			return AbsoluteAction(url, url.Action(action, controller, routeValues));
		}

    	private static string AbsoluteAction(UrlHelper url, string relativeUrl)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;
            string absoluteAction = string.Format("{0}://{1}{2}",
                                                  requestUrl.Scheme,
                                                  requestUrl.Authority,
                                                  relativeUrl);
            return absoluteAction;
        }
    }
}