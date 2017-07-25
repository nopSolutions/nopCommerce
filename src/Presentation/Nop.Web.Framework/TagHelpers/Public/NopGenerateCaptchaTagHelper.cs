using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("nop-generate-captcha", TagStructure = TagStructure.WithoutEndTag)]
    public class NopGenerateCaptchaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly CaptchaSettings _captchaSettings;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopGenerateCaptchaTagHelper(IHtmlHelper htmlHelper, CaptchaSettings captchaSettings)
        {
            _htmlHelper = htmlHelper;
            _captchaSettings = captchaSettings;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //generate captcha control
            var captchaControl = new GRecaptchaControl(_captchaSettings.ReCaptchaVersion)
            {
                Theme = _captchaSettings.ReCaptchaTheme,
                Id = "recaptcha",
                PublicKey = _captchaSettings.ReCaptchaPublicKey,
                Language = _captchaSettings.ReCaptchaLanguage
            };
            var captchaControlHtml = captchaControl.RenderControl();

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(captchaControlHtml);
        }
    }
}