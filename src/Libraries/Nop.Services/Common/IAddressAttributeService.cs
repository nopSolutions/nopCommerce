using System.Collections.Generic;
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
        void DeleteAddressAttribute(AddressAttribute addressAttribute);

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>Address attributes</returns>
        IList<AddressAttribute> GetAllAddressAttributes();

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>Address attribute</returns>
        AddressAttribute GetAddressAttributeById(int addressAttributeId);

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        void InsertAddressAttribute(AddressAttribute addressAttribute);

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        void UpdateAddressAttribute(AddressAttribute addressAttribute);

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        void DeleteAddressAttributeValue(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>Address attribute values</returns>
        IList<AddressAttributeValue> GetAddressAttributeValues(int addressAttributeId);

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>Address attribute value</returns>
        AddressAttributeValue GetAddressAttributeValueById(int addressAttributeValueId);

        /// <summary>
        /// Inserts a address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        void InsertAddressAttributeValue(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        void UpdateAddressAttributeValue(AddressAttributeValue addressAttributeValue);
    }
}
