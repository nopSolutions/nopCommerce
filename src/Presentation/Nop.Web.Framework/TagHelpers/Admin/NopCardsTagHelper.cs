using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// nop-card tag helper
    /// </summary>
    [HtmlTargetElement("nop-cards", Attributes = ID_ATTRIBUTE_NAME)]
    public class NopCardsTagHelper : TagHelper
    {
        private const string ID_ATTRIBUTE_NAME = "id";

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}