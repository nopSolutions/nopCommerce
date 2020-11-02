using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "nop-panels" tag helper
    /// </summary>
    [HtmlTargetElement("nop-panels", Attributes = ID_ATTRIBUTE_NAME)]
    public class NopPanelsTagHelper : TagHelper
    {
        #region Constants

        private const string ID_ATTRIBUTE_NAME = "id";

        #endregion

        #region Properties

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion
    }
}