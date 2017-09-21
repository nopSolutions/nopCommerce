using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Web.Framework.Extensions;
using Nop.Core.Domain.Common;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    [HtmlTargetElement("nop-nested-setting", Attributes = ForAttributeName)]
    public class NopNestedSettingTagHelper : TagHelper
    {
        private readonly AdminAreaSettings _adminAreaSettings;

        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopNestedSettingTagHelper(IHtmlGenerator generator, AdminAreaSettings adminAreaSettings)
        {
            Generator = generator;
            _adminAreaSettings = adminAreaSettings;
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

            var parentSettingName = For.Name;

            var random = CommonHelper.GenerateRandomInteger();
            var nestedSettingId = $"nestedSetting{random}";
            var parentSettingId = $"parentSetting{random}";

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "nested-setting");
            output.Attributes.Add("id", nestedSettingId);

            //use javascript
            if (_adminAreaSettings.UseNestedSettingJavascript)
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                                $"initNestedSetting('{parentSettingName}', '{parentSettingId}', '{nestedSettingId}');" +
                                            "});");
                output.PreContent.SetHtmlContent(script.RenderHtmlContent());
            }
        }
    }
}