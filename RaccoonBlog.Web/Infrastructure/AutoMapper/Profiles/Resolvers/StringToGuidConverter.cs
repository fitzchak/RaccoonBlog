using System;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class StringToGuidConverter : TypeConverter<string, Guid>
	{
		protected override Guid ConvertCore(string source)
		{
			Guid guid;
			if (Guid.TryParse(source, out guid) == false)
				return Guid.Empty;
			return guid;
		}
	}

	public class StringToNullableGuidConverter : TypeConverter<string, Guid?>
	{
		protected override Guid? ConvertCore(string source)
		{
			Guid guid;
			if (Guid.TryParse(source, out guid) == false)
				return null;
			return guid;
		}
	}
}
