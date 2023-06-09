using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Gift card service
    /// </summary>
    public partial class GiftCardService : IGiftCardService
    {
        #region Fields

        protected readonly ICustomerService _customerService;
        protected readonly IEventPublisher _eventPublisher;
        protected readonly IRepository<GiftCard> _giftCardRepository;
        protected readonly IRepository<GiftCardUsageHistory> _giftCardUsageHistoryRepository;
        protected readonly IRepository<OrderItem> _orderItemRepository;
        #endregion

        #region Ctor

        public GiftCardService(ICustomerService customerService,
            IEventPublisher eventPublisher,
            IRepository<GiftCard> giftCardRepository,
            IRepository<GiftCardUsageHistory> giftCardUsageHistoryRepository,
            IRepository<OrderItem> orderItemRepository)
        {
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _giftCardRepository = giftCardRepository;
            _giftCardUsageHistoryRepository = giftCardUsageHistoryRepository;
            _orderItemRepository = orderItemRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteGiftCardAsync(GiftCard giftCard)
        {
            await _giftCardRepository.DeleteAsync(giftCard);
        }

        /// <summary>
        /// Gets a gift card
        /// </summary>
        /// <param name="giftCardId">Gift card identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card entry
        /// </returns>
        public virtual async Task<GiftCard> GetGiftCardByIdAsync(int giftCardId)
        {
            return await _giftCardRepository.GetByIdAsync(giftCardId, cache => default);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift cards
        /// </returns>
        public virtual async Task<IPagedList<GiftCard>> GetAllGiftCardsAsync(int? purchasedWithOrderId = null, int? usedWithOrderId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            bool? isGiftCardActivated = null, string giftCardCouponCode = null,
            string recipientName = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var giftCards = await _giftCardRepository.GetAllPagedAsync(query =>
            {
                if (purchasedWithOrderId.HasValue)
                {
                    query = from gc in query
                            join oi in _orderItemRepository.Table on gc.PurchasedWithOrderItemId equals oi.Id
                            where oi.OrderId == purchasedWithOrderId.Value
                            select gc;
                }

                if (usedWithOrderId.HasValue)
                    query = from gc in query
                            join gcuh in _giftCardUsageHistoryRepository.Table on gc.Id equals gcuh.GiftCardId
                            where gcuh.UsedWithOrderId == usedWithOrderId
                            select gc;

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

                return query;
            }, pageIndex, pageSize);

            return giftCards;
        }

        /// <summary>
        /// Inserts a gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertGiftCardAsync(GiftCard giftCard)
        {
            await _giftCardRepository.InsertAsync(giftCard);
        }

        /// <summary>
        /// Updates the gift card
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateGiftCardAsync(GiftCard giftCard)
        {
            await _giftCardRepository.UpdateAsync(giftCard);
        }

        /// <summary>
        /// Gets gift cards by 'PurchasedWithOrderItemId'
        /// </summary>
        /// <param name="purchasedWithOrderItemId">Purchased with order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card entries
        /// </returns>
        public virtual async Task<IList<GiftCard>> GetGiftCardsByPurchasedWithOrderItemIdAsync(int purchasedWithOrderItemId)
        {
            if (purchasedWithOrderItemId == 0)
                return new List<GiftCard>();

            var query = _giftCardRepository.Table;
            query = query.Where(gc => gc.PurchasedWithOrderItemId.HasValue && gc.PurchasedWithOrderItemId.Value == purchasedWithOrderItemId);
            query = query.OrderBy(gc => gc.Id);

            var giftCards = await query.ToListAsync();

            return giftCards;
        }

        /// <summary>
        /// Get active gift cards that are applied by a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the active gift cards
        /// </returns>
        public virtual async Task<IList<GiftCard>> GetActiveGiftCardsAppliedByCustomerAsync(Customer customer)
        {
            var result = new List<GiftCard>();
            if (customer == null)
                return result;

            var couponCodes = await _customerService.ParseAppliedGiftCardCouponCodesAsync(customer);
            foreach (var couponCode in couponCodes)
            {
                var giftCards = await GetAllGiftCardsAsync(isGiftCardActivated: true, giftCardCouponCode: couponCode);
                foreach (var gc in giftCards)
                    if (await IsGiftCardValidAsync(gc))
                        result.Add(gc);
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
                result = result[0..length];
            return result;
        }

        /// <summary>
        /// Delete gift card usage history
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteGiftCardUsageHistoryAsync(Order order)
        {
            var giftCardUsageHistory = await GetGiftCardUsageHistoryAsync(order);

            await _giftCardUsageHistoryRepository.DeleteAsync(giftCardUsageHistory);

            var query = _giftCardRepository.Table;

            var giftCardIds = giftCardUsageHistory.Select(gcuh => gcuh.GiftCardId).ToArray();
            var giftCards = await query.Where(bp => giftCardIds.Contains(bp.Id)).ToListAsync();

            //event notification
            foreach (var giftCard in giftCards)
                await _eventPublisher.EntityUpdatedAsync(giftCard);
        }

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gift card remaining amount
        /// </returns>
        public virtual async Task<decimal> GetGiftCardRemainingAmountAsync(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            var result = giftCard.Amount;

            foreach (var gcuh in await GetGiftCardUsageHistoryAsync(giftCard))
                result -= gcuh.UsedValue;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Gets a gift card usage history entries
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(GiftCard giftCard)
        {
            if (giftCard is null)
                throw new ArgumentNullException(nameof(giftCard));

            return await _giftCardUsageHistoryRepository.Table
                .Where(gcuh => gcuh.GiftCardId == giftCard.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a gift card usage history entries
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<GiftCardUsageHistory>> GetGiftCardUsageHistoryAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            return await _giftCardUsageHistoryRepository.Table
                .Where(gcuh => gcuh.UsedWithOrderId == order.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Inserts a gift card usage history entry
        /// </summary>
        /// <param name="giftCardUsageHistory">Gift card usage history entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertGiftCardUsageHistoryAsync(GiftCardUsageHistory giftCardUsageHistory)
        {
            await _giftCardUsageHistoryRepository.InsertAsync(giftCardUsageHistory);
        }

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsGiftCardValidAsync(GiftCard giftCard)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            if (!giftCard.IsGiftCardActivated)
                return false;

            var remainingAmount = await GetGiftCardRemainingAmountAsync(giftCard);
            if (remainingAmount > decimal.Zero)
                return true;

            return false;
        }

        #endregion
    }
}