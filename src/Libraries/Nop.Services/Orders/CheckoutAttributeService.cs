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

        protected IStaticCacheManager StaticCacheManager { get; }
        protected IRepository<CheckoutAttribute> CheckoutAttributeRepository { get; }
        protected IRepository<CheckoutAttributeValue> CheckoutAttributeValueRepository { get; }
        protected IStoreMappingService StoreMappingService { get; }

        #endregion

        #region Ctor

        public CheckoutAttributeService(IStaticCacheManager staticCacheManager,
            IRepository<CheckoutAttribute> checkoutAttributeRepository,
            IRepository<CheckoutAttributeValue> checkoutAttributeValueRepository,
            IStoreMappingService storeMappingService)
        {
            StaticCacheManager = staticCacheManager;
            CheckoutAttributeRepository = checkoutAttributeRepository;
            CheckoutAttributeValueRepository = checkoutAttributeValueRepository;
            StoreMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute)
        {
            await CheckoutAttributeRepository.DeleteAsync(checkoutAttribute);
        }

        /// <summary>
        /// Deletes checkout attributes
        /// </summary>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCheckoutAttributesAsync(IList<CheckoutAttribute> checkoutAttributes)
        {
            if (checkoutAttributes == null)
                throw new ArgumentNullException(nameof(checkoutAttributes));

            foreach (var checkoutAttribute in checkoutAttributes) 
                await DeleteCheckoutAttributeAsync(checkoutAttribute);
        }

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attributes
        /// </returns>
        public virtual async Task<IList<CheckoutAttribute>> GetAllCheckoutAttributesAsync(int storeId = 0, bool excludeShippableAttributes = false)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributesAllCacheKey, storeId, excludeShippableAttributes);

            return await StaticCacheManager.GetAsync(key, async () =>
            {
                var checkoutAttributes = (await CheckoutAttributeRepository.GetAllAsync(query =>
                {
                    return from ca in query
                        orderby ca.DisplayOrder, ca.Id
                        select ca;
                })).ToAsyncEnumerable();

                if (storeId > 0)
                    //store mapping
                    checkoutAttributes = checkoutAttributes.WhereAwait(async ca => await StoreMappingService.AuthorizeAsync(ca, storeId));

                if (excludeShippableAttributes)
                    //remove attributes which require shippable products
                    checkoutAttributes = checkoutAttributes.Where(x => !x.ShippableProductRequired);

                return await checkoutAttributes.ToListAsync();
            });
        }

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute
        /// </returns>
        public virtual async Task<CheckoutAttribute> GetCheckoutAttributeByIdAsync(int checkoutAttributeId)
        {
            return await CheckoutAttributeRepository.GetByIdAsync(checkoutAttributeId, cache => default);
        }

        /// <summary>
        /// Gets checkout attributes 
        /// </summary>
        /// <param name="checkoutAttributeIds">Checkout attribute identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attributes
        /// </returns>
        public virtual async Task<IList<CheckoutAttribute>> GetCheckoutAttributeByIdsAsync(int[] checkoutAttributeIds)
        {
            return await CheckoutAttributeRepository.GetByIdsAsync(checkoutAttributeIds);
        }

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute)
        {
            await CheckoutAttributeRepository.InsertAsync(checkoutAttribute);
        }

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute)
        {
            await CheckoutAttributeRepository.UpdateAsync(checkoutAttribute);
        }

        #endregion

        #region Checkout attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue)
        {
            await CheckoutAttributeValueRepository.DeleteAsync(checkoutAttributeValue);
        }

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute values
        /// </returns>
        public virtual async Task<IList<CheckoutAttributeValue>> GetCheckoutAttributeValuesAsync(int checkoutAttributeId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopOrderDefaults.CheckoutAttributeValuesAllCacheKey, checkoutAttributeId);

            var query = from cav in CheckoutAttributeValueRepository.Table
                orderby cav.DisplayOrder, cav.Id
                where cav.CheckoutAttributeId == checkoutAttributeId
                select cav;
            
            var checkoutAttributeValues = await StaticCacheManager.GetAsync(key, async ()=> await query.ToListAsync());

            return checkoutAttributeValues;
        }

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout attribute value
        /// </returns>
        public virtual async Task<CheckoutAttributeValue> GetCheckoutAttributeValueByIdAsync(int checkoutAttributeValueId)
        {
            return await CheckoutAttributeValueRepository.GetByIdAsync(checkoutAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue)
        {
            await CheckoutAttributeValueRepository.InsertAsync(checkoutAttributeValue);
        }

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue)
        {
            await CheckoutAttributeValueRepository.UpdateAsync(checkoutAttributeValue);
        }

        #endregion

        #endregion
    }
}