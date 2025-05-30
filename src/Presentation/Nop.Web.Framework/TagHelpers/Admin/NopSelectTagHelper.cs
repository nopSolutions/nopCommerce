﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin;

/// <summary>
/// "nop-select" tag helper
/// </summary>
[HtmlTargetElement("nop-select", TagStructure = TagStructure.WithoutEndTag)]
public partial class NopSelectTagHelper : TagHelper
{
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string NAME_ATTRIBUTE_NAME = "asp-for-name";
    protected const string ITEMS_ATTRIBUTE_NAME = "asp-items";
    protected const string MULTIPLE_ATTRIBUTE_NAME = "asp-multiple";
    protected const string REQUIRED_ATTRIBUTE_NAME = "asp-required";

    #endregion

    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly IHtmlHelper _htmlHelper;

    #endregion

    #region Ctor

    public NopSelectTagHelper(AdminAreaSettings adminAreaSettings, IHtmlHelper htmlHelper)
    {
        _adminAreaSettings = adminAreaSettings;
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

        //clear the output
        output.SuppressOutput();

        //required asterisk
        if (bool.TryParse(IsRequired, out var required) && required)
        {
            output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
            output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
        }

        //contextualize IHtmlHelper
        var viewContextAware = _htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        //get htmlAttributes object
        var htmlAttributes = new Dictionary<string, object>();
        var attributes = context.AllAttributes;
        foreach (var attribute in attributes)
        {
            if (!attribute.Name.Equals(FOR_ATTRIBUTE_NAME) &&
                !attribute.Name.Equals(NAME_ATTRIBUTE_NAME) &&
                !attribute.Name.Equals(ITEMS_ATTRIBUTE_NAME) &&
                !attribute.Name.Equals(MULTIPLE_ATTRIBUTE_NAME) &&
                !attribute.Name.Equals(REQUIRED_ATTRIBUTE_NAME))
            {
                htmlAttributes.Add(attribute.Name, attribute.Value);
            }
        }

        //generate editor
        var tagName = For != null ? For.Name : Name;
        if (!string.IsNullOrEmpty(tagName))
        {
            var templateName = "Select";

            if (bool.TryParse(IsMultiple, out var multiple) && multiple)
                templateName = $"Multi{templateName}";

            IHtmlContent selectList;
            var modelType = For?.ModelExplorer.ModelType;
            var additionalData = new { htmlAttributes, SelectList = Items, MinimumItemsForSearch = _adminAreaSettings.MinimumDropdownItemsForSearch };

            if (modelType is null || new[] { typeof(List<string>), typeof(string) }.Contains(modelType))
            {
                selectList = _htmlHelper.Editor(tagName, $"{templateName}String", additionalData);
            }
            else if (new[] { typeof(List<int>), typeof(int) }.Contains(modelType))
            {
                selectList = _htmlHelper.Editor(tagName, templateName, additionalData);
            }
            else
            {
                if (!htmlAttributes.TryAdd("class", "form-control"))
                    htmlAttributes["class"] += " form-control";

                selectList = _htmlHelper.DropDownList(tagName, Items, htmlAttributes);
            }

            output.Content.SetHtmlContent(await selectList.RenderHtmlContentAsync());
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// Name for a dropdown list
    /// </summary>
    [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
    public string Name { get; set; }

    /// <summary>
    /// Items for a dropdown list
    /// </summary>
    [HtmlAttributeName(ITEMS_ATTRIBUTE_NAME)]
    public IEnumerable<SelectListItem> Items { set; get; } = new List<SelectListItem>();

    /// <summary>
    /// Indicates whether the field is required
    /// </summary>
    [HtmlAttributeName(REQUIRED_ATTRIBUTE_NAME)]
    public string IsRequired { set; get; }

    /// <summary>
    /// Indicates whether the input is multiple
    /// </summary>
    [HtmlAttributeName(MULTIPLE_ATTRIBUTE_NAME)]
    public string IsMultiple { set; get; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}