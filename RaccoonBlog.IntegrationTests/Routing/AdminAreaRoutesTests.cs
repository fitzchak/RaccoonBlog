using System.Web.Mvc;
using HibernatingRhinos.Loci.Common.Models;
using MvcContrib.TestHelper;
using RaccoonBlog.Web.Areas.Admin.Controllers;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Routing
{
	public class AdminAreaRoutesTests : RoutingTestBase
	{
		[Fact]
		public void LoginControllerRoutes()
		{
			"~/admin/login".ShouldMapTo<LoginController>(c => c.Index((string)null));
			"~/admin/login".WithMethod(HttpVerbs.Post).ShouldMapTo<LoginController>(c => c.Index((LogOnModel)null));

			"~/admin/logout".ShouldMapTo<LoginController>(c => c.LogOut(null));

			"~/admin/currentuser".ShouldMapTo<LoginController>(c => c.CurrentUser());
		}

		[Fact]
		public void Users()
		{
			"~/admin/users".ShouldMapTo<UsersController>(c => c.Index());

			"~/admin/users/add".ShouldMapTo<UsersController>(c => c.Add());
			"~/admin/users/4/edit".ShouldMapTo<UsersController>(c => c.Edit(4));

			"~/admin/users/4/changepass"
				.WithMethod(HttpVerbs.Get)
				.ShouldMapTo<UsersController>(c => c.ChangePassword(4));

			"~/admin/users/4/changepass"
				.WithMethod(HttpVerbs.Post)
				.ShouldMapTo<UsersController>(c => c.ChangePassword(null));

			var activateRoute = "~/admin/users/4/setactivation".WithMethod(HttpVerbs.Get);
			activateRoute.Values["isActive"] = bool.TrueString;
			activateRoute.ShouldMapTo<UsersController>(c => c.SetActivation(4, true));

			"~/admin/users/update".ShouldMapTo<UsersController>(c => c.Update(null));
		}

		[Fact]
		public void Sections()
		{
			"~/admin/sections".ShouldMapTo<SectionsController>(c => c.Index());

			"~/admin/sections/add".ShouldMapTo<SectionsController>(c => c.Add());
			"~/admin/sections/4/edit".ShouldMapTo<SectionsController>(c => c.Edit("sections/4"));

			"~/admin/sections/update".ShouldMapTo<SectionsController>(c => c.Update(null));

			var delete = "~/admin/sections/delete".WithMethod(HttpVerbs.Post);
			delete.Values["id"] = 4;
			delete.ShouldMapTo<SectionsController>(c => c.Delete("sections/4"));

			var setpostdate = "~/admin/sections/4/setposition".WithMethod(HttpVerbs.Post);
			setpostdate.Values["newPosition"] = 0;
			setpostdate.ShouldMapTo<SectionsController>(c => c.SetPosition("sections/4", 0));
		}

		[Fact]
		public void ConfigurationAdminControllerRoutes()
		{
			"~/admin/configuration".ShouldMapTo<SettingsController>(c => c.Index());
		}
	}
}