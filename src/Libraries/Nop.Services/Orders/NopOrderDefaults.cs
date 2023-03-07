using Nop.Core.Caching;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class NopOrderDefaults
    {
        #region Caching defaults

        #region Checkout attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : A value indicating whether we should exclude shippable attributes
        /// </remarks>
        public static CacheKey CheckoutAttributesAllCacheKey => new("Nop.checkoutattribute.all.{0}-{1}", NopEntityCacheDefaults<CheckoutAttribute>.AllPrefix);

        #endregion

        #region ShoppingCart

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer ID
        /// {1} : shopping cart type
        /// {2} : store ID
        /// {3} : product ID
        /// {4} : created from date
        /// {5} : created to date
        /// </remarks>
        public static CacheKey ShoppingCartItemsAllCacheKey => new("Nop.shoppingcartitem.all.{0}-{1}-{2}-{3}-{4}-{5}", ShoppingCartItemsByCustomerPrefix, NopEntityCacheDefaults<ShoppingCartItem>.AllPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string ShoppingCartItemsByCustomerPrefix => "Nop.shoppingcartitem.all.{0}";


        #endregion

        #endregion
    }
}