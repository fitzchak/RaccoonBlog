using System;

namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class TimeConverter
	{
		public static string DistanceOfTimeInWords(double minutes)
		{
			if (minutes < 0)
				throw new InvalidOperationException("Minutes can't be less than zero");

			if (minutes < 1)
			{
				return "less than a minute";
			}
			else if (minutes < 50)
			{
				return Math.Round(minutes) + " minutes";
			}
			else if (minutes < 90)
			{
				return "about one hour";
			}
			else if (minutes < 1080)
			{
				return Math.Round(new decimal((minutes / 60))) + " hours";
			}
			else if (minutes < 1440)
			{
				return "one day";
			}
			else if (minutes < 2880)
			{
				return "about one day";
			}
			else
			{
				return Math.Round(new decimal((minutes / 1440))) + " days";
			}
		}
	}
}