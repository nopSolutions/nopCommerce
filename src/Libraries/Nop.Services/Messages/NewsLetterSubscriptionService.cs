using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Customers;

namespace Nop.Services.Messages;

/// <summary>
/// Newsletter subscription service
/// </summary>
public partial class NewsLetterSubscriptionService : INewsLetterSubscriptionService
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
    protected readonly IRepository<NewsLetterSubscription> _subscriptionRepository;

    #endregion

    #region Ctor

    public NewsLetterSubscriptionService(ICustomerService customerService,
        IEventPublisher eventPublisher,
        IRepository<Customer> customerRepository,
        IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
        IRepository<NewsLetterSubscription> subscriptionRepository)
    {
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _customerRepository = customerRepository;
        _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Publishes the subscription event.
    /// </summary>
    /// <param name="subscription">The newsletter subscription.</param>
    /// <param name="isSubscribe">if set to <c>true</c> [is subscribe].</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PublishSubscriptionEventAsync(NewsLetterSubscription subscription, bool isSubscribe, bool publishSubscriptionEvents)
    {
        if (!publishSubscriptionEvents)
            return;

        if (isSubscribe)
        {
            await _eventPublisher.PublishNewsletterSubscribeAsync(subscription);
        }
        else
        {
            await _eventPublisher.PublishNewsletterUnsubscribeAsync(subscription);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscription);

        //Handle e-mail
        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        //Persist
        await _subscriptionRepository.InsertAsync(newsLetterSubscription);

        //Publish the subscription event 
        if (newsLetterSubscription.Active)
            await PublishSubscriptionEventAsync(newsLetterSubscription, true, publishSubscriptionEvents);
    }

    /// <summary>
    /// Updates a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscription);

        //Handle e-mail
        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        //Get original subscription record
        var originalSubscription = await _subscriptionRepository.LoadOriginalCopyAsync(newsLetterSubscription);

        //Persist
        await _subscriptionRepository.UpdateAsync(newsLetterSubscription);

        //Publish the subscription event 
        if ((originalSubscription.Active == false && newsLetterSubscription.Active) ||
            (newsLetterSubscription.Active && originalSubscription.Email != newsLetterSubscription.Email))
        {
            //If the previous entry was false, but this one is true, publish a subscribe.
            await PublishSubscriptionEventAsync(newsLetterSubscription, true, publishSubscriptionEvents);
        }

        if (originalSubscription.Active && newsLetterSubscription.Active &&
            originalSubscription.Email != newsLetterSubscription.Email)
        {
            //If the two emails are different publish an unsubscribe.
            await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
        }

        if (originalSubscription.Active && !newsLetterSubscription.Active)
            //If the previous entry was true, but this one is false
            await PublishSubscriptionEventAsync(originalSubscription, false, publishSubscriptionEvents);
    }

    /// <summary>
    /// Deletes a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">NewsLetter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscription);

        await _subscriptionRepository.DeleteAsync(newsLetterSubscription);

        //Publish the unsubscribe event 
        await PublishSubscriptionEventAsync(newsLetterSubscription, false, publishSubscriptionEvents);
    }

    /// <summary>
    /// Gets a newsletter subscription by newsletter subscription identifier
    /// </summary>
    /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription
    /// </returns>
    public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId)
    {
        return await _subscriptionRepository.GetByIdAsync(newsLetterSubscriptionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets a newsletter subscription by newsletter subscription GUID
    /// </summary>
    /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription
    /// </returns>
    public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByGuidAsync(Guid newsLetterSubscriptionGuid)
    {
        if (newsLetterSubscriptionGuid == Guid.Empty)
            return null;

        var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
            where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
            orderby nls.Id
            select nls;

        return await newsLetterSubscriptions.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a newsletter subscription by email and store ID
    /// </summary>
    /// <param name="email">The newsletter subscription email</param>
    /// <param name="storeId">Store identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetter subscription
    /// </returns>
    public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId)
    {
        if (!CommonHelper.IsValidEmail(email))
            return null;

        email = email.Trim();

        var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
            where nls.Email == email && nls.StoreId == storeId
            orderby nls.Id
            select nls;

        return await newsLetterSubscriptions.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets the newsletter subscription list
    /// </summary>
    /// <param name="email">Email to search or string. Empty to load all records.</param>
    /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
    /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
    /// <param name="storeId">Store identifier. 0 to load all records.</param>
    /// <param name="customerRoleId">Customer role identifier. Used to filter subscribers by customer role. 0 to load all records.</param>
    /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsLetterSubscription entities
    /// </returns>
    public virtual async Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int storeId = 0, bool? isActive = null, int customerRoleId = 0,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        if (customerRoleId == 0)
        {
            //do not filter by customer role
            var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(nls => nls.Email.Contains(email));
                if (createdFromUtc.HasValue)
                    query = query.Where(nls => nls.CreatedOnUtc >= createdFromUtc.Value);
                if (createdToUtc.HasValue)
                    query = query.Where(nls => nls.CreatedOnUtc <= createdToUtc.Value);
                if (storeId > 0)
                    query = query.Where(nls => nls.StoreId == storeId);
                if (isActive.HasValue)
                    query = query.Where(nls => nls.Active == isActive.Value);
                query = query.OrderBy(nls => nls.Email);

                return query;
            }, pageIndex, pageSize);

            return subscriptions;
        }

        //filter by customer role
        var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName)
                        ?? throw new NopException("'Guests' role could not be loaded");

        if (guestRole.Id == customerRoleId)
        {
            //guests
            var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(nls => nls.Email.Contains(email));
                if (createdFromUtc.HasValue)
                    query = query.Where(nls => nls.CreatedOnUtc >= createdFromUtc.Value);
                if (createdToUtc.HasValue)
                    query = query.Where(nls => nls.CreatedOnUtc <= createdToUtc.Value);
                if (storeId > 0)
                    query = query.Where(nls => nls.StoreId == storeId);
                if (isActive.HasValue)
                    query = query.Where(nls => nls.Active == isActive.Value);
                query = query.Where(nls => !_customerRepository.Table.Any(c => c.Email == nls.Email));
                query = query.OrderBy(nls => nls.Email);

                return query;
            }, pageIndex, pageSize);

            return subscriptions;
        }
        else
        {
            var subscriptions = await _subscriptionRepository.GetAllPagedAsync(query =>
            {
                //other customer roles (not guests)
                var joindQuery = query.Join(_customerRepository.Table,
                    nls => nls.Email,
                    c => c.Email,
                    (nls, c) => new { NewsletterSubscribers = nls, Customer = c });

                joindQuery = joindQuery.Where(x => _customerCustomerRoleMappingRepository.Table.Any(ccrm =>
                    ccrm.CustomerId == x.Customer.Id && ccrm.CustomerRoleId == customerRoleId));

                if (!string.IsNullOrEmpty(email))
                    joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.Email.Contains(email));
                if (createdFromUtc.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.CreatedOnUtc >= createdFromUtc.Value);
                if (createdToUtc.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.CreatedOnUtc <= createdToUtc.Value);
                if (storeId > 0)
                    joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.StoreId == storeId);
                if (isActive.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsletterSubscribers.Active == isActive.Value);

                joindQuery = joindQuery.OrderBy(x => x.NewsletterSubscribers.Email);

                return joindQuery.Select(x => x.NewsletterSubscribers);
            }, pageIndex, pageSize);

            return subscriptions;
        }
    }

    #endregion
}