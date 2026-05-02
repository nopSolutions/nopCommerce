using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Shared;

/// <summary>
/// "nop-antiforgery-token" tag helper
/// </summary>
[HtmlTargetElement("nop-antiforgery-token", TagStructure = TagStructure.WithoutEndTag)]
public partial class NopAntiForgeryTokenTagHelper : TagHelper
{
    #region Properties

    protected IHtmlGenerator Generator { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion

    #region Ctor

    public NopAntiForgeryTokenTagHelper(IHtmlGenerator generator)
    {
        Generator = generator;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Asynchronously executes the tag helper with the given context and output
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(output);

        //clear the output
        output.SuppressOutput();

        //generate antiforgery
        var antiforgeryTag = Generator.GenerateAntiforgery(ViewContext);
        if (antiforgeryTag != null)
            output.Content.SetHtmlContent(antiforgeryTag);

        return Task.CompletedTask;
    }

    #endregion
}