using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages;

/// <summary>
/// Newsletter subscription service interface
/// </summary>
public partial interface INewsLetterSubscriptionService
{
    /// <summary>
    /// Inserts a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

    /// <summary>
    /// Updates a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

    /// <summary>
    /// Deletes a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

    /// <summary>
    /// Gets a newsletter subscription by newsletter subscription identifier
    /// </summary>
    /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription
    /// </returns>
    Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId);

    /// <summary>
    /// Gets the newsletter subscription list by newsletter subscription GUID
    /// </summary>
    /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription list
    /// </returns>
    Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByGuidAsync(Guid newsLetterSubscriptionGuid);

    /// <summary>
    /// Gets newsletter subscriptions by the passed email (exact match)
    /// </summary>
    /// <param name="email">Email to search</param>
    /// <param name="storeId">Store identifier. Pass 0 to load all records.</param>
    /// <param name="subscriptionTypeId">Subscription type identifier. Pass 0 to load all records.</param>
    /// <param name="isActive">Value indicating whether subscriber record should be active or not; Pass null to load all records</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscriptions
    /// </returns>
    Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByEmailAsync(string email,
        int storeId = 0, int subscriptionTypeId = 0, bool? isActive = null);

    /// <summary>
    /// Gets the paged newsletter subscription list
    /// </summary>
    /// <param name="email">Email to search or string. Empty to load all records.</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="storeId">Store identifier. 0 to load all records.</param>
    /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
    /// <param name="customerRoleId">Customer role identifier. Used to filter subscribers by customer role. 0 to load all records.</param>
    /// <param name="subscriptionTypeId">Subscription type identifier. Used to filter subscribers by subscription type. 0 to load all records.</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription paged list
    /// </returns>
    Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int storeId = 0, bool? isActive = null, int customerRoleId = 0, int subscriptionTypeId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue);
}