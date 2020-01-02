using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// input tag helper
    /// </summary>
    [HtmlTargetElement("input", Attributes = ForAttributeName)]
    public class InputTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string DisabledAttributeName = "asp-disabled";

        /// <summary>
        /// Indicates whether the input is disabled
        /// </summary>
        [HtmlAttributeName(DisabledAttributeName)]
        public string IsDisabled { set; get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">IHTML generator</param>
        public InputTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //add disabled attribute
            bool.TryParse(IsDisabled, out bool disabled);
            if (disabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }

            base.Process(context, output);
        }
    }
}