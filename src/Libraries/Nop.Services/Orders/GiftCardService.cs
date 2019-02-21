using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Gift card service
    /// </summary>
    public partial class GiftCardService : IGiftCardService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<GiftCard> _giftCardRepository;
        private readonly IRepository<GiftCardUsageHistory> _giftCardUsageHistoryRepository;

        #endregion

        #region Ctor

        public GiftCardService(ICustomerService customerService,
            IEventPublisher eventPublisher,
            IRepository<GiftCard> giftCardRepository,
            IRepository<GiftCardUsageHistory> giftCardUsageHistoryRepository)
        {
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _giftCardRepository = giftCardRepository;
            _giftCardUsageHistoryRepository = giftCardUsageHistoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public virtual void DeleteGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            _giftCardRepository.Delete(giftCard);

            //event notification
            _eventPublisher.EntityDeleted(giftCard);
        }

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        public virtual GiftCard GetGiftCardById(int giftCardId)
        {
            if (giftCardId == 0)
                return null;

            return _giftCardRepository.GetById(giftCardId);
        }

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
        public virtual IPagedList<GiftCard> GetAllGiftCards(int? purchasedWithOrderId = null, int? usedWithOrderId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _giftCardRepository.Table;
            if (purchasedWithOrderId.HasValue)
                query = query.Where(gc => gc.PurchasedWithOrderItem != null && gc.PurchasedWithOrderItem.OrderId == purchasedWithOrderId.Value);
            if (usedWithOrderId.HasValue)
                query = query.Where(gc => gc.GiftCardUsageHistory.Any(history => history.UsedWithOrderId == usedWithOrderId));
            if (createdFromUtc.HasValue)
                query = query.Where(gc => createdFromUtc.Value <= gc.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(gc => createdToUtc.Value >= gc.CreatedOnUtc);
            if (isGiftCardActivated.HasValue)
                query = query.Where(gc => gc.IsGiftCardActivated == isGiftCardActivated.Value);
            if (!string.IsNullOrEmpty(giftCardCouponCode))
                query = query.Where(gc => gc.GiftCardCouponCode == giftCardCouponCode);
            if (!string.IsNullOrWhiteSpace(recipientName))
                query = query.Where(c => c.RecipientName.Contains(recipientName));
            query = query.OrderByDescending(gc => gc.CreatedOnUtc);

            var giftCards = new PagedList<GiftCard>(query, pageIndex, pageSize);
            return giftCards;
        }

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public virtual void InsertGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            _giftCardRepository.Insert(giftCard);

            //event notification
            _eventPublisher.EntityInserted(giftCard);
        }

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public virtual void UpdateGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            _giftCardRepository.Update(giftCard);

            //event notification
            _eventPublisher.EntityUpdated(giftCard);
        }

        /// <summary>
        /// Gets gift cards by 'PurchasedWithOrderItemId'
        /// </summary>
        /// <param name="purchasedWithOrderItemId">Purchased with order item identifier</param>
        /// <returns>Gift card entries</returns>
        public virtual IList<GiftCard> GetGiftCardsByPurchasedWithOrderItemId(int purchasedWithOrderItemId)
        {
            if (purchasedWithOrderItemId == 0)
                return new List<GiftCard>();

            var query = _giftCardRepository.Table;
            query = query.Where(gc => gc.PurchasedWithOrderItemId.HasValue && gc.PurchasedWithOrderItemId.Value == purchasedWithOrderItemId);
            query = query.OrderBy(gc => gc.Id);

            var giftCards = query.ToList();
            return giftCards;
        }

        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Active gift cards</returns>
        public virtual IList<GiftCard> GetActiveGiftCardsAppliedByCustomer(Customer customer)
        {
            var result = new List<GiftCard>();
            if (customer == null)
                return result;

            var couponCodes = _customerService.ParseAppliedGiftCardCouponCodes(customer);
            foreach (var couponCode in couponCodes)
            {
                var giftCards = GetAllGiftCards(isGiftCardActivated: true, giftCardCouponCode: couponCode);
                foreach (var gc in giftCards)
                {
                    if (IsGiftCardValid(gc))
                        result.Add(gc);
                }
            }

            return result;
        }

        /// <summary>
        /// Generate new gift card code
        /// </summary>
        /// <returns>Result</returns>
        public virtual string GenerateGiftCardCode()
        {
            var length = 13;
            var result = Guid.NewGuid().ToString();
            if (result.Length > length)
                result = result.Substring(0, length);
            return result;
        }

        /// <summary>
        /// Delete gift card usage history
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void DeleteGiftCardUsageHistory(Order order)
        {
            var giftCardUsageHistory = order.GiftCardUsageHistory.ToList();
            var giftCards = giftCardUsageHistory.Select(gcuh => gcuh.GiftCard).ToList();
            _giftCardUsageHistoryRepository.Delete(giftCardUsageHistory);

            foreach (var giftCard in giftCards)
            {
                UpdateGiftCard(giftCard);
            }
        }

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <returns>Gift card remaining amount</returns>
        public virtual decimal GetGiftCardRemainingAmount(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            var result = giftCard.Amount;

            foreach (var gcuh in giftCard.GiftCardUsageHistory)
                result -= gcuh.UsedValue;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>Result</returns>
        public virtual bool IsGiftCardValid(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            if (!giftCard.IsGiftCardActivated)
                return false;

            var remainingAmount = GetGiftCardRemainingAmount(giftCard);
            if (remainingAmount > decimal.Zero)
                return true;

            return false;
        }

        #endregion
    }
}