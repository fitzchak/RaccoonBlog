using System.Web;
using Rhino.Mocks;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Web.Controllers.RaccoonControllerTests.BlogConfigProperty
{
    public class WhenTheBlogConfigIsNotAvailable : WhenTestingTheProperty
    {
        protected HttpResponseBase HttpResponse { get; set; }

        public WhenTheBlogConfigIsNotAvailable()
        {
            RouteData.Values.Add("controller", "not the welcome controller");

            HttpResponse = MockRepository.GenerateMock<HttpResponseBase>();

            HttpContext.Stub(hc => hc.Response).Return(HttpResponse);
        }

        [Fact]
        public void ThePropertyShouldRedirectToRelativeWelcome()
        {
            var config = Controller.BlogConfig;

            HttpResponse.AssertWasCalled(hr => hr.Redirect("~/welcome", true));
        }
    }
}
