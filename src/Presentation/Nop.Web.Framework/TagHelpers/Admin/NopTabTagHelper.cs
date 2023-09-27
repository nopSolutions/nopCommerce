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
    /// "nop-tab" tag helper
    /// </summary>
    [HtmlTargetElement("nop-tab", ParentTag = "nop-tabs", Attributes = NAME_ATTRIBUTE_NAME)]
    public partial class NopTabTagHelper : TagHelper
    {
        #region Constants

        protected const string NAME_ATTRIBUTE_NAME = "asp-name";
        protected const string TITLE_ATTRIBUTE_NAME = "asp-title";
        protected const string DEFAULT_ATTRIBUTE_NAME = "asp-default";

        #endregion
        
        #region Fields

        protected readonly IHtmlHelper _htmlHelper;

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
    }
}