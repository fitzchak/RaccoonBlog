using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
	public static class StaticFileUrlHelperExtensions
	{
		private const string CssDir = "css";
		private const string ScriptDir = "js";

		public static string Css(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".css") && !fileName.EndsWith(".less"))
				fileName += ".css";

			return urlHelper.Content(string.Format("{0}/{1}", CssDir.TrimEnd('/'), fileName.TrimStart('/')));
		}

		public static string Script(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".js"))
				fileName += ".js";

			return urlHelper.Content(string.Format("{0}/{1}", ScriptDir.TrimEnd('/'), fileName.TrimStart('/')));
		}
	}
}