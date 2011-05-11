using System;
using System.Configuration;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Common
{
	public static class UrlHelperExtensions
	{
		public static string Abs(this UrlHelper urlHelper, string relativeOrAbsoluteUrl)
		{
			var uri = new Uri(relativeOrAbsoluteUrl, UriKind.RelativeOrAbsolute);
			if (uri.IsAbsoluteUri)
			{
				return relativeOrAbsoluteUrl;
			}
			return ConfigurationManager.AppSettings["MainUrl"] + relativeOrAbsoluteUrl;
		}

	}
}
