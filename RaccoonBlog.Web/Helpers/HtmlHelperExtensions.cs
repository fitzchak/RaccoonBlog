using System.Collections.Generic;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
    using System.Web;
    using System.Web.Optimization;

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

        public static IHtmlString RenderTheme(this HtmlHelper helper, string themeName)
        {
	        if (string.IsNullOrEmpty(themeName)) 
				return null;

            var oldValue = BundleTable.EnableOptimizations;
            try
			{
				BundleTable.EnableOptimizations = true;
				return Styles.Render(BundleConfig.ThemeDirectory + themeName);
            }
            finally
            {
                BundleTable.EnableOptimizations = oldValue;
            }
        }
	}
}