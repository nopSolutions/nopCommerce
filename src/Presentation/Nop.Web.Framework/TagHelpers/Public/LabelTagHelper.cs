using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// "label" tag helper
    /// </summary>
    [HtmlTargetElement("label", Attributes = FOR_ATTRIBUTE_NAME)]
    public partial class LabelTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper
    {
        #region Constants

        protected const string FOR_ATTRIBUTE_NAME = "asp-for";
        protected const string POSTFIX_ATTRIBUTE_NAME = "asp-postfix";

        #endregion
        
        #region Ctor

        public LabelTagHelper(IHtmlGenerator generator) : base(generator)
        {
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

            output.Content.Append(Postfix);

            await base.ProcessAsync(context, output);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the input is disabled
        /// </summary>
        [HtmlAttributeName(POSTFIX_ATTRIBUTE_NAME)]
        public string Postfix { get; set; }

        #endregion
    }
}