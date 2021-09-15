using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-editor" tag helper
    /// </summary>
    [HtmlTargetElement("nop-editor", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class NopEditorTagHelper : TagHelper
    {
        #region Constants

        private const string FOR_ATTRIBUTE_NAME = "asp-for";
        private const string DISABLED_ATTRIBUTE_NAME = "asp-disabled";
        private const string REQUIRED_ATTRIBUTE_NAME = "asp-required";
        private const string RENDER_FORM_CONTROL_CLASS_ATTRIBUTE_NAME = "asp-render-form-control-class";
        private const string TEMPLATE_ATTRIBUTE_NAME = "asp-template";
        private const string POSTFIX_ATTRIBUTE_NAME = "asp-postfix";
        private const string VALUE_ATTRIBUTE_NAME = "asp-value";
        private const string PLACEHOLDER_ATTRIBUTE_NAME = "placeholder";
        private const string AUTOCOMPLETE_ATTRIBUTE_NAME = "autocomplete";


        #endregion

        #region Properties

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Indicates whether the field is disabled
        /// </summary>
        [HtmlAttributeName(DISABLED_ATTRIBUTE_NAME)]
        public string IsDisabled { set; get; }

        /// <summary>
        /// Indicates whether the field is required
        /// </summary>
        [HtmlAttributeName(REQUIRED_ATTRIBUTE_NAME)]
        public string IsRequired { set; get; }

        /// <summary>
        /// Placeholder for the field
        /// </summary>
        [HtmlAttributeName(PLACEHOLDER_ATTRIBUTE_NAME)]
        public string Placeholder { set; get; }

        /// <summary>
        /// Autocomplete mode for the field
        /// </summary>
        [HtmlAttributeName(AUTOCOMPLETE_ATTRIBUTE_NAME)]
        public string Autocomplete { set; get; }

        /// <summary>
        /// Indicates whether the "form-control" class shold be added to the input
        /// </summary>
        [HtmlAttributeName(RENDER_FORM_CONTROL_CLASS_ATTRIBUTE_NAME)]
        public string RenderFormControlClass { set; get; }

        /// <summary>
        /// Editor template for the field
        /// </summary>
        [HtmlAttributeName(TEMPLATE_ATTRIBUTE_NAME)]
        public string Template { set; get; }

        /// <summary>
        /// Postfix
        /// </summary>
        [HtmlAttributeName(POSTFIX_ATTRIBUTE_NAME)]
        public string Postfix { set; get; }

        /// <summary>
        /// The value of the element
        /// </summary>
        [HtmlAttributeName(VALUE_ATTRIBUTE_NAME)]
        public string Value { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion

        #region Fields

        private readonly IHtmlHelper _htmlHelper;

        #endregion

        #region Ctor

        public NopEditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //clear the output
            output.SuppressOutput();

            //container for additional attributes
            var htmlAttributes = new Dictionary<string, object>();

            //set placeholder if exists
            if (!string.IsNullOrEmpty(Placeholder))
                htmlAttributes.Add("placeholder", Placeholder);

            //set autocomplete if exists
            if (!string.IsNullOrEmpty(Autocomplete))
                htmlAttributes.Add("autocomplete", Autocomplete);

            //set value if exists
            if (!string.IsNullOrEmpty(Value))
                htmlAttributes.Add("value", Value);

            //disabled attribute
            if (bool.TryParse(IsDisabled, out var disabled) && disabled)
                htmlAttributes.Add("disabled", "disabled");

            //required asterisk
            if (bool.TryParse(IsRequired, out var required) && required)
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

            return Task.CompletedTask;
        }

        #endregion
    }
}