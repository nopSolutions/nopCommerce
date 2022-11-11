using Nop.Core.Domain.Affiliates;
using Nop.Services.Caching;

namespace Nop.Services.Affiliates.Caching
{
    /// <summary>
    /// Represents an affiliate cache event consumer
    /// </summary>
    public partial class AffiliateCacheEventConsumer : CacheEventConsumer<Affiliate>
    {
    }
}
