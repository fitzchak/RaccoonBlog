using RaccoonBlog.Web.Models;
using Rhino.Mocks;
using Xunit;

namespace RaccoonBlog.IntegrationTests.Web.Controllers.RaccoonControllerTests.BlogConfigProperty
{
    public class WhenTheBlogConfigIsAvailable : WhenTestingTheProperty
    {
        protected BlogConfig Config { get; set; }

        public WhenTheBlogConfigIsAvailable()
            : base()
        {
            Config = new BlogConfig();

            DocumentSession.Stub(ds => ds.Load<BlogConfig>("Blog/Config")).Return(Config);
        }

        [Fact]
        public void ThePropertyShouldReturnTheConfig()
        {
            var config = Controller.BlogConfig;

            Assert.NotNull(Config);
            Assert.Equal(Config, config);
        }
    }
}
