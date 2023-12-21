using Nop.Core;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents the default INopAssetHelper implementation when bundling is disabled
/// </summary>
public partial class NopDefaultAssetHelper : INopAssetHelper
{
    #region Methods

    /// <summary>
    /// Return a bundle route with cache busting
    /// </summary>
    /// <param name="asset">Bundle</param>
    public string CacheBusting(IAsset asset)
    {
        return asset.Route;
    }

    /// <summary>
    /// Get or create a CSS bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateCssAsset(string bundleKey, params string[] sourceFiles)
    {
        return new DefaultAsset(bundleKey, MimeTypes.TextCss);
    }

    /// <summary>
    /// Get or create a JavaScript bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    public IAsset GetOrCreateJavaScriptAsset(string bundleKey, params string[] sourceFiles)
    {
        return new DefaultAsset(bundleKey, MimeTypes.TextJavascript);
    }

    #endregion
}