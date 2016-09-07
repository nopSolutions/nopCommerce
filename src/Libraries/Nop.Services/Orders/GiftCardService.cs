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
        
        private readonly IRepository<GiftCard> _giftCardRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="giftCardRepository">Gift card context</param>
        /// <param name="eventPublisher">Event published</param>
        public GiftCardService(IRepository<GiftCard> giftCardRepository, IEventPublisher eventPublisher)
        {
            _giftCardRepository = giftCardRepository;
            _eventPublisher = eventPublisher;
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
                throw new ArgumentNullException("giftCard");

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
        /// <param name="giftCardCouponCode">Gift card coupon code; nullto load all records</param>
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
            if (!String.IsNullOrEmpty(giftCardCouponCode))
                query = query.Where(gc => gc.GiftCardCouponCode == giftCardCouponCode);
            if (!String.IsNullOrWhiteSpace(recipientName))
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
                throw new ArgumentNullException("giftCard");

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
                throw new ArgumentNullException("giftCard");

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

            string[] couponCodes = customer.ParseAppliedGiftCardCouponCodes();
            foreach (var couponCode in couponCodes)
            {
                var giftCards = GetAllGiftCards(isGiftCardActivated: true, giftCardCouponCode: couponCode);
                foreach (var gc in giftCards)
                {
                    if (gc.IsGiftCardValid())
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
            int length = 13;
            string result = Guid.NewGuid().ToString();
            if (result.Length > length)
                result = result.Substring(0, length);
            return result;
        }

        #endregion
    }
}
