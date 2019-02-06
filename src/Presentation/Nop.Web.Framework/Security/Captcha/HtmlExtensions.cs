using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// HTML extensions
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// Generate reCAPTCHA Control
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <returns>Result</returns>
        public static IHtmlContent GenerateCaptcha(this IHtmlHelper helper)
        {
            var captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var lang = captchaSettings.ReCaptchaDefaultLanguage;

            if (captchaSettings.AutomaticallyChooseLanguage)
            {
                //this list got from this site: https://developers.google.com/recaptcha/docs/language, but we use languages only with two letters in the code
                var supportedLanguageCodes = new List<string> { "af", "am", "ar", "az", "bg", "bn", "ca", "cs", "da", "de", "el", "en", "es", "et", "eu", "fa", "fi", "fil", "fr", "gl", "gu", "hi", "hr", "hu", "hy", "id", "is", "it", "iw", "ja", "ka", "kn", "ko", "lo", "lt", "lv", "ml", "mn", "mr", "ms", "nl", "no", "pl", "pt", "ro", "ru", "si", "sk", "sl", "sr", "sv", "sw", "ta", "te", "th", "tr", "uk", "ur", "vi", "zu" };

                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var twoLetterIsoCode = workContext.WorkingLanguage != null
                    ? languageService.GetTwoLetterIsoLanguageName(workContext.WorkingLanguage).ToLower() 
                    : string.Empty;

                lang = supportedLanguageCodes.Contains(twoLetterIsoCode) ? twoLetterIsoCode : lang;
            }

            //generate captcha control
            var captchaControl = new GRecaptchaControl
            {
                Theme = captchaSettings.ReCaptchaTheme,
                Id = "recaptcha",
                PublicKey = captchaSettings.ReCaptchaPublicKey,
                Language =  lang
            };

            return new HtmlString(captchaControl.RenderControl());
        }
    }
}