using RaccoonBlog.Web.Areas.Admin.Controllers;
using RaccoonBlog.Web.Models;
using Rhino.Mocks;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Web.Controllers
{
	public class BlogConfigBehavior : RaccoonControllerTests
	{
		[Fact]
		public void WhenTheBlogConfigIsAvailable_ThePropertyShouldReturnTheConfig()
		{
			var config = new BlogConfig {Title = "Test Config", Id = "blog/config"};
			SetupData(session => session.Store(config));

			BlogConfig configFromController = null;
			ExecuteAction<LoginController>(controller => configFromController = controller.BlogConfig);

			Assert.Equal(config.Title, configFromController.Title);
		}

		[Fact]
		public void WhenTheBlogConfigIsNotAvailable_AndWhenNotOnWelcomeController_ThePropertyShouldRedirectToRelativeWelcome()
		{
			ExecuteAction<LoginController>(controller =>
			                               	{
			                               		controller.RouteData.Values.Add("controller", "not the welcome controller");
			                               		var _ = controller.BlogConfig;
			                               	});

			ControllerContext.HttpContext.Response.AssertWasCalled(x => x.Redirect("~/welcome", true));
		}

		[Fact]
		public void WhenTheBlogConfigIsNotAvailable_AndWhenOnWelcomeController_ThePropertyShouldNotRedirect()
		{
			ExecuteAction<LoginController>(controller =>
			                               	{
			                               		controller.RouteData.Values.Add("controller", "welcome");
			                               		var _ = controller.BlogConfig;
			                               	});

			ControllerContext.HttpContext.Response.AssertWasNotCalled(x => x.Redirect("~/welcome", true));
		}
	}
}