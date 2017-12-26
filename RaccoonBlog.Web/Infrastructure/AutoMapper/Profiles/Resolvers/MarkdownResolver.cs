using System;
using System.Text.RegularExpressions;
using System.Web;
using HeyRed.MarkdownSharp;
using Microsoft.AspNetCore.Html;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class MarkdownResolver
	{
        private static readonly Regex Backticks = new Regex(@"^```+\s*$", RegexOptions.Multiline);

		public static HtmlString Resolve(string inputBody)
		{
			var html = FormatMarkdown(inputBody);
			return new HtmlString(html);
		}

	    private static string NormalizeContent(string content)
	    {
	        return Backticks.Replace(content, "~~~");
	    }

		private static string FormatMarkdown(string content)
		{
		    var normalized = NormalizeContent(content);

			var md = GetMarkdownTransformer();

		    string result;

			try
			{
				result = md.Transform(normalized);
			}
			catch (Exception)
			{
				result = string.Format("<pre>{0}</pre>", HttpUtility.HtmlEncode(content));
			}

			return result;
		}

	    private static Markdown GetMarkdownTransformer()
	    {
	        var md = new Markdown();
	        /*md.ExtraMode = true;
	        md.SafeMode = true;
	        md.NoFollowLinks = true;
	        md.NewWindowForExternalLinks = true;
	        md.MarkdownInHtml = false;*/
	        return md;
	    }
	}
}