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

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents a return status
    /// </summary>
    public enum ReturnStatusEnum : int
    {
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 0,
        /// <summary>
        /// Received
        /// </summary>
        Received = 10,
        /// <summary>
        /// Return authorized
        /// </summary>
        ReturnAuthorized = 20,
        /// <summary>
        /// Item(s) repaired
        /// </summary>
        ItemsRepaired = 30,
        /// <summary>
        /// Item(s) refunded
        /// </summary>
        ItemsRefunded = 40,
        /// <summary>
        /// Request rejected
        /// </summary>
        RequestRejected = 50,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 60,
    }
}
