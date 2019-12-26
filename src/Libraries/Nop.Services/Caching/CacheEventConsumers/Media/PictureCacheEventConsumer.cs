using Nop.Core.Domain.Media;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Media
{
    public partial class PictureCacheEventConsumer : EntityCacheEventConsumer<Picture>
    {
        public override void ClearCashe(Picture entity)
        {
            _cacheManager.RemoveByPrefix(NopMediaCachingDefaults.ThumbsPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopMediaCachingDefaults.PicturesPrefixCacheKey);

            base.ClearCashe(entity);
        }
    }
}
