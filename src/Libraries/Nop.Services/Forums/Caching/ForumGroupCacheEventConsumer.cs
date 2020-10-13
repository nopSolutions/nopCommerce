using Nop.Core.Domain.Forums;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Forums.Caching
{
    /// <summary>
    /// Represents a forum group cache event consumer
    /// </summary>
    public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(ForumGroup entity)
        {
            await Remove(NopForumDefaults.ForumByForumGroupCacheKey, entity);
        }
    }
}
