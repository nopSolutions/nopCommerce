using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Data;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial class AddressAttributeService : IAddressAttributeService
    {
        #region Fields

        protected IRepository<AddressAttribute> AddressAttributeRepository { get; }
        protected IRepository<AddressAttributeValue> AddressAttributeValueRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }

        #endregion

        #region Ctor

        public AddressAttributeService(IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            AddressAttributeRepository = addressAttributeRepository;
            AddressAttributeValueRepository = addressAttributeValueRepository;
            StaticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAddressAttributeAsync(AddressAttribute addressAttribute)
        {
            await AddressAttributeRepository.DeleteAsync(addressAttribute);
        }

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attributes
        /// </returns>
        public virtual async Task<IList<AddressAttribute>> GetAllAddressAttributesAsync()
        {
            return await AddressAttributeRepository.GetAllAsync(query=>
            {
                return from aa in query
                    orderby aa.DisplayOrder, aa.Id
                    select aa;
            }, cache => default);
        }

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute
        /// </returns>
        public virtual async Task<AddressAttribute> GetAddressAttributeByIdAsync(int addressAttributeId)
        {
            return await AddressAttributeRepository.GetByIdAsync(addressAttributeId, cache => default);
        }

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAddressAttributeAsync(AddressAttribute addressAttribute)
        {
            await AddressAttributeRepository.InsertAsync(addressAttribute);
        }

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAddressAttributeAsync(AddressAttribute addressAttribute)
        {
            await AddressAttributeRepository.UpdateAsync(addressAttribute);
        }

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
        {
            await AddressAttributeValueRepository.DeleteAsync(addressAttributeValue);
        }

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute values
        /// </returns>
        public virtual async Task<IList<AddressAttributeValue>> GetAddressAttributeValuesAsync(int addressAttributeId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCommonDefaults.AddressAttributeValuesByAttributeCacheKey, addressAttributeId);

            var query = from aav in AddressAttributeValueRepository.Table
                orderby aav.DisplayOrder, aav.Id
                where aav.AddressAttributeId == addressAttributeId
                select aav;
            var addressAttributeValues = await StaticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return addressAttributeValues;
        }

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address attribute value
        /// </returns>
        public virtual async Task<AddressAttributeValue> GetAddressAttributeValueByIdAsync(int addressAttributeValueId)
        {
            return await AddressAttributeValueRepository.GetByIdAsync(addressAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
        {
            await AddressAttributeValueRepository.InsertAsync(addressAttributeValue);
        }

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue)
        {
            await AddressAttributeValueRepository.UpdateAsync(addressAttributeValue);
        }

        #endregion
    }
}