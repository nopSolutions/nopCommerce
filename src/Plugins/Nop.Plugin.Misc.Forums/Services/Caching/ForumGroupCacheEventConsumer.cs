using Nop.Core.Caching;
using Nop.Plugin.Misc.Forums.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.Forums.Services.Caching;

/// <summary>
/// Represents a forum group cache event consumer
/// </summary>
public class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
{
    #region Methods

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(ForumGroup entity)
    {
        await RemoveByPrefixAsync(NopEntityCacheDefaults<Forum>.Prefix);
        await RemoveAsync(ForumDefaults.ForumByForumGroupCacheKey, entity);
    }

    #endregion
}