using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Customers
{
    /// <summary>
    /// Customer content service interface
    /// </summary>
    public partial interface ICustomerContentService
    {
        /// <summary>
        /// Deletes a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        void DeleteCustomerContent(CustomerContent content);

        /// <summary>
        /// Gets all customer content
        /// </summary>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <returns>Customer content</returns>
        IList<CustomerContent> GetAllCustomerContent(int customerId, bool? approved);
        
        /// <summary>
        /// Gets all customer content
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="customerId">Customer identifier; 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param>
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <returns>Customer content</returns>
        IList<T> GetAllCustomerContent<T>(int customerId, bool? approved,
            DateTime? fromUtc = null, DateTime? toUtc = null) where T : CustomerContent;

        /// <summary>
        /// Gets a customer content
        /// </summary>
        /// <param name="contentId">Customer content identifier</param>
        /// <returns>Customer content</returns>
        CustomerContent GetCustomerContentById(int contentId);

        /// <summary>
        /// Inserts a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        void InsertCustomerContent(CustomerContent content);

        /// <summary>
        /// Updates a customer content
        /// </summary>
        /// <param name="content">Customer content</param>
        void UpdateCustomerContent(CustomerContent content);
    }
}
