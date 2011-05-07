
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

        /// <summary>
        /// Gets or sets a value indicating whether to show discount box on shopping cart page
        /// </summary>
        public bool ShowDiscountBox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show gift card box on shopping cart page
        /// </summary>
        public bool ShowGiftCardBox { get; set; }

        /// <summary>
        /// Gets or sets a number of "Cross-sells" on shopping cart page
        /// </summary>
        public int CrossSellsNumber { get; set; }
    }
}