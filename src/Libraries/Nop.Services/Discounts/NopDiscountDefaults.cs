using Nop.Core.Caching;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents default values related to discounts services
    /// </summary>
    public static partial class NopDiscountDefaults
    {
        /// <summary>
        /// Gets the query parameter name to retrieve discount coupon code from URL
        /// </summary>
        public static string DiscountCouponQueryParameter => "discountcoupon";

        #region Caching defaults

        /// <summary>
        /// Key for discount requirement of a certain discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static CacheKey DiscountRequirementsByDiscountCacheKey => new("Nop.discountrequirement.bydiscount.{0}");

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : coupon code
        /// {2} : discount name
        /// {3} : is active
        /// </remarks>
        public static CacheKey DiscountAllCacheKey => new("Nop.discount.all.{0}-{1}-{2}-{3}", NopEntityCacheDefaults<Discount>.AllPrefix);

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} - entity type
        /// {1} - entity id
        /// </remarks>
        public static CacheKey AppliedDiscountsCacheKey => new("Nop.discount.applied.{0}-{1}", AppliedDiscountsCachePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AppliedDiscountsCachePrefix => "Nop.discount.applied.";

        /// <summary>
        /// Key for category IDs of a discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryIdsByDiscountCacheKey => new("Nop.discount.categoryids.bydiscount.{0}-{1}-{2}", CategoryIdsByDiscountPrefix, CategoryIdsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static string CategoryIdsByDiscountPrefix => "Nop.discount.categoryids.bydiscount.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryIdsPrefix => "Nop.discount.categoryids.bydiscount.";

        /// <summary>
        /// Key for manufacturer IDs of a discount
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerIdsByDiscountCacheKey => new("Nop.discount.manufacturerids.bydiscount.{0}-{1}-{2}", ManufacturerIdsByDiscountPrefix, ManufacturerIdsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : discount id
        /// </remarks>
        public static string ManufacturerIdsByDiscountPrefix => "Nop.discount.manufacturerids.bydiscount.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturerIdsPrefix => "Nop.discount.manufacturerids.bydiscount.";

        #endregion
    }
}