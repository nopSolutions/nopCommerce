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
    /// nop-alert tag helper
    /// </summary>
    [HtmlTargetElement("nop-alert", Attributes = AlertNameId, TagStructure = TagStructure.WithoutEndTag)]
    public class NopAlertTagHelper : TagHelper
    {
        private const string AlertNameId = "asp-alert-id";
        private const string AlertMessageName = "asp-alert-message";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Alert identifier
        /// </summary>
        [HtmlAttributeName(AlertNameId)]
        public string AlertId { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Additional confirm text
        /// </summary>
        [HtmlAttributeName(AlertMessageName)]
        public string Message { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        /// <param name="htmlHelper">HTML helper</param>
        public NopAlertTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        {
            Generator = generator;
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            var modalId = new HtmlString(AlertId + "-action-alert").ToHtmlString();

            var actionAlertModel = new ActionAlertModel()
            {
                AlertId = AlertId,
                WindowId = modalId,
                AlertMessage = Message
            };

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add("id", modalId);
            output.Attributes.Add("class", "modal fade");
            output.Attributes.Add("tabindex", "-1");
            output.Attributes.Add("role", "dialog");
            output.Attributes.Add("aria-labelledby", $"{modalId}-title");
            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("Alert", actionAlertModel));

            //modal script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                            $"$('#{AlertId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" + "});");

            output.PostContent.SetHtmlContent(script.RenderHtmlContent());
        }
    }
}
