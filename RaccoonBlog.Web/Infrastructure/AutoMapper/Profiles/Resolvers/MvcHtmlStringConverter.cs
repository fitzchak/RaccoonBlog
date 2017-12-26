using AutoMapper;
using Microsoft.AspNetCore.Html;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class HtmlStringConverter : ITypeConverter<string, HtmlString>
	{
	    public HtmlString Convert(string source, HtmlString destination, ResolutionContext context)
	    {
	        return new HtmlString(source);
	    }
	}
}