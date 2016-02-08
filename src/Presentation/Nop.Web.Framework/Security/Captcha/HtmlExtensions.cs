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
            if (captchaSettings.ReCaptchaVersion == 1)
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
            else if (captchaSettings.ReCaptchaVersion == 2)
            {
                var theme = !string.IsNullOrEmpty(captchaSettings.ReCaptchaTheme)
                    ? captchaSettings.ReCaptchaTheme
                    : "light";
                var captchaControl = new GRecaptchaControl()
                {
                    ID = "recaptcha",
                    Theme = theme,
                    PublicKey = captchaSettings.ReCaptchaPublicKey
                };

                captchaControl.RenderControl(htmlWriter);
            }
            return htmlWriter.InnerWriter.ToString();
        }
    }
}
