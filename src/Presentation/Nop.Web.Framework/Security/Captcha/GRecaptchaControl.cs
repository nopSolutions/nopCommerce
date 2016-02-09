using System.Web.Mvc;
using System.Web.UI;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GRecaptchaControl
    {
        public string Id { get; set; }
        public string Theme { get; set; }
        public string PublicKey { get; set; }
        public string Language { get; set; }

        public void RenderControl(HtmlTextWriter writer)
        {
            var scriptCallbackTag = new TagBuilder("script");
            scriptCallbackTag.Attributes.Add("type", "text/javascript");
            scriptCallbackTag.InnerHtml =
                string.Format(
                    "var onloadCallback = function() {{grecaptcha.render('{0}', {{'sitekey' : '{1}', 'theme' : '{2}' }});}};",
                    Id, PublicKey, Theme);
            writer.Write(scriptCallbackTag.ToString(TagRenderMode.Normal));
            var captchaTag = new TagBuilder("div");
            captchaTag.Attributes.Add("id", Id);
            writer.Write(captchaTag.ToString(TagRenderMode.Normal));
            var scriptLoadApiTag = new TagBuilder("script");
            scriptLoadApiTag.Attributes.Add("src",
                "https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit" +
                (string.IsNullOrEmpty(Language) ? "" : string.Format("&hl={0}", Language)));
            scriptLoadApiTag.Attributes.Add("async", null);
            scriptLoadApiTag.Attributes.Add("defer", null);
            writer.Write(scriptLoadApiTag.ToString(TagRenderMode.Normal));
        }
    }
}