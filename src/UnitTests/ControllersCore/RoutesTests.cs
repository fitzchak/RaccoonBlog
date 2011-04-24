using System.Web.Routing;
using MvcContrib.TestHelper;
using RavenDbBlog.Controllers;
using Xunit;

namespace RavenDbBlog.UnitTests.ControllersCore
{
    public class RoutesTests
    {
        public RoutesTests()
        {
            new RouteConfigurator(RouteTable.Routes).Configure();
        }

        [Fact]
        public void DefaultRoute()
        {
            "~/".ShouldMapTo<PostController>(c => c.List(1));
        }
    }
}