
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


        /// <summary>
        /// Gets or sets a value indicating whether "wishlist" feature is enabled
        /// </summary>
        public bool WishlistEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "email a wishlist" feature is enabled
        /// </summary>
        public bool EmailWishlistEnabled { get; set; }


        /// <summary>Gets or sets a value indicating whether mini shopping catr is enabled
        /// </summary>
        public bool MiniShoppingCartEnabled { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to display products in mini shopping cart
        /// </summary>
        public bool MiniShoppingCartDisplayProducts { get; set; }


        //Round is already an issue. 
        //When enabled it can cause one issue: http://www.nopcommerce.com/boards/t/7679/vattax-rounding-error-important-fix.aspx
        //When disable it causes another one: http://www.nopcommerce.com/boards/t/11419/nop-20-order-of-steps-in-checkout.aspx?p=3#46924
        /// <summary>
        /// Gets or sets a value indicating whether to round calculated prices and total during calculation
        /// </summary>
        public bool RoundPricesDuringCalculation { get; set; }
    }
}