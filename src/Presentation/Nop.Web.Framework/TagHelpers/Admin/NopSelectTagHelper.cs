using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Dynamic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    [HtmlTargetElement("nop-select", Attributes = ForAttributeName)]
    public class NopSelectTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string ItemsAttributeName = "asp-items";
        private const string DisabledAttributeName = "asp-multiple";
        private const string RequiredAttributeName = "asp-required";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Items for a dropdown list
        /// </summary>
        [HtmlAttributeName(ItemsAttributeName)]
        public IEnumerable<SelectListItem> Items { set; get; }

        /// <summary>
        /// Indicates whether the field is required
        /// </summary>
        [HtmlAttributeName(RequiredAttributeName)]
        public string IsRequired { set; get; }

        /// <summary>
        /// Indicates whether the input is multiple
        /// </summary>
        [HtmlAttributeName(DisabledAttributeName)]
        public string IsMultiple { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopSelectTagHelper(IHtmlHelper htmlHelper)
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

            //required asterisk
            bool.TryParse(IsRequired, out bool required);
            if (required)
            {
                output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
                output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
            }

            //contextualize IHtmlHelper
            var viewContextAware
                = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //generate editor
            IHtmlContent selectList;
            bool.TryParse(IsMultiple, out bool multiple);
            if (multiple)
            {
                selectList = _htmlHelper.Editor(For.Name, "MultiSelect", Items != null && Items.Any() ? new {SelectList = Items} : null);
            }
            else
            {
                selectList = _htmlHelper.DropDownList(For.Name, Items, new { @class = "form-control" });
            }

            output.Content.SetHtmlContent(selectList.RenderHtmlContent());
        }
    }
}