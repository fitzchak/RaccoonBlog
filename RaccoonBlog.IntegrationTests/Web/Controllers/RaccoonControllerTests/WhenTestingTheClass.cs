using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Rhino.Mocks;

namespace RaccoonBlog.IntegrationTests.Web.Controllers.RaccoonControllerTests
{
    public abstract class WhenTestingTheClass : IDisposable
    {
        protected HttpContextBase HttpContext { get; set; }
        protected RouteData RouteData { get; set; }
        protected ControllerBase ControllerBase { get; set; }
        protected TestController Controller { get; set; }


        public WhenTestingTheClass()
        {
            HttpContext = MockRepository.GenerateMock<HttpContextBase>();
            RouteData = new RouteData();
            ControllerBase = MockRepository.GenerateMock<ControllerBase>();

            Controller = new TestController();
            Controller.ControllerContext = new ControllerContext(HttpContext, RouteData, ControllerBase);
        }

        public virtual void Dispose()
        {
            return;
        }
    }
}
