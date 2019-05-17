namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents default values related to shipping services
    /// </summary>
    public static partial class NopShippingDefaults
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
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string WarehousesPrefixCacheKey => "Nop.warehouse.";

        #endregion
    }
}