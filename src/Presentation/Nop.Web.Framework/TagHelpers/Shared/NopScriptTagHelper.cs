using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;
using Nop.Web.Framework.WebOptimizer;
using WebOptimizer;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// Script bundling tag helper
    /// </summary>
    [HtmlTargetElement(SCRIPT_TAG_NAME)]
    public partial class NopScriptTagHelper : UrlResolutionTagHelper
    {
        #region Constants
        private const string SCRIPT_TAG_NAME = "script";
        private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
        private const string DEBUG_SRC_ATTRIBUTE_NAME = "asp-debug-src";
        private const string LOCATION_ATTRIBUTE_NAME = "asp-location";
        private const string SRC_ATTRIBUTE_NAME = "src";

        #endregion

        #region Fields

        private readonly AppSettings _appSettings;
        private readonly IAssetPipeline _assetPipeline;
        private readonly INopHtmlHelper _nopHtmlHelper;
        private readonly IWebHelper _webHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public NopScriptTagHelper(AppSettings appSettings,
            HtmlEncoder htmlEncoder,
            IAssetPipeline assetPipeline,
            INopHtmlHelper nopHtmlHelper,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper,
            IWebHostEnvironment webHostEnvironment) : base(urlHelperFactory, htmlEncoder)
        {
            _appSettings = appSettings;
            _assetPipeline = assetPipeline ?? throw new ArgumentNullException(nameof(assetPipeline));
            _nopHtmlHelper = nopHtmlHelper;
            _webHelper = webHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region Utils

        private static async Task<string> BuildInlineScriptTagAsync(TagHelperOutput output)
        {
            //get JavaScript
            var scriptTag = new TagBuilder(SCRIPT_TAG_NAME);

            var childContent = await output.GetChildContentAsync();
            var script = childContent.GetContent();

            if (!string.IsNullOrEmpty(script))
                scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

            scriptTag.MergeAttributes(await output.GetAttributeDictionaryAsync(), replaceExisting: false);

            return await scriptTag.RenderHtmlContentAsync() + Environment.NewLine;
        }

        private void ProcessSrcAttribute(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(DebugSrc) && _webHostEnvironment.IsDevelopment())
                Src = DebugSrc;

            // Pass through attribute that is also a well-known HTML attribute.
            if (Src != null)
                output.CopyHtmlAttribute(SRC_ATTRIBUTE_NAME, context);

            // If there's no "src" attribute in output.Attributes this will noop.
            ProcessUrlAttribute(SRC_ATTRIBUTE_NAME, output);

            // Retrieve the TagHelperOutput variation of the "src" attribute in case other TagHelpers in the
            // pipeline have touched the value. If the value is already encoded this ScriptTagHelper may
            // not function properly.
            if (output.Attributes[SRC_ATTRIBUTE_NAME]?.Value is string srcAttribute)
                Src = srcAttribute;
        }

        private void ProcessAsset(TagHelperOutput output, WebOptimizerConfig woConfig)
        {
            if (string.IsNullOrEmpty(Src))
                return;

            var urlHelper = UrlHelperFactory.GetUrlHelper(ViewContext);
            if (!urlHelper.IsLocalUrl(Src))
            {
                output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, Src);
                return;
            }

            //remove the application path from the generated URL if exists
            var pathBase = ViewContext.HttpContext?.Request?.PathBase ?? PathString.Empty;
            PathString.FromUriComponent(Src).StartsWithSegments(pathBase, out var sourceFile);

            if (!_assetPipeline.TryGetAssetFromRoute(sourceFile, out var asset))
            {
                asset = _assetPipeline.AddJavaScriptBundle(sourceFile, sourceFile);
            }

            output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, $"{Src}?v={asset.GenerateCacheKey(ViewContext.HttpContext, woConfig)}");
        }

        #endregion

        #region Methods

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (_webHelper.IsAjaxRequest(ViewContext.HttpContext?.Request))
                return;

            output.TagMode = TagMode.StartTagAndEndTag;

            if (!output.Attributes.ContainsName("type")) // we don't touch other types e.g. text/template
                output.Attributes.SetAttribute("type", MimeTypes.TextJavascript);

            var woConfig = _appSettings.Get<WebOptimizerConfig>();

            if (Location == ResourceLocation.Auto)
            {
                // move script to the footer bundle when bundling is enabled
                Location = woConfig.EnableJavaScriptBundling ? ResourceLocation.Footer : ResourceLocation.None;
            }

            if (Location == ResourceLocation.None)
            {
                if (!string.IsNullOrEmpty(Src))
                {
                    ProcessSrcAttribute(context, output);
                    ProcessAsset(output, woConfig);
                }

                return;
            }

            if (string.IsNullOrEmpty(Src))
                _nopHtmlHelper.AddInlineScriptParts(Location, await BuildInlineScriptTagAsync(output));
            else
                _nopHtmlHelper.AddScriptParts(Location, Src, DebugSrc, ExcludeFromBundle);

            output.SuppressOutput();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Script path (e.g. full debug version). If empty, then minified version will be used
        /// </summary>
        [HtmlAttributeName(DEBUG_SRC_ATTRIBUTE_NAME)]
        public string DebugSrc { get; set; }

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LOCATION_ATTRIBUTE_NAME)]
        public ResourceLocation Location { set; get; }

        /// <summary>
        /// A value indicating if a file should be excluded from the bundle
        /// </summary>
        [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
        public bool ExcludeFromBundle { get; set; }

        /// <summary>
        /// Address of the external script to use
        /// </summary>
        [HtmlAttributeName(SRC_ATTRIBUTE_NAME)]
        public string Src { get; set; }

        #endregion

    }
}