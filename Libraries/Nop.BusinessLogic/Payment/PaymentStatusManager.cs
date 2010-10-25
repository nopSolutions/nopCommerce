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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Payment status manager
    /// </summary>
    public partial class PaymentStatusManager : IPaymentStatusManager
    {
        #region Constants
        private const string PAYMENTSTATUSES_ALL_KEY = "Nop.paymentstatus.all";
        private const string PAYMENTSTATUSES_BY_ID_KEY = "Nop.paymentstatus.id-{0}";
        private const string PAYMENTSTATUSES_PATTERN_KEY = "Nop.paymentstatus.";
        #endregion
        
        #region Methods

        /// <summary>
        /// Gets a payment status full name
        /// </summary>
        /// <param name="paymentStatusId">Payment status identifier</param>
        /// <returns>Payment status name</returns>
        public string GetPaymentStatusName(int paymentStatusId)
        {
            var paymentStatus = GetPaymentStatusById(paymentStatusId);
            if (paymentStatus != null)
            {
                string name = string.Empty;
                if (NopContext.Current != null)
                {
                    name = LocalizationManager.GetLocaleResourceString(string.Format("PaymentStatus.{0}", (PaymentStatusEnum)paymentStatus.PaymentStatusId), NopContext.Current.WorkingLanguage.LanguageId, true, paymentStatus.Name);
                }
                else
                {
                    name = paymentStatus.Name;
                }
                return name;
            }
            else
            {
                return ((PaymentStatusEnum)paymentStatusId).ToString();
            }
        }

        /// <summary>
        /// Gets a payment status by identifier
        /// </summary>
        /// <param name="paymentStatusId">payment status identifier</param>
        /// <returns>Payment status</returns>
        public PaymentStatus GetPaymentStatusById(int paymentStatusId)
        {
            if (paymentStatusId == 0)
                return null;

            string key = string.Format(PAYMENTSTATUSES_BY_ID_KEY, paymentStatusId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (PaymentStatus)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ps in context.PaymentStatuses
                        where ps.PaymentStatusId == paymentStatusId
                        select ps;
            var paymentStatus = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, paymentStatus);
            }
            return paymentStatus;
        }

        /// <summary>
        /// Gets all payment statuses
        /// </summary>
        /// <returns>Payment status collection</returns>
        public List<PaymentStatus> GetAllPaymentStatuses()
        {
            string key = string.Format(PAYMENTSTATUSES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<PaymentStatus>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from ps in context.PaymentStatuses
                        orderby ps.PaymentStatusId
                        select ps;
            var paymentStatuses = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, paymentStatuses);
            }
            return paymentStatuses;
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.PaymentStatusManager.CacheEnabled");
            }
        }
        #endregion
    }
}
