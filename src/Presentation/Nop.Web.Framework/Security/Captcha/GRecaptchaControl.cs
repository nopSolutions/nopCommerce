using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// Google reCAPTCHA control
    /// </summary>
    public class GRecaptchaControl
    {
        /// <summary>
        /// Recaptcha Api url address
        /// </summary>
        /// <remarks>
        /// {0} : Id of recaptcha instance on page
        /// </remarks>
        private const string RECAPTCHA_API_URL = "https://www.google.com/recaptcha/api.js?onload=onloadCallback{0}&render=explicit";

        /// <summary>
        /// Identifier
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// reCAPTCHA theme
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// reCAPTCHA public key
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }        

        /// <summary>
        /// Render control
        /// </summary>
        /// <returns></returns>
        public string RenderControl()
        {
            SetTheme();

            //id for js code
            var id = "Recaptcha_" + Id.ToString("N");

            var scriptCallbackTag = new TagBuilder("script")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            scriptCallbackTag.InnerHtml.AppendHtml(
                $"var onloadCallback{id} = function() {{grecaptcha.render('{id}', {{'sitekey' : '{PublicKey}', 'theme' : '{Theme}' }});}};");

            var captchaTag = new TagBuilder("div")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            captchaTag.Attributes.Add("id", id);

            var scriptLoadApiTag = new TagBuilder("script")
            {
                TagRenderMode = TagRenderMode.Normal
            };

            var recaptchaApiUrl = string.Format(RECAPTCHA_API_URL, id);

            scriptLoadApiTag.Attributes.Add("src", recaptchaApiUrl + (string.IsNullOrEmpty(Language) ? "" : $"&hl={Language}"
                                                   ));
            scriptLoadApiTag.Attributes.Add("async", null);
            scriptLoadApiTag.Attributes.Add("defer", null);

            return scriptCallbackTag.RenderHtmlContent() + captchaTag.RenderHtmlContent() + scriptLoadApiTag.RenderHtmlContent();
        }

        private void SetTheme()
        {
            if (Theme == null)
                Theme = "";
            Theme = Theme.ToLower();

            var themes = new[] { "white", "blackglass", "red", "clean", "light", "dark" };

            switch (Theme)
            {
                case "clean":
                case "red":
                case "white":
                    Theme = "light";
                    break;
                case "blackglass":
                    Theme = "dark";
                    break;
                default:
                    if (!themes.Contains(Theme))
                    {
                        Theme = "light";
                    }
                    break;
            }
        }
    }
}