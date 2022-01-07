using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// CSS bundling tag helper
    /// </summary>
    [HtmlTargetElement(LINK_TAG_NAME, Attributes = "[rel=stylesheet]")]
    public class NopLinkTagHelper : UrlResolutionTagHelper
    {
        #region Constants

        private const string LINK_TAG_NAME = "link";
        private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
        private const string HREF_ATTRIBUTE_NAME = "href";

        #endregion

        #region Fields
        private readonly INopHtmlHelper _nopHtmlHelper;

        #endregion

        #region Ctor

        public NopLinkTagHelper(HtmlEncoder htmlEncoder,
            INopHtmlHelper nopHtmlHelper,
            IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory, htmlEncoder)
        {
            _nopHtmlHelper = nopHtmlHelper;
        }

        #endregion

        #region Utils

        private void ProcessSrcAttribute(TagHelperContext context, TagHelperOutput output)
        {
            // Pass through attribute that is also a well-known HTML attribute.
            if (Href != null)
                output.CopyHtmlAttribute(HREF_ATTRIBUTE_NAME, context);

            // If there's no "src" attribute in output.Attributes this will noop.
            ProcessUrlAttribute(HREF_ATTRIBUTE_NAME, output);

            // Retrieve the TagHelperOutput variation of the "href" attribute in case other TagHelpers in the
            // pipeline have touched the value. If the value is already encoded this ScriptTagHelper may
            // not function properly.
            if (output.Attributes[HREF_ATTRIBUTE_NAME]?.Value is string hrefAttribute)
                Href = hrefAttribute;
        }

        #endregion

        #region Methods

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.Attributes.SetAttribute("type", MimeTypes.TextCss);
            output.TagMode = TagMode.SelfClosing;

            ProcessSrcAttribute(context, output);

            _nopHtmlHelper.AddCssFileParts(Href, string.Empty, ExcludeFromBundle);

            output.SuppressOutput();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A value indicating if a file should be excluded from the bundle
        /// </summary>
        [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
        public bool ExcludeFromBundle { get; set; }

        /// <summary>
        /// Address of the linked resource
        /// </summary>
        [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
        public string Href { get; set; }

        #endregion
    }
}