using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RavenDbBlog.Controllers;
using RavenDbBlog.ViewModels;
using Xunit;

namespace RavenDbBlog.UnitTests.ControllersCore
{
    public class RoutesTests : IDisposable
    {
        const int DefaultPage = 1;

        public RoutesTests()
        {
            new RouteConfigurator(RouteTable.Routes).Configure();
        }

        public void Dispose()
        {
            RouteTable.Routes.Clear();
        }

        [Fact]
        public void DefaultRoute()
        {
            "~/".ShouldMapTo<PostController>(c => c.List(DefaultPage));
        }

        [Fact]
        public void SyndicationControllerRoutes()
        {
            "~/rss".ShouldMapTo<SyndicationController>(c => c.Rss());
            "~/rss/tag-name".ShouldMapTo<SyndicationController>(c => c.Tag("tag-name"));
            "~/rsd".ShouldMapTo<SyndicationController>(c => c.Rsd());
        }

        [Fact]
        public void PostControllerRoutes()
        {
            "~/".ShouldMapTo<PostController>(c => c.List(DefaultPage));

            "~/1024".ShouldMapTo<PostController>(c => c.Item(1024, null));
            "~/1024/blog-post-title".ShouldMapTo<PostController>(c => c.Item(1024, "blog-post-title"));

            "~/1024/comment"
                .WithMethod(HttpVerbs.Get)
                .ShouldMapTo<PostController>(c => c.Item(1024, "comment"));

            "~/1024/comment"
                .WithMethod(HttpVerbs.Post)
                .ShouldMapTo<PostController>(c => c.Comment(null, 1024));

            "~/post/tagslist".ShouldMapTo<PostController>(c => c.TagsList());
            "~/post/archiveslist".ShouldMapTo<PostController>(c => c.ArchivesList());

            "~/tags/tag-name".ShouldMapTo<PostController>(c => c.Tag("tag-name", DefaultPage));

            // "~/archive".ShouldMapTo<ErrorController>(c => c.404());
            "~/archive/2011".ShouldMapTo<PostController>(c => c.ArchiveYear(2011, DefaultPage));
            "~/archive/2011/4".ShouldMapTo<PostController>(c => c.ArchiveYearMonth(2011, 4, DefaultPage));
            "~/archive/2011/4/24".ShouldMapTo<PostController>(c => c.ArchiveYearMonthDay(2011, 4, 24, DefaultPage));
            "~/archive/2011/4/24/legacy-post-title.aspx".ShouldMapTo<PostController>(c => c.RedirectLegacyPost(2011, 4, 24, "legacy-post-title"));
        }

        [Fact]
        public void LoginControllerRoutes()
        {
            "~/users/login".ShouldMapTo<LoginController>(c => c.Login((string)null));
            "~/users/login".WithMethod(HttpVerbs.Post).ShouldMapTo<LoginController>(c => c.Login((LoginInput)null));

            "~/users/logout".ShouldMapTo<LoginController>(c => c.LogOut(null));

            "~/users/currentuser".ShouldMapTo<LoginController>(c => c.CurrentUser());
        }

        [Fact]
        public void PostAdminControllerRoutes()
        {
            "~/admin/posts".ShouldMapTo<PostAdminController>(c => c.List(DefaultPage));
        }

        [Fact]
        public void UserAdminControllerRoutes()
        {
            "~/admin/users".ShouldMapTo<UserAdminController>(c => c.List(DefaultPage));

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
        public void IgnoreRoutes()
        {
            "~/WebResource.axd".ShouldBeIgnored();
        }
    }
}