using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Campaign service
    /// </summary>
    public partial class CampaignService : ICampaignService
    {
        #region Fields

        protected ICustomerService CustomerService { get; }
        protected IEmailSender EmailSender { get; }
        protected IMessageTokenProvider MessageTokenProvider { get; }
        protected IQueuedEmailService QueuedEmailService { get; }
        protected IRepository<Campaign> CampaignRepository { get; }
        protected IStoreContext StoreContext { get; }
        protected ITokenizer Tokenizer { get; }

        #endregion

        #region Ctor

        public CampaignService(ICustomerService customerService,
            IEmailSender emailSender,
            IMessageTokenProvider messageTokenProvider,
            IQueuedEmailService queuedEmailService,
            IRepository<Campaign> campaignRepository,
            IStoreContext storeContext,
            ITokenizer tokenizer)
        {
            CustomerService = customerService;
            EmailSender = emailSender;
            MessageTokenProvider = messageTokenProvider;
            QueuedEmailService = queuedEmailService;
            CampaignRepository = campaignRepository;
            StoreContext = storeContext;
            Tokenizer = tokenizer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>        
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCampaignAsync(Campaign campaign)
        {
            await CampaignRepository.InsertAsync(campaign);
        }

        /// <summary>
        /// Updates a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCampaignAsync(Campaign campaign)
        {
            await CampaignRepository.UpdateAsync(campaign);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCampaignAsync(Campaign campaign)
        {
            await CampaignRepository.DeleteAsync(campaign);
        }

        /// <summary>
        /// Gets a campaign by identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaign
        /// </returns>
        public virtual async Task<Campaign> GetCampaignByIdAsync(int campaignId)
        {
            return await CampaignRepository.GetByIdAsync(campaignId, cache => default);
        }

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the campaigns
        /// </returns>
        public virtual async Task<IList<Campaign>> GetAllCampaignsAsync(int storeId = 0)
        {
            var campaigns = await CampaignRepository.GetAllAsync(query =>
            {
                if (storeId > 0) 
                    query = query.Where(c => c.StoreId == storeId);

                query = query.OrderBy(c => c.CreatedOnUtc);

                return query;
            });

            return campaigns;
        }

        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the otal emails sent
        /// </returns>
        public virtual async Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
            IEnumerable<NewsLetterSubscription> subscriptions)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var totalEmailsSent = 0;

            foreach (var subscription in subscriptions)
            {
                var customer = await CustomerService.GetCustomerByEmailAsync(subscription.Email);
                //ignore deleted or inactive customers when sending newsletter campaigns
                if (customer != null && (!customer.Active || customer.Deleted))
                    continue;

                var tokens = new List<Token>();
                await MessageTokenProvider.AddStoreTokensAsync(tokens, await StoreContext.GetCurrentStoreAsync(), emailAccount);
                await MessageTokenProvider.AddNewsLetterSubscriptionTokensAsync(tokens, subscription);
                if (customer != null)
                    await MessageTokenProvider.AddCustomerTokensAsync(tokens, customer);

                var subject = Tokenizer.Replace(campaign.Subject, tokens, false);
                var body = Tokenizer.Replace(campaign.Body, tokens, true);

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.Low,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = subscription.Email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    DontSendBeforeDateUtc = campaign.DontSendBeforeDateUtc
                };
                await QueuedEmailService.InsertQueuedEmailAsync(email);
                totalEmailsSent++;
            }

            return totalEmailsSent;
        }

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="email">Email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var tokens = new List<Token>();
            await MessageTokenProvider.AddStoreTokensAsync(tokens, await StoreContext.GetCurrentStoreAsync(), emailAccount);
            var customer = await CustomerService.GetCustomerByEmailAsync(email);
            if (customer != null)
                await MessageTokenProvider.AddCustomerTokensAsync(tokens, customer);

            var subject = Tokenizer.Replace(campaign.Subject, tokens, false);
            var body = Tokenizer.Replace(campaign.Body, tokens, true);

            await EmailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
        }

        #endregion
    }
}