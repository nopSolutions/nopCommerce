//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

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