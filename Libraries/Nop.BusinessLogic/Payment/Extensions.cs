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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get payment status name
        /// </summary>
        /// <param name="ps">Payment status</param>
        /// <returns>Payment status name</returns>
        public static string GetPaymentStatusName(this PaymentStatusEnum ps)
        {
            string name = IoC.Resolve<ILocalizationManager>().GetLocaleResourceString(
                string.Format("PaymentStatus.{0}", ps.ToString()),
                NopContext.Current.WorkingLanguage.LanguageId,
                true,
                CommonHelper.ConvertEnum(ps.ToString()));
            
            return name;
        }
    }
}
