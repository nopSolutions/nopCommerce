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

namespace NopSolutions.NopCommerce.Payment.Methods.SecurePay
{
    public class XmlPaymentSettings
    {
        #region Properties
        public static bool TestMode
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.SecurePay.XmlPayment.TestMode", true);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.SecurePay.XmlPayment.TestMode", value.ToString());
            }
        }

        public static bool AuthorizeOnly
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.SecurePay.XmlPayment.AuthorizeOnly", false);
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.SecurePay.XmlPayment.AuthorizeOnly", value.ToString());
            }
        }
        #endregion
    }
}
