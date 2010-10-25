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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Payment.Methods.Svea
{
    /// <summary>
    /// Svea settings
    /// </summary>
    public class SveaSettings
    {
        /// <summary>
        /// Gets or sets the Svea username
        /// </summary>
        public static string Username
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Svea.Username");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Svea.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the Svea password
        /// </summary>
        public static string Password
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Svea.Password");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Svea.Password", value);
            }
        }
    }
}
