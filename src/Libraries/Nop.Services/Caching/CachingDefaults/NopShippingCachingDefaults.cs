namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class NopShippingCachingDefaults
    {
        #region Shipping methods

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : country identifier
        /// </remarks>
        public static string ShippingMethodsAllCacheKey => "Nop.shippingmethod.all-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ShippingMethodsPrefixCacheKey => "Nop.shippingmethod.";

        #endregion

        #region Warehouses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : warehouse ID
        /// </remarks>
        public static string WarehousesByIdCacheKey => "Nop.warehouse.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string WarehousesAllCacheKey => "Nop.warehouse.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string WarehousesPrefixCacheKey => "Nop.warehouse.";

        #endregion

        #region Date

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string DeliveryDatesAllCacheKey => "Nop.deliverydates.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string DeliveryDatesPrefixCacheKey => "Nop.deliverydates.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string ProductAvailabilityAllCacheKey => "Nop.productavailability.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAvailabilityPrefixCacheKey => "Nop.productavailability.";
        

        #endregion
    }
}