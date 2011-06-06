using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
	public static class StaticFileUrlHelperExtensions
	{
		private const string CssDir = "css";
		private const string ScriptDir = "js";
		private const int RevisionNumber = 1;

		public static string Css(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".css") && !fileName.EndsWith(".less"))
				fileName += ".css";

			return urlHelper.Content(string.Format("{0}/{1}?version={2}", CssDir.TrimEnd('/'), fileName.TrimStart('/'), RevisionNumber));
		}

		public static string Script(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".js"))
				fileName += ".js";

			return urlHelper.Content(string.Format("{0}/{1}?version={2}", ScriptDir.TrimEnd('/'), fileName.TrimStart('/'), RevisionNumber));
		}
	}
}