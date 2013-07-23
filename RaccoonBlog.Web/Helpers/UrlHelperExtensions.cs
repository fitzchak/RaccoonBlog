using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;

namespace RaccoonBlog.Web.Helpers
{
	public static class UrlHelperExtensions
	{
	    public static string AbsoluteAction(this UrlHelper url, [AspMvcAction] string action, object routeValues)
		{
			return AbsoluteActionUtil(url, url.Action(action, routeValues));
		}

		public static string AbsoluteAction(this UrlHelper url, [AspMvcAction] string action)
		{
			return AbsoluteActionUtil(url, url.Action(action));
		}

		public static string AbsoluteAction(this UrlHelper url, [AspMvcAction] string action, [AspMvcController] string controller)
		{
			return AbsoluteActionUtil(url, url.Action(action, controller));
		}

		public static string AbsoluteAction(this UrlHelper url, [AspMvcAction] string action, [AspMvcController] string controller, object routeValues)
		{
			return AbsoluteActionUtil(url, url.Action(action, controller, routeValues));
		}

		public static string RelativeToAbsolute(this UrlHelper url, string relativeUrl)
		{
			return AbsoluteActionUtil(url, relativeUrl);
		}

		private static string AbsoluteActionUtil(UrlHelper url, string relativeUrl)
		{
			var absoluteUrl = new UriBuilder(url.RequestContext.HttpContext.Request.Url)
			{
				Path = relativeUrl,
			}.ToString();
			return absoluteUrl;
		}


		public static MvcHtmlString ActionLinkWithArray(this UrlHelper url, [AspMvcAction] string action, [AspMvcController] string controller, object routeData)
		{
			string href = url.Action(action, controller, new {area = ""});

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