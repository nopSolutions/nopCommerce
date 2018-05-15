using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the shopping cart model factory
    /// </summary>
    public partial interface IShoppingCartModelFactory
    {
        /// <summary>
        /// Prepare the estimate shipping model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
        /// <returns>Estimate shipping model</returns>
        EstimateShippingModel PrepareEstimateShippingModel(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true);

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>Picture model</returns>
        PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize,bool showDefaultPicture, string productName);

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>Shopping cart model</returns>
        ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false);

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Wishlist model</returns>
        WishlistModel PrepareWishlistModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true);

        /// <summary>
        /// Prepare the mini shopping cart model
        /// </summary>
        /// <returns>Mini shopping cart model</returns>
        MiniShoppingCartModel PrepareMiniShoppingCartModel();

        /// <summary>
        /// Prepare selected checkout attributes
        /// </summary>
        /// <returns>Formatted attributes</returns>
        string FormatSelectedCheckoutAttributes();

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>Order totals model</returns>
        OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable);

        /// <summary>
        /// Prepare the estimate shipping result model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="stateProvinceId">State or province identifier</param>
        /// <param name="zipPostalCode">Zip postal code</param>
        /// <returns>Estimate shipping result model</returns>
        EstimateShippingResultModel PrepareEstimateShippingResultModel(IList<ShoppingCartItem> cart, int? countryId, int? stateProvinceId, string zipPostalCode);

        /// <summary>
        /// Prepare the wishlist email a friend model
        /// </summary>
        /// <param name="model">Wishlist email a friend model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Wishlist email a friend model</returns>
        WishlistEmailAFriendModel PrepareWishlistEmailAFriendModel(WishlistEmailAFriendModel model, bool excludeProperties);
    }
}
