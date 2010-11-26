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

namespace NopSolutions.NopCommerce.Payment.Methods.Assist
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Assist.HostedPayment.GatewayUrl", "https://test.assist.ru/shops/cardpayment.cfm");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.GatewayUrl", value);
            }
        }

        public static string ShopId
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Assist.HostedPayment.ShopID");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.ShopID", value);
            }
        }

        public static bool AuthorizeOnly
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Assist.HostedPayment.AuthorizeOnly", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.AuthorizeOnly", value.ToString());
            }
        }

        public static bool TestMode
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Assist.HostedPayment.TestMode", true);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.TestMode", value.ToString());
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Assist.HostedPayment.AdditionalFee");
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Assist.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
