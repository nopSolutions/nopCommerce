using System;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            bool.TryParse(IsDisabled, out var disabled);
            if (disabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }
            try
            {
                base.Process(context, output);
            }
            catch
            {
                //If the passed values differ in data type according to the model, we should try to initialize the component with a default value. 
                //If this is not possible, then we suppress the generation of html for this imput.
                try
                {
                    ViewContext.ModelState[For.Name].RawValue = Activator.CreateInstance(For.ModelExplorer.ModelType);
                    base.Process(context, output);
                }
                catch
                {
                    output.SuppressOutput();
                }
            }
        }
    }
}