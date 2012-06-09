using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RaccoonBlog.Web.Controllers;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Routing
{
	public class RoutesTests : RoutingTestBase
	{
		[Fact]
		public void DefaultRoute()
		{
			"~/".ShouldMapTo<PostsController>(c => c.Index());
		}

		[Fact]
		public void SyndicationControllerRoutes()
		{
			GetMethod("~/rss").ShouldMapTo<SyndicationController>(c => c.Rss(null, TestGuid.ToString()));
			GetMethod("~/rss/tag-name").ShouldMapTo<SyndicationController>(c => c.Rss("tag-name", TestGuid.ToString()));

			GetMethod("~/rsd").ShouldMapTo<SyndicationController>(c => c.Rsd());

			"~/rss.aspx".ShouldMapTo<SyndicationController>(c => c.LegacyRss());
		}

		[Fact]
		public void Posts()
		{
			"~/".ShouldMapTo<PostsController>(c => c.Index());

			"~/tags/tag-name".ShouldMapTo<PostsController>(c => c.Tag("tag-name"));

			// "~/archive".ShouldMapTo<ErrorController>(c => c.404());
			"~/archive/2011".ShouldMapTo<PostsController>(c => c.Archive(2011, null,null));
			"~/archive/2011/4".ShouldMapTo<PostsController>(c => c.Archive(2011, 4, null));
			"~/archive/2011/4/24".ShouldMapTo<PostsController>(c => c.Archive(2011, 4, 24));
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
		public void Css()
		{
			"~/css".ShouldMapTo<CssController>(c => c.Merge(null));
		}

		[Fact]
		public void IgnoreRoutes()
		{
			"~/WebResource.axd".ShouldBeIgnored();
		}
	}
}
