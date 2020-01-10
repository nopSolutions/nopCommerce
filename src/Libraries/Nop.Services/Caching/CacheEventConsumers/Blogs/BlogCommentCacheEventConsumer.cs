using Nop.Core.Domain.Blogs;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Blogs
{
    /// <summary>
    /// Represents a blog comment
    /// </summary>
    public partial class BlogCommentCacheEventConsumer : CacheEventConsumer<BlogComment>
    {
        protected override void ClearCache(BlogComment entity)
        {
            RemoveByPrefix(NopBlogsCachingDefaults.BlogCommentsPrefixCacheKey);
        }
    }
}