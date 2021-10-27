using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Services.Gdpr
{
    /// <summary>
    /// Represents the GDPR service
    /// </summary>
    public partial class GdprService : IGdprService
    {
        #region Fields

        protected IAddressService AddressService { get; }
        protected IBackInStockSubscriptionService BackInStockSubscriptionService { get; }
        protected IBlogService BlogService { get; }
        protected ICustomerService CustomerService { get; }
        protected IExternalAuthenticationService ExternalAuthenticationService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IForumService ForumService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected INewsService NewsService { get; }
        protected IProductService ProductService { get; }
        protected IRepository<GdprConsent> GdprConsentRepository { get; }
        protected IRepository<GdprLog> GdprLogRepository { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStoreService StoreService { get; }

        #endregion

        #region Ctor

        public GdprService(IAddressService addressService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBlogService blogService,
            ICustomerService customerService,
            IExternalAuthenticationService externalAuthenticationService,
            IEventPublisher eventPublisher,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IProductService productService,
            IRepository<GdprConsent> gdprConsentRepository,
            IRepository<GdprLog> gdprLogRepository,
            IShoppingCartService shoppingCartService,
            IStoreService storeService)
        {
            AddressService = addressService;
            BackInStockSubscriptionService = backInStockSubscriptionService;
            BlogService = blogService;
            CustomerService = customerService;
            ExternalAuthenticationService = externalAuthenticationService;
            EventPublisher = eventPublisher;
            ForumService = forumService;
            GenericAttributeService = genericAttributeService;
            NewsService = newsService;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            ProductService = productService;
            GdprConsentRepository = gdprConsentRepository;
            GdprLogRepository = gdprLogRepository;
            ShoppingCartService = shoppingCartService;
            StoreService = storeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="gdprLog">GDPR log</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertLogAsync(GdprLog gdprLog)
        {
            await GdprLogRepository.InsertAsync(gdprLog);
        }

        #endregion

        #region Methods

        #region GDPR consent

        /// <summary>
        /// Get a GDPR consent
        /// </summary>
        /// <param name="gdprConsentId">The GDPR consent identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent
        /// </returns>
        public virtual async Task<GdprConsent> GetConsentByIdAsync(int gdprConsentId)
        {
            return await GdprConsentRepository.GetByIdAsync(gdprConsentId, cache => default);
        }

        /// <summary>
        /// Get all GDPR consents
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR consent
        /// </returns>
        public virtual async Task<IList<GdprConsent>> GetAllConsentsAsync()
        {
            var gdprConsents = await GdprConsentRepository.GetAllAsync(query =>
            {
                return from c in query
                    orderby c.DisplayOrder, c.Id
                    select c;
            }, cache => default);

            return gdprConsents;
        }

        /// <summary>
        /// Insert a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertConsentAsync(GdprConsent gdprConsent)
        {
            await GdprConsentRepository.InsertAsync(gdprConsent);
        }

        /// <summary>
        /// Update the GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateConsentAsync(GdprConsent gdprConsent)
        {
            await GdprConsentRepository.UpdateAsync(gdprConsent);
        }

        /// <summary>
        /// Delete a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteConsentAsync(GdprConsent gdprConsent)
        {
            await GdprConsentRepository.DeleteAsync(gdprConsent);
        }

        /// <summary>
        /// Gets the latest selected value (a consent is accepted or not by a customer)
        /// </summary>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result; null if previous a customer hasn't been asked
        /// </returns>
        public virtual async Task<bool?> IsConsentAcceptedAsync(int consentId, int customerId)
        {
            //get latest record
            var log = (await GetAllLogAsync(customerId: customerId, consentId: consentId, pageIndex: 0, pageSize: 1)).FirstOrDefault();
            if (log == null)
                return null;

            return log.RequestType switch
            {
                GdprRequestType.ConsentAgree => true,
                GdprRequestType.ConsentDisagree => false,
                _ => null,
            };
        }

        #endregion

        #region GDPR log
        
        /// <summary>
        /// Get all GDPR log records
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="customerInfo">Customer info (Exact match)</param>
        /// <param name="requestType">GDPR request type</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR log records
        /// </returns>
        public virtual async Task<IPagedList<GdprLog>> GetAllLogAsync(int customerId = 0, int consentId = 0,
            string customerInfo = "", GdprRequestType? requestType = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await GdprLogRepository.GetAllPagedAsync(query =>
            {
                if (customerId > 0) 
                    query = query.Where(log => log.CustomerId == customerId);

                if (consentId > 0) 
                    query = query.Where(log => log.ConsentId == consentId);

                if (!string.IsNullOrEmpty(customerInfo)) 
                    query = query.Where(log => log.CustomerInfo == customerInfo);

                if (requestType != null)
                {
                    var requestTypeId = (int)requestType;
                    query = query.Where(log => log.RequestTypeId == requestTypeId);
                }

                query = query.OrderByDescending(log => log.CreatedOnUtc).ThenByDescending(log => log.Id);

                return query;
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="requestType">Request type</param>
        /// <param name="requestDetails">Request details</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertLogAsync(Customer customer, int consentId, GdprRequestType requestType, string requestDetails)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var gdprLog = new GdprLog
            {
                CustomerId = customer.Id,
                ConsentId = consentId,
                CustomerInfo = customer.Email,
                RequestType = requestType,
                RequestDetails = requestDetails,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertLogAsync(gdprLog);
        }
        
        #endregion

        #region Customer

        /// <summary>
        /// Permanent delete of customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PermanentDeleteCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //blog comments
            var blogComments = await BlogService.GetAllCommentsAsync(customerId: customer.Id);
            await BlogService.DeleteBlogCommentsAsync(blogComments);

            //news comments
            var newsComments = await NewsService.GetAllCommentsAsync(customerId: customer.Id);
            await NewsService.DeleteNewsCommentsAsync(newsComments);

            //back in stock subscriptions
            var backInStockSubscriptions = await BackInStockSubscriptionService.GetAllSubscriptionsByCustomerIdAsync(customer.Id);
            foreach (var backInStockSubscription in backInStockSubscriptions)
                await BackInStockSubscriptionService.DeleteSubscriptionAsync(backInStockSubscription);

            //product review
            var productReviews = await ProductService.GetAllProductReviewsAsync(customer.Id);
            var reviewedProducts = await ProductService.GetProductsByIdsAsync(productReviews.Select(p => p.ProductId).Distinct().ToArray());
            await ProductService.DeleteProductReviewsAsync(productReviews);
            //update product totals
            foreach (var product in reviewedProducts) 
                await ProductService.UpdateProductReviewTotalsAsync(product);

            //external authentication record
            foreach (var ear in await ExternalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
                await ExternalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

            //forum subscriptions
            var forumSubscriptions = await ForumService.GetAllSubscriptionsAsync(customer.Id);
            foreach (var forumSubscription in forumSubscriptions)
                await ForumService.DeleteSubscriptionAsync(forumSubscription);

            //shopping cart items
            foreach (var sci in await ShoppingCartService.GetShoppingCartAsync(customer))
                await ShoppingCartService.DeleteShoppingCartItemAsync(sci);

            //private messages (sent)
            foreach (var pm in await ForumService.GetAllPrivateMessagesAsync(0, customer.Id, 0, null, null, null, null))
                await ForumService.DeletePrivateMessageAsync(pm);

            //private messages (received)
            foreach (var pm in await ForumService.GetAllPrivateMessagesAsync(0, 0, customer.Id, null, null, null, null))
                await ForumService.DeletePrivateMessageAsync(pm);

            //newsletter
            var allStores = await StoreService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                var newsletter = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                if (newsletter != null)
                    await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);
            }

            //addresses
            foreach (var address in await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
            {
                await CustomerService.RemoveCustomerAddressAsync(customer, address);
                await CustomerService.UpdateCustomerAsync(customer);
                //now delete the address record
                await AddressService.DeleteAddressAsync(address);
            }

            //generic attributes
            var keyGroup = customer.GetType().Name;
            var genericAttributes = await GenericAttributeService.GetAttributesForEntityAsync(customer.Id, keyGroup);
            await GenericAttributeService.DeleteAttributesAsync(genericAttributes);

            //ignore ActivityLog
            //ignore ForumPost, ForumTopic, ignore ForumPostVote
            //ignore Log
            //ignore PollVotingRecord
            //ignore ProductReviewHelpfulness
            //ignore RecurringPayment 
            //ignore ReturnRequest
            //ignore RewardPointsHistory
            //and we do not delete orders

            //remove from Registered role, add to Guest one
            if (await CustomerService.IsRegisteredAsync(customer))
            {
                var registeredRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                await CustomerService.RemoveCustomerRoleMappingAsync(customer, registeredRole);
            }

            if (!await CustomerService.IsGuestAsync(customer))
            {
                var guestRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
                await CustomerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
            }

            var email = customer.Email;

            //clear other information
            customer.Email = string.Empty;
            customer.EmailToRevalidate = string.Empty;
            customer.Username = string.Empty;
            customer.Active = false;
            customer.Deleted = true;
            
            await CustomerService.UpdateCustomerAsync(customer);

            //raise event
            await EventPublisher.PublishAsync(new CustomerPermanentlyDeleted(customer.Id, email));
        }

        #endregion

        #endregion
    }
}