using System.Web.Mvc;

namespace RavenDbBlog.Helpers
{
    public static class HtmlExtantions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
        {
            string captcha = MvcReCaptcha.Helpers.ReCaptchaHelper.GenerateCaptcha(helper);
            return MvcHtmlString.Create(captcha);
        }
    }
}