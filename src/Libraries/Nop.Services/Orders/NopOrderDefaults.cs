namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class NopOrderDefaults
    {
        #region Checkout attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : >A value indicating whether we should exclude shippable attributes
        /// </remarks>
        public static string CheckoutAttributesAllCacheKey => "Nop.checkoutattribute.all-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        public static string CheckoutAttributesByIdCacheKey => "Nop.checkoutattribute.id-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute ID
        /// </remarks>
        public static string CheckoutAttributeValuesAllCacheKey => "Nop.checkoutattributevalue.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : checkout attribute value ID
        /// </remarks>
        public static string CheckoutAttributeValuesByIdCacheKey => "Nop.checkoutattributevalue.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CheckoutAttributesPrefixCacheKey => "Nop.checkoutattribute.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CheckoutAttributeValuesPrefixCacheKey => "Nop.checkoutattributevalue.";

        #endregion
    }
}