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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;

namespace NopSolutions.NopCommerce.Shipping.Methods.UPS
{
    /// <summary>
    /// UPS pickup type
    /// </summary>
    public enum UPSPickupType
    {
        /// <summary>
        /// Daily pickup
        /// </summary>
        DailyPickup,
        /// <summary>
        /// Customer counter
        /// </summary>
        CustomerCounter,
        /// <summary>
        /// One time pickup
        /// </summary>
        OneTimePickup,
        /// <summary>
        /// On call air
        /// </summary>
        OnCallAir,
        /// <summary>
        /// Suggested retail rates
        /// </summary>
        SuggestedRetailRates,
        /// <summary>
        /// Letter center
        /// </summary>
        LetterCenter,
        /// <summary>
        /// Air service center
        /// </summary>
        AirServiceCenter
    }
}
