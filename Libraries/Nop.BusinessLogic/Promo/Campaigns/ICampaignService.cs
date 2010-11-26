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

using System.Collections.Generic;
using NopSolutions.NopCommerce.BusinessLogic.Messages;

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns
{
    /// <summary>
    /// Message campaign service interface
    /// </summary>
    public partial interface ICampaignService
    {
        /// <summary>
        /// Gets a campaign by campaign identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>Message template</returns>
        Campaign GetCampaignById(int campaignId);

        /// <summary>
        /// Deletes a campaign
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        void DeleteCampaign(int campaignId);

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <returns>Campaign collection</returns>
        List<Campaign> GetAllCampaigns();

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        void InsertCampaign(Campaign campaign);

        /// <summary>
        /// Updates the campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        void UpdateCampaign(Campaign campaign);

        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Total emails sent</returns>
        int SendCampaign(int campaignId,
            List<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <param name="email">Email</param>
        void SendCampaign(int campaignId, string email);
    }
}
