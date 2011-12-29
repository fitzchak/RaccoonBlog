using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;

namespace RaccoonBlog.Web.Helpers
{
	public static class RecaptchHelperExtensions
	{
		public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
		{
			var control = new RecaptchaControl
			              {
			              	ID = "recaptcha",
			              	Theme = "clean",
			              	PublicKey = ConfigurationManager.AppSettings["ReCaptchaPublicKey"],
			              	PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"]
			              };
			var writer = new HtmlTextWriter(new StringWriter());
			control.RenderControl(writer);
			var html = writer.InnerWriter.ToString();
			return MvcHtmlString.Create(html);
		}
	}
}