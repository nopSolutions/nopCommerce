
using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Nop.Core.Domain.Customers
{
    public class RewardPointsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public bool RewardPointsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Reward Points exchange rate
        /// </summary>
        public decimal RewardPointsExchangeRate { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for registration
        /// </summary>
        public int RewardPointsForRegistration { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases (amount in primary store currency)
        /// </summary>
        public decimal RewardPointsForPurchases_Amount { get; set; }

        /// <summary>
        /// Gets or sets a number of points awarded for purchases
        /// </summary>
        public int RewardPointsForPurchases_Points { get; set; }

    }
}