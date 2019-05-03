using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// nop-override-store-checkbox tag helper
    /// </summary>
    [HtmlTargetElement("nop-override-store-checkbox", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class NopOverrideStoreCheckboxHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string InputAttributeName = "asp-input";
        private const string Input2AttributeName = "asp-input2";
        private const string StoreScopeAttributeName = "asp-store-scope";
        private const string ParentContainerAttributeName = "asp-parent-container";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The input #1
        /// </summary>
        [HtmlAttributeName(InputAttributeName)]
        public ModelExpression Input { set; get; }

        /// <summary>
        /// The input #2
        /// </summary>
        [HtmlAttributeName(Input2AttributeName)]
        public ModelExpression Input2 { set; get; }

        /// <summary>
        ///The store scope
        /// </summary>
        [HtmlAttributeName(StoreScopeAttributeName)]
        public int StoreScope { set; get; }

        /// <summary>
        /// Parent container
        /// </summary>
        [HtmlAttributeName(ParentContainerAttributeName)]
        public string ParentContainer { set; get; }

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
        public NopOverrideStoreCheckboxHelper(IHtmlHelper htmlHelper)
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

            //render only when a certain store is chosen
            if (StoreScope > 0)
            {
                //contextualize IHtmlHelper
                var viewContextAware = _htmlHelper as IViewContextAware;
                viewContextAware?.Contextualize(ViewContext);

                var dataInputIds = new List<string>();
                if (Input != null)
                    dataInputIds.Add(_htmlHelper.Id(Input.Name));
                if (Input2 != null)
                    dataInputIds.Add(_htmlHelper.Id(Input2.Name));

                const string cssClass = "multi-store-override-option";
                var dataInputSelector = "";
                if (!string.IsNullOrEmpty(ParentContainer))
                {
                    dataInputSelector = "#" + ParentContainer + " input, #" + ParentContainer + " textarea, #" + ParentContainer + " select";
                }
                if (dataInputIds.Any())
                {
                    dataInputSelector = "#" + string.Join(", #", dataInputIds);
                }
                var onClick = $"checkOverriddenStoreValue(this, '{dataInputSelector}')";

                var htmlAttributes = new
                {
                    @class = cssClass,
                    onclick = onClick,
                    data_for_input_selector = dataInputSelector
                };
                var htmlOutput = _htmlHelper.CheckBox(For.Name, null, htmlAttributes);
                output.Content.SetHtmlContent(htmlOutput.RenderHtmlContent());
            }
        }
    }
}