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

namespace NopSolutions.NopCommerce.Payment.Methods.USAePay
{
    /// <summary>
    /// Represents payment processor transaction mode for USA ePay (integrated) payment gateway
    /// </summary>
    public enum TransactMode : byte
    {
        /// <summary>
        /// Authorize transaction mode
        /// </summary>
        Authorize = 1,
        /// <summary>
        /// Authorize and capture transaction mode
        /// </summary>
        AuthorizeAndCapture= 2
    }
}
