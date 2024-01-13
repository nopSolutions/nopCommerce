using Nop.Services.Plugins;

namespace Nop.Services.Catalog;

/// <summary>
/// Provides an interface for creating search provider
/// </summary>
public partial interface ISearchProvider : IPlugin
{
    /// <summary>
    /// Get products identifiers by the specified keywords
    /// </summary>
    /// <param name="keywords">Keywords</param>
    /// <param name="isLocalized">A value indicating whether to search in localized properties</param>
    /// <returns>The task result contains product identifiers</returns>
    Task<List<int>> SearchProductsAsync(string keywords, bool isLocalized);
}