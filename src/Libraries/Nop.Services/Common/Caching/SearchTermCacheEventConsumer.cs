using Nop.Core.Domain.Common;
using Nop.Services.Caching;

namespace Nop.Services.Common.Caching;

/// <summary>
/// Represents a search term cache event consumer
/// </summary>
public partial class SearchTermCacheEventConsumer : CacheEventConsumer<SearchTerm>
{
    protected override async Task ClearCacheAsync(SearchTerm entity)
    {
        await RemoveAsync(NopCommonDefaults.SearchTermsCacheKey, entity.CustomerId, entity.StoreId);
    }
}