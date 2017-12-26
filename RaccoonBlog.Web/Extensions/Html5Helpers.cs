/*using System;
using HibernatingRhinos.Loci.Common.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RaccoonBlog.Web.Extensions
{
	public static class Html5Helpers
	{
		public static HtmlString Html5DateTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return HtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"), timestamp.ToString("dddd, dd MMMM yyyy")));
		}

		public static HtmlString Html5DateTimeTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return HtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"), timestamp.ToString("dddd, dd MMMM yyyy, HH:mm")));
		}

		public static HtmlString Html5MinutesAgoTag(this HtmlHelper html, DateTimeOffset timestamp)
		{
			return HtmlString.Create(string.Format(@"<time datetime=""{0}"">{1}</time>", timestamp.ToString("yyyy-MM-ddTHH:mm"),
				DateTimeOffset.Now.Subtract(timestamp).ToReadableString()
				));
		}
	}
}*/