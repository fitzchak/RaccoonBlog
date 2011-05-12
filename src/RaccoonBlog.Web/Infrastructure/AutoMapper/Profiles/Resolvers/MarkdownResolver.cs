using System.Web.Mvc;
using MarkdownSharp;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class MarkdownResolver
    {
        public static MvcHtmlString Resolve(string inputBody)
        {
        	var md = new MarkdownSharp.Markdown(new MarkdownOptions
        	{
        		AutoHyperlink = true,
        		AutoNewLines = true,
        		EncodeProblemUrlCharacters = true,
        		LinkEmails = false,
        		StrictBoldItalic = true
        	});
            var result = md.Transform(inputBody);
            return MvcHtmlString.Create(result);
        }
    }
}
