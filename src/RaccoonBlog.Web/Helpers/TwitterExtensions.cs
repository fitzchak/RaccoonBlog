using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
	public static class TwitterExtensions
	{
		public static MvcHtmlString TwitterButton(this HtmlHelper html,
		  string content,
		  TwitterButtonDataCount dataCount,
		  dynamic author)
		{
			return TwitterButton(html, content, dataCount, author, null, null);
		}

		public static MvcHtmlString TwitterButton(this HtmlHelper html,
			string content,
			TwitterButtonDataCount dataCount,
			dynamic author,
			string url, 
			string title)
		{
			var tag = new TagBuilder("a");
			tag.AddCssClass("twitter-share-button");
			tag.Attributes["href"] = "http://twitter.com/share";
			tag.Attributes["data-count"] = dataCount.ToString();

			if (string.IsNullOrEmpty(author.TwitterNick) == false)
				tag.Attributes["data-via"] = author.TwitterNick;

			if (string.IsNullOrEmpty(author.RelatedTwitterNick) == false)
			{
				if (string.IsNullOrEmpty(author.RelatedTwitterNickDesc) == false)
					tag.Attributes["data-related"] = author.RelatedTwitterNick + ":" + author.RelatedTwitterNickDesc;
				else
					tag.Attributes["data-related"] = author.RelatedTwitterNick;
			}

			if (string.IsNullOrEmpty(url) == false)
			{
				tag.Attributes["data-url"] = url;
				tag.Attributes["data-counturl"] = url;
			}


			if (string.IsNullOrEmpty(title) == false)
				tag.Attributes["data-text"] = title;

			tag.InnerHtml = content;

			return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
		}
	}
}
