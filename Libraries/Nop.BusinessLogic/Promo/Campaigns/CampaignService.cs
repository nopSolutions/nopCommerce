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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
 

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Campaigns
{
    /// <summary>
    /// Message campaign service
    /// </summary>
    public partial class CampaignService : ICampaignService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public CampaignService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a campaign by campaign identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>Message template</returns>
        public Campaign GetCampaignById(int campaignId)
        {
            if (campaignId == 0)
                return null;

            
            var query = from c in _context.Campaigns
                        where c.CampaignId == campaignId
                        select c;
            var campaign = query.SingleOrDefault();

            return campaign;
        }

        /// <summary>
        /// Deletes a campaign
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        public void DeleteCampaign(int campaignId)
        {
            var campaign = GetCampaignById(campaignId);
            if (campaign == null)
                return;

            
            if (!_context.IsAttached(campaign))
                _context.Campaigns.Attach(campaign);
            _context.DeleteObject(campaign);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <returns>Campaign collection</returns>
        public List<Campaign> GetAllCampaigns()
        {
            
            var query = from c in _context.Campaigns
                        orderby c.CreatedOn
                        select c;
            var campaigns = query.ToList();

            return campaigns;
        }

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        public void InsertCampaign(Campaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            campaign.Name = CommonHelper.EnsureNotNull(campaign.Name);
            campaign.Name = CommonHelper.EnsureMaximumLength(campaign.Name, 200);
            campaign.Subject = CommonHelper.EnsureNotNull(campaign.Subject);
            campaign.Subject = CommonHelper.EnsureMaximumLength(campaign.Subject, 200);
            campaign.Body = CommonHelper.EnsureNotNull(campaign.Body);

            
            
            _context.Campaigns.AddObject(campaign);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        public void UpdateCampaign(Campaign campaign)
        {
            campaign.Name = CommonHelper.EnsureNotNull(campaign.Name);
            campaign.Name = CommonHelper.EnsureMaximumLength(campaign.Name, 200);
            campaign.Subject = CommonHelper.EnsureNotNull(campaign.Subject);
            campaign.Subject = CommonHelper.EnsureMaximumLength(campaign.Subject, 200);
            campaign.Body = CommonHelper.EnsureNotNull(campaign.Body);

            
            if (!_context.IsAttached(campaign))
                _context.Campaigns.Attach(campaign);

            _context.SaveChanges();
        }

        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>Total emails sent</returns>
        public int SendCampaign(int campaignId,
            List<NewsLetterSubscription> subscriptions)
        {
            int totalEmailsSent = 0;
            var campaign = GetCampaignById(campaignId);

            if(campaign == null)
            {
                throw new NopException("Campaign could not be loaded");
            }


            var emailAccount = IoC.Resolve<IMessageService>().DefaultEmailAccount;

            foreach (var subscription in subscriptions)
            {
                string subject = IoC.Resolve<IMessageService>().ReplaceMessageTemplateTokens(subscription, campaign.Subject);
                string body = IoC.Resolve<IMessageService>().ReplaceMessageTemplateTokens(subscription, campaign.Body);
                var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                var to = new MailAddress(subscription.Email);
                IoC.Resolve<IMessageService>().InsertQueuedEmail(3, from, to, string.Empty, string.Empty,
                    subject, body, DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
                totalEmailsSent++;
            }
            return totalEmailsSent;
        }

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <param name="email">Email</param>
        public void SendCampaign(int campaignId, string email)
        {
            var campaign = GetCampaignById(campaignId);
            if(campaign == null)
            {
                throw new NopException("Campaign could not be loaded");
            }

            string subject = campaign.Subject;
            string body = campaign.Body;

            var emailAccount = IoC.Resolve<IMessageService>().DefaultEmailAccount;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(email);
            IoC.Resolve<IMessageService>().SendEmail(subject, body, from, to, emailAccount);
        }
        #endregion
    }
}
