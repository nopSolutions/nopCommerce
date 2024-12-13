using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Services.Orders
{

    public partial interface IRewardPointService
    {
        Task<DateTime?> GetSubscriptionExpiryDateAsync(int customerId, int? storeId, bool showNotActivated = false);
        Task<DateTime?> GetSubscriptionStartDateAsync(int customerId, int? storeId, bool showNotActivated = false);
        Task<int> GetSubscriptionAlottedCreditCountAsync(int customerId, int? storeId, bool showNotActivated = false);
        Task<int> GetSubscriptionUsedCreditCountAsync(int customerId, int? storeId, bool showNotActivated = false);

        Task<RewardPointsHistory> GetCustomerSubscriptionAsync(int customerId, int? storeId, bool showNotActivated = false);
        Task<List<RewardPointsHistory>> GetRewardPointsHistoryAsync(int customerId, int targetCustomerId);
    }

    public partial class RewardPointService
    {
        public virtual async Task<DateTime?> GetSubscriptionExpiryDateAsync(int customerId, int? storeId, bool showNotActivated = false)
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
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            query = query.Where(entry => entry.EndDateUtc.HasValue).OrderByDescending(entry => entry.CreatedOnUtc);

            //get reward points created with orders
            query = query.Where(entry => entry.UsedWithOrder.HasValue);

            return (await query.FirstOrDefaultAsync())?.EndDateUtc;
        }

        public virtual async Task<DateTime?> GetSubscriptionStartDateAsync(int customerId, int? storeId, bool showNotActivated = false)
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
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            query = query.Where(entry => entry.EndDateUtc.HasValue).OrderByDescending(entry => entry.EndDateUtc);

            return (await query.FirstOrDefaultAsync())?.CreatedOnUtc;
        }

        public virtual async Task<int> GetSubscriptionAlottedCreditCountAsync(int customerId, int? storeId, bool showNotActivated = false)
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
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            query = query.Where(entry => entry.EndDateUtc.HasValue).OrderByDescending(entry => entry.CreatedOnUtc);

            return (await query.FirstOrDefaultAsync()) != null ? (await query.FirstOrDefaultAsync()).Points : 0;
        }

        public virtual async Task<int> GetSubscriptionUsedCreditCountAsync(int customerId, int? storeId, bool showNotActivated = false)
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
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            //update points balance
            await UpdateRewardPointsBalanceAsync(query);

            query = query.Where(entry => entry.Points < 0 && entry.Message.Contains("Viewed"));

            return query.Sum(x => -(x.Points));

            //return Task.FromResult(query.Sum(x => -(x.Points)));
        }

        public virtual async Task<RewardPointsHistory> GetCustomerSubscriptionAsync(int customerId, int? storeId, bool showNotActivated = false)
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
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            //get reward points associated with order
            query = query.Where(entry => entry.EndDateUtc.HasValue & entry.UsedWithOrder.HasValue).OrderByDescending(entry => entry.CreatedOnUtc);

            //get reward points created with orders
            query = query.Where(entry => entry.UsedWithOrder.HasValue);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<List<RewardPointsHistory>> GetRewardPointsHistoryAsync(int customerId, int targetCustomerId)
        {
            var query = _rewardPointsHistoryRepository.Table;

            //filter by customer
            if (customerId > 0)
                query = query.Where(historyEntry => historyEntry.CustomerId == customerId);

            //query = query.Where(entry => entry.EndDateUtc.HasValue).OrderByDescending(entry => entry.CreatedOnUtc);

            //verify target customer list
            query = query.Where(entry => entry.Message.Contains("Viewed") & entry.Message.Contains(targetCustomerId.ToString()));

            return await query.ToListAsync();
        }

    }
}