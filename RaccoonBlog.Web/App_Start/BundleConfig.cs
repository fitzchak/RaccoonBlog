using System.Web.Optimization;

namespace RaccoonBlog.Web
{
    using RaccoonBlog.Web.Helpers;

    public class BundleConfig
	{
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
				.Add(new ScriptBundle("~/Content/js")
				.Include("~/Content/js/jquery.ae.image.resize.min.js")
				.Include("~/Content/js/raccoon-blog.js"));

			bundles
				.Add(new StyleBundle("~/Content/css/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.2/css/bootstrap.min.css")
                .Include("~/Content/css/bootstrap.css"));

            bundles.Add(new Bundle("~/Content/css/custom/ayende", new LessTransform(), new CssMinify())
                .Include("~/Content/css/custom/ayende.settings.less.css")
                .Include("~/Content/css/base.less.css")
                .Include("~/Content/css/custom/ayende.less.css"));

            bundles.Add(new Bundle("~/Content/css/custom/hibernatingrhinos", new Helpers.LessTransform(), new CssMinify())
                .Include("~/Content/css/custom/hibernatingrhinos.settings.less.css")
                .Include("~/Content/css/base.less.css")
                .Include("~/Content/css/custom/hibernatingrhinos.less.css"));

			//bundles.Add(new LessBundle("~/Content/less")
			//	.Include("~/Content/site.less"));
		}
	}
}
