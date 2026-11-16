using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Models.Cms;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-card" tag helper
/// </summary>
[HtmlTargetElement("nop-card", Attributes = NAME_ATTRIBUTE_NAME)]
public partial class NopCardTagHelper : TagHelper
{
    #region Constants

    protected const string NAME_ATTRIBUTE_NAME = "asp-name";
    protected const string TITLE_ATTRIBUTE_NAME = "asp-title";
    protected const string HIDE_BLOCK_ATTRIBUTE_NAME_ATTRIBUTE_NAME = "asp-hide-block-attribute-name";
    protected const string IS_HIDE_ATTRIBUTE_NAME = "asp-hide";
    protected const string IS_ADVANCED_ATTRIBUTE_NAME = "asp-advanced";
    protected const string CARD_ICON_ATTRIBUTE_NAME = "asp-icon";


    #endregion

    #region Fields

    protected readonly IViewComponentHelper _viewComponentHelper;
    protected readonly IWidgetModelFactory _widgetModelFactory;

    #endregion

    #region Ctor

    public NopCardTagHelper(IViewComponentHelper viewComponentHelper, IWidgetModelFactory widgetModelFactory)
    {
        _viewComponentHelper = viewComponentHelper;
        _widgetModelFactory = widgetModelFactory;
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
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //contextualize IHtmlHelper
        var viewContextAware = _viewComponentHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        //create card
        var card = new TagBuilder("div");
        card.Attributes.Add("id", Name);
        card.Attributes.Add("data-card-name", Name);
        card.AddCssClass("card card-secondary card-outline");

        if (IsAdvanced)
            card.AddCssClass("advanced-setting");

        if (!string.IsNullOrEmpty(HideBlockAttributeName))
            card.Attributes.Add("data-hideAttribute", HideBlockAttributeName);

        if (IsHide)
            card.AddCssClass("collapsed-card");

        //create card heading and append title and icon to it
        var cardHeader = new TagBuilder("div");
        cardHeader.AddCssClass("card-header");

        //card title
        var cardTitle = new TagBuilder("div");
        cardTitle.AddCssClass("card-title");

        if (!string.IsNullOrEmpty(CardIcon))
        {
            var cardIcon = new TagBuilder("i");
            cardIcon.AddCssClass(CardIcon);
            cardTitle.InnerHtml.AppendHtml(cardIcon);
            cardTitle.InnerHtml.Append(" ");
        }

        cardTitle.InnerHtml.Append(Title ?? string.Empty);
        cardHeader.InnerHtml.AppendHtml(cardTitle);

        //collapse button
        var collapseIconExpand = new TagBuilder("i");
        collapseIconExpand.Attributes.Add("data-lte-icon", "expand");
        collapseIconExpand.AddCssClass("bi bi-plus-lg");

        var collapseIconCollapse = new TagBuilder("i");
        collapseIconCollapse.Attributes.Add("data-lte-icon", "collapse");
        collapseIconCollapse.AddCssClass("bi bi-dash-lg");

        var cardButton = new TagBuilder("button");
        cardButton.AddCssClass("btn btn-tool");
        cardButton.MergeAttribute("type", "button");
        cardButton.MergeAttribute("data-lte-toggle", "card-collapse");
        cardButton.MergeAttribute("aria-label", "Collapse card");
        cardButton.InnerHtml.AppendHtml(collapseIconExpand);
        cardButton.InnerHtml.AppendHtml(collapseIconCollapse);

        var cardTools = new TagBuilder("div");
        cardTools.AddCssClass("card-tools");
        cardTools.InnerHtml.AppendHtml(cardButton);

        cardHeader.InnerHtml.AppendHtml(cardTools);

        // create card body
        var cardBody = new TagBuilder("div");
        cardBody.AddCssClass("card-body");

        var modelsBefore = await _widgetModelFactory.PrepareRenderWidgetModelAsync(
            AdminWidgetZones.CardBefore,
            new WidgetNopCardModel { CardName = Name });

        foreach (var model in modelsBefore)
        {
            var content = await _viewComponentHelper.InvokeAsync(model.WidgetViewComponent, model.WidgetViewComponentArguments);
            cardBody.InnerHtml.AppendHtml(content);
        }

        var childContent = await output.GetChildContentAsync();
        cardBody.InnerHtml.AppendHtml(childContent);

        var modelsAfter = await _widgetModelFactory.PrepareRenderWidgetModelAsync(AdminWidgetZones.CardAfter, new WidgetNopCardModel { CardName = Name });

        foreach (var model in modelsAfter)
        {
            var content = await _viewComponentHelper.InvokeAsync(model.WidgetViewComponent, model.WidgetViewComponentArguments);
            cardBody.InnerHtml.AppendHtml(content);
        }

        // assemble card
        card.InnerHtml.AppendHtml(cardHeader);
        card.InnerHtml.AppendHtml(cardBody);

        output.Content.AppendHtml(await card.RenderHtmlContentAsync());
    }

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
    public string CardIcon { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}