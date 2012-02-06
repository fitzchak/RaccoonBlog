using RaccoonBlog.Web.Controllers;
using Raven.Client;

namespace RaccoonBlog.IntegrationTests.Web.Controllers.RaccoonControllerTests
{
    public class TestController : RaccoonController
    {
        public void SetRavenSession(IDocumentSession documentSession)
        {
            this.RavenSession = documentSession;
        }
    }
}
