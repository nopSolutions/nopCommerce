using Nop.Core.Domain.Media;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Media
{
    public partial class PictureCacheEventConsumer : CacheEventConsumer<Picture>
    {
        protected override void ClearCache(Picture entity)
        {
            RemoveByPrefix(NopMediaCachingDefaults.ThumbsPrefixCacheKey);
            RemoveByPrefix(NopMediaCachingDefaults.PicturesPrefixCacheKey);
        }
    }
}
