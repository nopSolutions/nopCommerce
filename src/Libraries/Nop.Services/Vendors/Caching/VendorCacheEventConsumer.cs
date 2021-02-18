using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;

namespace Nop.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor cache event consumer
    /// </summary>
    public partial class VendorCacheEventConsumer : CacheEventConsumer<Vendor>
    {
    }
}
