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


using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a request for tax calculation
    /// </summary>
    public partial class CalculateTaxRequest
    {
        /// <summary>
        /// Gets or sets a customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets a tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }
    }
}
