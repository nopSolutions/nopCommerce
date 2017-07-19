using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Admin.NopTabs
{
    [HtmlTargetElement("nop-tab-content", ParentTag = "nop-tabs")]
    public class NopTabContentHelper : TagHelper
    {
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

            var tabsClass = "tab-content";
            //merge classes
            var classValue = output.Attributes.ContainsName("class")
                ? $"{output.Attributes["class"].Value} {tabsClass}"
                : tabsClass;
            output.Attributes.SetAttribute("class", classValue);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}