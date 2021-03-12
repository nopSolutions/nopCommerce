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
    /// "nop-card" tag helper
    /// </summary>
    [HtmlTargetElement("nop-card", Attributes = NAME_ATTRIBUTE_NAME)]
    public class NopCardTagHelper : TagHelper
    {
        #region Constants

        private const string NAME_ATTRIBUTE_NAME = "asp-name";
        private const string TITLE_ATTRIBUTE_NAME = "asp-title";
        private const string HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME = "asp-hide-block-attribute-name";
        private const string IS_HIDE_ATTRIBUTE_NAME = "asp-hide";
        private const string IS_ADVANCED_ATTRIBUTE_NAME = "asp-advanced";
        private const string CARD_ICON_ATTRIBUTE_NAME = "asp-icon";

        #endregion

        #region Properties

        /// <summary>
        /// Title of the card
        /// </summary>
        [HtmlAttributeName(TITLE_ATTRIBUTE_NAME)]
        public string Title { get; set; }

        /// <summary>
        /// Name of the card
        /// </summary>
        [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Name of the hide attribute of the card
        /// </summary>
        [HtmlAttributeName(HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME)]
        public string HideBlockAttributeName { get; set; }

        /// <summary>
        /// Indicates whether a block is hidden or not
        /// </summary>
        [HtmlAttributeName(IS_HIDE_ATTRIBUTE_NAME)]
        public bool IsHide { get; set; }

        /// <summary>
        /// Indicates whether a card is advanced or not
        /// </summary>
        [HtmlAttributeName(IS_ADVANCED_ATTRIBUTE_NAME)]
        public bool IsAdvanced { get; set; }

        /// <summary>
        /// Card icon
        /// </summary>
        [HtmlAttributeName(CARD_ICON_ATTRIBUTE_NAME)]
        public string CardIconIsAdvanced { get; set; }

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

        public NopCardTagHelper(IHtmlHelper htmlHelper)
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

            //create card
            var card = new TagBuilder("div")
            {
                Attributes =
                {
                    new KeyValuePair<string, string>("id", Name),
                    new KeyValuePair<string, string>("data-card-name", Name),
                }
            };
            card.AddCssClass("card card-secondary card-outline");
            if (context.AllAttributes.ContainsName(IS_ADVANCED_ATTRIBUTE_NAME)
                && context.AllAttributes[IS_ADVANCED_ATTRIBUTE_NAME].Value.Equals(true))
            {
                card.AddCssClass("advanced-setting");
            }
            card.Attributes.Add("data-hideAttribute", context.AllAttributes[HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME].Value.ToString());

            if (context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true))
            {
                card.AddCssClass("collapsed-card");
            }

            //create card heading and append title and icon to it
            var cardHeading = new TagBuilder("div");
            cardHeading.AddCssClass("card-header with-border clearfix");

            if (context.AllAttributes.ContainsName(CARD_ICON_ATTRIBUTE_NAME))
            {
                var cardIcon = new TagBuilder("i");
                cardIcon.AddCssClass(context.AllAttributes[CARD_ICON_ATTRIBUTE_NAME].Value.ToString());
                var iconContainer = new TagBuilder("div");
                iconContainer.AddCssClass("card-title");
                iconContainer.InnerHtml.AppendHtml(cardIcon);
                iconContainer.InnerHtml.AppendHtml($"{context.AllAttributes[TITLE_ATTRIBUTE_NAME].Value}");
                cardHeading.InnerHtml.AppendHtml(iconContainer);
            }

            var collapseIcon = new TagBuilder("i");
            collapseIcon.AddCssClass("fa");
            collapseIcon.AddCssClass("toggle-icon");
            collapseIcon.AddCssClass(context.AllAttributes[IS_HIDE_ATTRIBUTE_NAME].Value.Equals(true) ? "fa-plus" : "fa-minus");

            var cardToolContainer = new TagBuilder("div");
            cardToolContainer.AddCssClass("card-tools float-right");
            var cardbtnContainer = new TagBuilder("button");

            cardbtnContainer.AddCssClass("btn btn-tool");
            cardbtnContainer.MergeAttribute("type", "button");
            cardbtnContainer.MergeAttribute("data-card-widget", "collapse");
            cardbtnContainer.InnerHtml.AppendHtml(collapseIcon);

            cardToolContainer.InnerHtml.AppendHtml(cardbtnContainer);
            cardHeading.InnerHtml.AppendHtml(cardToolContainer);

            //add heading and container to card
            card.InnerHtml.AppendHtml(cardHeading);
            var childContent = await output.GetChildContentAsync();
            card.InnerHtml.AppendHtml(childContent.GetContent());
            output.Content.AppendHtml(await card.RenderHtmlContentAsync());
        }

        #endregion
    }
}