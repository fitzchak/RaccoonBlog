using System;

namespace HibernatingRhinos.Loci.Common.Extensions
{
	public static class TimeSpanExtenstions
	{
		public static string ToReadableAgeString(this TimeSpan span)
		{
			return string.Format("{0:0}", span.Days / 365.25);
		}

		public static string ToReadableString(this TimeSpan span)
		{
			var days = span.Days;
			if (days > 0)
			{
				if (span.Hours > 23) days++;
				if (days > 1)
					return string.Format("{0:0} days ago", days);
				return "About 1 day ago";
			}

			if (span.Hours > 0)
			{
				if (span.Hours == 1)
					return "About 1 hour ago";
				return string.Format("About {0:0} hours ago", span.Hours);
			}

			if (span.Minutes > 1)
			{
				return string.Format("{0:0} minutes ago", span.Minutes);
			}

			return "Just now";
		}

		public static string ToExactReadableString(this TimeSpan span)
		{
			var formatted = string.Format("{0}{1}{2}{3}",
											 span.Days > 0 ? string.Format("{0:0} days, ", span.Days) : string.Empty,
											 span.Hours > 0 ? string.Format("{0:0} hours, ", span.Hours) : string.Empty,
											 span.Minutes > 0 ? string.Format("{0:0} minutes, ", span.Minutes) : string.Empty,
											 span.Seconds > 0 ? string.Format("{0:0} seconds", span.Seconds) : string.Empty);

			if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

			return formatted;
		}
	}
}
