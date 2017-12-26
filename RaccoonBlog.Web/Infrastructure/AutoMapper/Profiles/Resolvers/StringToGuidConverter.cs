using System;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class StringToGuidConverter : ITypeConverter<string, Guid>
	{
	    public Guid Convert(string source, Guid destination, ResolutionContext context)
		{
		    if (Guid.TryParse(source, out var guid) == false)
				return Guid.Empty;
			return guid;
		}
	}

	public class StringToNullableGuidConverter : ITypeConverter<string, Guid?>
	{
	    public Guid? Convert(string source, Guid? destination, ResolutionContext context)
	    {
	        if (Guid.TryParse(source, out var guid) == false)
	            return null;
	        return guid;
	    }
	}
}
