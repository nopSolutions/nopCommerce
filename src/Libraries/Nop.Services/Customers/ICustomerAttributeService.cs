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
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attributes
        /// </returns>
        Task<IList<CustomerAttribute>> GetAllCustomerAttributesAsync();

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute
        /// </returns>
        Task<CustomerAttribute> GetCustomerAttributeByIdAsync(int customerAttributeId);

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerAttributeAsync(CustomerAttribute customerAttribute);

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute values
        /// </returns>
        Task<IList<CustomerAttributeValue>> GetCustomerAttributeValuesAsync(int customerAttributeId);

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer attribute value
        /// </returns>
        Task<CustomerAttributeValue> GetCustomerAttributeValueByIdAsync(int customerAttributeValueId);

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerAttributeValueAsync(CustomerAttributeValue customerAttributeValue);
    }
}
