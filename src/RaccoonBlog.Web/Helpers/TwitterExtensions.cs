using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
    public static class TwitterExtensions
    {
        public static MvcHtmlString TwitterButton(this HtmlHelper html, string content, TwitterButtonDataCount dataCount,
                                                  string twitterNick, string relaterTwitterNick,
                                                  string relaterTwitterNickDesc)
        {
            var tag = new TagBuilder("a");
            tag.AddCssClass("twitter-share-button");
            tag.Attributes["href"] = "http://twitter.com/share";
            tag.Attributes["data-count"] = dataCount.ToString();

            if (string.IsNullOrEmpty(twitterNick) == false)
                tag.Attributes["data-via"] = twitterNick;

            if (string.IsNullOrEmpty(relaterTwitterNick) == false)
            {
                if (string.IsNullOrEmpty(relaterTwitterNickDesc) == false)
                    tag.Attributes["data-related"] = relaterTwitterNick + ":" + relaterTwitterNickDesc;
                else
                    tag.Attributes["data-related"] = relaterTwitterNick;
            }

            tag.InnerHtml = content;

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }

    public enum TwitterButtonDataCount
    {
        None,
        Horizental,
        Vertical,
    }
}