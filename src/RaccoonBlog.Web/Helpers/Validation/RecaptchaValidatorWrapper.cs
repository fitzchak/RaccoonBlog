using System.Configuration;
using System.Web;
using Recaptcha;

namespace RaccoonBlog.Web.Helpers.Validation
{
    public class RecaptchaValidatorWrapper
    {
        private const string ChallengeFieldKey = "recaptcha_challenge_field";
        private const string ResponseFieldKey = "recaptcha_response_field";

        public static bool Validate(HttpContextBase context)
        {
            var request = context.Request;

        	var validator = new RecaptchaValidator
        	{
        		PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
        		RemoteIP = request.UserHostAddress,
        		Challenge = request.Form[ChallengeFieldKey],
        		Response = request.Form[ResponseFieldKey]
        	};
        	RecaptchaResponse response = validator.Validate();
            return response.IsValid;
        }
    }
}