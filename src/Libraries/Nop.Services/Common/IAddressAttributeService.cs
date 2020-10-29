using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial interface IAddressAttributeService
    {
        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        Task DeleteAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        Task<IList<AddressAttribute>> GetAllAddressAttributesAsync();

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute</returns>
        Task<AddressAttribute> GetAddressAttributeByIdAsync(int addressAttributeId);

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        Task InsertAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        Task UpdateAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        Task DeleteAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>Address attribute values</returns>
        Task<IList<AddressAttributeValue>> GetAddressAttributeValuesAsync(int addressAttributeId);

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>Address attribute value</returns>
        Task<AddressAttributeValue> GetAddressAttributeValueByIdAsync(int addressAttributeValueId);

        /// <summary>
        /// Inserts a address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        Task InsertAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        Task UpdateAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);
    }
}
