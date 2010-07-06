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

namespace NopSolutions.NopCommerce.Payment.Methods.Svea
{
    /// <summary>
    /// Svea hosted payment method settings
    /// </summary>
    public class HostedPaymentSettings : SveaSettings
    {
        /// <summary>
        /// Gets or sets the payment gateway URL
        /// </summary>
        public static string GatewayUrl
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Svea.HostedPayment.GatewayUrl", "https://partnerweb.sveaekonomi.se/webpayhosted2/InitiatePayment.aspx");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Svea.HostedPayment.GatewayUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets the hosted payment method name
        /// </summary>
        public static string PaymentMethod
        {
            get
            {
                return SettingManager.GetSettingValue("PaymentMethod.Svea.HostedPayment.PaymentMethod", "invoice");
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Svea.HostedPayment.PaymentMethod", value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test mode is enabled
        /// </summary>
        public static bool UseSandbox
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("PaymentMethod.Svea.HostedPayment.UseSandbox", true);
            }
            set
            {
                SettingManager.SetParam("PaymentMethod.Svea.HostedPayment.UseSandbox", value.ToString());
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return SettingManager.GetSettingValueDecimalNative("PaymentMethod.Svea.HostedPayment.AdditionalFee");
            }
            set
            {
                SettingManager.SetParamNative("PaymentMethod.Svea.HostedPayment.AdditionalFee", value);
            }
        }
    }
}
