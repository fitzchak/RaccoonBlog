using RavenDbBlog.Core;
using Xunit;

namespace RavenDbBlog.UnitTests
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