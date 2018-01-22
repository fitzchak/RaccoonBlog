using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RaccoonBlog.Web.Helpers
{
    public class Recaptcha2Helper
    {
        private static string RecaptchaSecret => ConfigurationManager.AppSettings["Recaptcha/Secret"];

        private static string SiteKey => ConfigurationManager.AppSettings["ReCaptcha/SiteKey"];

        public const string ModelStateErrorKey = "CaptchaNotValid";

        public static async Task<bool> Validate(ModelStateDictionary modelState)
        {
            var result = await Recaptcha2Verifier.VerifyResponse(RecaptchaSecret);
            if (result.IsValid)
            {
                return true;
            }

            modelState.AddModelError(ModelStateErrorKey, result.ErrorMessage);
            return false;
        }

        public static MvcHtmlString ScriptRef()
        {
            return new MvcHtmlString("<script src='https://www.google.com/recaptcha/api.js'></script>");
        }

        public static MvcHtmlString Widget()
        {
            var result = $"<div class='g-recaptcha' data-sitekey='{SiteKey}'></div>";
            return new MvcHtmlString(result);
        }
    }
}