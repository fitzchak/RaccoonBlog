using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RaccoonBlog.Web;
using RaccoonBlog.Web.Controllers;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Routing
{
	public class RoutesTests : RoutingTestBase
	{
		[Fact]
		public void DefaultRoute()
		{
			"~/".ShouldMapTo<PostController>(c => c.List());
		}

		[Fact]
		public void SyndicationControllerRoutes()
		{
			GetMethod("~/rss").ShouldMapTo<SyndicationController>(c => c.Rss(null, TestGuid));
			GetMethod("~/rss/tag-name").ShouldMapTo<SyndicationController>(c => c.Rss("tag-name", TestGuid));

			GetMethod("~/rsd").ShouldMapTo<SyndicationController>(c => c.Rsd());

			"~/rss.aspx".ShouldMapTo<SyndicationController>(c => c.LegacyRss());
		}

		[Fact]
		public void PostControllerRoutes()
		{
			"~/".ShouldMapTo<PostController>(c => c.List());

			"~/tags/tag-name".ShouldMapTo<PostController>(c => c.Tag("tag-name"));

			// "~/archive".ShouldMapTo<ErrorController>(c => c.404());
			"~/archive/2011".ShouldMapTo<PostController>(c => c.Archive(2011, null,null));
			"~/archive/2011/4".ShouldMapTo<PostController>(c => c.Archive(2011, 4, null));
			"~/archive/2011/4/24".ShouldMapTo<PostController>(c => c.Archive(2011, 4, 24));
		}

		[Fact]
		public void LegacyPostControllerRoutes()
		{
			"~/archive/2011/4/24/legacy-post-title.aspx".ShouldMapTo<LegacyPostController>(c => c.RedirectLegacyPost(2011, 4, 24, "legacy-post-title"));
			"~/archive/2011/4/24.aspx".ShouldMapTo<LegacyPostController>(c => c.RedirectLegacyArchive(2011, 4, 24));
		}

		[Fact]
		public void PostDetailsControllerRoutes()
		{
			GetMethod("~/1024").ShouldMapTo<PostDetailsController>(c => c.Details(1024, null, TestGuid));
			GetMethod("~/1024/blog-post-title").ShouldMapTo<PostDetailsController>(c => c.Details(1024, "blog-post-title", TestGuid));

			GetMethod("~/1024/comment").ShouldMapTo<PostDetailsController>(c => c.Details(1024, "comment", TestGuid));
			GetMethod("~/1024/comment", HttpVerbs.Post).ShouldMapTo<PostDetailsController>(c => c.Comment(null, 1024, TestGuid));
		}

		private RouteData GetMethod(string url, HttpVerbs method = HttpVerbs.Get)
		{
			var route = url.WithMethod(method);
			route.Values["key"] = TestGuid;
			return route;	
		}

		[Fact]
		public void SectionControllerRoutes()
		{
			"~/section/list".ShouldMapTo<SectionController>(c => c.List());

			"~/section/tagslist".ShouldMapTo<SectionController>(c => c.TagsList());
			"~/section/futureposts".ShouldMapTo<SectionController>(c => c.FuturePosts());
			"~/section/archiveslist".ShouldMapTo<SectionController>(c => c.ArchivesList());
			"~/section/postsstatistics".ShouldMapTo<SectionController>(c => c.PostsStatistics());
		}

		[Fact]
		public void SearchControllerRoutes()
		{
			"~/search".ShouldMapTo<SearchController>(c => c.SearchResult(null));
			"~/search/google_cse.xml".ShouldMapTo<SearchController>(c => c.GoogleCse());
		}

		[Fact]
		public void CssControllerRoutes()
		{
			"~/css".ShouldMapTo<CssController>(c => c.Merge(null));
		}

		[Fact]
		public void SocialLoginControllerRoutes()
		{
			"~/users/authenticate".ShouldMapTo<SocialLoginController>(c => c.Authenticate(null, null));
		}

		[Fact]
		public void IgnoreRoutes()
		{
			"~/WebResource.axd".ShouldBeIgnored();
		}
	}
}
