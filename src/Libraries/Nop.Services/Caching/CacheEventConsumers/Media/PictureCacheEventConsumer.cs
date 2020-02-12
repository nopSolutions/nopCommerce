using Nop.Core.Domain.Media;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Media
{
    /// <summary>
    /// Represents a picture cache event consumer
    /// </summary>
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Picture entity)
        {
            RemoveByPrefix(NopMediaCachingDefaults.ThumbsPrefixCacheKey);
            RemoveByPrefix(NopMediaCachingDefaults.PicturesPrefixCacheKey);
        }
    }
}
