using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Framework.TagHelpers.Public;

/// <summary>
/// "label" tag helper
/// </summary>
[HtmlTargetElement("label", Attributes = FOR_ATTRIBUTE_NAME)]
public partial class LabelTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper
{
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string POSTFIX_ATTRIBUTE_NAME = "asp-postfix";

    #endregion

    #region Fields

    protected readonly ILocalizationService _localizationService;

    #endregion

    #region Ctor

    public LabelTagHelper(IHtmlGenerator generator, ILocalizationService localizationService) : base(generator)
    {
        _localizationService = localizationService;
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

        var type = For.Metadata.ContainerType;
        var propertyName = For.Metadata.Name;

        if (type != null && !string.IsNullOrEmpty(propertyName))
        {
            var resourceName = type.GetProperty(propertyName)
                ?.GetCustomAttributes(typeof(NopResourceDisplayNameAttribute), true)
                .OfType<NopResourceDisplayNameAttribute>()
                .FirstOrDefault()?.ResourceKey;

            if (!string.IsNullOrEmpty(resourceName))
                //get locale resource value
                resourceValue = await _localizationService.GetResourceAsync(resourceName);
        }

        if (!string.IsNullOrEmpty(resourceValue))
            resourceValue += Postfix;

        //generate label
        var tagBuilder = Generator.GenerateLabel(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            labelText: resourceValue,
            htmlAttributes: null);

        if (tagBuilder != null)
        {
            //add label
            output.Content.SetHtmlContent(tagBuilder);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether the input is disabled
    /// </summary>
    [HtmlAttributeName(POSTFIX_ATTRIBUTE_NAME)]
    public string Postfix { get; set; }

    #endregion
}