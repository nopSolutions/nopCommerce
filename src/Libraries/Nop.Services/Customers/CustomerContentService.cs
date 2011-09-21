using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer content service
    /// </summary>
    public partial class CustomerContentService : ICustomerContentService
    {
        #region Fields

        private readonly IRepository<CustomerContent> _contentRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="contentRepository">Customer content repository</param>
        /// <param name="eventPublisher"></param>
        public CustomerContentService(ICacheManager cacheManager,
            IRepository<CustomerContent> contentRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _contentRepository = contentRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        public virtual void DeleteCustomerContent(CustomerContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Delete(content);

            //event notification
            _eventPublisher.EntityDeleted(content);
        }

        /// <summary>
        /// Gets all customer content
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>Customer content</returns>
        public virtual IList<CustomerContent> GetAllCustomerContent(int customerId, bool? approved)
        {
            var query = from c in _contentRepository.Table
                        orderby c.CreatedOnUtc
                        where !approved.HasValue || c.IsApproved == approved &&
                        (customerId == 0 || c.CustomerId == customerId)
                        select c;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets all customer content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>Customer content</returns>
        public virtual IList<T> GetAllCustomerContent<T>(int customerId, bool? approved) where T: CustomerContent
        {
            var query = from c in _contentRepository.Table
                        orderby c.CreatedOnUtc
                        where !approved.HasValue || c.IsApproved == approved &&
                        (customerId == 0 || c.CustomerId == customerId)
                        select c;
            var content = query.OfType<T>().ToList();
            return content;
        }

        /// <summary>
        /// Gets a customer content
        /// </summary>
        /// <param name="contentId">Customer content identifier</param>
        /// <returns>Customer content</returns>
        public virtual CustomerContent GetCustomerContentById(int contentId)
        {
            if (contentId == 0)
                return null;

            return _contentRepository.GetById(contentId);
                                          
        }

        /// <summary>
        /// Inserts a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        public virtual void InsertCustomerContent(CustomerContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Insert(content);

            //event notification
            _eventPublisher.EntityInserted(content);
        }

        /// <summary>
        /// Updates a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        public virtual void UpdateCustomerContent(CustomerContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            _contentRepository.Update(content);

            //event notification
            _eventPublisher.EntityUpdated(content);
        }

        #endregion
    }
}
