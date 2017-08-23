using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("label",Attributes =ForAttributeName)]
    public class LabelTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.LabelTagHelper
    {
        private const string ForAttributeName = "asp-for";

        public LabelTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            output.Content.Append(":");
        }
    }
}
