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

namespace NopSolutions.NopCommerce.Payment.Methods.Assist
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Assist.HostedPayment.GatewayUrl", "https://test.assist.ru/shops/cardpayment.cfm");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.GatewayUrl", value);
            }
        }

        public static string ShopId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Assist.HostedPayment.ShopID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.ShopID", value);
            }
        }

        public static bool AuthorizeOnly
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Assist.HostedPayment.AuthorizeOnly", false);
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.AuthorizeOnly", value.ToString());
            }
        }

        public static bool TestMode
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.Assist.HostedPayment.TestMode", true);
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Assist.HostedPayment.TestMode", value.ToString());
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Assist.HostedPayment.AdditionalFee");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Assist.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
