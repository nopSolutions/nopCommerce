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

namespace NopSolutions.NopCommerce.Payment.Methods.iDeal
{
    public class iDealBasicPaymentSettings
    {
        #region Properties
        /// <summary>
        /// Gets or sets the merchant ID
        /// </summary>
        public static string MerchantId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.iDeal.Basic.MerchantID");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.iDeal.Basic.MerchantID", value);
            }
        }

        /// <summary>
        /// Gets or sets the sub ID
        /// </summary>
        public static string SubId
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.iDeal.Basic.SubID", "0");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.iDeal.Basic.SubID", value);
            }
        }

        /// <summary>
        /// Gets or sets the hash key
        /// </summary>
        public static string HashKey
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.iDeal.Basic.HashKey");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.iDeal.Basic.HashKey", value);
            }
        }

        public static string Url
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.iDeal.Basic.Url", "https://idealtest.secure-ing.com/ideal/mpiPayInitIng.do");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.iDeal.Basic.Url", value);
            }
        }

        public static decimal AdditionalFee
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.iDeal.Basic.AdditionalFee");
            }
            set
            {
                IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.iDeal.Basic.AdditionalFee", value);
            }
        }
        #endregion
    }
}
