using System.Linq;
using RaccoonBlog.Web.Infrastructure.AutoMapper.Profiles.Resolvers;
using Xunit;

/* References:
 * http://refactormycode.com/codes/333-sanitize-html
 */
namespace RaccoonBlog.IntegrationTests
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
		public void ImagesCannotContainScriptsCode()
		{
			Assert.DoesNotContain("img", MarkdownResolve("<img src=\"javascript:alert('hack!')\">"));
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
			var blacklistTags = "abbr|acronym|address|applet|area|base|basefont|bdo|big|body|button|caption|center|cite|col|colgroup|dir|div|dfn|embed|fieldset|font|form|frame|frameset|head|html|iframe|img|input|ins|isindex|label|legend|link|map|menu|meta|noframes|noscript|object|optgroup|option|param|q|samp|script|select|small|span|style|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|var|xmp";
			var tags = blacklistTags.Split('|').Select(tag => string.Format("<{0}>{0}</{0}>", tag));
			foreach (var tag in tags)
			{
				var result = MarkdownResolve(tag);
				Assert.DoesNotContain(tag, result);
			}
		}

		[Fact]
		public void WhitlistHtmlTags()
		{
			var blacklistTags = "code|dd|del|dl|dt|kbd|pre|s|strike";
			var tags = blacklistTags.Split('|').Select(tag => string.Format("<{0}>{0}</{0}>", tag));
			foreach (var tag in tags)
			{
				var result = MarkdownResolve(tag);
				Assert.Contains(tag, result);
			}
		}

		[Fact]
		public void DoNotSanitizeTheLowerThenTag_InCaseThatItsContainsASpace()
		{
			const string input = @"
<pre>string s = """";
for (int i = 0; i < 13000; i++)
{
	s += (char) i;
}</pre>";
			
			var result = MarkdownResolve(input);
			Assert.Contains("for (int i = 0; i < 13000; i++)", result);
		}

		[Fact]
		public void WillNotCrushForThisComment()
		{
			const string input = @"
 public static IEnumerable
<t RobustEnumerating
<t(
  
            IEnumerable
<t input, Func
<ienumerable<t, IEnumerable
<t> func)
  
        {
  
            // how to do this?
  
            IList
<t list = new List
<t();
  
            int counter = 0;
  
            foreach (var enumerable in input)
  
            {
  
                if (counter % 2 != 0)
  
                    list.Add(enumerable);
  
                counter++;           
  
            }
  
            input = list.AsEnumerable();
  
            return func(input);
  
  
        }
>
";
			Assert.DoesNotThrow(() => MarkdownResolve(input));
		}
	}
}
