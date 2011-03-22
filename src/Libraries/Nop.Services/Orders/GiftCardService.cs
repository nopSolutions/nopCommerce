

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Nop.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Customers;
using System.Xml;
using System.Diagnostics;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Gift card service
    /// </summary>
    public partial class GiftCardService : IGiftCardService
    {
        #region Fields
        
        private readonly IRepository<GiftCard> _giftCardRepository;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="giftCardRepository">Gift card context</param>
        public GiftCardService(IRepository<GiftCard> giftCardRepository)
        {
            this._giftCardRepository = giftCardRepository;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public void DeleteGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            _giftCardRepository.Delete(giftCard);
        }

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>Gift card entry</returns>
        public GiftCard GetGiftCardById(int giftCardId)
        {
            if (giftCardId == 0)
                return null;

            var giftCard = _giftCardRepository.GetById(giftCardId);
            return giftCard;
        }

        /// <summary>
        /// Gets all gift cards
        /// </summary>
        /// <param name="startTime">Order start time; null to load all records</param>
        /// <param name="endTime">Order end time; null to load all records</param>
        /// <param name="isGiftCardActivated">Value indicating whether gift card is activated; null to load all records</param>
        /// <param name="giftCardCouponCode">Gift card coupon code; null or string.empty to load all records</param>
        /// <returns>Gift cards</returns>
        public IList<GiftCard> GetAllGiftCards(DateTime? startTime = null, DateTime? endTime = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = "")
        {
            var query = _giftCardRepository.Table;
            if (startTime.HasValue)
                query = query.Where(gc => startTime.Value <= gc.CreatedOnUtc);
            if (endTime.HasValue)
                query = query.Where(gc => endTime.Value >= gc.CreatedOnUtc);
            if (isGiftCardActivated.HasValue)
                query = query.Where(gc => gc.IsGiftCardActivated == isGiftCardActivated.Value);
            if (!String.IsNullOrEmpty(giftCardCouponCode))
                query = query.Where(gc => gc.GiftCardCouponCode == giftCardCouponCode);
            query = query.OrderByDescending(gc => gc.CreatedOnUtc);

            var giftCards = query.ToList();
            return giftCards;
        }

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public void InsertGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            _giftCardRepository.Insert(giftCard);
        }

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        public void UpdateGiftCard(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            _giftCardRepository.Update(giftCard);
        }


        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>Active gift cards</returns>
        public IList<GiftCard> GetActiveGiftCardsAppliedByCustomer(Customer customer)
        {
            var result = new List<GiftCard>();
            if (customer == null)
                return result;

            string[] couponCodes = customer.ParseAppliedGiftCardCouponCodes();
            foreach (var couponCode in couponCodes)
            {
                var giftCards = GetAllGiftCards(null, null, true, couponCode);
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
        public string GenerateGiftCardCode()
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
