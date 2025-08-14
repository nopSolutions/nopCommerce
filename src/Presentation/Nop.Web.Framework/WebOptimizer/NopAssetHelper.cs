using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Web.Framework.WebOptimizer.Processors;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents a bundle helper 
/// </summary>
public partial class NopAssetHelper : INopAssetHelper
{
    #region Fields

    private readonly IAssetPipeline _assetPipeline;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WebOptimizerConfig _webOptimizerConfig;

    #endregion

    #region Ctor


    public NopAssetHelper(AppSettings appSettings,
        IAssetPipeline assetPipeline,
        IHttpContextAccessor httpContextAccessor)
    {
        _assetPipeline = assetPipeline;
        _httpContextAccessor = httpContextAccessor;
        _webOptimizerConfig = appSettings.Get<WebOptimizerConfig>();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get or create a JavaScript bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateJavaScriptAsset(string bundleKey, params string[] sourceFiles)
    {
        ArgumentException.ThrowIfNullOrEmpty(bundleKey);

        if (sourceFiles.Length == 0)
            sourceFiles = [bundleKey];

        if (!_assetPipeline.TryGetAssetFromRoute(bundleKey, out var asset))
        {
            asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextJavascript}; charset=UTF-8", sourceFiles)
                .EnforceFileExtensions(".js", ".es5", ".es6")
                .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopJsMinifier());

            asset.Concatenate();
        }
        else if (asset.SourceFiles.Count != sourceFiles.Length || !asset.SourceFiles.SequenceEqual(sourceFiles))
        {
            asset.SourceFiles.Clear();
            foreach (var source in sourceFiles)
                asset.TryAddSourceFile(source);
        }

        return asset;
    }

    /// <summary>
    /// Get or create a CSS bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateCssAsset(string bundleKey, params string[] sourceFiles)
    {
        ArgumentException.ThrowIfNullOrEmpty(bundleKey);

        if (sourceFiles.Length == 0)
            sourceFiles = [bundleKey];

        if (!_assetPipeline.TryGetAssetFromRoute(bundleKey, out var asset))
        {
            asset = _assetPipeline.AddBundle(bundleKey, $"{MimeTypes.TextCss}; charset=UTF-8", sourceFiles)
                .EnforceFileExtensions(".css")
                .AdjustRelativePaths()
                .AddResponseHeader(HeaderNames.XContentTypeOptions, "nosniff");

            //to more precisely log problem files we minify them before concatenating
            asset.Processors.Add(new NopCssMinifier());

            asset.Concatenate();
        }
        else if (asset.SourceFiles.Count != sourceFiles.Length || !asset.SourceFiles.SequenceEqual(sourceFiles))
        {
            asset.SourceFiles.Clear();
            foreach (var source in sourceFiles)
                asset.TryAddSourceFile(source);
        }

        return asset;
    }

    /// <summary>
    /// Return a bundle route with cache busting
    /// </summary>
    /// <param name="asset">Bundle</param>
    public string CacheBusting(IAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        try
        {
            var hash = asset.GenerateCacheKey(_httpContextAccessor.HttpContext, _webOptimizerConfig);
            return QueryHelpers.AddQueryString(asset.Route, "v", hash);
        }
        catch
        {
            //in some cases WO can't handle assets (e.g. IIS sub-application with PathBase)
            return asset.Route;
        }
    }

    #endregion
}