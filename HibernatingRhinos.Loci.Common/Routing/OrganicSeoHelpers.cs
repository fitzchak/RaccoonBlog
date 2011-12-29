using System.Text.RegularExpressions;
using System.Web;

namespace HibernatingRhinos.Loci.Common.Routing
{
	public static class OrganicSeoHelpers
	{
		public static void FixUrlOnBeginRequest(System.Web.HttpApplication app)
		{
			if (HttpContext.Current.Request.HttpMethod != "GET")
				return;

			// Get the requested URL so we can do some validation on it.
			// We exclude the query string, and add that later, so it's not included
			// in the validation
			var url = (app.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath);
			var changed = false;

			// If we're not a request for the root, and end with a slash, strip it off
			if (HttpContext.Current.Request.Url.AbsolutePath != "/" && HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/"))
			{
				url = url.Substring(0, url.Length - 1) + HttpContext.Current.Request.Url.Query;
				changed = true;
			}

			// If we have double-slashes, strip them out
			if (HttpContext.Current.Request.Url.AbsolutePath.Contains("//"))
			{
				url = url.Replace("//", "/") + HttpContext.Current.Request.Url.Query;
				changed = true;
			}

			// If we've got uppercase characters, fix
			if (Regex.IsMatch(url, @"[A-Z]"))
			{
				url = url.ToLower() + HttpContext.Current.Request.Url.Query;
				changed = true;
			}

			if (changed)
			{
				app.Response.Clear();
				app.Response.Status = "301 Moved Permanently";
				app.Response.AddHeader("Location", url);
				app.Response.End();
			}
		}
	}
}
