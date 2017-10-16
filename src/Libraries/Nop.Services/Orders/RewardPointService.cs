using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;

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

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rphRepository">RewardPointsHistory repository</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="eventPublisher">Event published</param>
        public RewardPointService(IRepository<RewardPointsHistory> rphRepository,
            RewardPointsSettings rewardPointsSettings,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            this._rphRepository = rphRepository;
            this._rewardPointsSettings = rewardPointsSettings;
            this._storeContext = storeContext;
            this._eventPublisher = eventPublisher;
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
            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;
            var notActivatedRph = query.Where(rph => !rph.PointsBalance.HasValue && rph.CreatedOnUtc < nowUtc).ToList();

            //nothing to update
            if (!notActivatedRph.Any())
                return;

            //get current points balance, LINQ to entities does not support Last method, thus order by desc and use First one
            var lastActive = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id).FirstOrDefault(rph => rph.PointsBalance.HasValue);
            var currentPointsBalance = lastActive != null ? lastActive.PointsBalance : 0;

            //update appropriate records
            foreach (var rph in notActivatedRph)
            {
                rph.PointsBalance = currentPointsBalance + rph.Points;
                UpdateRewardPointsHistoryEntry(rph);
                currentPointsBalance = rph.PointsBalance;
            }
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

                //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
                //That's why we pass the date value
                var nowUtc = DateTime.UtcNow;
                query = query.Where(rph => rph.CreatedOnUtc < nowUtc);
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
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">The order for which points were redeemed (spent) as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <returns>Reward points history entry identifier</returns>
        public virtual int AddRewardPointsHistoryEntry(Customer customer, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (storeId <= 0)
                throw new ArgumentException("Store ID should be valid");

            var rph = new RewardPointsHistory
            {
                Customer = customer,
                StoreId = storeId,
                UsedWithOrder = usedWithOrder,
                Points = points,
                PointsBalance = activatingDate.HasValue ? null : (int?)(GetRewardPointsBalance(customer.Id, storeId) + points),
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = activatingDate ?? DateTime.UtcNow
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
        /// <returns>Balance</returns>
        public virtual int GetRewardPointsBalance(int customerId, int storeId)
        {
            var query = _rphRepository.Table;
            if (customerId > 0)
                query = query.Where(rph => rph.CustomerId == customerId);
            if (!_rewardPointsSettings.PointsAccumulatedForAllStores)
                query = query.Where(rph => rph.StoreId == storeId);

            //show only the points that already activated
            //The function 'CurrentUtcDateTime' is not supported by SQL Server Compact. 
            //That's why we pass the date value
            var nowUtc = DateTime.UtcNow;
            query = query.Where(rph => rph.CreatedOnUtc < nowUtc);

            //first update points balance
            UpdateRewardPointsBalance(query);

            query = query.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id);

            var lastRph = query.FirstOrDefault();
            return lastRph != null && lastRph.PointsBalance.HasValue ? lastRph.PointsBalance.Value : 0;
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
                throw new ArgumentNullException(nameof(rewardPointsHistory));

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
                throw new ArgumentNullException(nameof(rewardPointsHistory));

            _rphRepository.Update(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityUpdated(rewardPointsHistory);
        }

        #endregion
    }
}
