using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers.Admin.NopTabs
{
    [HtmlTargetElement("nop-tabs", Attributes = IdAttributeName)]
    public class NopTabsTagHelper : TagHelper
    {
        private const string IdAttributeName = "id";

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

            var tabsClass = "nav-tabs-custom";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            //render tabs script
            if (output.Attributes.ContainsName("id"))
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("$(document).ready(function () {bindBootstrapTabSelectEvent('" + output.Attributes["id"].Value + "');});");
                output.PostContent.SetHtmlContent(script.RenderHtmlContent());
            }
        }
    }
}