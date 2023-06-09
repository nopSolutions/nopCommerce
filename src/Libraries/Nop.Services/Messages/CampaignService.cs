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

        protected readonly ICustomerService _customerService;
        protected readonly IEmailSender _emailSender;
        protected readonly IMessageTokenProvider _messageTokenProvider;
        protected readonly IQueuedEmailService _queuedEmailService;
        protected readonly IRepository<Campaign> _campaignRepository;
        protected readonly IStoreContext _storeContext;
        protected readonly ITokenizer _tokenizer;

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
            _customerService = customerService;
            _emailSender = emailSender;
            _messageTokenProvider = messageTokenProvider;
            _queuedEmailService = queuedEmailService;
            _campaignRepository = campaignRepository;
            _storeContext = storeContext;
            _tokenizer = tokenizer;
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
            await _campaignRepository.InsertAsync(campaign);
        }

        /// <summary>
        /// Updates a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.UpdateAsync(campaign);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.DeleteAsync(campaign);
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
            return await _campaignRepository.GetByIdAsync(campaignId, cache => default);
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
            var campaigns = await _campaignRepository.GetAllAsync(query =>
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
                var customer = await _customerService.GetCustomerByEmailAsync(subscription.Email);
                //ignore deleted or inactive customers when sending newsletter campaigns
                if (customer != null && (!customer.Active || customer.Deleted))
                    continue;

                var tokens = new List<Token>();
                await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount);
                await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(tokens, subscription);
                if (customer != null)
                    await _messageTokenProvider.AddCustomerTokensAsync(tokens, customer);

                var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
                var body = _tokenizer.Replace(campaign.Body, tokens, true);

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
                await _queuedEmailService.InsertQueuedEmailAsync(email);
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
            await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount);
            var customer = await _customerService.GetCustomerByEmailAsync(email);
            if (customer != null)
                await _messageTokenProvider.AddCustomerTokensAsync(tokens, customer);

            var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
            var body = _tokenizer.Replace(campaign.Body, tokens, true);

            await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
        }

        #endregion
    }
}