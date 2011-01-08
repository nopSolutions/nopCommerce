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
        void DeleteLanguage(CustomerContent content);

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
        /// <returns>Customer content</returns>
        IList<T> GetAllCustomerContent<T>(int customerId, bool? approved) where T : CustomerContent;

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
        void UpdateCustomerContente(CustomerContent content);
    }
}
