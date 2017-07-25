using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("nop-bb-code-editor", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class NopBBCodeEditorTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

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

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "bb-code-editor-wrapper");

            var storeLocation = EngineContext.Current.Resolve<IWebHelper>().GetStoreLocation();
            var bbEditorWebRoot = $"{storeLocation}js/";

            var script1 = new TagBuilder("script");
            script1.Attributes.Add("src", $"{storeLocation}js/bbeditor/ed.js");
            script1.Attributes.Add("type", MimeTypes.TextJavascript);

            var script2 = new TagBuilder("script");
            script2.Attributes.Add("language", "javascript");
            script2.Attributes.Add("type", MimeTypes.TextJavascript);
            script2.InnerHtml.AppendHtml($"edToolbar('{For.Name}','{bbEditorWebRoot}');");

            output.Content.AppendHtml(script1);
            output.Content.AppendHtml(script2);
        }
    }
}