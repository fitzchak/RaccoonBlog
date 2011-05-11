using RaccoonBlog.Web;
using RaccoonBlog.Web.Common;
using Xunit;

namespace RaccoonBlog.IntegrationTests
{
    public class SlugConverterTests
    {
        [Fact]
        public void ReplaceSpacesWithDashes()
        {
            Assert.Equal("ef-prof", C("EF Prof"));
        }

        private string C(string title)
        {
            return SlugConverter.TitleToSlag(title);
        }
    }
}
