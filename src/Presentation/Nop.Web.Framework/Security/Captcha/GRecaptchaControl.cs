using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GRecaptchaControl
    {
        private const string RECAPTCHA_API_URL_HTTP_VERSION1 = "http://www.google.com/recaptcha/api/challenge?k={0}";
        private const string RECAPTCHA_API_URL_HTTPS_VERSION1 = "https://www.google.com/recaptcha/api/challenge?k={0}";
        private const string RECAPTCHA_API_URL_VERSION2 = "https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit";

        public string Id { get; set; }
        public string Theme { get; set; }
        public string PublicKey { get; set; }
        public string Language { get; set; }

        private readonly ReCaptchaVersion _version;

        public GRecaptchaControl(ReCaptchaVersion version = ReCaptchaVersion.Version1)
        {
            _version = version;
        }

        public string RenderControl()
        {
            SetTheme();

            if (_version == ReCaptchaVersion.Version1)
            {
                var scriptCaptchaOptionsTag = new TagBuilder("script");
                scriptCaptchaOptionsTag.TagRenderMode = TagRenderMode.Normal;
                scriptCaptchaOptionsTag.Attributes.Add("type", MimeTypes.TextJavascript);
                scriptCaptchaOptionsTag.InnerHtml.AppendHtml(string.Format("var RecaptchaOptions = {{ theme: '{0}', tabindex: 0 }}; ", Theme));
                
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                var scriptLoadApiTag = new TagBuilder("script");
                scriptLoadApiTag.TagRenderMode = TagRenderMode.Normal;
                var scriptSrc = webHelper.IsCurrentConnectionSecured() ? 
                    string.Format(RECAPTCHA_API_URL_HTTPS_VERSION1, PublicKey) :
                    string.Format(RECAPTCHA_API_URL_HTTP_VERSION1, PublicKey);
                scriptLoadApiTag.Attributes.Add("src", scriptSrc);

                return scriptCaptchaOptionsTag.RenderTagBuilder() + scriptLoadApiTag.RenderTagBuilder();
            }
            else if (_version == ReCaptchaVersion.Version2)
            {
                var scriptCallbackTag = new TagBuilder("script");
                scriptCallbackTag.TagRenderMode = TagRenderMode.Normal;
                scriptCallbackTag.Attributes.Add("type", MimeTypes.TextJavascript);
                scriptCallbackTag.InnerHtml.AppendHtml(string.Format("var onloadCallback = function() {{grecaptcha.render('{0}', {{'sitekey' : '{1}', 'theme' : '{2}' }});}};", Id, PublicKey, Theme));
               
                var captchaTag = new TagBuilder("div");
                captchaTag.TagRenderMode = TagRenderMode.Normal;
                captchaTag.Attributes.Add("id", Id);
               
                var scriptLoadApiTag = new TagBuilder("script");
                scriptLoadApiTag.TagRenderMode = TagRenderMode.Normal;
                scriptLoadApiTag.Attributes.Add("src", RECAPTCHA_API_URL_VERSION2 + (string.IsNullOrEmpty(Language) ? "" : string.Format("&hl={0}", Language)));
                scriptLoadApiTag.Attributes.Add("async", null);
                scriptLoadApiTag.Attributes.Add("defer", null);

                return scriptCallbackTag.RenderTagBuilder() + captchaTag.RenderTagBuilder() + scriptLoadApiTag.RenderTagBuilder();
            }

            throw new NotSupportedException("Specified version is not supported");
        }

        private void SetTheme()
        {
            var themes = new[] {"white", "blackglass", "red", "clean", "light", "dark"};

            if (_version == ReCaptchaVersion.Version1)
            {
                switch (Theme.ToLower())
                {
                    case "light":
                        Theme = "white";
                        break;
                    case "dark":
                        Theme = "blackglass";
                        break;
                    default:
                        if (!themes.Contains(Theme.ToLower()))
                        {
                            Theme = "white";
                        }
                        break;
                }
            }
            else if (_version == ReCaptchaVersion.Version2)
            {
                switch (Theme.ToLower())
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
                        if (!themes.Contains(Theme.ToLower()))
                        {
                            Theme = "light";
                        }
                        break;
                }
            }
        }
    }
}