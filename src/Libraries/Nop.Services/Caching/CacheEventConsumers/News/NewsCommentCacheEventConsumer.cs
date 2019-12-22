using Nop.Core.Domain.News;

namespace Nop.Services.Caching.CacheEventConsumers.News
{
    /// <summary>
    /// Represents a news comment
    /// </summary>
    public partial class NewsCommentCacheEventConsumer : EntityCacheEventConsumer<NewsComment>
    {
    }
}