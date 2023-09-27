using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Stores;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute service extensions
    /// </summary>
    public static partial class CheckoutAttributeServiceExtensions
    {
        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="service">Checkout attribute service</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attributes
        /// </returns>
        public static async Task<IList<CheckoutAttribute>> GetAllAttributesAsync(this IAttributeService<CheckoutAttribute, CheckoutAttributeValue> service, IStaticCacheManager staticCacheManager, IStoreMappingService storeMappingService, int storeId = 0, bool excludeShippableAttributes = false)
        {
            var key = staticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);

            return await staticCacheManager.GetAsync(key, async () =>
            {
                var checkoutAttributes = (await service.GetAllAttributesAsync()).ToAsyncEnumerable();

                if (storeId > 0)
                    //store mapping
                    checkoutAttributes = checkoutAttributes.WhereAwait(async ca => await storeMappingService.AuthorizeAsync(ca, storeId));

                if (excludeShippableAttributes)
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired);

                return await checkoutAttributes.ToListAsync();
            });
        }
    }
}