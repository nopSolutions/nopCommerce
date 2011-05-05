
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Orders
{
    public class ShoppingCartSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the shopping cart
        /// </summary>
        public int MaximumShoppingCartItems { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating maximum number of items in the wishlist
        /// </summary>
        public int MaximumWishlistItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product image on shopping cart page
        /// </summary>
        public bool ShowProductImagesOnShoppingCart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show product image on wishlist page
        /// </summary>
        public bool ShowProductImagesOnWishList { get; set; }
    }
}