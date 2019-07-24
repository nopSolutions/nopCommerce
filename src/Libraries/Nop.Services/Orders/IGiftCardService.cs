using System;
using System.Collections.Generic;
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
        void DeleteGiftCard(GiftCard giftCard);

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        GiftCard GetGiftCardById(int giftCardId);

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
        /// <returns>Gift cards</returns>
        IPagedList<GiftCard> GetAllGiftCards(int? purchasedWithOrderId = null, int? usedWithOrderId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        void InsertGiftCard(GiftCard giftCard);

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        void UpdateGiftCard(GiftCard giftCard);

        /// <summary>
        /// Gets gift cards by 'PurchasedWithOrderItemId'
        /// </summary>
        /// <param name="purchasedWithOrderItemId">Purchased with order item identifier</param>
        /// <returns>Gift card entries</returns>
        IList<GiftCard> GetGiftCardsByPurchasedWithOrderItemId(int purchasedWithOrderItemId);

        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Active gift cards</returns>
        IList<GiftCard> GetActiveGiftCardsAppliedByCustomer(Customer customer);

        /// <summary>
        /// Generate new gift card code
        /// </summary>
        /// <returns>Result</returns>
        string GenerateGiftCardCode();

        /// <summary>
        /// Delete gift card usage history
        /// </summary>
        /// <param name="order">Order</param>
        void DeleteGiftCardUsageHistory(Order order);

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <returns>Gift card remaining amount</returns>
        decimal GetGiftCardRemainingAmount(GiftCard giftCard);

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Result</returns>
        bool IsGiftCardValid(GiftCard giftCard);
    }
}