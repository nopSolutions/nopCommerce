using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Factories
{
    public partial interface IShoppingCartModelFactory
    {
        PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize,bool showDefaultPicture, string productName);

        void PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false);

        void PrepareWishlistModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true);

        MiniShoppingCartModel PrepareMiniShoppingCartModel();

        OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable);

        EstimateShippingResultModel PrepareEstimateShippingResultModel(IList<ShoppingCartItem> cart, int? countryId, int? stateProvinceId, string zipPostalCode);

        WishlistEmailAFriendModel PrepareWishlistEmailAFriendModel(WishlistEmailAFriendModel model, bool excludeProperties);
    }
}
