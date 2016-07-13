using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Newsletter subscription service
    /// </summary>
    public class NewsLetterSubscriptionService : INewsLetterSubscriptionService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IDbContext _context;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public NewsLetterSubscriptionService(IDbContext context,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IRepository<Customer> customerRepository,
            IEventPublisher eventPublisher,
            ICustomerService customerService)
        {
            this._context = context;
            this._subscriptionRepository = subscriptionRepository;
            this._customerRepository = customerRepository;
            this._eventPublisher = eventPublisher;
            this._customerService = customerService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Publishes the subscription event.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="isSubscribe">if set to <c>true</c> [is subscribe].</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        private void PublishSubscriptionEvent(string email, bool isSubscribe, bool publishSubscriptionEvents)
        {
            if (publishSubscriptionEvents)
            {
                if (isSubscribe)
                {
                    _eventPublisher.PublishNewsletterSubscribe(email);
                }
                else
                {
                    _eventPublisher.PublishNewsletterUnsubscribe(email);
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null)
            {
                throw new ArgumentNullException("newsLetterSubscription");
            }

            //Handle e-mail
            newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

            //Persist
            _subscriptionRepository.Insert(newsLetterSubscription);

            //Publish the subscription event 
            if (newsLetterSubscription.Active)
            {
                PublishSubscriptionEvent(newsLetterSubscription.Email, true, publishSubscriptionEvents);
            }

            //Publish event
            _eventPublisher.EntityInserted(newsLetterSubscription);
        }

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null)
            {
                throw new ArgumentNullException("newsLetterSubscription");
            }

            //Handle e-mail
            newsLetterSubscription.Email = CommonHelper.EnsureSubscriberEmailOrThrow(newsLetterSubscription.Email);

            //Get original subscription record
            var originalSubscription = _context.LoadOriginalCopy(newsLetterSubscription);

            //Persist
            _subscriptionRepository.Update(newsLetterSubscription);

            //Publish the subscription event 
            if ((originalSubscription.Active == false && newsLetterSubscription.Active) ||
                (newsLetterSubscription.Active && (originalSubscription.Email != newsLetterSubscription.Email)))
            {
                //If the previous entry was false, but this one is true, publish a subscribe.
                PublishSubscriptionEvent(newsLetterSubscription.Email, true, publishSubscriptionEvents);
            }
            
            if ((originalSubscription.Active && newsLetterSubscription.Active) && 
                (originalSubscription.Email != newsLetterSubscription.Email))
            {
                //If the two emails are different publish an unsubscribe.
                PublishSubscriptionEvent(originalSubscription.Email, false, publishSubscriptionEvents);
            }

            if ((originalSubscription.Active && !newsLetterSubscription.Active))
            {
                //If the previous entry was true, but this one is false
                PublishSubscriptionEvent(originalSubscription.Email, false, publishSubscriptionEvents);
            }

            //Publish event
            _eventPublisher.EntityUpdated(newsLetterSubscription);
        }

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        public virtual void DeleteNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true)
        {
            if (newsLetterSubscription == null) throw new ArgumentNullException("newsLetterSubscription");

            _subscriptionRepository.Delete(newsLetterSubscription);

            //Publish the unsubscribe event 
            PublishSubscriptionEvent(newsLetterSubscription.Email, false, publishSubscriptionEvents);

            //event notification
            _eventPublisher.EntityDeleted(newsLetterSubscription);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionId)
        {
            if (newsLetterSubscriptionId == 0) return null;

            return _subscriptionRepository.GetById(newsLetterSubscriptionId);
        }

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionGuid)
        {
            if (newsLetterSubscriptionGuid == Guid.Empty) return null;

            var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
                                          where nls.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
                                          orderby nls.Id
                                          select nls;

            return newsLetterSubscriptions.FirstOrDefault();
        }

        /// <summary>
        /// Gets a newsletter subscription by email and store ID
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>NewsLetter subscription</returns>
        public virtual NewsLetterSubscription GetNewsLetterSubscriptionByEmailAndStoreId(string email, int storeId)
        {
            if (!CommonHelper.IsValidEmail(email)) 
                return null;

            email = email.Trim();

            var newsLetterSubscriptions = from nls in _subscriptionRepository.Table
                                          where nls.Email == email && nls.StoreId == storeId
                                          orderby nls.Id
                                          select nls;

            return newsLetterSubscriptions.FirstOrDefault();
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
        /// <returns>NewsLetterSubscription entities</returns>
        public virtual IPagedList<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int storeId = 0, bool? isActive = null, int customerRoleId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (customerRoleId == 0)
            {
                //do not filter by customer role
                var query = _subscriptionRepository.Table;
                if (!String.IsNullOrEmpty(email))
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

                var subscriptions = new PagedList<NewsLetterSubscription>(query, pageIndex, pageSize);
                return subscriptions;
            }
            else
            {
                //filter by customer role
                var guestRole = _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
                if (guestRole == null)
                    throw new NopException("'Guests' role could not be loaded");

                if (guestRole.Id == customerRoleId)
                {
                    //guests
                    var query = _subscriptionRepository.Table;
                    if (!String.IsNullOrEmpty(email))
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
                    
                    var subscriptions = new PagedList<NewsLetterSubscription>(query, pageIndex, pageSize);
                    return subscriptions;
                }
                else
                {
                    //other customer roles (not guests)
                    var query = _subscriptionRepository.Table.Join(_customerRepository.Table,
                        nls => nls.Email,
                        c => c.Email,
                        (nls, c) => new
                        {
                            NewsletterSubscribers = nls,
                            Customer = c
                        });
                    query = query.Where(x => x.Customer.CustomerRoles.Any(cr => cr.Id == customerRoleId));
                    if (!String.IsNullOrEmpty(email))
                        query = query.Where(x => x.NewsletterSubscribers.Email.Contains(email));
                    if (createdFromUtc.HasValue)
                        query = query.Where(x => x.NewsletterSubscribers.CreatedOnUtc >= createdFromUtc.Value);
                    if (createdToUtc.HasValue)
                        query = query.Where(x => x.NewsletterSubscribers.CreatedOnUtc <= createdToUtc.Value);
                    if (storeId > 0)
                        query = query.Where(x => x.NewsletterSubscribers.StoreId == storeId);
                    if (isActive.HasValue)
                        query = query.Where(x => x.NewsletterSubscribers.Active == isActive.Value);
                    query = query.OrderBy(x => x.NewsletterSubscribers.Email);

                    var subscriptions = new PagedList<NewsLetterSubscription>(query.Select(x=>x.NewsletterSubscribers), pageIndex, pageSize);
                    return subscriptions;
                }
            }
        }

        #endregion
    }
}