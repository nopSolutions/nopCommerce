using System.Collections.Generic;
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
        /// <param name="showHidden">A value indicating whether to show hidden records (filter by current store if possible)</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history records</returns>
        IPagedList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, bool showHidden = false, 
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">the order for which points were redeemed as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        void AddRewardPointsHistoryEntry(Customer customer,
            int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M);

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass </param>
        /// <returns>Balance</returns>
        int GetRewardPointsBalance(int customerId, int storeId);

        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);
    }
}
