using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Nop.Core;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GRecaptchaControl
    {
        private const string RECAPTCHA_API_URL_VERSION1 = "http://www.google.com/recaptcha/api/challenge?k={0}";
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

        public void RenderControl(HtmlTextWriter writer)
        {
            SetTheme();

            if (_version == ReCaptchaVersion.Version1)
            {
                var scriptCaptchaOptionsTag = new TagBuilder("script");
                scriptCaptchaOptionsTag.Attributes.Add("type", MimeTypes.TextJavascript);
                scriptCaptchaOptionsTag.InnerHtml =
                    string.Format("var RecaptchaOptions = {{ theme: '{0}', tabindex: 0 }}; ", Theme);
                writer.Write(scriptCaptchaOptionsTag.ToString(TagRenderMode.Normal));

                var scriptLoadApiTag = new TagBuilder("script");
                scriptLoadApiTag.Attributes.Add("src", string.Format(RECAPTCHA_API_URL_VERSION1, PublicKey));
                writer.Write(scriptLoadApiTag.ToString(TagRenderMode.Normal));
            }
            else if (_version == ReCaptchaVersion.Version2)
            {
                var scriptCallbackTag = new TagBuilder("script");
                scriptCallbackTag.Attributes.Add("type", MimeTypes.TextJavascript);
                scriptCallbackTag.InnerHtml = string.Format("var onloadCallback = function() {{grecaptcha.render('{0}', {{'sitekey' : '{1}', 'theme' : '{2}' }});}};", Id, PublicKey, Theme);
                writer.Write(scriptCallbackTag.ToString(TagRenderMode.Normal));

                var captchaTag = new TagBuilder("div");
                captchaTag.Attributes.Add("id", Id);
                writer.Write(captchaTag.ToString(TagRenderMode.Normal));

                var scriptLoadApiTag = new TagBuilder("script");
                scriptLoadApiTag.Attributes.Add("src", RECAPTCHA_API_URL_VERSION2 + (string.IsNullOrEmpty(Language) ? "" : string.Format("&hl={0}", Language)));
                scriptLoadApiTag.Attributes.Add("async", null);
                scriptLoadApiTag.Attributes.Add("defer", null);
                writer.Write(scriptLoadApiTag.ToString(TagRenderMode.Normal));
            }
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