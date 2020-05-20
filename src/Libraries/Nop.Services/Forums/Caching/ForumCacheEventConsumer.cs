using Nop.Core.Domain.Forums;
using Nop.Services.Caching;

namespace Nop.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum cache event consumer
    /// </summary>
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Forum entity)
        {
            var cacheKey = _cacheKeyService.PrepareKey(NopForumDefaults.ForumAllByForumGroupIdCacheKey, entity.ForumGroupId);
            Remove(cacheKey);
        }
    }
}
