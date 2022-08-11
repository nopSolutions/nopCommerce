using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-tabs" tag helper
    /// </summary>
    [HtmlTargetElement("nop-tabs", Attributes = ID_ATTRIBUTE_NAME)]
    public partial class NopTabsTagHelper : TagHelper
    {
        #region Constants

        private const string ID_ATTRIBUTE_NAME = "id";
        private const string TAB_NAME_TO_SELECT_ATTRIBUTE_NAME = "asp-tab-name-to-select";
        private const string RENDER_SELECTED_TAB_INPUT_ATTRIBUTE_NAME = "asp-render-selected-tab-input";

        #endregion

        #region Properties

        /// <summary>
        /// Name of the tab which should be selected
        /// </summary>
        [HtmlAttributeName(TAB_NAME_TO_SELECT_ATTRIBUTE_NAME)]
        public string TabNameToSelect { set; get; }

        /// <summary>
        /// Indicates whether the tab is default
        /// </summary>
        [HtmlAttributeName(RENDER_SELECTED_TAB_INPUT_ATTRIBUTE_NAME)]
        public string RenderSelectedTabInput { set; get; }

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

        public NopTabsTagHelper(IHtmlHelper htmlHelper)
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

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //create context item
            var tabContext = new List<NopTabContextItem>();
            context.Items.Add(typeof(NopTabsTagHelper), tabContext);

            //get tab name which should be selected
            //first try get tab name from query
            var tabNameToSelect = ViewContext.HttpContext.Request.Query["tabNameToSelect"];

            //then from attribute
            if (!string.IsNullOrEmpty(TabNameToSelect))
                tabNameToSelect = TabNameToSelect;

            //then save tab name in tab context to access it in tab item
            if (!string.IsNullOrEmpty(tabNameToSelect))
                context.Items.Add("tabNameToSelect", tabNameToSelect);

            //execute child tag helpers
            await output.GetChildContentAsync();

            //tabs title
            var tabsTitlediv = new TagBuilder("div");
            tabsTitlediv.AddCssClass("card-header p-0 pt-1 border-bottom-0");

            var tabsTitle = new TagBuilder("ul");
            tabsTitle.AddCssClass("nav");
            tabsTitle.AddCssClass("nav-tabs");
            tabsTitle.Attributes.Add("id", "custom-content-above-tab");
            tabsTitle.Attributes.Add("role", "tablist");

            //tabs content
            var tabsContentout = new TagBuilder("div");
            tabsContentout.AddCssClass("card-body");

            var tabsContent = new TagBuilder("div");
            tabsContent.AddCssClass("tab-content");

            var outputiner = new TagBuilder("div");
            outputiner.AddCssClass("card card-primary card-outline card-outline-tabs");

            foreach (var tabItem in tabContext)
            {
                tabsTitle.InnerHtml.AppendHtml(tabItem.Title);
                tabsContent.InnerHtml.AppendHtml(tabItem.Content);
            }

            tabsTitlediv.InnerHtml.AppendHtml(await tabsTitle.RenderHtmlContentAsync());
            tabsContentout.InnerHtml.AppendHtml(await tabsContent.RenderHtmlContentAsync());
            //append data
            output.Content.AppendHtml(await tabsTitlediv.RenderHtmlContentAsync());
            output.Content.AppendHtml(await tabsContentout.RenderHtmlContentAsync());

            _ = bool.TryParse(RenderSelectedTabInput, out var renderSelectedTabInput);
            if (string.IsNullOrEmpty(RenderSelectedTabInput) || renderSelectedTabInput)
            {
                //render input contains selected tab name
                var selectedTabInput = new TagBuilder("input");
                selectedTabInput.Attributes.Add("type", "hidden");
                selectedTabInput.Attributes.Add("id", "selected-tab-name");
                selectedTabInput.Attributes.Add("name", "selected-tab-name");
                selectedTabInput.Attributes.Add("value", _htmlHelper.GetSelectedTabName());
                output.PreContent.SetHtmlContent(await selectedTabInput.RenderHtmlContentAsync());

                //render tabs script
                if (output.Attributes.ContainsName("id"))
                {
                    var script = new TagBuilder("script");
                    script.InnerHtml.AppendHtml(
                        "$(document).ready(function () {" +
                            "bindBootstrapTabSelectEvent('" + output.Attributes["id"].Value + "', 'selected-tab-name');" +
                        "});");
                    var scriptTag = await script.RenderHtmlContentAsync();
                    output.PostContent.SetHtmlContent(scriptTag);
                }
            }

            output.TagName = "div";

            var itemClass = "card card-primary card-outline card-outline-tabs nav-tabs-custom";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {itemClass}"
                : itemClass;

            output.Attributes.SetAttribute("class", classValue);
        }

        #endregion
    }

    /// <summary>
    /// "nop-tab" tag helper
    /// </summary>
    [HtmlTargetElement("nop-tab", ParentTag = "nop-tabs", Attributes = NAME_ATTRIBUTE_NAME)]
    public partial class NopTabTagHelper : TagHelper
    {
        #region Constants

        private const string NAME_ATTRIBUTE_NAME = "asp-name";
        private const string TITLE_ATTRIBUTE_NAME = "asp-title";
        private const string DEFAULT_ATTRIBUTE_NAME = "asp-default";

        #endregion

        #region Properties

        /// <summary>
        /// Title of the tab
        /// </summary>
        [HtmlAttributeName(TITLE_ATTRIBUTE_NAME)]
        public string Title { set; get; }

        /// <summary>
        /// Indicates whether the tab is default
        /// </summary>
        [HtmlAttributeName(DEFAULT_ATTRIBUTE_NAME)]
        public string IsDefault { set; get; }

        /// <summary>
        /// Name of the tab
        /// </summary>
        [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
        public string Name { set; get; }

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

        public NopTabTagHelper(IHtmlHelper htmlHelper)
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

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            _ = bool.TryParse(IsDefault, out var isDefaultTab);

            //get name of the tab should be selected
            var tabNameToSelect = context.Items.ContainsKey("tabNameToSelect")
                ? context.Items["tabNameToSelect"].ToString()
                : string.Empty;

            if (string.IsNullOrEmpty(tabNameToSelect))
                tabNameToSelect = _htmlHelper.GetSelectedTabName();

            if (string.IsNullOrEmpty(tabNameToSelect) && isDefaultTab)
                tabNameToSelect = Name;

            //tab title
            var tabTitle = new TagBuilder("li");
            tabTitle.AddCssClass("nav-item");
            var linkTag = new TagBuilder("a")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("data-tab-name", Name),
                    new KeyValuePair<string, string>("href", $"#{Name}"),
                    new KeyValuePair<string, string>("data-toggle", "pill"),
                    new KeyValuePair<string, string>("role", "tab"),
                    new KeyValuePair<string, string>("aria-selected", "false"),
                }
            };
            //active class
            if (tabNameToSelect == Name)
                linkTag.AddCssClass("nav-link active");
            else
                linkTag.AddCssClass("nav-link");

            linkTag.InnerHtml.AppendHtml(Title);

            //merge classes
            if (context.AllAttributes.ContainsName("class"))
                tabTitle.AddCssClass(context.AllAttributes["class"].Value.ToString());

            tabTitle.InnerHtml.AppendHtml(await linkTag.RenderHtmlContentAsync());

            //tab content
            var tabContenttop = new TagBuilder("div");
            tabContenttop.AddCssClass("card-body");

            var tabContent = new TagBuilder("div");
            tabContent.AddCssClass("tab-pane fade{0}");
            tabContent.Attributes.Add("id", Name);
            tabContent.Attributes.Add("role", "tabpanel");

            var childContent = await output.GetChildContentAsync();
            tabContent.InnerHtml.AppendHtml(childContent.GetContent());

            tabContenttop.InnerHtml.AppendHtml(await tabContent.RenderHtmlContentAsync());

            //active class
            if (tabNameToSelect == Name)
                tabContent.AddCssClass("active");

            //add to context
            var tabContext = (List<NopTabContextItem>)context.Items[typeof(NopTabsTagHelper)];
            tabContext.Add(new NopTabContextItem
            {
                Title = await tabTitle.RenderHtmlContentAsync(),
                Content = await tabContent.RenderHtmlContentAsync(),
                IsDefault = isDefaultTab
            });

            //generate nothing
            output.SuppressOutput();
        }

        #endregion
    }

    /// <summary>
    /// Tab context item
    /// </summary>
    public partial class NopTabContextItem
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { set; get; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { set; get; }

        /// <summary>
        /// Is default tab
        /// </summary>
        public bool IsDefault { set; get; }
    }
}