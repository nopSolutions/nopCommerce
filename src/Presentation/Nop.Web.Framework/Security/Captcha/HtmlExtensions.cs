using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// HTML extensions
    /// </summary>
    public static class HtmlExtensions
    {
        #region Utilities

        /// <summary>
        /// Get the reCAPTCHA language
        /// </summary>
        /// <param name="captchaSettings">Captcha settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the language code
        /// </returns>
        private static async Task<string> GetReCaptchaLanguageAsync(CaptchaSettings captchaSettings)
        {
            var language = (captchaSettings.ReCaptchaDefaultLanguage ?? string.Empty).ToLowerInvariant();
            if (captchaSettings.AutomaticallyChooseLanguage)
            {
                //this list got from this site: https://developers.google.com/recaptcha/docs/language
                //but we use languages only with two letters in the code
                var supportedLanguageCodes = new List<string> { "af", "am", "ar", "az", "bg", "bn", "ca", "cs", "da", "de", "el", "en", "es", "et", "eu", "fa", "fi", "fil", "fr", "gl", "gu", "hi", "hr", "hu", "hy", "id", "is", "it", "iw", "ja", "ka", "kn", "ko", "lo", "lt", "lv", "ml", "mn", "mr", "ms", "nl", "no", "pl", "pt", "ro", "ru", "si", "sk", "sl", "sr", "sv", "sw", "ta", "te", "th", "tr", "uk", "ur", "vi", "zu" };

                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                var currentLanguage = await workContext.GetWorkingLanguageAsync();
                var twoLetterIsoCode = currentLanguage != null
                    ? languageService.GetTwoLetterIsoLanguageName(currentLanguage).ToLowerInvariant()
                    : string.Empty;

                language = supportedLanguageCodes.Contains(twoLetterIsoCode) ? twoLetterIsoCode : language;
            }

            return language;
        }

        /// <summary>
        /// Generate API script tag
        /// </summary>
        /// <param name="captchaSettings">Captcha settings</param>
        /// <param name="captchaId">Captcha ID</param>
        /// <param name="render">Render</param>
        /// <param name="language">Language</param>
        /// <returns>Script tag</returns>
        private static TagBuilder GenerateLoadApiScriptTag(CaptchaSettings captchaSettings, string captchaId, string render, string language)
        {
            var hl = !string.IsNullOrEmpty(language)
                ? $"&hl={language}"
                : string.Empty;
            var url = string.Format($"{captchaSettings.ReCaptchaApiUrl}{NopSecurityDefaults.RecaptchaScriptPath}", captchaId, render, hl);
            var scriptLoadApiTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
            scriptLoadApiTag.Attributes.Add("src", url);
            scriptLoadApiTag.Attributes.Add("async", null);
            scriptLoadApiTag.Attributes.Add("defer", null);

            return scriptLoadApiTag;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate reCAPTCHA Control
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="captchaSettings">Captcha settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static async Task<IHtmlContent> GenerateCheckBoxReCaptchaV2Async(this IHtmlHelper helper, CaptchaSettings captchaSettings)
        {
            //prepare language
            var language = await GetReCaptchaLanguageAsync(captchaSettings);

            //prepare theme
            var theme = (captchaSettings.ReCaptchaTheme ?? string.Empty).ToLowerInvariant();
            theme = theme switch
            {
                "blackglass" or "dark" => "dark",
                "clean" or "red" or "white" or "light" => "light",
                _ => "light",
            };

            //prepare identifier
            var id = $"captcha_{CommonHelper.GenerateRandomInteger()}";

            //prepare public key
            var publicKey = captchaSettings.ReCaptchaPublicKey ?? string.Empty;

            //generate reCAPTCHA Control
            var scriptCallbackTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
            scriptCallbackTag.InnerHtml
                .AppendHtml($"var onloadCallback{id} = function() {{grecaptcha.render('{id}', {{'sitekey' : '{publicKey}', 'theme' : '{theme}' }});}};");

            var captchaTag = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            captchaTag.Attributes.Add("id", id);

            var scriptLoadApiTag = GenerateLoadApiScriptTag(captchaSettings, id, "explicit", language);

            return new HtmlString(await scriptCallbackTag.RenderHtmlContentAsync() + await captchaTag.RenderHtmlContentAsync() + await scriptLoadApiTag.RenderHtmlContentAsync());
        }

        /// <summary>
        /// Generate reCAPTCHA v3 Control
        /// </summary>
        /// <param name="helper">HTML helper</param>
        /// <param name="captchaSettings">Captcha settings</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public static async Task<IHtmlContent> GenerateReCaptchaV3Async(this IHtmlHelper helper, CaptchaSettings captchaSettings)
        {
            //prepare language
            var language = await GetReCaptchaLanguageAsync(captchaSettings);

            //prepare identifier
            var id = $"captcha_{CommonHelper.GenerateRandomInteger()}";

            //prepare public key
            var publicKey = captchaSettings.ReCaptchaPublicKey ?? string.Empty;

            //prepare reCAPTCHA script
            var actionName = helper.ViewContext.RouteData.Values["action"].ToString();
            var scriptCallback = $@"
                var onloadCallback{id} = function() {{
                    var form = $('input[id=""g-recaptcha-response_{id}""]').closest('form');
                    var btn = $(form.find(':submit')[0]);

                    var loaded = false;
                    var isBusy = false;
                    btn.on('click', function (e) {{
                        if (!isBusy) {{
                            isBusy = true;
                            grecaptcha.execute('{publicKey}', {{ 'action': '{actionName}' }}).then(function(token) {{
                                $('#g-recaptcha-response_{id}', form).val(token);
                                loaded = true;
                                btn.click();
                            }});
                        }}
                        return loaded;
                    }});
                }}
            ";
            var scriptCallbackTag = new TagBuilder("script") { TagRenderMode = TagRenderMode.Normal };
            scriptCallbackTag.InnerHtml.AppendHtml(scriptCallback);

            //prepare reCAPTCHA token input
            var captchaTokenInput = new TagBuilder("input") { TagRenderMode = TagRenderMode.Normal };
            captchaTokenInput.Attributes.Add("type", "hidden");
            captchaTokenInput.Attributes.Add("id", $"g-recaptcha-response_{id}");
            captchaTokenInput.Attributes.Add("name", "g-recaptcha-response");

            var scriptLoadApiTag = GenerateLoadApiScriptTag(captchaSettings, id, publicKey, language);

            return new HtmlString(await captchaTokenInput.RenderHtmlContentAsync() + await scriptCallbackTag.RenderHtmlContentAsync() + await scriptLoadApiTag.RenderHtmlContentAsync());
        }

        #endregion
    }
}