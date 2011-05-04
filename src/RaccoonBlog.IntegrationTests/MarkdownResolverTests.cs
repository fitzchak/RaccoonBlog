using System.Linq;
using RavenDbBlog.Infrastructure.AutoMapper.Profiles.Resolvers;
using Xunit;

/* References:
 * http://refactormycode.com/codes/333-sanitize-html
 */
namespace RavenDbBlog.UnitTests
{
    public class MarkdownResolverTests
    {
        [Fact]
        public void AllowMarkdownLink()
        {
            var input = "[example](http://example.com \"merely an example\")";
            var result = MarkdownResolve(input);
            var expected = "<a href=\"http://example.com\" title=\"merely an example\">example</a>";
            Assert.Contains(expected, result);
        }

        private static string MarkdownResolve(string input)
        {
            return MarkdownResolver.Resolve(input).ToString();
        }

        [Fact]
        public void AllowMarkdownBold()
        {
            var input = "**bold**";
            var result = MarkdownResolve(input);
            var expected = "<strong>bold</strong>";
            Assert.Contains(expected, result);
        }

        [Fact]
        public void AllowMarkdownItalic()
        {
            var input = "*italic*, _italic_";
            var result = MarkdownResolve(input);
            var expected = "<em>italic</em>, <em>italic</em>";
            Assert.Contains(expected, result);
        }

        [Fact]
        public void AllowMarkdownCodeBlock()
        {
            var input = "`CodeBlock`";
            var result = MarkdownResolve(input);
            var expected = "<code>CodeBlock</code>";
            Assert.Contains(expected, result);
        }

        [Fact]
        public void AllowRawLink()
        {
            var url = "http://example.com?query=value#item";
            var input = url;
            var result = MarkdownResolve(input);
            var expected = string.Format("<a href=\"{0}\">{0}</a>", url);
            Assert.Contains(expected, result);
        }

        [Fact]
        public void BlockAllOtherHtmlTags()
        {
            var blacklistTags = "abbr|acronym|address|applet|area|base|basefont|bdo|big|body|button|caption|center|cite|code|col|colgroup|dd|del|dir|div|dfn|dl|dt|embed|fieldset|font|form|frame|frameset|head|html|iframe|img|input|ins|isindex|kbd|label|legend|link|map|menu|meta|noframes|noscript|object|optgroup|option|param|pre|q|s|samp|script|select|small|span|strike|style|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|var|xmp";
            var tags = blacklistTags.Split('|').Select(tag => string.Format("<{0}>{0}</{0}>", tag));
            foreach (var tag in tags)
            {
                var result = MarkdownResolve(tag);
                Assert.DoesNotContain(string.Format("<{0}>{0}</{0}>", tag), result);
            }
        }
    }
}