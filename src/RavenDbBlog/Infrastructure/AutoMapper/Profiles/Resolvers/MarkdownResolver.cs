using System.Web.Mvc;

namespace RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class MarkdownResolver
    {
        public static MvcHtmlString Resolve(string inputBody)
        {
            var md = new MarkdownDeep.Markdown {SafeMode = true};
            var result = md.Transform(inputBody);
            return MvcHtmlString.Create(result);
        }
    }
}