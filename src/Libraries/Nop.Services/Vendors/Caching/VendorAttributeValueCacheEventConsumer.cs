using Nop.Core.Domain.Vendors;
using Nop.Services.Caching;
using System.Threading.Tasks;

namespace Nop.Services.Vendors.Caching
{
    /// <summary>
    /// Represents a vendor attribute value cache event consumer
    /// </summary>
    public partial class VendorAttributeValueCacheEventConsumer : CacheEventConsumer<VendorAttributeValue>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCache(VendorAttributeValue entity)
        {
            await Remove(NopVendorDefaults.VendorAttributeValuesByAttributeCacheKey, entity.VendorAttributeId);
        }
    }
}
