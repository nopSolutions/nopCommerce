using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-panel tag helper
    /// </summary>
    [HtmlTargetElement("nop-panel", Attributes = NAME_ATTRIBUTE_NAME)]
    public class NopPanelTagHelper : TagHelper
    {
        private const string NAME_ATTRIBUTE_NAME = "asp-name";
        private const string TITLE_ATTRIBUTE_NAME = "asp-title";
        private const string HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME = "asp-hide-block-attribute-name";
        private const string IS_HIDE_ATTRIBUTE_NAME = "asp-hide";
        private const string IS_ADVANCED_ATTRIBUTE_NAME = "asp-advanced";
        private const string PANEL_ICON_ATTRIBUTE_NAME = "asp-icon";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Title of the panel
        /// </summary>
        [HtmlAttributeName(TITLE_ATTRIBUTE_NAME)]
        public string Title { get; set; }

        /// <summary>
        /// Name of the panel
        /// </summary>
        [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Name of the hide attribute of the panel
        /// </summary>
        [HtmlAttributeName(HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME)]
        public string HideBlockAttributeName { get; set; }

        /// <summary>
        /// Indicates whether a block is hidden or not
        /// </summary>
        [HtmlAttributeName(IS_HIDE_ATTRIBUTE_NAME)]
        public bool IsHide { get; set; }

        /// <summary>
        /// Indicates whether a panel is advanced or not
        /// </summary>
        [HtmlAttributeName(IS_ADVANCED_ATTRIBUTE_NAME)]
        public bool IsAdvanced { get; set; }

        /// <summary>
        /// Panel icon
        /// </summary>
        [HtmlAttributeName(PANEL_ICON_ATTRIBUTE_NAME)]
        public string PanelIconIsAdvanced { get; set; }

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
            var panel = new TagBuilder("div")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("id", Name),
                    new KeyValuePair<string, string>("data-panel-name", Name),
                }
            };
            panel.AddCssClass("panel panel-default collapsible-panel");
            if (context.AllAttributes.ContainsName(IS_ADVANCED_ATTRIBUTE_NAME) && context.AllAttributes[IS_ADVANCED_ATTRIBUTE_NAME].Value.Equals(true))
            {
                panel.AddCssClass("advanced-setting");
            }

            //create panel heading and append title and icon to it
            var panelHeading = new TagBuilder("div");
            panelHeading.AddCssClass("panel-heading");
            panelHeading.Attributes.Add("data-hideAttribute", context.AllAttributes[HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME].Value.ToString());

            if (context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(false))
            {
                panelHeading.AddCssClass("opened");
            }

            if (context.AllAttributes.ContainsName(PANEL_ICON_ATTRIBUTE_NAME))
            {
                var panelIcon = new TagBuilder("i");
                panelIcon.AddCssClass("panel-icon");
                panelIcon.AddCssClass(context.AllAttributes[PANEL_ICON_ATTRIBUTE_NAME].Value.ToString());
                var iconContainer = new TagBuilder("div");
                iconContainer.AddCssClass("icon-container");
                iconContainer.InnerHtml.AppendHtml(panelIcon);
                panelHeading.InnerHtml.AppendHtml(iconContainer);
            }

            panelHeading.InnerHtml.AppendHtml($"<span>{context.AllAttributes[TITLE_ATTRIBUTE_NAME].Value}</span>");

            var collapseIcon = new TagBuilder("i");
            collapseIcon.AddCssClass("fa");
            collapseIcon.AddCssClass("toggle-icon");
            collapseIcon.AddCssClass(context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true) ? "fa-plus" : "fa-minus");
            panelHeading.InnerHtml.AppendHtml(collapseIcon);

            //create inner panel container to toggle on click and add data to it
            var panelContainer = new TagBuilder("div");
            panelContainer.AddCssClass("panel-container");
            if (context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true))
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