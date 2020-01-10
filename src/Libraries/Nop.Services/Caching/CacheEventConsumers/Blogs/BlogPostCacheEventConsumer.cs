using Nop.Core.Domain.Blogs;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Blogs
{
    /// <summary>
    /// Represents a blog post
    /// </summary>
    public partial class BlogPostCacheEventConsumer : CacheEventConsumer<BlogPost>
    {
        protected override void ClearCache(BlogPost entity)
        {
            RemoveByPrefix(NopBlogsCachingDefaults.BlogPrefixCacheKey);
        }
    }
}