//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nop.Core.Domain;
using Nop.Core;

namespace Nop.Services
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface ICustomerService
    {
        #region Customers

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="registrationFrom">Customer registration from; null to load all customers</param>
        /// <param name="registrationTo">Customer registration to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Customer collection</returns>
        PagedList<Customer> GetAllCustomers(DateTime? registrationFrom,
            DateTime? registrationTo, string email, string username,
           int pageIndex, int pageSize);

        /// <summary>
        /// Gets all customers by customer role id
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer collection</returns>
        IList<Customer> GetCustomersByCustomerRoleId(int customerRoleId);

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        void DeleteCustomer(Customer customer);

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="email">Customer Email</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByEmail(string email);

        /// <summary>
        /// Gets a customer by email
        /// </summary>
        /// <param name="username">Customer username</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByUsername(string username);

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        Customer GetCustomerById(int customerId);

        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>A customer</returns>
        Customer GetCustomerByGuid(Guid customerGuid);

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        void UpdateCustomer(Customer customer);

        #endregion

        #region Customer roles

        /// <summary>
        /// Delete a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        void DeleteCustomerRole(CustomerRole customerRole);

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>Customer role</returns>
        CustomerRole GetCustomerRoleById(int customerRoleId);

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <returns>Customer role collection</returns>
        IList<CustomerRole> GetAllCustomerRoles();

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer role collection</returns>
        IList<CustomerRole> GetCustomerRolesByCustomerId(int customerId);

        /// <summary>
        /// Gets customer roles by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Customer role collection</returns>
        IList<CustomerRole> GetCustomerRolesByCustomerId(int customerId, bool showHidden);

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        void InsertCustomerRole(CustomerRole customerRole);

        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        void UpdateCustomerRole(CustomerRole customerRole);

        #endregion

        #region Customer sessions

        /// <summary>
        /// Gets a customer session
        /// </summary>
        /// <param name="customerSessionId">Customer session ID</param>
        /// <returns>Customer session</returns>
        CustomerSession GetCustomerSessionById(int customerSessionId);

        /// <summary>
        /// Gets a customer session
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <returns>Customer session</returns>
        CustomerSession GetCustomerSessionByGuid(Guid customerSessionGuid);

        /// <summary>
        /// Gets a customer session by customer identifier
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>Customer session</returns>
        CustomerSession GetCustomerSessionByCustomerId(int customerId);

        /// <summary>
        /// Deletes a customer session
        /// </summary>
        /// <param name="customerSession">Customer session</param>
        void DeleteCustomerSession(CustomerSession customerSession);

        /// <summary>
        /// Gets all customer sessions
        /// </summary>
        /// <returns>Customer session collection</returns>
        IList<CustomerSession> GetAllCustomerSessions();

        /// <summary>
        /// Saves a customer session to the data storage if it exists or creates new one
        /// </summary>
        /// <param name="customerSessionGuid">Customer session GUID</param>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="lastAccessedUtc">The last accessed date and time</param>
        /// <param name="isExpired">A value indicating whether the customer session is expired</param>
        /// <returns>Customer session</returns>
        CustomerSession SaveCustomerSession(Guid customerSessionGuid,
            int? customerId, DateTime lastAccessedUtc, bool isExpired);

        #endregion
    }
}