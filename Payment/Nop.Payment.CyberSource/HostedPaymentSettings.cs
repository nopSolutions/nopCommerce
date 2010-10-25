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

namespace NopSolutions.NopCommerce.Payment.Methods.CyberSource
{
    public class HostedPaymentSettings
    {
        #region Properties
        public static string GatewayUrl
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CyberSource.HostedPayment.GatewayUrl", "https://orderpagetest.ic3.com/hop/orderform.jsp");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.CyberSource.HostedPayment.GatewayUrl", value);
            }
        }

        public static string MerchantId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CyberSource.HostedPayment.MerchantID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.CyberSource.HostedPayment.MerchantID", value);
            }
        }

        public static string PublicKey
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CyberSource.HostedPayment.PublicKey");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.CyberSource.HostedPayment.PublicKey", value);
            }
        }

        public static string SerialNumber
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.CyberSource.HostedPayment.SerialNumber");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.CyberSource.HostedPayment.SerialNumber", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.CyberSource.HostedPayment.AdditionalFee");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.CyberSource.HostedPayment.AdditionalFee", value);
            }
        }
        #endregion
    }
}
