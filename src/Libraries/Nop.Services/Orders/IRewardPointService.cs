using System;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Reward point service interface
    /// </summary>
    public partial interface IRewardPointService
    {
        /// <summary>
        /// Load reward point history records
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
        /// <param name="orderGuid">Order Guid; pass null to load all record</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history records</returns>
        IPagedList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, int? storeId = null,
            bool showNotActivated = false, Guid? orderGuid = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets reduced reward points balance per order
        /// </summary>
        /// <param name="rewardPointsBalance">Reward points balance</param>
        /// <returns>Reduced balance</returns>
        int GetReducedPointsBalance(int rewardPointsBalance);

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Balance</returns>
        int GetRewardPointsBalance(int customerId, int storeId);

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">the order for which points were redeemed as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <param name="endDate">Date and time when the reward points will no longer be valid; pass null to add date termless points</param>
        /// <returns>Reward points history entry identifier</returns>
        int AddRewardPointsHistoryEntry(Customer customer, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        RewardPointsHistory GetRewardPointsHistoryEntryById(int rewardPointsHistoryId);

        /// <summary>
        /// Insert the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void InsertRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);
        
        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);
    }
}