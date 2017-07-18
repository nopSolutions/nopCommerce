using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Admin
{
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

        public NopOverrideStoreCheckboxHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
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
                string dataInputSelector = "";
                if (!String.IsNullOrEmpty(ParentContainer))
                {
                    dataInputSelector = "#" + ParentContainer + " input, #" + ParentContainer + " textarea, #" + ParentContainer + " select";
                }
                if (dataInputIds.Any())
                {
                    dataInputSelector = "#" + String.Join(", #", dataInputIds);
                }
                var onClick = string.Format("checkOverriddenStoreValue(this, '{0}')", dataInputSelector);

                var htmlAttributes = new
                {
                    @class = cssClass,
                    onclick = onClick,
                    data_for_input_selector = dataInputSelector
                };
                var s = _htmlHelper.CheckBox(For.Name, null, htmlAttributes);
                string htmlOutput;
                using (var writer = new StringWriter())
                {
                    s.WriteTo(writer, HtmlEncoder.Default);
                    htmlOutput = writer.ToString();
                }

                output.Content.SetHtmlContent(htmlOutput);
            }
        }
    }
}