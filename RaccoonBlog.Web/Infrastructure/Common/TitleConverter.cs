namespace RaccoonBlog.Web.Infrastructure.Common
{
	public static class TitleConverter
	{
		public static string ToSeriesTitle(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;

			var titles = value.Split(':');

			if (titles.Length > 1)
			{
				return titles[0].Trim();
			}

			return string.Empty;
		}

		public static string ToPostTitle(string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;

			var titles = value.Split(':');

			if (titles.Length > 1)
			{
				return titles[1].Trim();
			}

			return string.Empty;
		} 
	}
}