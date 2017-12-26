using System.Web.Mvc;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class MvcHtmlStringConverter : ITypeConverter<string, MvcHtmlString>
	{
	    public MvcHtmlString Convert(string source, MvcHtmlString destination, ResolutionContext context)
	    {
	        return MvcHtmlString.Create(source);
	    }
	}
}