using System.Web.Mvc;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
    public class MvcHtmlStringConverter : TypeConverter<string , MvcHtmlString>
    {
        protected override MvcHtmlString ConvertCore(string source)
        {
            return MvcHtmlString.Create(source);
        }
    }
}
