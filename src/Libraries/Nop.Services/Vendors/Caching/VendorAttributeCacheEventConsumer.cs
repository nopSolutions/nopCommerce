using Nop.Core.Caching;
using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor attribute cache event consumer
    /// </summary>
    public partial class VendorAttributeCacheEventConsumer : CacheEventConsumer<VendorAttribute>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(VendorAttribute entity)
        {
            await Remove(NopVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity);
        }
    }
}
