using System;
using System.Web;
using System.Web.Mvc;
using MarkdownDeep;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class MarkdownResolver
	{
		public static MvcHtmlString Resolve(string inputBody)
		{
			var html = FormatMarkdown(inputBody);
			html = SanitizeHtml.Sanitize(html);
			return MvcHtmlString.Create(html);
		}

		private static string FormatMarkdown(string content)
		{
			var md = new Markdown();
			string result;
			
			try
			{
				result = md.Transform(content);
			}
			catch (Exception)
			{
				result = string.Format("<pre>{0}</pre>", HttpUtility.HtmlEncode(content));
			}

			return result;
		}
	}
}