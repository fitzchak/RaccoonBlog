using System.Configuration;
using System.Web;
using Recaptcha;

namespace RavenDbBlog.Helpers.Validation
{
    public class RecaptchaValidatorWrapper
    {
        private const string ChallengeFieldKey = "recaptcha_challenge_field";
        private const string ResponseFieldKey = "recaptcha_response_field";

        public static bool Validate(HttpContextBase context)
        {
            var request = context.Request;
            string str = request.Form["recaptcha_challenge_field"];
            string str2 = request.Form["recaptcha_response_field"];

            var validator = new RecaptchaValidator();
            validator.PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];
            validator.RemoteIP = request.UserHostAddress;
            validator.Challenge = str;
            validator.Response = str2;
            RecaptchaResponse response = validator.Validate();
            return response.IsValid;
        }
    }
}