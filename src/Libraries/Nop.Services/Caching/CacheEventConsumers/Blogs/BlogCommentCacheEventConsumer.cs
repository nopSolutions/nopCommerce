using Nop.Core.Domain.Blogs;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Blogs
{
    /// <summary>
    /// Represents a blog comment cache event consumer
    /// </summary>
    public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(BlogComment entity)
        {
            RemoveByPrefix(NopBlogsCachingDefaults.BlogCommentsPrefixCacheKey);
        }
    }
}