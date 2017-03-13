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
        /// <param name="showHidden">A value indicating whether to show hidden records (filter by current store if possible)</param>
        /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Reward point history records</returns>
        IPagedList<RewardPointsHistory> GetRewardPointsHistory(int customerId = 0, bool showHidden = false,
            bool showNotActivated = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Add reward points history record.
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
        int AddRewardPointsHistoryEntry(Customer customer,
            RewardPoints rewardPoints, int storeId, string message = "",
            Order usedWithOrder = null, bool hasUsedAmount = false, DateTime? activatingDate = null, OrderItem orderItem = null);

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="storeId">Store identifier; pass </param>
        /// <returns>RewardPoints class</returns>
        RewardPoints GetRewardPointsBalance(int customerId, int storeId);

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>Reward point history entry</returns>
        RewardPointsHistory GetRewardPointsHistoryEntryById(int rewardPointsHistoryId);

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void DeleteRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);

        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        void UpdateRewardPointsHistoryEntry(RewardPointsHistory rewardPointsHistory);

        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>Converted value</returns>
        decimal ConvertRewardPointsToAmount(int rewardPoints);

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="overridenExchangeRate">overriden exchange rate</param>
        /// <returns>Converted value</returns>
        int ConvertAmountToRewardPoints(decimal amount, decimal? overridenExchangeRate = null);

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints);

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Base amount (in primary store currency) for points calculation</param>
        /// <returns>Number of reward points</returns>
        int CalculateRewardPoints(Customer customer, decimal amount);

        /// <summary>
        /// Calculate base amount for reward points calculation
        /// </summary>
        /// <param name="amount">Base amount (in primary store currency) for points calculation</param>
        /// <param name="purchasedPointsAmount">Amount of used purchased reward points</param>
        /// <returns>base amount</returns>
        decimal GetRewardPointsBaseAmount(decimal amount, decimal purchasedPointsAmount);
    }
}
