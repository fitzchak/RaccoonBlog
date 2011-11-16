using System;

namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class DateTimeOffsetUtil
	{
		public static DateTimeOffset AsMinutes(this DateTimeOffset self)
		{
			return new DateTimeOffset(self.Year, self.Month, self.Day, self.Hour, self.Minute, 0, 0, self.Offset);
		}

		public static DateTimeOffset ConvertFromUnixTimestamp(long timestamp)
		{
			var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
			return origin.AddSeconds(timestamp);
		}

		public static DateTimeOffset ConvertFromJsTimestamp(long timestamp)
		{
			var origin = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
			return origin.AddMilliseconds(timestamp);
		}
	}
}
