namespace RaccoonBlog.Web.Helpers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web.Optimization;
    using dotless.Core;
    using dotless.Core.configuration;

    public class LessTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var builder = new StringBuilder();

            foreach (var bundleFile in response.Files)
            {
                var pathAllowed = context.HttpContext.Server.MapPath("~/Content/css/");
                var normalizeFile = context.HttpContext.Server.MapPath(bundleFile.IncludedVirtualPath);

                if (normalizeFile.StartsWith(pathAllowed) == false)
                {
                    throw new Exception("Path not allowed");
                }

                if (File.Exists(normalizeFile))
                {
                    builder.AppendLine(File.ReadAllText(normalizeFile));
                }
            }

            response.Content = Less.Parse(builder.ToString(), new DotlessConfiguration());
            response.ContentType = "text/css";
        }
    }
}