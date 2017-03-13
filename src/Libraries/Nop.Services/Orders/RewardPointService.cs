using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Catalog;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Reward point service
    /// </summary>
    public partial class RewardPointService : IRewardPointService
    {
        #region Fields

        private readonly IRepository<RewardPointsHistory> _rphRepository;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rphRepository">RewardPointsHistory repository</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="shoppingCartSettings">Shopping cart settings</param>
        public RewardPointService(IRepository<RewardPointsHistory> rphRepository,
            RewardPointsSettings rewardPointsSettings,
            IStoreContext storeContext,
            IEventPublisher eventPublisher,
            ShoppingCartSettings shoppingCartSettings)
        {
            this._rphRepository = rphRepository;
            this._rewardPointsSettings = rewardPointsSettings;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
            this._shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Update reward points balance if necessary
        /// </summary>
        /// <param name="query">Input query</param>
        protected void UpdateRewardPointsBalance(IQueryable<RewardPointsHistory> query)
        {
            //order history by date
            query = query.OrderBy(rph => rph.CreatedOnUtc).ThenBy(rph => rph.Id);

            //get has not yet activated points, but it's time to do it
            //no need to search for !rph.PointsBalancePurchased.HasValue additionally to !rph.PointsBalance as both values will be set contemporarily
            var notActivatedRph = query.Where(rph => (!rph.PointsBalance.HasValue) && rph.CreatedOnUtc < DateTime.UtcNow).ToList();

            //nothing to update
            if (!notActivatedRph.Any())
                return;

            //get current points balance, LINQ to entities does not support Last method, thus order by desc and use First one
            var lastActive = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id).FirstOrDefault(rph => rph.PointsBalance.HasValue);
            //no need to get lastActivePurchased
            //var lastActivePurchased = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id).FirstOrDefault(rph => rph.PointsBalancePurchased.HasValue);
            var currentPointsBalance = lastActive != null ? lastActive.PointsBalance : 0;
            var currentPointsBalancePurchased = lastActive != null ? lastActive.PointsBalancePurchased : 0;

            //update appropriate records
            foreach (var rph in notActivatedRph)
            {
                rph.PointsBalance = currentPointsBalance + rph.Points;
                rph.PointsBalancePurchased = currentPointsBalancePurchased + rph.PointsPurchased;
                UpdateRewardPointsHistoryEntry(rph);
                currentPointsBalance = rph.PointsBalance;
                currentPointsBalancePurchased = rph.PointsBalancePurchased;
            }
        }

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertRewardPointsToAmount(int rewardPoints)
        {
            //convert all
            //if (rewardPoints <= 0)
            //    return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = RoundingHelper.RoundAmount(result);
            return result;
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        public virtual int ConvertAmountToRewardPoints(decimal amount, decimal? overridenExchangeRate = null)
        {
            int result = 0;
            //convert all
            //if (amount <= 0)
            //    return 0;
            var exchangeRate = overridenExchangeRate.HasValue ? overridenExchangeRate.Value : _rewardPointsSettings.ExchangeRate;

            if (exchangeRate > 0)
                result = (int)Math.Ceiling(amount / exchangeRate);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Base amount (in primary store currency) for points calculation</param>
        /// <returns>Number of reward points</returns>
        public virtual int CalculateRewardPoints(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || customer.IsGuest())
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount * _rewardPointsSettings.PointsForPurchases_Points);
            return points;
        }

        /// <summary>
        /// Calculate base amount for reward points calculation
        /// </summary>
        /// <param name="amount">Base amount (in primary store currency) for points calculation</param>
        /// <param name="purchasedPointsAmount">Amount of used purchased reward points</param>
        /// <returns>base amount</returns>
        public virtual decimal GetRewardPointsBaseAmount(decimal amount, decimal purchasedPointsAmount)
        {
            if (_rewardPointsSettings.EarnRewardPointsOnlyWhenUsingPurchasedRewardPoints)
            {
                return Math.Min(amount, purchasedPointsAmount);
            }
            else
                return amount;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Load reward point history records
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records (filter by current store if possible)</param>
        /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history records</returns>
        public virtual IPagedList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, bool showHidden = false,
            bool showNotActivated = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _rphRepository.Table;
            if (customerId > 0)
                query = query.Where(rph => rph.CustomerId == customerId);
            if (!showHidden && !_rewardPointsSettings.PointsAccumulatedForAllStores)
            {
                //filter by store
                var currentStoreId = _storeContext.CurrentStore.Id;
                query = query.Where(rph => rph.StoreId == currentStoreId);
            }
            if (!showNotActivated)
            {
                //show only the points that already activated
                query = query.Where(rph => rph.CreatedOnUtc < DateTime.UtcNow);
            }

            //update points balance
            UpdateRewardPointsBalance(query);

            query = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id);

            var records = new PagedList<RewardPointsHistory>(query, pageIndex, pageSize);
            return records;
        }

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="rewardPoints">Reward points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">The order for which points were redeemed (spent) as a payment</param>
        /// <param name="hasUsedAmount">Flag that indicates if passed amounts in points are used amounts</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <param name="orderItem">OrderItem used to purchase reward points</param>
        /// <returns>Reward points history entry identifier</returns>
        public virtual int AddRewardPointsHistoryEntry(Customer customer, RewardPoints rewardPoints, int storeId, string message = "",
            Order usedWithOrder = null, bool hasUsedAmount = false, DateTime? activatingDate = null, OrderItem orderItem = null)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (storeId <= 0)
                throw new ArgumentException("Store ID should be valid");

            RewardPoints rewardPointsBalance = new RewardPoints(this);

            if (!activatingDate.HasValue)
                rewardPointsBalance = GetRewardPointsBalance(customer.Id, storeId);

            var rph = new RewardPointsHistory
            {
                Customer = customer,
                StoreId = storeId,
                UsedWithOrder = usedWithOrder,
                Points = rewardPoints.Points,
                PointsPurchased = rewardPoints.PointsPurchased,
                PointsBalance = activatingDate.HasValue ? null : (int?)(rewardPointsBalance.Points + rewardPoints.Points),
                PointsBalancePurchased = activatingDate.HasValue ? null : (int?)(rewardPointsBalance.PointsPurchased + rewardPoints.PointsPurchased),
                //revert sign of used amounts as rph entry has opposite sign
                UsedAmount = hasUsedAmount ? -rewardPoints.Amount : 0M,
                UsedAmountPurchased = hasUsedAmount ? -rewardPoints.AmountPurchased : 0M,
                Message = message,
                CreatedOnUtc = activatingDate ?? DateTime.UtcNow,
                PurchasedWithOrderItem = orderItem
            };

            _rphRepository.Insert(rph);

            //event notification
            _eventPublisher.EntityInserted(rph);

            return rph.Id;
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass </param>
        /// <returns>RewardPoints class</returns>
        public virtual RewardPoints GetRewardPointsBalance(int customerId, int storeId)
        {
            var result = new RewardPoints(this);

            var query = _rphRepository.Table;
            if (customerId > 0)
                query = query.Where(rph => rph.CustomerId == customerId);
            if (!_rewardPointsSettings.PointsAccumulatedForAllStores)
                query = query.Where(rph => rph.StoreId == storeId);

            //show only the points that already activated
            query = query.Where(rph => rph.CreatedOnUtc < DateTime.UtcNow);

            //first update points balance
            UpdateRewardPointsBalance(query);

            query = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id);

            var lastRph = query.FirstOrDefault();
            var totBalance = lastRph != null && lastRph.PointsBalance.HasValue ? lastRph.PointsBalance.Value : 0;
            var totBalancePurchased = lastRph != null && lastRph.PointsBalancePurchased.HasValue ? lastRph.PointsBalancePurchased.Value : 0;
            result.Points = totBalance;
            result.PointsPurchased = totBalancePurchased;
            return result;
        }


        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        public virtual RewardPointsHistory GetRewardPointsHistoryEntryById(int rewardPointsHistoryId)
        {
            if (rewardPointsHistoryId == 0)
                return null;

            return _rphRepository.GetById(rewardPointsHistoryId);
        }

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException("rewardPointsHistory");

            _rphRepository.Delete(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityDeleted(rewardPointsHistory);
        }

        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException("rewardPointsHistory");

            _rphRepository.Update(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityUpdated(rewardPointsHistory);
        }

        #endregion
    }
}