using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-label" tag helper
    /// </summary>
    [HtmlTargetElement("nop-label", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public partial class NopLabelTagHelper : TagHelper
    {
        #region Constants

        private const string FOR_ATTRIBUTE_NAME = "asp-for";
        private const string DISPLAY_HINT_ATTRIBUTE_NAME = "asp-display-hint";

        #endregion

        #region Properties

        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Indicates whether the hint should be displayed
        /// </summary>
        [HtmlAttributeName(DISPLAY_HINT_ATTRIBUTE_NAME)]
        public bool DisplayHint { get; set; } = true;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion

        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public NopLabelTagHelper(IHtmlGenerator generator, ILocalizationService localizationService, IWorkContext workContext)
        {
            Generator = generator;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //generate label
            var tagBuilder = Generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, null, new { @class = "col-form-label" });
            if (tagBuilder != null)
            {
                //create a label wrapper
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;

                //merge classes
                var classValue = output.Attributes.ContainsName("class")
                    ? $"{output.Attributes["class"].Value} label-wrapper"
                    : "label-wrapper";
                output.Attributes.SetAttribute("class", classValue);

                //add label
                output.Content.SetHtmlContent(tagBuilder);

                //add hint
                if (DisplayHint && For.Metadata.AdditionalValues.TryGetValue("NopResourceDisplayNameAttribute", out var value)
                    && value is NopResourceDisplayNameAttribute resourceDisplayName)
                {
                    var language = await _workContext.GetWorkingLanguageAsync();
                    var hintResource = await _localizationService
                        .GetResourceAsync($"{resourceDisplayName.ResourceKey}.Hint", language.Id, returnEmptyIfNotFound: true, logIfNotFound: false);

                    if (!string.IsNullOrEmpty(hintResource))
                    {
                        var hintContent = $"<div title='{WebUtility.HtmlEncode(hintResource)}' data-toggle='tooltip' class='ico-help'><i class='fas fa-question-circle'></i></div>";
                        output.Content.AppendHtml(hintContent);
                    }
                }
            }
        }

        #endregion
    }
}