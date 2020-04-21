using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// nop-editor tag helper
    /// </summary>
    [HtmlTargetElement("nop-editor", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class NopEditorTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string DisabledAttributeName = "asp-disabled";
        private const string RequiredAttributeName = "asp-required";
        private const string RenderFormControlClassAttributeName = "asp-render-form-control-class";
        private const string TemplateAttributeName = "asp-template";
        private const string PostfixAttributeName = "asp-postfix";
        private const string ValueAttributeName = "asp-value";
        private const string PlaceholderAttributeName = "placeholder";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Indicates whether the field is disabled
        /// </summary>
        [HtmlAttributeName(DisabledAttributeName)]
        public string IsDisabled { set; get; }

        /// <summary>
        /// Indicates whether the field is required
        /// </summary>
        [HtmlAttributeName(RequiredAttributeName)]
        public string IsRequired { set; get; }

        /// <summary>
        /// Placeholder for the field
        /// </summary>
        [HtmlAttributeName(PlaceholderAttributeName)]
        public string Placeholder { set; get; }

        /// <summary>
        /// Indicates whether the "form-control" class shold be added to the input
        /// </summary>
        [HtmlAttributeName(RenderFormControlClassAttributeName)]
        public string RenderFormControlClass { set; get; }

        /// <summary>
        /// Editor template for the field
        /// </summary>
        [HtmlAttributeName(TemplateAttributeName)]
        public string Template { set; get; }

        /// <summary>
        /// Postfix
        /// </summary>
        [HtmlAttributeName(PostfixAttributeName)]
        public string Postfix { set; get; }

        /// <summary>
        /// The value of the element
        /// </summary>
        [HtmlAttributeName(ValueAttributeName)]
        public string Value { set; get; }

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
        public NopEditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
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

            //clear the output
            output.SuppressOutput();

            //container for additional attributes
            var htmlAttributes = new Dictionary<string, object>();

            //set placeholder if exists
            if (!string.IsNullOrEmpty(Placeholder))
                htmlAttributes.Add("placeholder", Placeholder);

            //set value if exists
            if (!string.IsNullOrEmpty(Value))
                htmlAttributes.Add("value", Value);

            //disabled attribute
            bool.TryParse(IsDisabled, out var disabled);
            if (disabled)
            {
                htmlAttributes.Add("disabled", "disabled");
            }

            //required asterisk
            bool.TryParse(IsRequired, out var required);
            if (required)
            {
                output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
                output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //add form-control class
            bool.TryParse(RenderFormControlClass, out var renderFormControlClass);
            if (string.IsNullOrEmpty(RenderFormControlClass) && For.Metadata.ModelType.Name.Equals("String") || renderFormControlClass)
                htmlAttributes.Add("class", "form-control");

            //generate editor
            var pattern = $"{nameof(ILocalizedModel<object>.Locales)}" + @"(?=\[\w+\]\.)";
            if (!_htmlHelper.ViewData.ContainsKey(For.Name) && Regex.IsMatch(For.Name, pattern))
                _htmlHelper.ViewData.Add(For.Name, For.Model);

            var htmlOutput = _htmlHelper.Editor(For.Name, Template, new { htmlAttributes, postfix = Postfix });
            output.Content.SetHtmlContent(htmlOutput);
        }
    }
}