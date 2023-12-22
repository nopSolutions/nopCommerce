using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents a bundle helper 
/// </summary>
public partial interface INopAssetHelper
{
    /// <summary>
    /// Get or create a JavaScript bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    IAsset GetOrCreateJavaScriptAsset(string bundleKey, params string[] sourceFiles);

    /// <summary>
    /// Get or create a CSS bundle
    /// </summary>
    /// <param name="bundleKey">A unique route to bundle</param>
    /// <param name="sourceFiles">A list of relative file names of the sources to optimize</param>
    IAsset GetOrCreateCssAsset(string bundleKey, params string[] sourceFiles);

    /// <summary>
    /// Return a bundle route with cache busting
    /// </summary>
    /// <param name="asset">Bundle</param>
    string CacheBusting(IAsset asset);
}