﻿using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-label" tag helper
/// </summary>
[HtmlTargetElement("nop-label", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopLabelTagHelper : TagHelper
{
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string DISPLAY_HINT_ATTRIBUTE_NAME = "asp-display-hint";

    #endregion

    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public NopLabelTagHelper(IHtmlGenerator generator, ILocalizationService localizationService, IWorkContext workContext)
    {
        Generator = generator;
        _localizationService = localizationService;
        _workContext = workContext;
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

        string resourceValue = null;
        string resourceName = null;
        var type = For.Metadata.ContainerType;
        var propertyName = For.Metadata.Name;

        if (type != null && !string.IsNullOrEmpty(propertyName))
        {
            resourceName = type.GetProperty(propertyName)
                ?.GetCustomAttributes(typeof(NopResourceDisplayNameAttribute), true)
                .OfType<NopResourceDisplayNameAttribute>()
                .FirstOrDefault()?.ResourceKey;
            
            if (!string.IsNullOrEmpty(resourceName))
                //get locale resource value
                resourceValue = await _localizationService.GetResourceAsync(resourceName);
        }

        //generate label
        var tagBuilder = Generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, resourceValue, new { @class = "col-form-label" });
        if (tagBuilder != null)
        {
            //create a label wrapper
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} label-wrapper"
                : "label-wrapper";
            output.Attributes.SetAttribute("class", classValue);

            //add label
            output.Content.SetHtmlContent(tagBuilder);

            //add hint
            if (DisplayHint && !string.IsNullOrEmpty(resourceName))
            {
                var language = await _workContext.GetWorkingLanguageAsync();
                var hintResource = await _localizationService
                    .GetResourceAsync($"{resourceName}.Hint", language.Id, returnEmptyIfNotFound: true, logIfNotFound: false);

                if (!string.IsNullOrEmpty(hintResource))
                {
                    var hintContent = $"<div title='{WebUtility.HtmlEncode(hintResource)}' data-toggle='tooltip' class='ico-help'><i class='fas fa-circle-question'></i></div>";
                    output.Content.AppendHtml(hintContent);
                }
            }
        }
    }

    #endregion

    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// Indicates whether the hint should be displayed
    /// </summary>
    [HtmlAttributeName(DISPLAY_HINT_ATTRIBUTE_NAME)]
    public bool DisplayHint { get; set; } = true;

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}