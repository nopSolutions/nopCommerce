using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer attribute service
    /// </summary>
    public partial interface ICustomerAttributeService
    {
        /// <summary>
        /// Deletes a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        Task DeleteCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        Task<IList<CustomerAttribute>> GetAllCustomerAttributesAsync();

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute</returns>
        Task<CustomerAttribute> GetCustomerAttributeByIdAsync(int customerAttributeId);

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        Task InsertCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        Task UpdateCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        Task DeleteCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        Task<IList<CustomerAttributeValue>> GetCustomerAttributeValuesAsync(int customerAttributeId);

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        Task<CustomerAttributeValue> GetCustomerAttributeValueByIdAsync(int customerAttributeValueId);

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        Task InsertCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        Task UpdateCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);
    }
}
