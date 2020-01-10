using Nop.Core.Domain.Seo;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Seo
{
    public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
    {
        protected override void ClearCache(UrlRecord entity)
        {
            RemoveByPrefix(NopSeoCachingDefaults.UrlRecordPrefixCacheKey);
        }
    }
}
