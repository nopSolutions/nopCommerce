using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Domain.Security;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// nop-captcha tag helper
    /// </summary>
    [HtmlTargetElement("nop-captcha", TagStructure = TagStructure.WithoutEndTag)]
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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="htmlHelper">HTML helper</param>
        public NopGenerateCaptchaTagHelper(IHtmlHelper htmlHelper, CaptchaSettings captchaSettings)
        {
            _htmlHelper = htmlHelper;
            _captchaSettings = captchaSettings;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
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

            IHtmlContent captchaHtmlContent;
            switch (_captchaSettings.CaptchaType)
            {
                case CaptchaType.CheckBoxReCaptchaV2:
                    output.Attributes.Add("class", "captcha-box");
                    captchaHtmlContent = _htmlHelper.GenerateCheckBoxReCaptchaV2(_captchaSettings);
                    break;
                case CaptchaType.ReCaptchaV3:
                    captchaHtmlContent = _htmlHelper.GenerateReCaptchaV3(_captchaSettings);
                    break;
                default:
                    throw new InvalidOperationException("Invalid captcha type.");
            }

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(captchaHtmlContent);
        }
    }
}