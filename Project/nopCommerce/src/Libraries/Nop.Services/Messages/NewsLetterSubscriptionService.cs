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
            await _eventPublisher.PublishNewsLetterSubscribeAsync(subscription);
        else
            await _eventPublisher.PublishNewsLetterUnsubscribeAsync(subscription);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Inserts a newsletter subscription
    /// </summary>
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscription);

        //Handle email
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
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
    /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
    {
        ArgumentNullException.ThrowIfNull(newsLetterSubscription);

        //Handle email
        newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

        //Get original subscription record
        var originalSubscription = await _subscriptionRepository.LoadOriginalCopyAsync(newsLetterSubscription);

        //Persist
        await _subscriptionRepository.UpdateAsync(newsLetterSubscription);

        //Publish the subscription event 
        if ((!originalSubscription.Active && newsLetterSubscription.Active) ||
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
    /// <param name="newsLetterSubscription">Newsletter subscription</param>
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
    /// The task result contains the newsletter subscription
    /// </returns>
    public virtual async Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId)
    {
        return await _subscriptionRepository.GetByIdAsync(newsLetterSubscriptionId, cache => default, useShortTermCache: true);
    }

    /// <summary>
    /// Gets the newsletter subscription list by newsletter subscription GUID
    /// </summary>
    /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the newsletter subscription list
    /// </returns>
    public virtual async Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByGuidAsync(Guid newsLetterSubscriptionGuid)
    {
        if (newsLetterSubscriptionGuid == Guid.Empty)
            return new List<NewsLetterSubscription>();

        var newsLetterSubscriptions =
            from nls in _subscriptionRepository.Table
            where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
            orderby nls.Id
            select nls;

        return await newsLetterSubscriptions.ToListAsync();
    }

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
    public virtual async Task<IList<NewsLetterSubscription>> GetNewsLetterSubscriptionsByEmailAsync(string email,
        int storeId = 0, int subscriptionTypeId = 0, bool? isActive = null)
    {
        email = email?.Trim();
        if (!CommonHelper.IsValidEmail(email))
            return new List<NewsLetterSubscription>();

        return await _subscriptionRepository.GetAllAsync(query =>
        {
            if (!string.IsNullOrEmpty(email))
                query = query.Where(subscription => subscription.Email == email);

            if (storeId > 0)
                query = query.Where(subscription => subscription.StoreId == storeId);

            if (subscriptionTypeId > 0)
                query = query.Where(subscription => subscription.TypeId == subscriptionTypeId);

            if (isActive.HasValue)
                query = query.Where(subscription => subscription.Active == isActive.Value);

            query = query
                .OrderBy(subscription => subscription.Email)
                .ThenBy(subscription => subscription.StoreId)
                .ThenBy(subscription => subscription.TypeId)
                .ThenBy(subscription => subscription.Id);

            return query;
        });
    }

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
    public virtual async Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
        DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        int storeId = 0, bool? isActive = null, int customerRoleId = 0, int subscriptionTypeId = 0,
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
                if (subscriptionTypeId > 0)
                    query = query.Where(nls => nls.TypeId == subscriptionTypeId);
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
                if (subscriptionTypeId > 0)
                    query = query.Where(nls => nls.TypeId == subscriptionTypeId);
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
                    (nls, c) => new { NewsLetterSubscription = nls, Customer = c });

                joindQuery = joindQuery.Where(x => _customerCustomerRoleMappingRepository.Table.Any(ccrm =>
                    ccrm.CustomerId == x.Customer.Id && ccrm.CustomerRoleId == customerRoleId));

                if (!string.IsNullOrEmpty(email))
                    joindQuery = joindQuery.Where(x => x.NewsLetterSubscription.Email.Contains(email));
                if (createdFromUtc.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsLetterSubscription.CreatedOnUtc >= createdFromUtc.Value);
                if (createdToUtc.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsLetterSubscription.CreatedOnUtc <= createdToUtc.Value);
                if (storeId > 0)
                    joindQuery = joindQuery.Where(x => x.NewsLetterSubscription.StoreId == storeId);
                if (subscriptionTypeId > 0)
                    query = query.Where(nls => nls.TypeId == subscriptionTypeId);
                if (isActive.HasValue)
                    joindQuery = joindQuery.Where(x => x.NewsLetterSubscription.Active == isActive.Value);

                joindQuery = joindQuery.OrderBy(x => x.NewsLetterSubscription.Email);

                return joindQuery.Select(x => x.NewsLetterSubscription);
            }, pageIndex, pageSize);

            return subscriptions;
        }
    }

    #endregion
}