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

using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

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
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.GatewayUrl", "https://esplusqa.moneris.com/DPHPP/index.php");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.GatewayUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets the hpp ID
        /// </summary>
        public static string HppId
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.HppID");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.HppID", value);
            }
        }

        /// <summary>
        /// Gets or sets the hpp key
        /// </summary>
        public static string HppKey
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Moneris.HostedPayment.HppKey");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.HppKey", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether order should be marked as authorized on success response; othewise order will be marked as paid.
        /// </summary>
        public static bool AuthorizeOnly
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Moneris.HostedPayment.AuthorizeOnly", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Moneris.HostedPayment.AuthorizeOnly", value.ToString());
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Moneris.HostedPayment.AdditionalFee");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Moneris.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
