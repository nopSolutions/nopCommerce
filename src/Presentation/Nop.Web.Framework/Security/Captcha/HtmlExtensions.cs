using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security.Captcha
{
    public static class HtmlExtensions
    {
        public static string GenerateCaptcha(this HtmlHelper helper)
        {
            var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
            var htmlWriter = new HtmlTextWriter(new StringWriter());

            if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version1)
            {
                var theme = !string.IsNullOrEmpty(captchaSettings.ReCaptchaTheme)
                    ? captchaSettings.ReCaptchaTheme
                    : "white";
                var captchaControl = new Recaptcha.RecaptchaControl
                {
                    ID = "recaptcha",
                    Theme = theme,
                    PublicKey = captchaSettings.ReCaptchaPublicKey,
                    PrivateKey = captchaSettings.ReCaptchaPrivateKey
                };
                captchaControl.RenderControl(htmlWriter);
            }
            else if (captchaSettings.ReCaptchaVersion == ReCaptchaVersion.Version2)
            {
                var theme = !string.IsNullOrEmpty(captchaSettings.ReCaptchaTheme)
                    ? captchaSettings.ReCaptchaTheme
                    : "light";
                var language = !string.IsNullOrEmpty(captchaSettings.ReCaptchaLanguage)
                    ? captchaSettings.ReCaptchaLanguage
                    : "";
                var captchaControl = new GRecaptchaControl()
                {
                    Id = "recaptcha",
                    Theme = theme,
                    Language = language,
                    PublicKey = captchaSettings.ReCaptchaPublicKey
                };

                captchaControl.RenderControl(htmlWriter);
            }
            return htmlWriter.InnerWriter.ToString();
        }
    }
}
