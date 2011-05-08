using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

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


        public static MvcHtmlString ActionLinkWithArray(this UrlHelper url, string action, string controller, object routeData)
        {
            string href = url.Action(action, controller);

            var rv = new RouteValueDictionary(routeData);
            var parameters = new List<string>();
            if (routeData != null)
            {
                foreach (var key in rv.Keys)
                {
                    var propertyInfo = routeData.GetType().GetProperty(key);
                    var value = propertyInfo.GetValue(routeData, null);
                    var array = value as IEnumerable;
                    if (array != null && !(array is string))
                    {
                        foreach (string val in array)
                        {
                            parameters.Add(string.Format("{0}={1}", key, val));
                        }
                    }
                    else
                    {
                        parameters.Add(string.Format("{0}={1}", key, value));
                    }
                }
               
            }

            string paramString = string.Join("&", parameters.ToArray());
            if (!string.IsNullOrEmpty(paramString))
            {
                href += "?" + paramString;
            }
            return MvcHtmlString.Create(href);
        }
    }
}