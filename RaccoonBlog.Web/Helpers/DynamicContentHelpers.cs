/*using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HibernatingRhinos.Loci.Common.Models;
using MarkdownDeep;

namespace RaccoonBlog.Web.Helpers
{
	public static class DynamicContentHelpers
	{
		private static readonly Regex CodeBlockFinder = new Regex(@"\[code lang=(.+?)\s*\](.*?)\[/code\]", RegexOptions.Compiled | RegexOptions.Singleline);
		private static readonly Regex FirstLineSpacesFinder = new Regex(@"^(\s|\t)+", RegexOptions.Compiled);

		public static HtmlString CompiledContent(this IDynamicContent contentItem, bool trustContent)
		{
			if (contentItem == null) return HtmlString.Empty;

			switch (contentItem.ContentType)
			{
				case DynamicContentType.Markdown:
					var md = new Markdown
					{
						AutoHeadingIDs = true,
						ExtraMode = true,
						NoFollowLinks = !trustContent,
						SafeMode = false,
						NewWindowForExternalLinks = true,
					};

					var contents = contentItem.Body;
					contents = CodeBlockFinder.Replace(contents, match => GenerateCodeBlock(match.Groups[1].Value.Trim(), match.Groups[2].Value));

					try
					{
						contents = md.Transform(contents);
					}
					catch (Exception)
					{
						contents = string.Format("<pre>{0}</pre>", HttpUtility.HtmlEncode(contents));
					}

					return HtmlString.Create(contents);
				case DynamicContentType.Html:
					return trustContent ? HtmlString.Create(contentItem.Body) : HtmlString.Empty;
			}
			return HtmlString.Empty;
		}

		private static string GenerateCodeBlock(string lang, string code)
		{
			code = HttpContext.Current.Server.HtmlDecode(code);
			return string.Format("<pre class=\"brush: {2}\">{0}{1}</pre>{0}", Environment.NewLine,
								 ConvertMarkdownCodeStatment(code).Replace("<", "&lt;"), // to support syntax highlighting on pre tags
								 lang
				);
		}

		private static string ConvertMarkdownCodeStatment(string code)
		{
			var line = code.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var firstLineSpaces = GetFirstLineSpaces(line.FirstOrDefault());
			var firstLineSpacesLength = firstLineSpaces.Length;
			var formattedLines = line.Select(l => string.Format("    {0}", l.Substring(l.Length < firstLineSpacesLength ? 0 : firstLineSpacesLength)));
			return string.Join(Environment.NewLine, formattedLines);
		}

		private static string GetFirstLineSpaces(string firstLine)
		{
			if (firstLine == null)
				return string.Empty;

			var match = FirstLineSpacesFinder.Match(firstLine);
			if (match.Success)
			{
				return firstLine.Substring(0, match.Length);
			}
			return string.Empty;
		}
	}
}*/