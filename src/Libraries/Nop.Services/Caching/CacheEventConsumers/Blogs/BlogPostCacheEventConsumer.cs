using Nop.Core.Domain.Blogs;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Blogs
{
    /// <summary>
    /// Represents a blog post cache event consumer
    /// </summary>
    public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(BlogPost entity)
        {
            RemoveByPrefix(NopBlogsCachingDefaults.BlogPrefixCacheKey);
        }
    }
}