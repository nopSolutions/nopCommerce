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
// Contributor(s): Stephen Kennedy - VAT support, _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;



namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Represents the VAT number status enumeration
    /// </summary>
    public enum VatNumberStatusEnum : int
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Empty
        /// </summary>
        Empty = 10,
        /// <summary>
        /// Valid
        /// </summary>
        Valid = 20,
        /// <summary>
        /// Invalid
        /// </summary>
        Invalid = 30
    }
}
