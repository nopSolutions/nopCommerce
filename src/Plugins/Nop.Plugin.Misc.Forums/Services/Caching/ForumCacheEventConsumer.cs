using Nop.Plugin.Misc.Forums.Domain;
using Nop.Services.Caching;

namespace Nop.Plugin.Misc.Forums.Services.Caching;

/// <summary>
/// Represents a forum cache event consumer
/// </summary>
public class ForumCacheEventConsumer : CacheEventConsumer<Forum>
{
    #region Methods

    /// <summary>
    /// Clear cache data
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected override async Task ClearCacheAsync(Forum entity)
    {
        await RemoveAsync(ForumDefaults.ForumByForumGroupCacheKey, entity.ForumGroupId);
    }

    #endregion
}