using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Reward point service
    /// </summary>
    public partial class RewardPointService : IRewardPointService
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<RewardPointsHistory> _rewardPointsHistoryRepository;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public RewardPointService(IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<RewardPointsHistory> rewardPointsHistoryRepository,
            RewardPointsSettings rewardPointsSettings)
        {
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _rewardPointsHistoryRepository = rewardPointsHistoryRepository;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get query to load reward points history
        /// </summary>
        /// <param name="customerId">Customer identifier; pass 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <param name="showNotActivated">Whether to load reward points that did not yet activated</param>
        /// <returns>Query to load reward points history</returns>
        protected virtual IQueryable<RewardPointsHistory> GetRewardPointsQuery(int customerId, int? storeId, bool showNotActivated = false)
        {
            var query = _rewardPointsHistoryRepository.Table;

            //filter by customer
            if (customerId > 0)
                query = query.Where(historyEntry => historyEntry.CustomerId == customerId);

            //filter by store
            if (!_rewardPointsSettings.PointsAccumulatedForAllStores && storeId > 0)
                query = query.Where(historyEntry => historyEntry.StoreId == storeId);

            //whether to show only the points that already activated
            if (!showNotActivated)
            {
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);
            }

            //update points balance
            UpdateRewardPointsBalance(query);

            return query;
        }

        /// <summary>
        /// Update reward points balance if necessary
        /// </summary>
        /// <param name="query">Input query</param>
        protected virtual void UpdateRewardPointsBalance(IQueryable<RewardPointsHistory> query)
        {
            //get expired points
            var nowUtc = DateTime.UtcNow;
            var expiredPoints = query
                .Where(historyEntry => historyEntry.EndDateUtc < nowUtc && historyEntry.ValidPoints > 0)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();

            //reduce the balance for these points
            foreach (var historyEntry in expiredPoints)
            {
                InsertRewardPointsHistoryEntry(new RewardPointsHistory
                {
                    CustomerId = historyEntry.CustomerId,
                    StoreId = historyEntry.StoreId,
                    Points = -historyEntry.ValidPoints.Value,
                    Message = string.Format(_localizationService.GetResource("RewardPoints.Expired"),
                        _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc)),
                    CreatedOnUtc = historyEntry.EndDateUtc.Value
                });

                historyEntry.ValidPoints = 0;
                UpdateRewardPointsHistoryEntry(historyEntry);
            }

            //get has not yet activated points, but it's time to do it
            var notActivatedPoints = query
                .Where(historyEntry => !historyEntry.PointsBalance.HasValue && historyEntry.CreatedOnUtc < nowUtc)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();
            if (!notActivatedPoints.Any())
                return;

            //get current points balance
            //LINQ to entities does not support Last method, thus order by desc and use First one
            var currentPointsBalance = query
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id)
                .FirstOrDefault(historyEntry => historyEntry.PointsBalance.HasValue)
                ?.PointsBalance ?? 0;

            //update appropriate records
            foreach (var historyEntry in notActivatedPoints)
            {
                currentPointsBalance += historyEntry.Points;
                historyEntry.PointsBalance = currentPointsBalance;
                UpdateRewardPointsHistoryEntry(historyEntry);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load reward point history records
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history records</returns>
        public virtual IPagedList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, int? storeId = null,
            bool showNotActivated = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = GetRewardPointsQuery(customerId, storeId, showNotActivated)
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

            //return paged reward points history
            return new PagedList<RewardPointsHistory>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Balance</returns>
        public virtual int GetRewardPointsBalance(int customerId, int storeId)
        {
            var query = GetRewardPointsQuery(customerId, storeId)
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

            //return point balance of the first actual history entry
            return query.FirstOrDefault()?.PointsBalance ?? 0;
        }

        /// <summary>
        /// Gets reduced reward points balance per order
        /// </summary>
        /// <param name="rewardPointsBalance">Reward points balance</param>
        /// <returns>Reduced balance</returns>
        public int GetReducedPointsBalance(int rewardPointsBalance)
        {
            if (_rewardPointsSettings.MaximumRewardPointsToUsePerOrder > 0 &&
                rewardPointsBalance > _rewardPointsSettings.MaximumRewardPointsToUsePerOrder)
                return _rewardPointsSettings.MaximumRewardPointsToUsePerOrder;

            return rewardPointsBalance;
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
        /// <param name="endDate">Date and time when the reward points will no longer be valid; pass null to add date termless points</param>
        /// <returns>Reward points history entry identifier</returns>
        public virtual int AddRewardPointsHistoryEntry(Customer customer, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null, DateTime? endDate = null)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (storeId == 0)
                throw new ArgumentException("Store ID should be valid");

            if (points < 0 && endDate.HasValue)
                throw new ArgumentException("End date is available only for positive points amount");

            //insert new history entry
            var newHistoryEntry = new RewardPointsHistory
            {
                CustomerId = customer.Id,
                StoreId = storeId,
                Points = points,
                PointsBalance = activatingDate.HasValue ? null : (int?)(GetRewardPointsBalance(customer.Id, storeId) + points),
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = activatingDate ?? DateTime.UtcNow,
                EndDateUtc = endDate,
                ValidPoints = points > 0 ? (int?)points : null
            };
            InsertRewardPointsHistoryEntry(newHistoryEntry);

            //reduce valid points of previous entries
            if (points >= 0) 
                return newHistoryEntry.Id;

            var withValidPoints = GetRewardPointsQuery(customer.Id, storeId)
                .Where(historyEntry => historyEntry.ValidPoints > 0)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();
            foreach (var historyEntry in withValidPoints)
            {
                points += historyEntry.ValidPoints.Value;
                historyEntry.ValidPoints = Math.Max(points, 0);
                UpdateRewardPointsHistoryEntry(historyEntry);

                if (points >= 0)
                    break;
            }

            return newHistoryEntry.Id;
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

            return _rewardPointsHistoryRepository.GetById(rewardPointsHistoryId);
        }

        /// <summary>
        /// Insert the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void InsertRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException(nameof(rewardPointsHistory));

            _rewardPointsHistoryRepository.Insert(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityInserted(rewardPointsHistory);
        }

        /// <summary>
        /// Update the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException(nameof(rewardPointsHistory));

            _rewardPointsHistoryRepository.Update(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityUpdated(rewardPointsHistory);
        }

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        public virtual void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory)
        {
            if (rewardPointsHistory == null)
                throw new ArgumentNullException(nameof(rewardPointsHistory));

            _rewardPointsHistoryRepository.Delete(rewardPointsHistory);

            //event notification
            _eventPublisher.EntityDeleted(rewardPointsHistory);
        }

        #endregion
    }
}