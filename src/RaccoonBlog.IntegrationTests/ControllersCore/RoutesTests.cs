using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RaccoonBlog.Web;
using RaccoonBlog.Web.Controllers;
using RaccoonBlog.Web.ViewModels;
using Xunit;

namespace RavenDbBlog.UnitTests.ControllersCore
{
    public class RoutesTests : IDisposable
    {
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
            "~/".ShouldMapTo<PostController>(c => c.List());
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
            "~/".ShouldMapTo<PostController>(c => c.List());

            "~/1024".ShouldMapTo<PostController>(c => c.Details(1024, null));
            "~/1024/blog-post-title".ShouldMapTo<PostController>(c => c.Details(1024, "blog-post-title"));

            "~/1024/comment"
                .WithMethod(HttpVerbs.Get)
                .ShouldMapTo<PostController>(c => c.Details(1024, "comment"));

            "~/1024/comment"
                .WithMethod(HttpVerbs.Post)
                .ShouldMapTo<PostController>(c => c.Comment(null, 1024));

            "~/post/tagslist".ShouldMapTo<PostController>(c => c.TagsList());
            "~/post/archiveslist".ShouldMapTo<PostController>(c => c.ArchivesList());

            "~/tags/tag-name".ShouldMapTo<PostController>(c => c.Tag("tag-name"));

            // "~/archive".ShouldMapTo<ErrorController>(c => c.404());
            "~/archive/2011".ShouldMapTo<PostController>(c => c.ArchiveYear(2011));
            "~/archive/2011/4".ShouldMapTo<PostController>(c => c.ArchiveYearMonth(2011, 4));
            "~/archive/2011/4/24".ShouldMapTo<PostController>(c => c.ArchiveYearMonthDay(2011, 4, 24));
            "~/archive/2011/4/24/legacy-post-title.aspx".ShouldMapTo<PostController>(c => c.RedirectLegacyPost(2011, 4, 24, "legacy-post-title"));
        }

        [Fact]
        public void LoginControllerRoutes()
        {
            "~/users/login".ShouldMapTo<LoginController>(c => c.Login((string)null));
            "~/users/login".WithMethod(HttpVerbs.Post).ShouldMapTo<LoginController>(c => c.Login((LoginInput)null));

            "~/users/logout".ShouldMapTo<LoginController>(c => c.LogOut(null));

            "~/users/currentuser".ShouldMapTo<LoginController>(c => c.CurrentUser());
            "~/users/administrationpanel".ShouldMapTo<LoginController>(c => c.AdministrationPanel());
        }

        [Fact]
        public void PostAdminControllerRoutes()
        {
            "~/admin/posts".ShouldMapTo<PostAdminController>(c => c.List());

            var routeData = "~/admin/posts/feed".WithMethod(HttpVerbs.Get);
            routeData.Values["start"] = 0;
            routeData.Values["end"] = 0;
            routeData.ShouldMapTo<PostAdminController>(c => c.ListFeed(0, 0));

            "~/admin/posts/1024".ShouldMapTo<PostAdminController>(c => c.Details(1024, null));
            "~/admin/posts/1024/edit".ShouldMapTo<PostAdminController>(c => c.Edit(1024));
            "~/admin/posts/1024/blog-post-title".ShouldMapTo<PostAdminController>(c => c.Details(1024, "blog-post-title"));

            "~/admin/posts/update".ShouldMapTo<PostAdminController>(c => c.Update(null));
            var delete = "~/admin/posts/delete".WithMethod(HttpVerbs.Post);
            delete.Values["id"] = 1024;
            delete.ShouldMapTo<PostAdminController>(c => c.Delete(1024));

            var setpostdate = "~/admin/posts/1024/setpostdate".WithMethod(HttpVerbs.Post);
            setpostdate.Values["date"] = 0;
            setpostdate.ShouldMapTo<PostAdminController>(c => c.SetPostDate(1024, 0));

            "~/admin/posts/1024/setpostdate"
                .WithMethod(HttpVerbs.Get)
                .ShouldMapTo<PostAdminController>(c => c.Details(1024, "setpostdate"));

            var commentsadmin = "~/admin/posts/1024/commentsadmin".WithMethod(HttpVerbs.Post);
            commentsadmin.ShouldMapTo<PostAdminController>(c => c.CommentsAdmin(1024, null, null));

            "~/admin/posts/1024/commentsadmin"
                .WithMethod(HttpVerbs.Get)
                .ShouldMapTo<PostAdminController>(c => c.Details(1024, "commentsadmin"));
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
        }

        [Fact]
        public void SectionAdminControllerRoutes()
        {
            "~/admin/sections".ShouldMapTo<SectionAdminController>(c => c.List());

            "~/admin/sections/add".ShouldMapTo<SectionAdminController>(c => c.Add());
            "~/admin/sections/4/edit".ShouldMapTo<SectionAdminController>(c => c.Edit(4));

            var setpostdate = "~/admin/sections/4/setposition".WithMethod(HttpVerbs.Post);
            setpostdate.Values["newPosition"] = 0;
            setpostdate.ShouldMapTo<SectionAdminController>(c => c.SetPosition(4, 0));
        }

        [Fact]
        public void ElmahControllerRoutes()
        {
            "~/admin/elmah".ShouldMapTo<ElmahController>(c => c.Index(null));
            "~/admin/elmah/stylesheet".ShouldMapTo<ElmahController>(c => c.Index("stylesheet"));
        }

        [Fact]
        public void IgnoreRoutes()
        {
            "~/WebResource.axd".ShouldBeIgnored();
        }
    }
}