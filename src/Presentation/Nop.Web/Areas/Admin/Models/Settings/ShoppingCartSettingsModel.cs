using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a shopping cart settings model
    /// </summary>
    public partial class ShoppingCartSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayCartAfterAddingProduct")]
        public bool DisplayCartAfterAddingProduct { get; set; }
        public bool DisplayCartAfterAddingProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.DisplayWishlistAfterAddingProduct")]
        public bool DisplayWishlistAfterAddingProduct { get; set; }
        public bool DisplayWishlistAfterAddingProduct_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumShoppingCartItems")]
        public int MaximumShoppingCartItems { get; set; }
        public bool MaximumShoppingCartItems_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MaximumWishlistItems")]
        public int MaximumWishlistItems { get; set; }
        public bool MaximumWishlistItems_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowOutOfStockItemsToBeAddedToWishlist")]
        public bool AllowOutOfStockItemsToBeAddedToWishlist { get; set; }
        public bool AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MoveItemsFromWishlistToCart")]
        public bool MoveItemsFromWishlistToCart { get; set; }
        public bool MoveItemsFromWishlistToCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CartsSharedBetweenStores")]
        public bool CartsSharedBetweenStores { get; set; }
        public bool CartsSharedBetweenStores_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnShoppingCart")]
        public bool ShowProductImagesOnShoppingCart { get; set; }
        public bool ShowProductImagesOnShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesOnWishList")]
        public bool ShowProductImagesOnWishList { get; set; }
        public bool ShowProductImagesOnWishList_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowDiscountBox")]
        public bool ShowDiscountBox { get; set; }
        public bool ShowDiscountBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowGiftCardBox")]
        public bool ShowGiftCardBox { get; set; }
        public bool ShowGiftCardBox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.CrossSellsNumber")]
        public int CrossSellsNumber { get; set; }
        public bool CrossSellsNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.EmailWishlistEnabled")]
        public bool EmailWishlistEnabled { get; set; }
        public bool EmailWishlistEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowAnonymousUsersToEmailWishlist")]
        public bool AllowAnonymousUsersToEmailWishlist { get; set; }
        public bool AllowAnonymousUsersToEmailWishlist_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartEnabled")]
        public bool MiniShoppingCartEnabled { get; set; }
        public bool MiniShoppingCartEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.ShowProductImagesInMiniShoppingCart")]
        public bool ShowProductImagesInMiniShoppingCart { get; set; }
        public bool ShowProductImagesInMiniShoppingCart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.MiniShoppingCartProductNumber")]
        public int MiniShoppingCartProductNumber { get; set; }
        public bool MiniShoppingCartProductNumber_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.AllowCartItemEditing")]
        public bool AllowCartItemEditing { get; set; }
        public bool AllowCartItemEditing_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.ShoppingCart.GroupTierPricesForDistinctShoppingCartItems")]
        public bool GroupTierPricesForDistinctShoppingCartItems { get; set; }
        public bool GroupTierPricesForDistinctShoppingCartItems_OverrideForStore { get; set; }

        #endregion
    }
}