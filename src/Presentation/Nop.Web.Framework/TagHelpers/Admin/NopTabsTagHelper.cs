using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    [HtmlTargetElement("nop-tabs", Attributes = IdAttributeName)]
    public class NopTabsTagHelper : TagHelper
    {
        private const string IdAttributeName = "id";
        private const string TabNameToSelectAttributeName = "asp-tab-name-to-select";
        private const string RenderSelectedTabInputAttributeName = "asp-render-selected-tab-input";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Name of the tab which should be selected
        /// </summary>
        [HtmlAttributeName(TabNameToSelectAttributeName)]
        public string TabNameToSelect { set; get; }

        /// <summary>
        /// Indicates whether the tab is default
        /// </summary>
        [HtmlAttributeName(RenderSelectedTabInputAttributeName)]
        public string RenderSelectedTabInput { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopTabsTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

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

            //save tab name which should be selected
            if (!string.IsNullOrEmpty(TabNameToSelect))
                context.Items.Add("tabNameToSelect", TabNameToSelect);

            var tabsClass = "nav-tabs-custom";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            bool.TryParse(RenderSelectedTabInput, out bool renderSelectedTabInput);
            if (string.IsNullOrEmpty(RenderSelectedTabInput) || renderSelectedTabInput)
            {
                //render input contains selected tab name
                var selectedTabInput = new TagBuilder("input");
                selectedTabInput.Attributes.Add("type", "hidden");
                selectedTabInput.Attributes.Add("id", "selected-tab-name");
                selectedTabInput.Attributes.Add("name", "selected-tab-name");
                selectedTabInput.Attributes.Add("value", _htmlHelper.GetSelectedTabName());
                output.PreContent.SetHtmlContent(selectedTabInput.RenderHtmlContent());

                //render tabs script
                if (output.Attributes.ContainsName("id"))
                {
                    var script = new TagBuilder("script");
                    script.InnerHtml.AppendHtml("$(document).ready(function () {bindBootstrapTabSelectEvent('" + output.Attributes["id"].Value + "');});");
                    output.PostContent.SetHtmlContent(script.RenderHtmlContent());
                }
            }

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    [HtmlTargetElement("nop-tab-header", ParentTag = "nop-tabs")]
    public class NopTabHeaderHelper : TagHelper
    {
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

            var tabsClass = "nav nav-tabs";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "ul";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    [HtmlTargetElement("nop-tab-header-item", ParentTag = "nop-tab-header", Attributes = NameAttributeName)]
    public class NopTabHeaderItemTagHelper : TagHelper
    {
        private const string NameAttributeName = "asp-name";
        private const string DefaultAttributeName = "asp-default";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Name of the tab
        /// </summary>
        [HtmlAttributeName(NameAttributeName)]
        public string Name { set; get; }

        /// <summary>
        /// Indicates whether the tab is default
        /// </summary>
        [HtmlAttributeName(DefaultAttributeName)]
        public string IsDefault { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopTabHeaderItemTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

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

            bool.TryParse(IsDefault, out bool isDefaultTab);

            //get name of the tab should be selected
            var tabNameToSelect = context.Items.ContainsKey("tabNameToSelect")
                ? context.Items["tabNameToSelect"].ToString()
                : "";

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = _htmlHelper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = Name;

            var itemClass = string.Empty;
            if (tabNameToSelect == Name)
                itemClass = "active";

            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {itemClass}"
                : itemClass;
            output.Attributes.SetAttribute("class", classValue);

            var a = new TagBuilder("a")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("data-tab-name", Name),
                    new KeyValuePair<string, string>("href", $"#{Name}"),
                    new KeyValuePair<string, string>("data-toggle", "tab"),
                }
            };

            var content = output.GetChildContentAsync().Result;
            a.InnerHtml.AppendHtml(content.GetContent());

            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.SetHtmlContent(a);
        }
    }

    [HtmlTargetElement("nop-tab-content", ParentTag = "nop-tabs")]
    public class NopTabContentHelper : TagHelper
    {
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

            var tabsClass = "tab-content";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

    [HtmlTargetElement("nop-tab-content-item", ParentTag = "nop-tab-content", Attributes = NameAttributeName)]
    public class NopTabContentItemTagHelper : TagHelper
    {
        private const string NameAttributeName = "asp-name";
        private const string DefaultAttributeName = "asp-default";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Name of the tab
        /// </summary>
        [HtmlAttributeName(NameAttributeName)]
        public string Name { set; get; }

        /// <summary>
        /// Indicates whether the tab is default
        /// </summary>
        [HtmlAttributeName(DefaultAttributeName)]
        public string IsDefault { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NopTabContentItemTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

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

            bool.TryParse(IsDefault, out bool isDefaultTab);

            //get name of the tab should be selected
            var tabNameToSelect = context.Items.ContainsKey("tabNameToSelect")
                ? context.Items["tabNameToSelect"].ToString()
                : "";

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = _htmlHelper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = Name;

            var itemClass = "tab-pane";
            if (tabNameToSelect == Name)
                itemClass += " active";

            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {itemClass}"
                : itemClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "div";
            output.Attributes.SetAttribute("id", Name);
        }
    }
}