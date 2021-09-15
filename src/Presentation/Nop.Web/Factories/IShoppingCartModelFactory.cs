using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the estimate shipping model
        /// </returns>
        Task<EstimateShippingModel> PrepareEstimateShippingModelAsync(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true);

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart model
        /// </returns>
        Task<ShoppingCartModel> PrepareShoppingCartModelAsync(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false);

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the wishlist model
        /// </returns>
        Task<WishlistModel> PrepareWishlistModelAsync(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true);

        /// <summary>
        /// Prepare the mini shopping cart model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the mini shopping cart model
        /// </returns>
        Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync();

        /// <summary>
        /// Prepare selected checkout attributes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted attributes
        /// </returns>
        Task<string> FormatSelectedCheckoutAttributesAsync();

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order totals model
        /// </returns>
        Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart, bool isEditable);

        /// <summary>
        /// Prepare the estimate shipping result model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="request">Request to get shipping options</param>
        /// <param name="cacheOfferedShippingOptions">Indicates whether to cache offered shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the estimate shipping result model
        /// </returns>
        Task<EstimateShippingResultModel> PrepareEstimateShippingResultModelAsync(IList<ShoppingCartItem> cart, EstimateShippingModel request, bool cacheOfferedShippingOptions);

        /// <summary>
        /// Prepare the wishlist email a friend model
        /// </summary>
        /// <param name="model">Wishlist email a friend model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the wishlist email a friend model
        /// </returns>
        Task<WishlistEmailAFriendModel> PrepareWishlistEmailAFriendModelAsync(WishlistEmailAFriendModel model, bool excludeProperties);

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="productName">Product name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the picture model
        /// </returns>
        Task<PictureModel> PrepareCartItemPictureModelAsync(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName);
    }
}
