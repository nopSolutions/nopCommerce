using System.Threading.Tasks;
using Nop.Core.Domain.Gdpr;
using Nop.Services.Caching;

namespace Nop.Services.Gdpr.Caching
{
    /// <summary>
    /// Represents a GDPR consent cache event consumer
    /// </summary>
    public partial class GdprConsentCacheEventConsumer : CacheEventConsumer<GdprConsent>
    {
    }
}