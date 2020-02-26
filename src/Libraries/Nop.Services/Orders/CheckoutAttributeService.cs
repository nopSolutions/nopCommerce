using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Caching.CachingDefaults;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial class CheckoutAttributeService : ICheckoutAttributeService
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<CheckoutAttributeValue> _checkoutAttributeValueRepository;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CheckoutAttributeService(IStaticCacheManager cacheManager,
            IEventPublisher eventPublisher,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IStoreMappingService storeMappingService)
        {
            _cacheManager = cacheManager;
            _eventPublisher = eventPublisher;
            _checkoutAttributeRepository = checkoutAttributeRepository;
            _checkoutAttributeValueRepository = checkoutAttributeValueRepository;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void DeleteCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            _checkoutAttributeRepository.Delete(checkoutAttribute);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttribute);
        }

        /// <summary>
        /// Deletes checkout attributes
        /// </summary>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        public virtual void DeleteCheckoutAttributes(IList<CheckoutAttribute> checkoutAttributes)
        {
            if (checkoutAttributes == null)
                throw new ArgumentNullException(nameof(checkoutAttributes));

            foreach (var checkoutAttribute in checkoutAttributes)
            {
                DeleteCheckoutAttribute(checkoutAttribute);
            }
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>Checkout attributes</returns>
        public virtual IList<CheckoutAttribute> GetAllCheckoutAttributes(int storeId = 0, bool excludeShippableAttributes = false)
        {
            var key = string.Format(NopOrderCachingDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);
            return _cacheManager.Get(key, () =>
            {
                var query = from ca in _checkoutAttributeRepository.Table
                            orderby ca.DisplayOrder, ca.Id
                            select ca;

                var checkoutAttributes = query.ToList();
                if (storeId > 0)
                {
                    //store mapping
                    checkoutAttributes = checkoutAttributes.Where(ca => _storeMappingService.Authorize(ca)).ToList();
                }

                if (excludeShippableAttributes)
                {
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired).ToList();
                }

                return checkoutAttributes;
            });
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        public virtual CheckoutAttribute GetCheckoutAttributeById(int checkoutAttributeId)
        {
            if (checkoutAttributeId == 0)
                return null;

            var key = string.Format(NopOrderCachingDefaults.CheckoutAttributesByIdCacheKey, checkoutAttributeId);

            return _checkoutAttributeRepository.ToCachedGetById(checkoutAttributeId, key);
        }

        /// <summary>
        /// Gets checkout attributes 
        /// </summary>
        /// <param name="checkoutAttributeIds">Checkout attribute identifiers</param>
        /// <returns>Checkout attributes</returns>
        public virtual IList<CheckoutAttribute> GetCheckoutAttributeByIds(int[] checkoutAttributeIds)
        {
            if (checkoutAttributeIds == null || checkoutAttributeIds.Length == 0)
                return new List<CheckoutAttribute>();

            var query = from p in _checkoutAttributeRepository.Table
                        where checkoutAttributeIds.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            _checkoutAttributeRepository.Insert(checkoutAttribute);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttribute);
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual void UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            if (checkoutAttribute == null)
                throw new ArgumentNullException(nameof(checkoutAttribute));

            _checkoutAttributeRepository.Update(checkoutAttribute);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttribute);
        }

        #endregion

        #region Checkout attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void DeleteCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            _checkoutAttributeValueRepository.Delete(checkoutAttributeValue);

            //event notification
            _eventPublisher.EntityDeleted(checkoutAttributeValue);
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute values</returns>
        public virtual IList<CheckoutAttributeValue> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            var key = string.Format(NopOrderCachingDefaults.CheckoutAttributeValuesAllCacheKey, checkoutAttributeId);

            var query = from cav in _checkoutAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CheckoutAttributeId == checkoutAttributeId
                select cav;
            var checkoutAttributeValues = query.ToCachedList(key);

            return checkoutAttributeValues;
        }

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        public virtual CheckoutAttributeValue GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            if (checkoutAttributeValueId == 0)
                return null;

            var key = string.Format(NopOrderCachingDefaults.CheckoutAttributeValuesByIdCacheKey, checkoutAttributeValueId);

            return _checkoutAttributeValueRepository.ToCachedGetById(checkoutAttributeValueId, key);
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            _checkoutAttributeValueRepository.Insert(checkoutAttributeValue);

            //event notification
            _eventPublisher.EntityInserted(checkoutAttributeValue);
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual void UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            if (checkoutAttributeValue == null)
                throw new ArgumentNullException(nameof(checkoutAttributeValue));

            _checkoutAttributeValueRepository.Update(checkoutAttributeValue);

            //event notification
            _eventPublisher.EntityUpdated(checkoutAttributeValue);
        }

        #endregion

        #endregion
    }
}