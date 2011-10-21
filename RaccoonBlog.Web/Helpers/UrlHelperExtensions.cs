using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace RaccoonBlog.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, object routeValues)
        {
            return AbsoluteActionUtil(url, url.Action(action, routeValues));
        }

        public static string AbsoluteAction(this UrlHelper url, string action)
        {
            return AbsoluteActionUtil(url, url.Action(action));
        }

        public static string AbsoluteAction(this UrlHelper url, string action, string controller)
        {
            return AbsoluteActionUtil(url, url.Action(action, controller));
        }

		public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues)
		{
			return AbsoluteActionUtil(url, url.Action(action, controller, routeValues));
		}

        public static string RelativeToAbsolute(this UrlHelper url, string relativeUrl)
        {
            return AbsoluteActionUtil(url, relativeUrl);
        }

    	private static string AbsoluteActionUtil(UrlHelper url, string relativeUrl)
    	{
    		var request = url.RequestContext.HttpContext.Request;
    		Uri requestUrl = request.Url;
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
