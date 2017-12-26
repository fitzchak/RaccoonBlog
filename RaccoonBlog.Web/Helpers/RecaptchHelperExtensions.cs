/*using System.Configuration;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.IdentityModel.Protocols;
using Recaptcha;

namespace RaccoonBlog.Web.Helpers
{
	public static class RecaptchHelperExtensions
	{
		public static HtmlString GenerateCaptcha(this HtmlHelper helper)
		{
			var control = new RecaptchaControl
			              {
			              	ID = "recaptcha",
			              	Theme = "white",
			              	PublicKey = ConfigurationManager<>.AppSettings["ReCaptchaPublicKey"],
			              	PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"]
			              };
			var writer = new HtmlTextWriter(new StringWriter());
			control.RenderControl(writer);
			var html = writer.InnerWriter.ToString();
			return HtmlString.Create(html);
		}
	}
}*/