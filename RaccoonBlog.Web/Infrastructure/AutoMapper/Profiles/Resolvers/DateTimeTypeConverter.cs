using System;
using AutoMapper;

namespace RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers
{
	public class DateTimeTypeConverter : TypeConverter<DateTimeOffset, DateTime>
	{
		protected override DateTime ConvertCore(DateTimeOffset source)
		{
			return source.DateTime;
		}
	}
}
