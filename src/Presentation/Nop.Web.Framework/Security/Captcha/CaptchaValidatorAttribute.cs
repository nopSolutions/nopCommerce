using System.Web.Mvc;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security.Captcha
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string CHALLENGE_FIELD_KEY = "recaptcha_challenge_field";
        private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";
        private const string G_RESPONSE_FIELD_KEY = "g-recaptcha-response";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool valid = false;
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[CHALLENGE_FIELD_KEY];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY];
            var gCaptchaResponseValue = filterContext.HttpContext.Request.Form[G_RESPONSE_FIELD_KEY];
            if ((!string.IsNullOrEmpty(captchaChallengeValue) && !string.IsNullOrEmpty(captchaResponseValue)) || !string.IsNullOrEmpty(gCaptchaResponseValue))
            {
                var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
                if (captchaSettings.Enabled)
                {
                    //validate captcha
                    if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version1)
                    {
                        var captchaValidtor = new Recaptcha.RecaptchaValidator
                        {
                            PrivateKey = captchaSettings.ReCaptchaPrivateKey,
                            RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                            Challenge = captchaChallengeValue,
                            Response = captchaResponseValue
                        };

                        var recaptchaResponse = captchaValidtor.Validate();
                        valid = recaptchaResponse.IsValid;
                    }
                    else if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version2)
                    {
                        var captchaValidtor = new GReCaptchaValidator()
                        {
                            SecretKey = captchaSettings.ReCaptchaPrivateKey,
                            RemoteIp = filterContext.HttpContext.Request.UserHostAddress,
                            Response = gCaptchaResponseValue
                        };

                        var recaptchaResponse = captchaValidtor.Validate();
                        valid = recaptchaResponse.IsValid;
                    }
                }
            }

            //this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = valid;

            base.OnActionExecuting(filterContext);
        }
    }
}
