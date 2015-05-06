using System.Linq;

namespace RaccoonBlog.Web.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Web.Optimization;
	using dotless.Core;
	using dotless.Core.configuration;

	public class ThemeLessTransform : IBundleTransform
	{
		private static readonly Dictionary<string, string> FilesToNormalize = new Dictionary<string, string>
			                                                             {
				                                                             {"bootstrap.custom.less", "~/Content/css/bootstrap"}
			                                                             };

		public void Process(BundleContext context, BundleResponse response)
		{
			var pathsAllowed = new[]
				                   {
					                   context.HttpContext.Server.MapPath("~/Content/css/"),
					                   context.HttpContext.Server.MapPath(BundleConfig.AdminThemeDirectory)
				                   };

			var builder = new StringBuilder();
			foreach (var bundleFile in response.Files)
			{
				string normalizeFile = context.HttpContext.Server.MapPath(bundleFile.IncludedVirtualPath);
				if (pathsAllowed.Any(normalizeFile.StartsWith) == false)
					throw new Exception("Path not allowed");

				if (File.Exists(normalizeFile) == false)
					continue;

				var content = File.ReadAllText(normalizeFile);
				string path;
				if (FilesToNormalize.TryGetValue(bundleFile.VirtualFile.Name, out path))
					content = NormalizeImports(content, context.HttpContext.Server.MapPath(path));

				builder.AppendLine(content);
			}

			response.Content = Less.Parse(builder.ToString(), new DotlessConfiguration { DisableUrlRewriting = true });
			response.ContentType = "text/css";
		}

		private static string NormalizeImports(string content, string path)
		{
			if (string.IsNullOrEmpty(content))
				return string.Empty;

			return content.Replace("{path}", path);
		}
	}
}