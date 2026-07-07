using Nop.Core.Domain.Catalog;
using Nop.Services.Plugins;

namespace Nop.Services.ArtificialIntelligence;

/// <summary>
/// Provides an interface for creating AI-powered recommendation provider
/// </summary>
public partial interface IAiRecommendationPlugin : IPlugin
{
    /// <summary>
    /// Get products identifiers by the specified parameters
    /// </summary>
    /// <param name="keywords">Keywords</param>
    /// <param name="categoryIds">Category identifiers</param>
    /// <param name="manufacturerIds">Manufacturer identifiers</param>
    /// <param name="productTagId">Product tag identifier</param>
    /// <param name="filteredSpecOptions">Filtered specification options</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains product identifiers and total count of products found
    /// </returns>
    Task<(List<int>, int)> SearchProductsAsync(string keywords,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        int productTagId = 0,
        IList<SpecificationAttributeOption> filteredSpecOptions = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue);
}