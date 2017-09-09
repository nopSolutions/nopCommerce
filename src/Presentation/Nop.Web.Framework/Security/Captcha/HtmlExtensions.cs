using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.Security.Captcha
{
    public static class HtmlExtensions
    {
        public static IHtmlContent GenerateCaptcha(this IHtmlHelper helper)
        {
            var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();

            var captchaControl = new GRecaptchaControl(captchaSettings.ReCaptchaVersion)
            {
                Theme = captchaSettings.ReCaptchaTheme,
                Id = "recaptcha",
                PublicKey = captchaSettings.ReCaptchaPublicKey,
                Language = captchaSettings.ReCaptchaLanguage
            };
            var captchaControlHtml = captchaControl.RenderControl();
            return new HtmlString(captchaControlHtml);
        }
    }
}