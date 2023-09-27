using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Seo;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.WebOptimizer;
using Nop.Web.Framework.WebOptimizer.Processors;
using WebOptimizer;

namespace Nop.Web.Framework.UI
{
    /// <summary>
    /// Represents the HTML helper implementation
    /// </summary>
    public partial class NopHtmlHelper : INopHtmlHelper
    {
        #region Fields

        protected readonly AppSettings _appSettings;
        protected readonly HtmlEncoder _htmlEncoder;
        protected readonly IActionContextAccessor _actionContextAccessor;
        protected readonly IAssetPipeline _assetPipeline;
        protected readonly Lazy<ILocalizationService> _localizationService;
        protected readonly IStoreContext _storeContext;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly SeoSettings _seoSettings;

        protected readonly Dictionary<ResourceLocation, List<ScriptReferenceMeta>> _scriptParts = new();
        protected readonly Dictionary<ResourceLocation, List<string>> _inlineScriptParts = new();
        protected readonly List<CssReferenceMeta> _cssParts = new();

        protected readonly List<string> _canonicalUrlParts = new();
        protected readonly List<string> _headCustomParts = new();
        protected readonly List<string> _metaDescriptionParts = new();
        protected readonly List<string> _metaKeywordParts = new();
        protected readonly List<string> _pageCssClassParts = new();
        protected readonly List<string> _titleParts = new();

        protected string _activeAdminMenuSystemName;
        protected string _editPageUrl;

        #endregion

        #region Ctor

        public NopHtmlHelper(AppSettings appSettings,
            HtmlEncoder htmlEncoder,
            IActionContextAccessor actionContextAccessor,
            IAssetPipeline assetPipeline,
            Lazy<ILocalizationService> localizationService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IWebHostEnvironment webHostEnvironment,
            SeoSettings seoSettings)
        {
            _appSettings = appSettings;
            _htmlEncoder = htmlEncoder;
            _actionContextAccessor = actionContextAccessor;
            _localizationService = localizationService;
            _assetPipeline = assetPipeline;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _webHostEnvironment = webHostEnvironment;
            _seoSettings = seoSettings;
        }

        #endregion

        #region Utilities

        protected IAsset CreateCssAsset(string bundleKey, string[] assetFiles)
        {
            var asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextCss}; charset=UTF-8", assetFiles)
                .EnforceFileExtensions(".css")
                .AdjustRelativePaths()
                .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopCssMinifier());

            asset.Concatenate();

            return asset;
        }

        protected IAsset CreateJavaScriptAsset(string bundleKey, string[] assetFiles)
        {
            var asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextJavascript}; charset=UTF-8", assetFiles)
                        .EnforceFileExtensions(".js", ".es5", ".es6")
                        .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopJsMinifier());

            asset.Concatenate();

            return asset;
        }

        protected static string GetAssetKey(string[] keys, string suffix)
        {
            if (keys is null || keys.Length == 0)
                throw new ArgumentNullException(nameof(keys));

            var hashInput = string.Join(',', keys);

            using var sha = MD5.Create();
            var input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));

            var key = string.Concat(WebEncoders.Base64UrlEncode(input));

            if (!string.IsNullOrEmpty(suffix))
                key += suffix;

            return key.ToLower();
        }

        /// <summary>
        /// Get or create an asset to the optimization pipeline.
        /// </summary>
        /// <param name="bundlePath">A registered route</param>
        /// <param name="createAsset">The function which creates a bundle</param>
        /// <param name="sourceFiles">Relative file names of the sources to optimize; if not specified, will use <paramref name="bundlePath"/></param>
        /// <returns>The bundle</returns>
        protected IAsset GetOrCreateBundle(string bundlePath, Func<string, string[], IAsset> createAsset, params string[] sourceFiles)
        {
            if (string.IsNullOrEmpty(bundlePath))
                throw new ArgumentNullException(nameof(bundlePath));

            if (createAsset is null)
                throw new ArgumentNullException(nameof(createAsset));

            if (sourceFiles.Length == 0)
                sourceFiles = new[] { bundlePath };

            //remove the base path from the generated URL if exists
            var pathBase = _actionContextAccessor.ActionContext?.HttpContext.Request.PathBase ?? PathString.Empty;
            sourceFiles = sourceFiles.Select(src => src.RemoveApplicationPathFromRawUrl(pathBase)).ToArray();

            if (!_assetPipeline.TryGetAssetFromRoute(bundlePath, out var bundleAsset))
            {
                bundleAsset = createAsset(bundlePath, sourceFiles);
            }
            else if (bundleAsset.SourceFiles.Count != sourceFiles.Length || !bundleAsset.SourceFiles.SequenceEqual(sourceFiles))
            {
                bundleAsset.SourceFiles.Clear();
                foreach (var source in sourceFiles)
                    bundleAsset.TryAddSourceFile(source);
            }

            return bundleAsset;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Title part</param>
        public virtual void AddTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Add(part);
        }

        /// <summary>
        /// Append title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Title part</param>
        public virtual void AppendTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all title parts
        /// </summary>
        /// <param name="addDefaultTitle">A value indicating whether to insert a default title</param>
        /// <param name="part">Title part</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains generated HTML string</returns>
        public virtual async Task<IHtmlContent> GenerateTitleAsync(bool addDefaultTitle = true, string part = "")
        {
            AppendTitleParts(part);
            var store = await _storeContext.GetCurrentStoreAsync();
            var defaultTitle = await _localizationService.Value.GetLocalizedAsync(store, s => s.DefaultTitle);

            var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
            string result;
            if (!string.IsNullOrEmpty(specificTitle))
            {
                if (addDefaultTitle)
                    //store name + page title
                    switch (_seoSettings.PageTitleSeoAdjustment)
                    {
                        case PageTitleSeoAdjustment.PagenameAfterStorename:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, defaultTitle, specificTitle);
                            }
                            break;
                        case PageTitleSeoAdjustment.StorenameAfterPagename:
                        default:
                            {
                                result = string.Join(_seoSettings.PageTitleSeparator, specificTitle, defaultTitle);
                            }
                            break;
                    }
                else
                    //page title only
                    result = specificTitle;
            }
            else
                //store name only
                result = defaultTitle;

            return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
        }

        /// <summary>
        /// Add meta description element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Meta description part</param>
        public virtual void AddMetaDescriptionParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaDescriptionParts.Add(part);
        }

        /// <summary>
        /// Append meta description element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Meta description part</param>
        public virtual void AppendMetaDescriptionParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaDescriptionParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all description parts
        /// </summary>
        /// <param name="part">Meta description part</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains generated HTML string</returns>
        public virtual async Task<IHtmlContent> GenerateMetaDescriptionAsync(string part = "")
        {
            AppendMetaDescriptionParts(part);

            var metaDescription = string.Join(", ", _metaDescriptionParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaDescription)
                ? metaDescription
                : await _localizationService.Value.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(),
                    s => s.DefaultMetaDescription);

            return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
        }

        /// <summary>
        /// Add meta keyword element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Meta keyword part</param>
        public virtual void AddMetaKeywordParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaKeywordParts.Add(part);
        }

        /// <summary>
        /// Append meta keyword element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Meta keyword part</param>
        public virtual void AppendMetaKeywordParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _metaKeywordParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all keyword parts
        /// </summary>
        /// <param name="part">Meta keyword part</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains generated HTML string</returns>
        public virtual async Task<IHtmlContent> GenerateMetaKeywordsAsync(string part = "")
        {
            AppendMetaKeywordParts(part);

            var metaKeyword = string.Join(", ", _metaKeywordParts.AsEnumerable().Reverse().ToArray());
            var result = !string.IsNullOrEmpty(metaKeyword)
                ? metaKeyword
                : await _localizationService.Value.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(),
                    s => s.DefaultMetaKeywords);

            return new HtmlString(_htmlEncoder.Encode(result ?? string.Empty));
        }

        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public virtual void AddScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                src = debugSrc;

            if (_actionContextAccessor.ActionContext == null)
                throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            _scriptParts[location].Add(new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsLocal = urlHelper.IsLocalUrl(src),
                Src = urlHelper.Content(src)
            });
        }

        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public virtual void AppendScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                src = debugSrc;

            if (_actionContextAccessor.ActionContext == null)
                throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            _scriptParts[location].Insert(0, new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsLocal = urlHelper.IsLocalUrl(src),
                Src = urlHelper.Content(src)
            });
        }

        /// <summary>
        /// Generate all script parts
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <returns>Generated HTML string</returns>
        public virtual IHtmlContent GenerateScripts(ResourceLocation location)
        {
            if (!_scriptParts.ContainsKey(location) || _scriptParts[location] == null)
                return HtmlString.Empty;

            if (!_scriptParts.Any())
                return HtmlString.Empty;

            var result = new StringBuilder();
            var woConfig = _appSettings.Get<WebOptimizerConfig>();

            var httpContext = _actionContextAccessor.ActionContext.HttpContext;

            if (woConfig.EnableJavaScriptBundling && _scriptParts[location].Any(item => !item.ExcludeFromBundle))
            {
                var sources = _scriptParts[location]
                   .Where(item => !item.ExcludeFromBundle && item.IsLocal)
                   .Select(item => item.Src)
                   .Distinct().ToArray();

                var bundleKey = string.Concat("/js/", GetAssetKey(sources, woConfig.JavaScriptBundleSuffix), ".js");

                var bundleAsset = GetOrCreateBundle(bundleKey, CreateJavaScriptAsset, sources);

                var pathBase = _actionContextAccessor.ActionContext?.HttpContext.Request.PathBase ?? PathString.Empty;
                result.AppendFormat("<script type=\"{0}\" src=\"{1}{2}?v={3}\"></script>",
                    MimeTypes.TextJavascript, pathBase, bundleAsset.Route, bundleAsset.GenerateCacheKey(httpContext, woConfig));
            }

            var scripts = _scriptParts[location]
                .Where(item => !woConfig.EnableJavaScriptBundling || item.ExcludeFromBundle || !item.IsLocal)
                .Distinct();

            foreach (var item in scripts)
            {
                if (!item.IsLocal)
                {
                    result.AppendFormat("<script type=\"{0}\" src=\"{1}\"></script>", MimeTypes.TextJavascript, item.Src);
                    result.Append(Environment.NewLine);
                    continue;
                }

                var asset = GetOrCreateBundle(item.Src, CreateJavaScriptAsset);

                result.AppendFormat("<script type=\"{0}\" src=\"{1}?v={2}\"></script>",
                    MimeTypes.TextJavascript, asset.Route, asset.GenerateCacheKey(httpContext, woConfig));

                result.Append(Environment.NewLine);
            }

            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Add inline script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public virtual void AddInlineScriptParts(ResourceLocation location, string script)
        {
            if (!_inlineScriptParts.ContainsKey(location))
                _inlineScriptParts.Add(location, new());

            if (string.IsNullOrEmpty(script))
                return;

            if (_inlineScriptParts[location].Contains(script))
                return;

            _inlineScriptParts[location].Add(script);
        }

        /// <summary>
        /// Append inline script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public virtual void AppendInlineScriptParts(ResourceLocation location, string script)
        {
            if (!_inlineScriptParts.ContainsKey(location))
                _inlineScriptParts.Add(location, new());

            if (string.IsNullOrEmpty(script))
                return;

            if (_inlineScriptParts[location].Contains(script))
                return;

            _inlineScriptParts[location].Insert(0, script);
        }

        /// <summary>
        /// Generate all inline script parts
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <returns>Generated HTML string</returns>
        public virtual IHtmlContent GenerateInlineScripts(ResourceLocation location)
        {
            if (!_inlineScriptParts.ContainsKey(location) || _inlineScriptParts[location] == null)
                return HtmlString.Empty;

            if (!_inlineScriptParts.Any())
                return HtmlString.Empty;

            var result = new StringBuilder();
            foreach (var item in _inlineScriptParts[location])
            {
                result.Append(item);
                result.Append(Environment.NewLine);
            }
            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
        public virtual void AddCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            if (string.IsNullOrEmpty(src))
                return;

            if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                src = debugSrc;

            if (_actionContextAccessor.ActionContext == null)
                throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            _cssParts.Add(new CssReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsLocal = urlHelper.IsLocalUrl(src),
                Src = urlHelper.Content(src)
            });
        }

        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
        public virtual void AppendCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            if (string.IsNullOrEmpty(src))
                return;

            if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                src = debugSrc;

            if (_actionContextAccessor.ActionContext == null)
                throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            _cssParts.Insert(0, new CssReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsLocal = urlHelper.IsLocalUrl(src),
                Src = urlHelper.Content(src)
            });
        }

        /// <summary>
        /// Generate all CSS parts
        /// </summary>
        /// <returns>Generated HTML string</returns>
        public virtual IHtmlContent GenerateCssFiles()
        {            
            if (_cssParts.Count == 0)
                return HtmlString.Empty;

            if (_actionContextAccessor.ActionContext == null)
                throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

            var result = new StringBuilder();

            var woConfig = _appSettings.Get<WebOptimizerConfig>();
            var httpContext = _actionContextAccessor.ActionContext.HttpContext;

            if (woConfig.EnableCssBundling && _cssParts.Any(item => !item.ExcludeFromBundle))
            {
                var bundleSuffix = woConfig.CssBundleSuffix;

                if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                    bundleSuffix += ".rtl";

                var sources = _cssParts
                    .Where(item => !item.ExcludeFromBundle && item.IsLocal)
                    .Distinct()
                    //remove the application path from the generated URL if exists
                    .Select(item => item.Src).ToArray();

                var bundleKey = string.Concat("/css/", GetAssetKey(sources, bundleSuffix), ".css");

                var bundleAsset = GetOrCreateBundle(bundleKey, CreateCssAsset, sources);

                var pathBase = _actionContextAccessor.ActionContext?.HttpContext.Request.PathBase ?? PathString.Empty;
                result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}{2}?v={3}\" />",
                    MimeTypes.TextCss, pathBase, bundleAsset.Route, bundleAsset.GenerateCacheKey(httpContext, woConfig));
            }

            var styles = _cssParts
                    .Where(item => !woConfig.EnableCssBundling || item.ExcludeFromBundle || !item.IsLocal)
                    .Distinct();

            foreach (var item in styles)
            {
                if (!item.IsLocal)
                {
                    result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}\" />", MimeTypes.TextCss, item.Src);
                    result.Append(Environment.NewLine);
                    continue;
                }

                var asset = GetOrCreateBundle(item.Src, CreateCssAsset);

                result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}?v={2}\" />",
                    MimeTypes.TextCss, asset.Route, asset.GenerateCacheKey(httpContext, woConfig));
                result.AppendLine();
            }

            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Add canonical URL element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Canonical URL part</param>
        /// <param name="withQueryString">Whether to use canonical URLs with query string parameters</param>
        public virtual void AddCanonicalUrlParts(string part, bool withQueryString = false)
        {
            if (string.IsNullOrEmpty(part))
                return;

            if (withQueryString)
            {
                //add ordered query string parameters
                var queryParameters = _actionContextAccessor.ActionContext.HttpContext.Request.Query.OrderBy(parameter => parameter.Key)
                    .ToDictionary(parameter => parameter.Key, parameter => parameter.Value.ToString());
                part = QueryHelpers.AddQueryString(part, queryParameters);
            }

            _canonicalUrlParts.Add(part);
        }

        /// <summary>
        /// Append canonical URL element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Canonical URL part</param>
        public virtual void AppendCanonicalUrlParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _canonicalUrlParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all canonical URL parts
        /// </summary>
        /// <returns>Generated HTML string</returns>
        public virtual IHtmlContent GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                result.Append(Environment.NewLine);
            }
            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Add any custom element to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
        public virtual void AddHeadCustomParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _headCustomParts.Add(part);
        }

        /// <summary>
        /// Append any custom element to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">The entire element. For example, <![CDATA[<meta name="msvalidate.01" content="123121231231313123123" />]]></param>
        public virtual void AppendHeadCustomParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _headCustomParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all custom elements
        /// </summary>
        /// <returns>Generated HTML string</returns>
        public virtual IHtmlContent GenerateHeadCustom()
        {
            //use only distinct rows
            var distinctParts = _headCustomParts.Distinct().ToList();
            if (!distinctParts.Any())
                return HtmlString.Empty;

            var result = new StringBuilder();
            foreach (var path in distinctParts)
            {
                result.Append(path);
                result.Append(Environment.NewLine);
            }
            return new HtmlString(result.ToString());
        }

        /// <summary>
        /// Add CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">CSS class</param>
        public virtual void AddPageCssClassParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _pageCssClassParts.Add(part);
        }

        /// <summary>
        /// Append CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">CSS class</param>
        public virtual void AppendPageCssClassParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _pageCssClassParts.Insert(0, part);
        }

        /// <summary>
        /// Generate all title parts
        /// </summary>
        /// <param name="part">CSS class</param>
        /// <returns>Generated string</returns>
        public virtual string GeneratePageCssClasses(string part = "")
        {
            AppendPageCssClassParts(part);

            var result = string.Join(" ", _pageCssClassParts.AsEnumerable().Reverse().ToArray());

            if (string.IsNullOrEmpty(result))
                return string.Empty;

            return _htmlEncoder.Encode(result);
        }

        /// <summary>
        /// Specify "edit page" URL
        /// </summary>
        /// <param name="url">URL</param>
        public virtual void AddEditPageUrl(string url)
        {
            _editPageUrl = url;
        }

        /// <summary>
        /// Get "edit page" URL
        /// </summary>
        /// <returns>URL</returns>
        public virtual string GetEditPageUrl()
        {
            return _editPageUrl;
        }

        /// <summary>
        /// Specify system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="systemName">System name</param>
        public virtual void SetActiveMenuItemSystemName(string systemName)
        {
            _activeAdminMenuSystemName = systemName;
        }

        /// <summary>
        /// Get system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <returns>System name</returns>
        public virtual string GetActiveMenuItemSystemName()
        {
            return _activeAdminMenuSystemName;
        }

        /// <summary>
        /// Get the route name associated with the request rendering this page
        /// </summary>
        /// <param name="handleDefaultRoutes">A value indicating whether to build the name using engine information unless otherwise specified</param>
        /// <returns>Route name</returns>
        public virtual string GetRouteName(bool handleDefaultRoutes = false)
        {
            var actionContext = _actionContextAccessor.ActionContext;

            if (actionContext is null)
                return string.Empty;

            var httpContext = actionContext.HttpContext;
            var routeName = httpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName ?? string.Empty;

            if (!string.IsNullOrEmpty(routeName) && routeName != "areaRoute")
                return routeName;

            //then try to get a generic one (actually it's an action name, not the route)
            if (httpContext.GetRouteValue(NopRoutingDefaults.RouteValue.SeName) is not null &&
                httpContext.GetRouteValue(NopRoutingDefaults.RouteValue.Action) is string actionName)
                return actionName;

            if (handleDefaultRoutes)
                return actionContext.ActionDescriptor switch
                {
                    ControllerActionDescriptor controllerAction => string.Concat(controllerAction.ControllerName, controllerAction.ActionName),
                    CompiledPageActionDescriptor compiledPage => string.Concat(compiledPage.AreaName, compiledPage.ViewEnginePath.Replace("/", "")),
                    PageActionDescriptor pageAction => string.Concat(pageAction.AreaName, pageAction.ViewEnginePath.Replace("/", "")),
                    _ => actionContext.ActionDescriptor.DisplayName?.Replace("/", "") ?? string.Empty
                };

            return routeName;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// JS file meta data
        /// </summary>
        protected partial record ScriptReferenceMeta
        {
            /// <summary>
            /// A value indicating whether to exclude the script from bundling
            /// </summary>
            public bool ExcludeFromBundle { get; init; }

            /// <summary>
            /// A value indicating whether the src is local
            /// </summary>
            public bool IsLocal { get; init; }

            /// <summary>
            /// Src for production
            /// </summary>
            public string Src { get; init; }
        }

        /// <summary>
        /// CSS file meta data
        /// </summary>
        protected partial record CssReferenceMeta
        {
            /// <summary>
            /// A value indicating whether to exclude the script from bundling
            /// </summary>
            public bool ExcludeFromBundle { get; init; }

            /// <summary>
            /// Src for production
            /// </summary>
            public string Src { get; init; }

            /// <summary>
            /// A value indicating whether the Src is local
            /// </summary>
            public bool IsLocal { get; init; }
        }

        #endregion
    }
}