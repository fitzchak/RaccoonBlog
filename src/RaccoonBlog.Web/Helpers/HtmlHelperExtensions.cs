using System.Collections.Generic;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Link(this HtmlHelper helper, string text, string href, object htmlAttributes)
        {
            var tag = new TagBuilder("a");
            tag.InnerHtml = text;

            if (string.IsNullOrEmpty(href) == false)
                tag.Attributes["href"] = href;

            IDictionary<string, object> attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var attribute in attributes)
            {
                var val = attribute.Value.ToString();
                if (string.IsNullOrEmpty(val) == false)
                    tag.Attributes[attribute.Key] = val;
            }

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

		public static MvcHtmlString AdminImage(this HtmlHelper helper, string path)
		{
			return MvcHtmlString.Create("/areas/admin/content/images/" + path.Replace('\\', '/'));
		}

		public static MvcHtmlString AdminScript(this HtmlHelper helper, string path)
		{
			return MvcHtmlString.Create("/areas/admin/content/js/" + path.Replace('\\', '/'));
		}

		public static MvcHtmlString AdminStyle(this HtmlHelper helper, string path)
		{
			return MvcHtmlString.Create("/areas/admin/content/css/" + path.Replace('\\', '/'));
		}
    }
}
