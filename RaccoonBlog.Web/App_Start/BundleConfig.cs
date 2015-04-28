using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

using RaccoonBlog.Web.Helpers;

namespace RaccoonBlog.Web
{
	public static class BundleConfig
	{
		public const string AdminThemeDirectory = "~/Areas/Admin/Content/css/";

		public const string ThemeDirectory = "~/Content/css/custom/";

		private const string ThemeVariablesExtension = ".variables.less";

		private const string ThemeStylesExtension = ".styles.less";

		private static bool themeBundlesRegistered;

		private static readonly object Locker = new object();

		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.UseCdn = true;

			bundles
				.Add(new ScriptBundle("~/Content/js/jquery", "https://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js")
				.Include("~/Content/js/jquery-1.11.2.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/jquery-migrate", "http://code.jquery.com/jquery-migrate-1.2.1.min.js")
				.Include("~/Content/js/jquery-migrate-1.2.1.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js")
				.Include("~/Content/js/bootstrap.js"));

            bundles
                .Add(new ScriptBundle("~/Admin/Content/js/bootstrap")
                .Include("~/Areas/Admin/Content/js/bootstrap.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/main")
				.Include("~/Content/js/jquery.ae.image.resize.min.js")
				.Include("~/Content/js/raccoon-blog.js")
                .Include("~/Content/js/setup.js")
				.Include("~/Content/js/jquery.twbsPagination.js")
				.Include("~/Content/js/jquery.validate.js")
				.Include("~/Content/js/jquery.validate.unobtrusive.js")
				.Include("~/Content/js/jquery.openid.js")
				.Include("~/Content/js/openid-en.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/admin/main")
				.Include("~/Areas/Admin/Content/js/setup.js")
				.Include("~/Content/js/moment.js")
				.Include("~/Areas/Admin/Content/js/fullcalendar.js")
				.Include("~/Areas/Admin/Content/js/tinymce/tinymce.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/themes/modern/theme.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/advlist/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/autolink/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/lists/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/link/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/image/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/charmap/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/print/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/hr/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/anchor/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/pagebreak/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/searchreplace/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/wordcount/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/visualblocks/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/visualchars/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/code/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/fullscreen/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/insertdatetime/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/media/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/nonbreaking/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/save/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/table/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/directionality/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/emoticons/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/template/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/paste/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/textcolor/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/colorpicker/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/textpattern/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/preview/plugin.min.js")
				.Include("~/Areas/Admin/Content/js/tinymce/plugins/contextmenu/plugin.min.js"));

            bundles
                .Add(new ScriptBundle("~/Content/js/respond")
                .Include("~/Content/js/respond.src.js"));

			bundles
				.Add((new StyleBundle("~/Content/css/styles"))
				.Include("~/Content/css/socicon.css")
				.Include("~/Content/css/openid/openid.css"));

			bundles
				.Add((new StyleBundle("~/Areas/Admin/Content/js/tinymce/skins/lightgray/styles"))
				.Include("~/Areas/Admin/Content/js/tinymce/skins/lightgray/skin.min.css")
				.Include("~/Areas/Admin/Content/js/tinymce/skins/lightgray/content.min.css"));

            bundles
				.Add(new StyleBundle("~/Areas/Admin/Content/css/styles")
                .Include("~/Areas/Admin/Content/css/fullcalendar.css"));
		}

		public static void RegisterThemeBundles(HttpContext context, BundleCollection bundles)
		{
			if (context == null)
				return;

			if (themeBundlesRegistered)
				return;

			lock (Locker)
			{
				if (themeBundlesRegistered)
					return;

                foreach (var bundle in CreateThemeBundles(context, ThemeDirectory))
                    bundles.Add(bundle);

                foreach (var bundle in CreateThemeBundles(context, AdminThemeDirectory))
					bundles.Add(bundle);

				themeBundlesRegistered = true;
			}
		}

		private static IEnumerable<Bundle> CreateThemeBundles(HttpContext context, string themeDirectory)
		{
            var themePath = context.Server.MapPath(themeDirectory);
			if (Directory.Exists(themePath) == false)
				yield break;

			foreach (var file in Directory.GetFiles(themePath, "*" + ThemeVariablesExtension).Select(x => new FileInfo(x)))
			{
				var themeName = file.Name.Substring(0, file.Name.IndexOf(ThemeVariablesExtension, StringComparison.OrdinalIgnoreCase));
                var themeBundleName = (themeDirectory + themeName).ToLowerInvariant();
				var bundle = new Bundle(themeBundleName, new ThemeLessTransform())
                .Include(themeDirectory + file.Name)
				.Include("~/Content/css/bootstrap/bootstrap.custom.less");

			    if (themeDirectory.Equals(ThemeDirectory))
			    {
			        bundle.Include("~/Content/css/styles.less");
			    }
                else if (themeDirectory.Equals(AdminThemeDirectory))
			    {
			        bundle.Include("~/Areas/Admin/Content/css/bootstrap/bootstrap-extend.less");
			    }

				var stylesFile = themeName + ThemeStylesExtension;
				if (File.Exists(themePath + stylesFile))
                    bundle.Include(themeDirectory + stylesFile);

#if !DEBUG
				bundle.Transforms.Add(new CssMinify());
#endif

				yield return bundle;
			}
		}
	}
}
