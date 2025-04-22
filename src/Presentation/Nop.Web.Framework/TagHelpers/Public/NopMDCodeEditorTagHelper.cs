using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;

namespace Nop.Web.Framework.TagHelpers.Public;

/// <summary>
/// "nop-md-code-editor" tag helper
/// </summary>
[HtmlTargetElement("nop-md-code-editor", Attributes = FOR_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
public partial class NopMDCodeEditorTagHelper : TagHelper
{
    #region Constants

    protected const string FOR_ATTRIBUTE_NAME = "asp-for";

    #endregion

    #region Fields

    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public NopMDCodeEditorTagHelper(IWebHelper webHelper)
    {
        _webHelper = webHelper;
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

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "markdown-editor-container");
        output.Attributes.SetAttribute("id", "md-editor");

        var storeLocation = _webHelper.GetStoreLocation();
        var mdEditorWebRoot = $"{storeLocation}js/mdeditor/icons/";

        // Add marked.js
        var markedScript = new TagBuilder("script");
        markedScript.Attributes.Add("src", $"{storeLocation}lib_npm/marked/marked.min.js");

        // Add editor stylesheet
        var styleLink = new TagBuilder("link");
        styleLink.Attributes.Add("rel", "stylesheet");
        styleLink.Attributes.Add("href", $"{storeLocation}js/mdeditor/style.css");

        var editorHtml = new TagBuilder("div");
        editorHtml.InnerHtml.AppendHtml(@$"
                <div class='editor-header'>
                    <div class='editor-tabs'>
                        <button type='button' class='tab-button active' data-tab='write'>Write</button>
                        <button type='button' class='tab-button' data-tab='preview'>Preview</button>
                    </div>
                    <div class='toolbar' id='editor-toolbar'></div>
                </div>
                <div class='editor-content'>
                    <div class='tab-panel active' id='write-panel'>
                        <textarea id='{For.Name}' name='{For.Name}' class='forum-post-text'>{For.Model}</textarea>
                        <span class='field-validation-valid' data-valmsg-for='{For.Name}' data-valmsg-replace='true'></span>
                    </div>
                    <div class='tab-panel' id='preview-panel'>
                        <div class='preview-output' id='html-output'></div>
                    </div>
                </div>");

        // Add the resources and content in order
        output.Content.AppendHtml(markedScript);
        output.Content.AppendHtml(styleLink);
        output.Content.AppendHtml(editorHtml.InnerHtml);

        var mdEditorScript = new TagBuilder("script");
        mdEditorScript.Attributes.Add("src", $"{storeLocation}js/mdeditor/script.js");

        var initialScript = new TagBuilder("script");
        initialScript.Attributes.Add("language", "javascript");
        initialScript.InnerHtml.AppendHtml($"document.addEventListener('DOMContentLoaded', () => {{ initializeMarkdownEditor('{For.Name}','{mdEditorWebRoot}'); setupTabs('md-editor', '{For.Name}');}});");

        output.Content.AppendHtml(mdEditorScript);
        output.Content.AppendHtml(initialScript);

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    #endregion
}
