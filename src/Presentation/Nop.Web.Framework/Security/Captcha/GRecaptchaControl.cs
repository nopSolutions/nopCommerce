using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Nop.Web.Framework.Security.Captcha
{
    public class GRecaptchaControl
    {
        public string ID { get; set; }
        public string Theme { get; set; }
        public string PublicKey { get; set; }
        public string Language { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }

        public GRecaptchaControl() { }

        public void RenderControl(HtmlTextWriter writer)
        {
            var scriptCallbackTag = new TagBuilder("script");
            scriptCallbackTag.Attributes.Add("type", "text/javascript");
            scriptCallbackTag.InnerHtml =
                $"var onloadCallback = function() {{grecaptcha.render('{ID}', {{'sitekey' : '{PublicKey}', 'theme' : '{Theme}', 'type' : '{Type}', 'size' : '{Size}'}});}};";
            writer.Write(scriptCallbackTag.ToString(TagRenderMode.Normal));
            var captchaTag = new TagBuilder("div");
            captchaTag.Attributes.Add("id", ID);
            writer.Write(captchaTag.ToString(TagRenderMode.Normal));
            var scriptLoadApiTag = new TagBuilder("script");
            scriptLoadApiTag.Attributes.Add("src", "https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit"+(string.IsNullOrEmpty(Language)?"":$"&hl={Language}"));
            scriptLoadApiTag.Attributes.Add("async", null);
            scriptLoadApiTag.Attributes.Add("defer", null);
            writer.Write(scriptLoadApiTag.ToString(TagRenderMode.Normal));
        }
    }
}