using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-delete-confirmation" tag helper
    /// </summary>
    [HtmlTargetElement("nop-delete-confirmation", Attributes = MODEL_ID_ATTRIBUTE_NAME + "," + BUTTON_ID_ATTRIBUTE_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public partial class NopDeleteConfirmationTagHelper : TagHelper
    {
        #region Constants

        private const string MODEL_ID_ATTRIBUTE_NAME = "asp-model-id";
        private const string BUTTON_ID_ATTRIBUTE_NAME = "asp-button-id";
        private const string ACTION_ATTRIBUTE_NAME = "asp-action";

        #endregion

        #region Properties

        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Model identifier
        /// </summary>
        [HtmlAttributeName(MODEL_ID_ATTRIBUTE_NAME)]
        public string ModelId { get; set; }

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

        public NopDeleteConfirmationTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
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
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            if (string.IsNullOrEmpty(Action))
                Action = "Delete";

            var modelName = _htmlHelper.ViewData.ModelMetadata.ModelType.Name.ToLowerInvariant();
            if (!string.IsNullOrEmpty(Action))
                modelName += "-" + Action;
            var modalId = await new HtmlString(modelName + "-delete-confirmation").RenderHtmlContentAsync();

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = ModelId,
                ControllerName = _htmlHelper.ViewContext.RouteData.Values["controller"].ToString(),
                ActionName = Action,
                WindowId = modalId
            };

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add("id", modalId);
            output.Attributes.Add("class", "modal fade");
            output.Attributes.Add("tabindex", "-1");
            output.Attributes.Add("role", "dialog");
            output.Attributes.Add("aria-labelledby", $"{modalId}-title");

            var partialView = await _htmlHelper.PartialAsync("Delete", deleteConfirmationModel);
            output.Content.SetHtmlContent(partialView);

            //modal script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml(
                "$(document).ready(function () {" +
                    $"$('#{ButtonId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" +
                "});");
            var scriptTag = await script.RenderHtmlContentAsync();
            output.PostContent.SetHtmlContent(scriptTag);
        }

        #endregion
    }
}