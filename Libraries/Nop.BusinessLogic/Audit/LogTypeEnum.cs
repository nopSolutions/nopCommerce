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

namespace NopSolutions.NopCommerce.BusinessLogic.Audit
{
    /// <summary>
    /// Represents a log item type (need to be synchronize with [Nop_LogType] table)
    /// </summary>
    public enum LogTypeEnum : int
    {
        /// <summary>
        /// Customer error log item type
        /// </summary>
        CustomerError = 1,
        /// <summary>
        /// Mail error log item type
        /// </summary>
        MailError = 2,
        /// <summary>
        /// Order error log item type
        /// </summary>
        OrderError = 3,
        /// <summary>
        /// Administration area log item type
        /// </summary>
        AdministrationArea = 4,
        /// <summary>
        /// Common error log item type
        /// </summary>
        CommonError = 5,
        /// <summary>
        /// Shipping error log item type
        /// </summary>
        ShippingError = 6,
        /// <summary>
        /// Tax error log item type
        /// </summary>
        TaxError = 7,
        /// <summary>
        /// Unknown log item type
        /// </summary>
        Unknown = 20,
    }
}
