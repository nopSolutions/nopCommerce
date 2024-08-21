using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// Campaign service
/// </summary>
public partial interface ICampaignService
{
    /// <summary>
    /// Inserts a campaign
    /// </summary>
    /// <param name="campaign">Campaign</param>        
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertCampaignAsync(Campaign campaign);

    /// <summary>
    /// Updates a campaign
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateCampaignAsync(Campaign campaign);

    /// <summary>
    /// Deleted a queued email
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteCampaignAsync(Campaign campaign);

    /// <summary>
    /// Gets a campaign by identifier
    /// </summary>
    /// <param name="campaignId">Campaign identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaign
    /// </returns>
    Task<Campaign> GetCampaignByIdAsync(int campaignId);

    /// <summary>
    /// Gets all campaigns
    /// </summary>
    /// <param name="storeId">Store identifier; 0 to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the campaigns
    /// </returns>
    Task<IList<Campaign>> GetAllCampaignsAsync(int storeId = 0);

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
    Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
        IEnumerable<NewsLetterSubscription> subscriptions);

    /// <summary>
    /// Sends a campaign to specified email
    /// </summary>
    /// <param name="campaign">Campaign</param>
    /// <param name="emailAccount">Email account</param>
    /// <param name="email">Email</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email, int languageId);
}