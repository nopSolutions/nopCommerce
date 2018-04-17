using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the gift card model factory
    /// </summary>
    public partial interface IGiftCardModelFactory
    {
        /// <summary>
        /// Prepare gift card search model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>Gift card search model</returns>
        GiftCardSearchModel PrepareGiftCardSearchModel(GiftCardSearchModel searchModel);

        /// <summary>
        /// Prepare paged gift card list model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>Gift card list model</returns>
        GiftCardListModel PrepareGiftCardListModel(GiftCardSearchModel searchModel);

        /// <summary>
        /// Prepare gift card model
        /// </summary>
        /// <param name="model">Gift card model</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Gift card model</returns>
        GiftCardModel PrepareGiftCardModel(GiftCardModel model, GiftCard giftCard, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged gift usage history card list model
        /// </summary>
        /// <param name="searchModel">Gift card usage history search model</param>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Gift card usage history list model</returns>
        GiftCardUsageHistoryListModel PrepareGiftCardUsageHistoryListModel(GiftCardUsageHistorySearchModel searchModel,
            GiftCard giftCard);
    }
}