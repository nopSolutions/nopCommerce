using Nop.Core.Domain.Seo;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Seo
{
    public partial class UrlRecordCacheEventConsumer : CacheEventConsumer<UrlRecord>
    {
        public override void ClearCashe(UrlRecord entity)
        {
            RemoveByPrefix(NopSeoCachingDefaults.UrlRecordPrefixCacheKey);
        }
    }
}
