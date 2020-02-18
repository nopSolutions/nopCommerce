using Nop.Core.Domain.Gdpr;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
        protected override void ClearCache(GdprConsent entity)
        {
            RemoveByPrefix(NopGdprCachingDefaults.ConsentsPrefixCacheKey);
        }
    }
}