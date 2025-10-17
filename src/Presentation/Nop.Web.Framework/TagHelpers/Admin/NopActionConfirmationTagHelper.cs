﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-action-confirmation" tag helper
/// </summary>
[HtmlTargetElement("nop-action-confirmation", Attributes = BUTTON_ID_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopActionConfirmationTagHelper : TagHelper
{
    #region Constants

    protected const string BUTTON_ID_ATTRIBUTE_NAME = "asp-button-id";
    protected const string ACTION_ATTRIBUTE_NAME = "asp-action";
    protected const string ADDITIONAL_CONFIRM_TEXT = "asp-additional-confirm";

    #endregion

    #region Fields

    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopActionConfirmationTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
    {
        Generator = generator;
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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        //contextualize IHtmlHelper
        var viewContextAware = _htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        if (string.IsNullOrEmpty(Action))
            Action = _htmlHelper.ViewContext.RouteData.Values["action"].ToString();

        var modalId = await new HtmlString(ButtonId + "-action-confirmation").RenderHtmlContentAsync();

        var actionConfirmationModel = new ActionConfirmationModel()
        {
            ControllerName = _htmlHelper.ViewContext.RouteData.Values["controller"].ToString(),
            ActionName = Action,
            WindowId = modalId,
            ButtonId = ButtonId,
            AdditonalConfirmText = ConfirmText
        };

        //tag details
        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.Add("id", modalId);
        output.Attributes.Add("class", "modal fade");
        output.Attributes.Add("tabindex", "-1");
        output.Attributes.Add("role", "dialog");
        output.Attributes.Add("aria-labelledby", $"{modalId}-title");

        var partialView = await _htmlHelper.PartialAsync("Confirm", actionConfirmationModel);
        output.Content.SetHtmlContent(partialView);
    }

    #endregion

    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// Button identifier
    /// </summary>
    [HtmlAttributeName(BUTTON_ID_ATTRIBUTE_NAME)]
    public string ButtonId { get; set; }

    /// <summary>
    /// Delete action name
    /// </summary>
    [HtmlAttributeName(ACTION_ATTRIBUTE_NAME)]
    public string Action { get; set; }

    /// <summary>
    /// Additional confirm text
    /// </summary>
    [HtmlAttributeName(ADDITIONAL_CONFIRM_TEXT)]
    public string ConfirmText { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}