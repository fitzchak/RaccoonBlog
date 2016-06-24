using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Optimization;

using RaccoonBlog.Web.Models;
using RaccoonBlog.Web.ViewModels;

namespace RaccoonBlog.Web.Helpers
{
	public static class HtmlHelperExtensions
	{
		private static readonly MvcHtmlString Empty = new MvcHtmlString(string.Empty);

	    public static MvcHtmlString Glyphicon(this HtmlHelper helper, string iconName)
	    {
	        return MvcHtmlString.Create($"<i class=\"glyphicon glyphicon-{iconName}\"></i>");
	    }

		public static bool IsSectionActive(this HtmlHelper helper, string sectionTitle)
		{
			var sections = helper.ViewBag.Sections as List<Section>;
			if (sections == null)
				return false;

			var section = sections.FirstOrDefault(x => string.Equals(x.Title, sectionTitle, StringComparison.OrdinalIgnoreCase) && x.IsActive);
			if (section == null)
				return false;

			return true;
		}

		public static MvcHtmlString RenderSection(this HtmlHelper helper, string sectionTitle)
		{
			var sections = helper.ViewBag.Sections as List<Section>;
			if (sections == null)
				return null;

			var section = sections.FirstOrDefault(x => string.Equals(x.Title, sectionTitle, StringComparison.OrdinalIgnoreCase) && x.IsActive);
			if (section == null)
				return null;

			if (string.IsNullOrEmpty(section.ActionName) == false && string.IsNullOrEmpty(section.ControllerName) == false)
				return helper.Action(section.ActionName, section.ControllerName);

			return new MvcHtmlString(section.Body);
		}

		public static string ConvertSectionTitleToId(this HtmlHelper helper, string sectionTitle)
		{
			if (string.IsNullOrEmpty(sectionTitle))
				return string.Empty;

			return sectionTitle
				.Trim()
				.Replace(" ", "-")
				.ToLowerInvariant();
		}

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
			return RenderThemeInternal(BundleConfig.ThemeDirectory, themeName);
		}

		public static IHtmlString RenderAdminTheme(this HtmlHelper helper)
		{
			return RenderThemeInternal(BundleConfig.AdminThemeDirectory, "admin");
		}

		private static IHtmlString RenderThemeInternal(string themeDirectory, string themeName)
		{
			if (string.IsNullOrEmpty(themeName))
				return null;

			var oldValue = BundleTable.EnableOptimizations;
			try
			{
				BundleTable.EnableOptimizations = true;
				return Styles.Render(themeDirectory + themeName);
			}
			finally
			{
				BundleTable.EnableOptimizations = oldValue;
			}
		}
	}
}