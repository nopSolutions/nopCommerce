using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Web.Framework.Configuration;
using Nop.Web.Framework.UI;
using WebOptimizer;
using WebOptimizer.Extensions;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// CSS bundling tag helper
    /// </summary>
    [HtmlTargetElement(LINK_TAG_NAME)]
    [HtmlTargetElement(BUNDLE_TAG_NAME)]
    public class NopLinkTagHelper : UrlResolutionTagHelper
    {
        #region Constants

        private const string LINK_TAG_NAME = "link";
        private const string BUNDLE_TAG_NAME = "style-bundle";
        private const string BUNDLE_DESTINATION_KEY_NAME = "asp-bundle-dest-key";
        private const string BUNDLE_KEY_NAME = "asp-bundle-key";
        private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
        private const string HREF_ATTRIBUTE_NAME = "href";

        #endregion

        #region Fields

        private readonly AppSettings _appSettings;
        private readonly IAssetPipeline _assetPipeline;
        private readonly INopHtmlHelper _nopHtmlHelper;

        #endregion

        #region Ctor

        public NopLinkTagHelper(AppSettings appSettings,
            HtmlEncoder htmlEncoder,
            IAssetPipeline assetPipeline,
            INopHtmlHelper nopHtmlHelper,
            IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory, htmlEncoder)
        {
            _appSettings = appSettings;
            _assetPipeline = assetPipeline ?? throw new ArgumentNullException(nameof(assetPipeline));
            _nopHtmlHelper = nopHtmlHelper;
        }

        #endregion

        #region Utils

        private void ProcessSrcAttribute(TagHelperContext context, TagHelperOutput output)
        {
            // Pass through attribute that is also a well-known HTML attribute.
            if (Href != null)
            {
                output.CopyHtmlAttribute(HREF_ATTRIBUTE_NAME, context);
            }

            // If there's no "src" attribute in output.Attributes this will noop.
            ProcessUrlAttribute(HREF_ATTRIBUTE_NAME, output);

            // Retrieve the TagHelperOutput variation of the "href" attribute in case other TagHelpers in the
            // pipeline have touched the value. If the value is already encoded this ScriptTagHelper may
            // not function properly.
            if (output.Attributes[HREF_ATTRIBUTE_NAME]?.Value is not string hrefAttribute)
                return;

            var sourceFile = hrefAttribute;
            var pathBase = ViewContext.HttpContext?.Request?.PathBase.Value;
            if (!string.IsNullOrEmpty(pathBase) && hrefAttribute.StartsWith(pathBase))
                sourceFile = hrefAttribute[pathBase.Length..];

            if (!_assetPipeline.TryGetAssetFromRoute(sourceFile, out var asset))
            {
                asset = _assetPipeline.AddFiles(MimeTypes.TextCss, sourceFile).First();
            }

            Href = $"{hrefAttribute}?v={asset.GenerateCacheKey(ViewContext.HttpContext)}";
            output.Attributes.SetAttribute(HREF_ATTRIBUTE_NAME, Href);
        }

        private string GetBundleSuffix()
        {
            var bundleSuffix = _appSettings.Get<WebOptimizerConfig>().CssBundleSuffix;

            //to avoid collisions in controllers with the same names
            if (ViewContext.RouteData.Values.TryGetValue("area", out var area))
                bundleSuffix = $"{bundleSuffix}.{area}".ToLowerInvariant();

            return bundleSuffix;
        }

        #endregion

        #region Methods

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.TagName = LINK_TAG_NAME;
            output.Attributes.SetAttribute("type", MimeTypes.TextCss);
            output.Attributes.SetAttribute("rel", "stylesheet");
            output.TagMode = TagMode.SelfClosing;

            var config = _appSettings.Get<WebOptimizerConfig>();

            //bundling
            if (config.EnableCssBundling)
            {
                var defaultBundleBuffix = GetBundleSuffix();
                if (string.Equals(context.TagName, BUNDLE_TAG_NAME))
                {
                    output.HandleCssBundle(_assetPipeline, ViewContext, config, Href, string.Empty, BundleDestinationKey ?? defaultBundleBuffix);
                    return Task.CompletedTask;
                }

                if (Href is not null && !ExcludeFromBundle)
                {
                    output.HandleCssBundle(_assetPipeline, ViewContext, config, Href, BundleKey ?? defaultBundleBuffix, string.Empty);
                    return Task.CompletedTask;
                }
            }

            ProcessSrcAttribute(context, output);

            _nopHtmlHelper.AddCssFileParts(ResourceLocation.Head, Href, string.Empty);
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
        /// A key of a bundle to collect
        /// </summary>
        [HtmlAttributeName(BUNDLE_KEY_NAME)]
        public string BundleKey { get; set; }

        /// <summary>
        /// A key that defines the destination for the bundle.
        /// </summary>
        [HtmlAttributeName(BUNDLE_DESTINATION_KEY_NAME)]
        public string BundleDestinationKey { get; set; }

        /// <summary>
        /// Address of the linked resource
        /// </summary>
        [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
        public string Href { get; set; }

        #endregion
    }
}