using System;
using System.Web.Mvc;

namespace HibernatingRhinos.Loci.Common.Extensions
{
	public static class Html5Helpers
	{
		public static MvcHtmlString Html5DateTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return MvcHtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"), timestamp.ToString("dddd, dd MMMM yyyy")));
		}

		public static MvcHtmlString Html5DateTimeTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return MvcHtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"), timestamp.ToString("dddd, dd MMMM yyyy, HH:mm")));
		}

		public static MvcHtmlString Html5MinutesAgoTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return MvcHtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"),
				DateTimeOffset.Now.Subtract(timestamp).ToReadableString()
				));
		}
	}
}