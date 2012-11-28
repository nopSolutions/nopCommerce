using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Settings
{
    public partial class ShoppingCartSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct")]
        public bool DisplayCartAfterAddingProduct { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct")]
        public bool DisplayWishlistAfterAddingProduct { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumShoppingCartItems")]
        public int MaximumShoppingCartItems { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumWishlistItems")]
        public int MaximumWishlistItems { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist")]
        public bool AllowOutOfStockItemsToBeAddedToWishlist { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart")]
        public bool MoveItemsFromWishlistToCart { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnShoppingCart")]
        public bool ShowProductImagesOnShoppingCart { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnWishList")]
        public bool ShowProductImagesOnWishList { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowDiscountBox")]
        public bool ShowDiscountBox { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowGiftCardBox")]
        public bool ShowGiftCardBox { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CrossSellsNumber")]
        public int CrossSellsNumber { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.EmailWishlistEnabled")]
        public bool EmailWishlistEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist")]
        public bool AllowAnonymousUsersToEmailWishlist { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartEnabled")]
        public bool MiniShoppingCartEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesInMiniShoppingCart")]
        public bool ShowProductImagesInMiniShoppingCart { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartProductNumber")]
        public int MiniShoppingCartProductNumber { get; set; }
    }
}