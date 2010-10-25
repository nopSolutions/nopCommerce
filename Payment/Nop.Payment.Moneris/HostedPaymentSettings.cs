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
// Contributor(s): 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.Moneris
{
    public class HostedPaymentSettings
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Gateway URL
        /// </summary>
        public static string GatewayUrl
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.GatewayUrl", "https://esplusqa.moneris.com/DPHPP/index.php");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.GatewayUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets the hpp ID
        /// </summary>
        public static string HppId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.HppID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.HppID", value);
            }
        }

        /// <summary>
        /// Gets or sets the hpp key
        /// </summary>
        public static string HppKey
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.HppKey");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.HppKey", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether order should be marked as authorized on success response; othewise order will be marked as paid.
        /// </summary>
        public static bool AuthorizeOnly
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Moneris.HostedPayment.AuthorizeOnly", false);
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.AuthorizeOnly", value.ToString());
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Moneris.HostedPayment.AdditionalFee");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Moneris.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
