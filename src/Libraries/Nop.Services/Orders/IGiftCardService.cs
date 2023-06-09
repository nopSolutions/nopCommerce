using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Gift card service interface
    /// </summary>
    public partial interface IGiftCardService
    {
        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteGiftCardAsync(GiftCard giftCard);

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card entry
        /// </returns>
        Task<GiftCard> GetGiftCardByIdAsync(int giftCardId);

        /// <summary>
        /// Gets all gift cards
        /// </summary>
        /// <param name="purchasedWithOrderId">Associated order ID; null to load all records</param>
        /// <param name="usedWithOrderId">The order ID in which the gift card was used; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated; null to load all records</param>
        /// <param name="giftCardCouponCode">Gift card coupon code; null to load all records</param>
        /// <param name="recipientName">Recipient name; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift cards
        /// </returns>
        Task<IPagedList<GiftCard>> GetAllGiftCardsAsync(int? purchasedWithOrderId = null, int? usedWithOrderId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertGiftCardAsync(GiftCard giftCard);

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateGiftCardAsync(GiftCard giftCard);

        /// <summary>
        /// Gets gift cards by 'PurchasedWithOrderItemId'
        /// </summary>
        /// <param name="purchasedWithOrderItemId">Purchased with order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card entries
        /// </returns>
        Task<IList<GiftCard>> GetGiftCardsByPurchasedWithOrderItemIdAsync(int purchasedWithOrderItemId);

        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the active gift cards
        /// </returns>
        Task<IList<GiftCard>> GetActiveGiftCardsAppliedByCustomerAsync(Customer customer);

        /// <summary>
        /// Generate new gift card code
        /// </summary>
        /// <returns>Result</returns>
        string GenerateGiftCardCode();

        /// <summary>
        /// Delete gift card usage history
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteGiftCardUsageHistoryAsync(Order order);

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card remaining amount
        /// </returns>
        Task<decimal> GetGiftCardRemainingAmountAsync(GiftCard giftCard);

        /// <summary>
        /// Gets a gift card usage history entries
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(GiftCard giftCard);

        /// <summary>
        /// Gets a gift card usage history entries
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(Order order);

        /// <summary>
        /// Inserts a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistory">Gift card usage history entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertGiftCardUsageHistoryAsync(GiftCardUsageHistory giftCardUsageHistory);

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<bool> IsGiftCardValidAsync(GiftCard giftCard);
    }
}