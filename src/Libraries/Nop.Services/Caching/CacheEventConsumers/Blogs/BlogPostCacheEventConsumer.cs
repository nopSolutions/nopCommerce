using Nop.Core.Domain.Blogs;

namespace Nop.Services.Caching.CacheEventConsumers.Blogs
{
    /// <summary>
    /// Represents a blog post
    /// </summary>
    public partial class BlogPostCacheEventConsumer : EntityCacheEventConsumer<BlogPost>
    {
    }
}