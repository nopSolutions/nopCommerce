using Nop.Core.Domain.Gdpr;

namespace Nop.Services.Caching.CacheEventConsumers.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
    }
}