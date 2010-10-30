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

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping status manager
    /// </summary>
    public partial class ShippingStatusManager : IShippingStatusManager
    {
        #region Constants
        private const string SHIPPINGTATUSES_ALL_KEY = "Nop.shippingstatus.all";
        private const string SHIPPINGTATUSES_BY_ID_KEY = "Nop.shippingstatus.id-{0}";
        private const string SHIPPINGTATUSES_PATTERN_KEY = "Nop.shippingstatus.";
        #endregion

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public ShippingStatusManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a shipping status full name
        /// </summary>
        /// <param name="shippingStatusId">Shipping status identifier</param>
        /// <returns>Shipping status name</returns>
        public string GetShippingStatusName(int shippingStatusId)
        {
            var shippingStatus = GetShippingStatusById(shippingStatusId);
            if (shippingStatus != null)
            {
                string name = string.Empty;
                if (NopContext.Current != null)
                {
                    name = LocalizationManager.GetLocaleResourceString(string.Format("ShippingStatus.{0}", (ShippingStatusEnum)shippingStatus.ShippingStatusId), NopContext.Current.WorkingLanguage.LanguageId, true, shippingStatus.Name);
                }
                else
                {
                    name = shippingStatus.Name;
                }
                return name;
            }
            else
            {
                return ((ShippingStatusEnum)shippingStatusId).ToString();
            }
        }

        /// <summary>
        /// Gets a shipping status by identifier
        /// </summary>
        /// <param name="shippingStatusId">Shipping status identifier</param>
        /// <returns>Shipping status</returns>
        public ShippingStatus GetShippingStatusById(int shippingStatusId)
        {
            if (shippingStatusId == 0)
                return null;

            string key = string.Format(SHIPPINGTATUSES_BY_ID_KEY, shippingStatusId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingStatus)obj2;
            }

            
            var query = from ss in _context.ShippingStatuses
                        where ss.ShippingStatusId == shippingStatusId
                        select ss;
            var shippingStatus = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingStatus);
            }
            return shippingStatus;
        }

        /// <summary>
        /// Gets all shipping statuses
        /// </summary>
        /// <returns>Shipping status collection</returns>
        public List<ShippingStatus> GetAllShippingStatuses()
        {
            string key = string.Format(SHIPPINGTATUSES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ShippingStatus>)obj2;
            }

            
            var query = from ss in _context.ShippingStatuses
                        orderby ss.ShippingStatusId
                        select ss;
            var shippingStatuses = query.ToList();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingStatuses);
            }
            return shippingStatuses;
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ShippingStatusManager.CacheEnabled");
            }
        }
        #endregion
    }
}
