using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RaccoonBlog.Web.Areas.Admin.Controllers;
using RaccoonBlog.Web.Areas.Admin.ViewModels;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Routing
{
	public class AdminAreaRoutesTests : RoutingTestBase
	{
		[Fact]
		public void LoginControllerRoutes()
		{
			"~/admin/login".ShouldMapTo<LoginController>(c => c.Index((string)null));
			"~/admin/login".WithMethod(HttpVerbs.Post).ShouldMapTo<LoginController>(c => c.Index((LoginInput)null));

			"~/admin/logout".ShouldMapTo<LoginController>(c => c.LogOut(null));

			"~/admin/currentuser".ShouldMapTo<LoginController>(c => c.CurrentUser());
		}

		[Fact]
		public void PostsControllerRoutes()
		{
			"~/admin/posts".ShouldMapTo<PostsController>(c => c.List());

			var routeData = "~/admin/posts/feed".WithMethod(HttpVerbs.Get);
			routeData.Values["start"] = 0;
			routeData.Values["end"] = 0;
			routeData.ShouldMapTo<PostsController>(c => c.ListFeed(0, 0));

			"~/admin/posts/1024".ShouldMapTo<PostsController>(c => c.Details(1024));
			"~/admin/posts/1024/edit".ShouldMapTo<PostsController>(c => c.Edit(1024));
			"~/admin/posts/1024/blog-post-title".ShouldMapTo<PostsController>(c => c.Details(1024));

			"~/admin/posts/update".ShouldMapTo<PostsController>(c => c.Update(null));
			var delete = "~/admin/posts/delete".WithMethod(HttpVerbs.Post);
			delete.Values["id"] = 1024;
			delete.ShouldMapTo<PostsController>(c => c.Delete(1024));

			var setpostdate = "~/admin/posts/1024/setpostdate".WithMethod(HttpVerbs.Post);
			setpostdate.Values["date"] = 0;
			setpostdate.ShouldMapTo<PostsController>(c => c.SetPostDate(1024, 0));

			"~/admin/posts/1024/setpostdate"
				.WithMethod(HttpVerbs.Get)
				.ShouldMapTo<PostsController>(c => c.Details(1024, "setpostdate"));

			var commentsadmin = "~/admin/posts/1024/commentsadmin".WithMethod(HttpVerbs.Post);
			commentsadmin.Values["command"] = "Delete";
			commentsadmin.ShouldMapTo<PostsController>(c => c.CommentsAdmin(1024, CommentCommandOptions.Delete, null));

			"~/admin/posts/1024/commentsadmin"
				.WithMethod(HttpVerbs.Get)
				.ShouldMapTo<PostsController>(c => c.Details(1024, "commentsadmin"));
		}

		[Fact]
		public void UserAdminControllerRoutes()
		{
			"~/admin/users".ShouldMapTo<UserAdminController>(c => c.List());

			"~/admin/users/add".ShouldMapTo<UserAdminController>(c => c.Add());
			"~/admin/users/4/edit".ShouldMapTo<UserAdminController>(c => c.Edit(4));

			"~/admin/users/4/changepass"
				.WithMethod(HttpVerbs.Get)
				.ShouldMapTo<UserAdminController>(c => c.ChangePass(4));

			"~/admin/users/4/changepass"
				.WithMethod(HttpVerbs.Post)
				.ShouldMapTo<UserAdminController>(c => c.ChangePass(null));

			var activateRoute = "~/admin/users/4/setactivation".WithMethod(HttpVerbs.Get);
			activateRoute.Values["isActive"] = bool.TrueString;
			activateRoute.ShouldMapTo<UserAdminController>(c => c.SetActivation(4, true));

			"~/admin/users/update".ShouldMapTo<UserAdminController>(c => c.Update(null));
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
		public void SectionAdminControllerRoutes()
		{
			"~/admin/sections".ShouldMapTo<SectionAdminController>(c => c.List());

			"~/admin/sections/add".ShouldMapTo<SectionAdminController>(c => c.Add());
			"~/admin/sections/4/edit".ShouldMapTo<SectionAdminController>(c => c.Edit(4));

			"~/admin/sections/update".ShouldMapTo<SectionAdminController>(c => c.Update(null));

			var delete = "~/admin/sections/delete".WithMethod(HttpVerbs.Post);
			delete.Values["id"] = 4;
			delete.ShouldMapTo<SectionAdminController>(c => c.Delete(4));

			var setpostdate = "~/admin/sections/4/setposition".WithMethod(HttpVerbs.Post);
			setpostdate.Values["newPosition"] = 0;
			setpostdate.ShouldMapTo<SectionAdminController>(c => c.SetPosition(4, 0));
		}

		[Fact]
		public void ElmahControllerRoutes()
		{
			"~/admin/elmah".ShouldMapTo<ElmahController>(c => c.Index(null));
			"~/admin/elmah/detail".ShouldMapTo<ElmahController>(c => c.Index("detail"));
			"~/admin/elmah/stylesheet".ShouldMapTo<ElmahController>(c => c.Index("stylesheet"));
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

		[Fact]
		public void ConfigurationAdminControllerRoutes()
		{
			"~/admin/configuration".ShouldMapTo<ConfigurationAdminController>(c => c.Index());
		}
	}
}
