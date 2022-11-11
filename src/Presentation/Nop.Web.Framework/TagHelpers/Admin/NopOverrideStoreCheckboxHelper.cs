using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-override-store-checkbox" tag helper
    /// </summary>
    [HtmlTargetElement("nop-override-store-checkbox", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public partial class NopOverrideStoreCheckboxHelper : TagHelper
    {
        #region Constants

        private const string FOR_ATTRIBUTE_NAME = "asp-for";
        private const string INPUT_ATTRIBUTE_NAME = "asp-input";
        private const string INPUT_2_ATTRIBUTE_NAME = "asp-input2";
        private const string STORE_SCOPE_ATTRIBUTE_NAME = "asp-store-scope";
        private const string PARENT_CONTAINER_ATTRIBUTE_NAME = "asp-parent-container";

        #endregion

        #region Properties

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The input #1
        /// </summary>
        [HtmlAttributeName(INPUT_ATTRIBUTE_NAME)]
        public ModelExpression Input { set; get; }

        /// <summary>
        /// The input #2
        /// </summary>
        [HtmlAttributeName(INPUT_2_ATTRIBUTE_NAME)]
        public ModelExpression Input2 { set; get; }

        /// <summary>
        ///The store scope
        /// </summary>
        [HtmlAttributeName(STORE_SCOPE_ATTRIBUTE_NAME)]
        public int StoreScope { set; get; }

        /// <summary>
        /// Parent container
        /// </summary>
        [HtmlAttributeName(PARENT_CONTAINER_ATTRIBUTE_NAME)]
        public string ParentContainer { set; get; }

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

        public NopOverrideStoreCheckboxHelper(IHtmlHelper htmlHelper)
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
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

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
                    dataInputSelector = "#" + ParentContainer + " input, #" + ParentContainer + " textarea, #" + ParentContainer + " select";

                if (dataInputIds.Any())
                    dataInputSelector = "#" + string.Join(", #", dataInputIds);

                var onClick = $"checkOverriddenStoreValue(this, '{dataInputSelector}')";

                var htmlAttributes = new
                {
                    @class = cssClass,
                    onclick = onClick,
                    data_for_input_selector = dataInputSelector
                };
                var htmlOutput = await _htmlHelper.CheckBox(For.Name, null, htmlAttributes).RenderHtmlContentAsync();
                output.Content.SetHtmlContent(htmlOutput);
            }
        }

        #endregion
    }
}