using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
	public static class StaticFileUrlHelperExtensions
	{
		private const string CssDir = "css";
		private const string ScriptDir = "js";
		private const int RevisionNumber = 2;

		public static string Css(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".css") && !fileName.EndsWith(".less"))
				fileName += ".css";

			return urlHelper.Content(string.Format("~/Content/{0}/{1}?version={2}", CssDir, fileName, RevisionNumber));
		}

		public static string Script(this UrlHelper urlHelper, string fileName)
		{
			if (!fileName.EndsWith(".js"))
				fileName += ".js";

			return urlHelper.Content(string.Format("~/Content/{0}/{1}?version={2}", ScriptDir, fileName, RevisionNumber));
		}
	}
}
