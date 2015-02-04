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
                .Add(new ScriptBundle("~/Content/js/jquery-validate")
                .Include("~/Content/js/jquery.validate.js")
                .Include("~/Content/js/jquery.validate.unobtrusive.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/jquery-migrate", "http://code.jquery.com/jquery-migrate-1.2.1.min.js")
				.Include("~/Content/js/jquery-migrate-1.2.1.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/js/bootstrap.min.js")
				.Include("~/Content/js/bootstrap.js"));

			bundles
				.Add(new ScriptBundle("~/Content/js")
				.Include("~/Content/js/jquery.ae.image.resize.min.js")
				.Include("~/Content/js/raccoon-blog.js")
                .Include("~/Content/js/setup.js"));

			bundles
				.Add(new StyleBundle("~/Content/css/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css")
				.Include("~/Content/css/bootstrap.css")
				.Include("~/Content/css/bootstrap-theme.css"));

			bundles
				.Add((new StyleBundle("~/Content/css/socicon"))
				.Include("~/Content/css/socicon.css"));
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

				foreach (var bundle in CreateThemeBundles(context))
					bundles.Add(bundle);

				themeBundlesRegistered = true;
			}
		}

		private static IEnumerable<Bundle> CreateThemeBundles(HttpContext context)
		{
			var themePath = context.Server.MapPath(ThemeDirectory);
			if (Directory.Exists(themePath) == false)
				yield break;

			foreach (var file in Directory.GetFiles(themePath, "*" + ThemeVariablesExtension).Select(x => new FileInfo(x)))
			{
				var themeName = file.Name.Substring(0, file.Name.IndexOf(ThemeVariablesExtension, StringComparison.OrdinalIgnoreCase));
				var themeBundleName = (ThemeDirectory + themeName).ToLowerInvariant();
				var bundle = new Bundle(themeBundleName, new ThemeLessTransform())
				.Include(ThemeDirectory + file.Name)
				.Include("~/Content/css/bootstrap/bootstrap.custom.less")
				.Include("~/Content/css/styles.less");

				var stylesFile = themeName + ThemeStylesExtension;
				if (File.Exists(themePath + stylesFile))
					bundle.Include(ThemeDirectory + stylesFile);

#if !DEBUG
				bundle.Transforms.Add(new CssMinify());
#endif

				yield return bundle;
			}
		}
	}
}
