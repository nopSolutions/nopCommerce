using Nop.Core.Domain.Catalog;
using Nop.Services.Caching;

namespace Nop.Services.Catalog.Caching;

/// <summary>
/// Represents a cross-sell product cache event consumer
/// </summary>
public partial class CrossSellProductCacheEventConsumer : CacheEventConsumer<CrossSellProduct>
{
}