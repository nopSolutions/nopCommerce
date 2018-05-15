using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// script tag helper
    /// </summary>
    [HtmlTargetElement("script", Attributes = LocationAttributeName)]
    public class ScriptTagHelper : TagHelper
    {
        private const string LocationAttributeName = "asp-location";
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LocationAttributeName)]
        public ResourceLocation Location { set; get; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="htmlHelper">HTML helper</param>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        public ScriptTagHelper(IHtmlHelper htmlHelper, IHttpContextAccessor httpContextAccessor)
        {
            this._htmlHelper = htmlHelper;
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //allow developers to ignore this parameter
            //for example, when a request is made using AJAX and is rendered without head and footer
            if (_httpContextAccessor.HttpContext.Items["nop.IgnoreScriptTagLocation"] != null &&
                Convert.ToBoolean(_httpContextAccessor.HttpContext.Items["nop.IgnoreScriptTagLocation"]))
                return;

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //get JavaScript
            var script = output.GetChildContentAsync().Result.GetContent();

            //build script tag
            var scriptTag = new TagBuilder("script");
            scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

            //merge attributes
            foreach (var attribute in context.AllAttributes)
                if (!attribute.Name.StartsWith("asp-"))
                    scriptTag.Attributes.Add(attribute.Name, attribute.Value.ToString());

            _htmlHelper.AddInlineScriptParts(Location, scriptTag.RenderHtmlContent());

            //generate nothing
            output.SuppressOutput();
        }
    }

}