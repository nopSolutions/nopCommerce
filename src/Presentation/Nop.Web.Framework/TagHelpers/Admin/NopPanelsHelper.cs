using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// nop-panel tag helper
    /// </summary>
    [HtmlTargetElement("nop-panels", Attributes = IdAttributeName)]
    public class NopPanelsTagHelper : TagHelper
    {
        private const string IdAttributeName = "id";

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }

    /// <summary>
    /// "nop-panel tag helper
    /// </summary>
    [HtmlTargetElement("nop-panel", ParentTag = "nop-panels", Attributes = NameAttributeName)]
    public class NopPanelTagHelper : TagHelper
    {
        private const string NameAttributeName = "asp-name";
        private const string TitleAttributeName = "asp-title";
        private const string HideBlockAttributeNameAttributeName = "asp-hide-block-attribute-name";
        private const string IsHideAttributeName = "asp-hide";
        private const string IsAdvancedAttributeName = "asp-advanced";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Title of the panel
        /// </summary>
        [HtmlAttributeName(TitleAttributeName)]
        public string Title { set; get; }

        /// <summary>
        /// Name of the panel
        /// </summary>
        [HtmlAttributeName(NameAttributeName)]
        public string Name { set; get; }

        /// <summary>
        /// Name of the hide attribute of the panel
        /// </summary>
        [HtmlAttributeName(HideBlockAttributeNameAttributeName)]
        public string HideBlockAttributeName { set; get; }

        /// <summary>
        /// Indicates whether a block is hidden or not
        /// </summary>
        [HtmlAttributeName(IsHideAttributeName)]
        public bool IsHide { set; get; }

        /// <summary>
        /// Indicates whether a panel is advanced or not
        /// </summary>
        [HtmlAttributeName(IsAdvancedAttributeName)]
        public bool IsAdvanced { set; get; }

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
        public NopPanelTagHelper(IHtmlHelper htmlHelper)
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

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //create panel
            var panel = new TagBuilder("div");
            panel.AddCssClass("panel panel-default collapsible-panel");
            if (context.AllAttributes["asp-advanced"].Value.Equals(true))
            {
                panel.AddCssClass("advanced-setting");
            }

            //create panel heading and append title and icon to it
            var panelHeading = new TagBuilder("div");
            panelHeading.AddCssClass("panel-heading");
            panelHeading.Attributes.Add("data-hideAttribute", context.AllAttributes["asp-hide-block-attribute-name"].Value.ToString());

            if (context.AllAttributes["asp-hide"].Value.Equals(false))
            {
                panelHeading.AddCssClass("opened");
            }

            panelHeading.InnerHtml.AppendHtml($"<span>{context.AllAttributes["asp-title"].Value}</span>");

            var collapseIcon = new TagBuilder("i");
            collapseIcon.AddCssClass("fa");
            collapseIcon.AddCssClass(context.AllAttributes["asp-hide"].Value.Equals(true) ? "fa-plus" : "fa-minus");
            panelHeading.InnerHtml.AppendHtml(collapseIcon);

            //create inner panel container to toggle on click and add data to it
            var panelContainer = new TagBuilder("div");
            panelContainer.AddCssClass("panel-container");
            if (context.AllAttributes["asp-hide"].Value.Equals(true))
            {
                panelContainer.AddCssClass("collapsed");
            }

            panelContainer.InnerHtml.AppendHtml(output.GetChildContentAsync().Result.GetContent());

            //add heading and container to panel
            panel.InnerHtml.AppendHtml(panelHeading);
            panel.InnerHtml.AppendHtml(panelContainer);

            output.Content.AppendHtml(panel.RenderHtmlContent());
        }
    }
}