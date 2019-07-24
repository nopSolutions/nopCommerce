using System.Collections.Generic;
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
        void DeleteCustomerAttribute(CustomerAttribute customerAttribute);

        /// <summary>
        /// Gets all customer attributes
        /// </summary>
        /// <returns>Customer attributes</returns>
        IList<CustomerAttribute> GetAllCustomerAttributes();

        /// <summary>
        /// Gets a customer attribute 
        /// </summary>
        /// <param name="customerAttributeId">Customer attribute identifier</param>
        /// <returns>Customer attribute</returns>
        CustomerAttribute GetCustomerAttributeById(int customerAttributeId);

        /// <summary>
        /// Inserts a customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        void InsertCustomerAttribute(CustomerAttribute customerAttribute);

        /// <summary>
        /// Updates the customer attribute
        /// </summary>
        /// <param name="customerAttribute">Customer attribute</param>
        void UpdateCustomerAttribute(CustomerAttribute customerAttribute);

        /// <summary>
        /// Deletes a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        void DeleteCustomerAttributeValue(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Gets customer attribute values by customer attribute identifier
        /// </summary>
        /// <param name="customerAttributeId">The customer attribute identifier</param>
        /// <returns>Customer attribute values</returns>
        IList<CustomerAttributeValue> GetCustomerAttributeValues(int customerAttributeId);

        /// <summary>
        /// Gets a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValueId">Customer attribute value identifier</param>
        /// <returns>Customer attribute value</returns>
        CustomerAttributeValue GetCustomerAttributeValueById(int customerAttributeValueId);

        /// <summary>
        /// Inserts a customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        void InsertCustomerAttributeValue(CustomerAttributeValue customerAttributeValue);

        /// <summary>
        /// Updates the customer attribute value
        /// </summary>
        /// <param name="customerAttributeValue">Customer attribute value</param>
        void UpdateCustomerAttributeValue(CustomerAttributeValue customerAttributeValue);
    }
}
