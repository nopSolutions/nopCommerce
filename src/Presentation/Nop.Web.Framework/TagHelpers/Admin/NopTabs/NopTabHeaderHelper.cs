using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Admin.NopTabs
{
    [HtmlTargetElement("nop-tab-header", ParentTag = "nop-tabs")]
    public class NopTabHeaderHelper : TagHelper
    {
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

            var tabsClass = "nav nav-tabs";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "ul";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}