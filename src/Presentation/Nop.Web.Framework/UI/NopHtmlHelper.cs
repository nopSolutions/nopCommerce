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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain.Seo;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Framework.UI;

/// <summary>
/// Represents the HTML helper implementation
/// </summary>
public partial class NopHtmlHelper : INopHtmlHelper
{
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IHtmlHelper _htmlHelper;
    protected readonly INopAssetHelper _bundleHelper;
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
        IHtmlHelper htmlHelper,
        INopAssetHelper bundleHelper,
        Lazy<ILocalizationService> localizationService,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IWebHostEnvironment webHostEnvironment,
        SeoSettings seoSettings)
    {
        _appSettings = appSettings;
        _htmlEncoder = htmlEncoder;
        _actionContextAccessor = actionContextAccessor;
        _htmlHelper = htmlHelper;
        _bundleHelper = bundleHelper;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _urlHelperFactory = urlHelperFactory;
        _webHostEnvironment = webHostEnvironment;
        _seoSettings = seoSettings;
    }

    #endregion

    #region Utilities

    protected static string GetAssetKey(string[] keys, string suffix)
    {
        ArgumentNullException.ThrowIfNull(keys?.Length > 0 ? keys : null, nameof(keys));
            
        var hashInput = string.Join(',', keys);
        var input = MD5.HashData(Encoding.Unicode.GetBytes(hashInput));

        var key = string.Concat(WebEncoders.Base64UrlEncode(input));

        if (!string.IsNullOrEmpty(suffix))
            key += suffix;

        return key.ToLower();
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

        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
        var isLocal = urlHelper.IsLocalUrl(src);

        _scriptParts[location].Add(new ScriptReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = isLocal ? urlHelper.Content(src).RemoveApplicationPathFromRawUrl(pathBase) : src
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

        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
        var isLocal = urlHelper.IsLocalUrl(src);

        _scriptParts[location].Insert(0, new ScriptReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = isLocal ? urlHelper.Content(src).RemoveApplicationPathFromRawUrl(pathBase) : src
        });
    }

    /// <summary>
    /// Generate all script parts
    /// </summary>
    /// <param name="location">A location of the script element</param>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateScripts(ResourceLocation location)
    {
        if (!_scriptParts.TryGetValue(location, out var value) || value == null)
            return HtmlString.Empty;

        if (!_scriptParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
        var woConfig = _appSettings.Get<WebOptimizerConfig>();
        var pathBase = _actionContextAccessor.ActionContext?.HttpContext.Request.PathBase ?? PathString.Empty;

        if (woConfig.EnableJavaScriptBundling && value.Any(item => !item.ExcludeFromBundle))
        {
            var sources = value.Where(item => !item.ExcludeFromBundle && item.IsLocal)
                .Select(item => item.Src)
                .Distinct().ToArray();

            var bundleKey = $"/js/{GetAssetKey(sources, woConfig.JavaScriptBundleSuffix)}.js";

            var bundleAsset = _bundleHelper.GetOrCreateJavaScriptAsset(bundleKey, sources);
            var route = _bundleHelper.CacheBusting(bundleAsset);

            result.AppendFormat("<script type=\"{0}\" src=\"{1}{2}\"></script>",
                MimeTypes.TextJavascript, pathBase, route);
        }

        var scripts = value.Where(item => !woConfig.EnableJavaScriptBundling || item.ExcludeFromBundle || !item.IsLocal)
            .Distinct();

        foreach (var item in scripts)
        {
            if (!item.IsLocal)
            {
                result.AppendFormat("<script type=\"{0}\" src=\"{1}\"></script>", MimeTypes.TextJavascript, item.Src);
                result.Append(Environment.NewLine);
                continue;
            }

            var asset = _bundleHelper.GetOrCreateJavaScriptAsset(item.Src);
            var route = _bundleHelper.CacheBusting(asset);

            result.AppendFormat("<script type=\"{0}\" src=\"{1}{2}\"></script>",
                MimeTypes.TextJavascript, pathBase, route);

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
        if (!_inlineScriptParts.TryGetValue(location, out var value) || value == null)
            return HtmlString.Empty;

        if (!_inlineScriptParts.Any())
            return HtmlString.Empty;

        var result = new StringBuilder();
        foreach (var item in value)
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

        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
        var isLocal = urlHelper.IsLocalUrl(src);

        _cssParts.Add(new CssReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = isLocal ? urlHelper.Content(src).RemoveApplicationPathFromRawUrl(pathBase) : src
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

        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
        var isLocal = urlHelper.IsLocalUrl(src);

        _cssParts.Insert(0, new CssReferenceMeta
        {
            ExcludeFromBundle = excludeFromBundle,
            IsLocal = isLocal,
            Src = isLocal ? urlHelper.Content(src).RemoveApplicationPathFromRawUrl(pathBase) : src
        });
    }

    /// <summary>
    /// Generate all CSS parts
    /// </summary>
    /// <returns>Generated HTML string</returns>
    public virtual IHtmlContent GenerateCssFiles()
    {
        if (!_cssParts.Any())
            return HtmlString.Empty;

        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        var result = new StringBuilder();

        var woConfig = _appSettings.Get<WebOptimizerConfig>();
        var pathBase = _actionContextAccessor.ActionContext?.HttpContext.Request.PathBase ?? PathString.Empty;

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

            var bundleKey = $"/css/{GetAssetKey(sources, bundleSuffix)}.css";

            var bundleAsset = _bundleHelper.GetOrCreateCssAsset(bundleKey, sources);
            var route = _bundleHelper.CacheBusting(bundleAsset);


            result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}{2}\" />",
                MimeTypes.TextCss, pathBase, route);
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

            var asset = _bundleHelper.GetOrCreateCssAsset(item.Src);
            var route = _bundleHelper.CacheBusting(asset);

            result.AppendFormat("<link rel=\"stylesheet\" type=\"{0}\" href=\"{1}{2}\" />",
                MimeTypes.TextCss, pathBase, route);
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

    /// <summary>
    /// Add JSON-LD to the <![CDATA[<head>]]> element
    /// </summary>
    /// <param name="jsonLd">The JSON-LD serialized model></param>
    public virtual void AddJsonLdParts(string jsonLd)
    {
        if(_seoSettings.MicrodataEnabled) 
            AddHeadCustomParts("<script type=\"application/ld+json\">" +  _htmlHelper.Raw(jsonLd) + "</script>");
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