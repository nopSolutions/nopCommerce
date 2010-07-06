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
using System.Net;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common;


namespace NopSolutions.NopCommerce.Payment.Methods.Worldpay
{
    /// <summary>
    /// Worldpay settings
    /// </summary>
    public static class WorldpayConstants
    {
        /// <summary>
        /// Setting
        /// </summary>
        public const string SETTING_CREDITCARD_CODE_PROPERTY = "PaymentMethod.Worldpay.CardPayment";

        /// <summary>
        /// Setting
        /// </summary>
        public const string SETTING_USE_SANDBOX = "PaymentMethod.Worldpay.UseSandbox";

        /// <summary>
        /// Setting
        /// </summary>
        public const string SETTING_INSTANCEID = "PaymentMethod.Worldpay.InstanceID";

        /// <summary>
        /// Setting
        /// </summary>
        public const string SETTING_CALLBACK_PASSWORD = "PaymentMethod.Worldpay.CallbackPassword";

        /// <summary>
        /// Setting
        /// </summary>
        public const string SETTING_WorldPayCSSName = "PaymentMethod.Worldpay.WorldPayCSSName";
    }
}