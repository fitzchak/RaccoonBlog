using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using RaccoonBlog.Web;

namespace RaccoonBlog.IntegrationTests.Routing
{
	public class RoutingTestBase : IDisposable
	{
		protected static readonly Guid TestGuid = Guid.NewGuid();

		public RoutingTestBase()
		{
			new RouteConfigurator(RouteTable.Routes).Configure();
		}

		public void Dispose()
		{
			RouteTable.Routes.Clear();
		}

		protected RouteData GetMethod(string url, HttpVerbs method = HttpVerbs.Get)
		{
			var route = url.WithMethod(method);
			route.Values["key"] = TestGuid;
			return route;
		}
	}
}