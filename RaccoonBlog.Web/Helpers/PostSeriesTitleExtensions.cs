namespace RaccoonBlog.Web.Helpers
{
    public static class PostSeriesTitleExtensions
    {
        public static string ToSeriesTitle(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string result = string.Empty;
            var titles = value.Split(':');

            if (titles.Length > 1)
            {
                result = titles[0].Trim();
            }

            return result;
        }

        public static string ToPostTitle(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string result = string.Empty;
            var titles = value.Split(':');

            if (titles.Length > 1)
            {
                result = titles[1].Trim();
            }

            return result;
        }
    }
}