using Microsoft.AspNetCore.Http;
using WebOptimizer;

namespace Nop.Web.Framework.WebOptimizer;

/// <summary>
/// Represents the default bundle implementation
/// </summary>
public partial class DefaultAsset : IAsset
{
    #region Ctor

    public DefaultAsset(string route, string contentType)
    {
        Route = route;
        ContentType = contentType;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the processors and returns the modified content
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="options">Configuration options</param>
    public Task<byte[]> ExecuteAsync(HttpContext context, IWebOptimizerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the cache key associated with this version of the asset
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="options">Configuration options</param>
    /// <returns></returns>
    public string GenerateCacheKey(HttpContext context, IWebOptimizerOptions options)
    {
        return string.Empty;
    }

    /// <summary>
    /// Adds a source file to the asset
    /// </summary>
    /// <param name="route">Relative path of a source file</param>
    public void TryAddSourceFile(string route)
    {
        throw new NotImplementedException();
    }


    #endregion

    #region Properties

    /// <summary>
    /// Gets the content type produced by the transform
    /// </summary>
    public string ContentType { get; init; }

    /// <summary>
    /// Gets a list of processors
    /// </summary>
    public IList<IProcessor> Processors => new List<IProcessor>();

    /// <summary>
    /// Gets the items collection for the asset
    /// </summary>
    public IDictionary<string, object> Items => new Dictionary<string, object>();

    /// <summary>
    /// Gets the route to the bundle output
    /// </summary>
    public string Route { get; init; }

    /// <summary>
    /// Gets files to exclude from output results
    /// </summary>
    public IList<string> ExcludeFiles => new List<string>();

    /// <summary>
    /// Gets the webroot relative source files
    /// </summary>
    public HashSet<string> SourceFiles => new();

    #endregion
}