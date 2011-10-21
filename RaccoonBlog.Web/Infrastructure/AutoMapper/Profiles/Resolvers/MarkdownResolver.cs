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
            
            return md.Transform(content);
        }
    }
}
