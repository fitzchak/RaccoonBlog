using System;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class GuidToStringConverter : ITypeConverter<Guid, string>
	{
	    public string Convert(Guid source, string destination, ResolutionContext context)
	    {
	        return source.ToString("N");
	    }
	}
}