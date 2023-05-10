using Nop.Core.Domain.News;
using Nop.Services.Caching;

namespace Nop.Services.News.Caching
{
    /// <summary>
    /// Represents a news comment cache event consumer
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : CacheEventConsumer<NewsComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(NewsComment entity)
        {
            await RemoveByPrefixAsync(NopNewsDefaults.NewsCommentsNumberPrefix, entity.NewsItemId);
        }
    }
}