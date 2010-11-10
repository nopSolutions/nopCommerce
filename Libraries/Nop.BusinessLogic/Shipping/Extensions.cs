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
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get shipping status name
        /// </summary>
        /// <param name="ss">Shipping status</param>
        /// <returns>Shipping status name</returns>
        public static string GetShippingStatusName(this ShippingStatusEnum ss)
        {
            string name = IoCFactory.Resolve<ILocalizationManager>().GetLocaleResourceString(
                string.Format("ShippingStatus.{0}", ss.ToString()),
                NopContext.Current.WorkingLanguage.LanguageId,
                true, 
                CommonHelper.ConvertEnum(ss.ToString()));
            
            return name;
        }
    }
}
