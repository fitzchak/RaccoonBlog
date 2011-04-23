using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;

namespace RavenDbBlog.Helpers
{
    public static class RecaptchHelperExtantions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
        {
            var control = new RecaptchaControl();
            control.ID = "recaptcha";
            control.Theme = "clean";
            control.PublicKey = ConfigurationManager.AppSettings["ReCaptchaPublicKey"];
            control.PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
            var writer = new HtmlTextWriter(new StringWriter());
            control.RenderControl(writer);
            var html = writer.InnerWriter.ToString();
            return MvcHtmlString.Create(html);
        }
    }
}