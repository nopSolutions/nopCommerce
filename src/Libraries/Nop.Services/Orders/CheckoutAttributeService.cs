using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Stores;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial class CheckoutAttributeService : ICheckoutAttributeService
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<CheckoutAttribute> _checkoutAttributeRepository;
        private readonly IRepository<CheckoutAttributeValue> _checkoutAttributeValueRepository;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CheckoutAttributeService(IStaticCacheManager staticCacheManager,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IStoreMappingService storeMappingService)
        {
            _staticCacheManager = staticCacheManager;
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
        public virtual async Task DeleteCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            await _checkoutAttributeRepository.Delete(checkoutAttribute);
        }

        /// <summary>
        /// Deletes checkout attributes
        /// </summary>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        public virtual async Task DeleteCheckoutAttributes(IList<CheckoutAttribute> checkoutAttributes)
        {
            if (checkoutAttributes == null)
                throw new ArgumentNullException(nameof(checkoutAttributes));

            foreach (var checkoutAttribute in checkoutAttributes) 
                await DeleteCheckoutAttribute(checkoutAttribute);
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>Checkout attributes</returns>
        public virtual async Task<IList<CheckoutAttribute>> GetAllCheckoutAttributes(int storeId = 0, bool excludeShippableAttributes = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);

            return await _staticCacheManager.Get(key, async () =>
            {
                var checkoutAttributes = await _checkoutAttributeRepository.GetAll(query =>
                {
                    return from ca in query
                        orderby ca.DisplayOrder, ca.Id
                        select ca;
                });

                if (storeId > 0)
                    //store mapping
                    checkoutAttributes = checkoutAttributes.Where(ca => _storeMappingService.Authorize(ca, storeId).Result).ToList();

                if (excludeShippableAttributes)
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired).ToList();

                return checkoutAttributes;
            });
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>Checkout attribute</returns>
        public virtual async Task<CheckoutAttribute> GetCheckoutAttributeById(int checkoutAttributeId)
        {
            return await _checkoutAttributeRepository.GetById(checkoutAttributeId, cache => default);
        }

        /// <summary>
        /// Gets checkout attributes 
        /// </summary>
        /// <param name="checkoutAttributeIds">Checkout attribute identifiers</param>
        /// <returns>Checkout attributes</returns>
        public virtual async Task<IList<CheckoutAttribute>> GetCheckoutAttributeByIds(int[] checkoutAttributeIds)
        {
            return await _checkoutAttributeRepository.GetByIds(checkoutAttributeIds);
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual async Task InsertCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            await _checkoutAttributeRepository.Insert(checkoutAttribute);
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        public virtual async Task UpdateCheckoutAttribute(CheckoutAttribute checkoutAttribute)
        {
            await _checkoutAttributeRepository.Update(checkoutAttribute);
        }

        #endregion

        #region Checkout attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual async Task DeleteCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            await _checkoutAttributeValueRepository.Delete(checkoutAttributeValue);
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>Checkout attribute values</returns>
        public virtual async Task<IList<CheckoutAttributeValue>> GetCheckoutAttributeValues(int checkoutAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributeValuesAllCacheKey, checkoutAttributeId);

            var query = from cav in _checkoutAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CheckoutAttributeId == checkoutAttributeId
                select cav;
            
            var checkoutAttributeValues = await _staticCacheManager.Get(key, async ()=> await query.ToAsyncEnumerable().ToListAsync());

            return checkoutAttributeValues;
        }

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>Checkout attribute value</returns>
        public virtual async Task<CheckoutAttributeValue> GetCheckoutAttributeValueById(int checkoutAttributeValueId)
        {
            return await _checkoutAttributeValueRepository.GetById(checkoutAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual async Task InsertCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            await _checkoutAttributeValueRepository.Insert(checkoutAttributeValue);
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        public virtual async Task UpdateCheckoutAttributeValue(CheckoutAttributeValue checkoutAttributeValue)
        {
            await _checkoutAttributeValueRepository.Update(checkoutAttributeValue);
        }

        #endregion

        #endregion
    }
}