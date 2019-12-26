using Nop.Core.Domain.Seo;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Seo
{
    public partial class UrlRecordCacheEventConsumer : EntityCacheEventConsumer<UrlRecord>
    {
        public override void ClearCashe(UrlRecord entity)
        {
            _cacheManager.RemoveByPrefix(NopSeoCachingDefaults.UrlRecordPrefixCacheKey);

            base.ClearCashe(entity);
        }
    }
}
