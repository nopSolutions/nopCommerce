using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using Nop.Services.Messages;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Back in stock subscription service
    /// </summary>
    public partial class BackInStockSubscriptionService : IBackInStockSubscriptionService
    {
        #region Fields

        private readonly IRepository<BackInStockSubscription> _backInStockSubscriptionRepository;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="backInStockSubscriptionRepository">Back in stock subscription repository</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="eventPublisher">Event publisher</param>
        public BackInStockSubscriptionService(IRepository<BackInStockSubscription> backInStockSubscriptionRepository,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher)
        {
            this._backInStockSubscriptionRepository = backInStockSubscriptionRepository;
            this._workflowMessageService = workflowMessageService;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a back in stock subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void DeleteSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Delete(subscription);

            //event notification
            _eventPublisher.EntityDeleted(subscription);
        }

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Subscriptions</returns>
        public virtual IPagedList<BackInStockSubscription> GetAllSubscriptionsByCustomerId(int customerId,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _backInStockSubscriptionRepository.Table;
            //customer
            query = query.Where(biss => biss.CustomerId == customerId);
            //product
            query = query.Where(biss => !biss.ProductVariant.Deleted);
            if (!showHidden)
                query = query.Where(biss => biss.ProductVariant.Published);
            query = query.Where(biss => !biss.ProductVariant.Product.Deleted);
            if (!showHidden)
                query = query.Where(biss => biss.ProductVariant.Product.Published);
            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            return new PagedList<BackInStockSubscription>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Subscriptions</returns>
        public virtual IPagedList<BackInStockSubscription> GetAllSubscriptionsByProductVariantId(int productVariantId,
            int pageIndex, int pageSize, bool showHidden = false)
        {
            var query = _backInStockSubscriptionRepository.Table;
            //product
            query = query.Where(biss => biss.ProductVariantId == productVariantId);
            //customer
            query = query.Where(biss => !biss.Customer.Deleted);
            if (!showHidden)
                query = query.Where(biss => biss.Customer.Active);
            query = query.OrderByDescending(biss => biss.CreatedOnUtc);
            return new PagedList<BackInStockSubscription>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all subscriptions
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Subscriptions</returns>
        public virtual BackInStockSubscription FindSubscription(int customerId, int productVariantId)
        {
            var query = _backInStockSubscriptionRepository.Table;
            query = query.Where(biss => biss.CustomerId == customerId);
            query = query.Where(biss => biss.ProductVariantId == productVariantId);
            query = query.OrderByDescending(biss => biss.CreatedOnUtc);

            var subscription = query.FirstOrDefault();
            return subscription;
        }

        /// <summary>
        /// Gets a subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription identifier</param>
        /// <returns>Subscription</returns>
        public virtual BackInStockSubscription GetSubscriptionById(int subscriptionId)
        {
            if (subscriptionId == 0)
                return null;

            var subscription = _backInStockSubscriptionRepository.GetById(subscriptionId);
            return subscription;
        }

        /// <summary>
        /// Inserts subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void InsertSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Insert(subscription);

            //event notification
            _eventPublisher.EntityInserted(subscription);
        }

        /// <summary>
        /// Updates subscription
        /// </summary>
        /// <param name="subscription">Subscription</param>
        public virtual void UpdateSubscription(BackInStockSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");

            _backInStockSubscriptionRepository.Update(subscription);

            //event notification
            _eventPublisher.EntityUpdated(subscription);
        }

        /// <summary>
        /// Send notification to subscribers
        /// </summary>
        /// <param name="productVariant"></param>
        /// <returns>Number of sent email</returns>
        public virtual int SendNotificationsToSubscribers(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            int result = 0;
            var subscriptions = GetAllSubscriptionsByProductVariantId(productVariant.Id, 0, int.MaxValue);
            foreach (var subscription in subscriptions)
            {
                //ensure that customer is registered (simple and fast way)
                if (CommonHelper.IsValidEmail(subscription.Customer.Email))
                {
                    _workflowMessageService.SendBackInStockNotification(subscription, subscription.Customer.LanguageId.HasValue ? subscription.Customer.LanguageId.Value : 0);
                    result++;
                }
            }
            for (int i = 0; i <= subscriptions.Count - 1; i++)
                DeleteSubscription(subscriptions[i]);
            return result;
        }
        
        #endregion
    }
}
