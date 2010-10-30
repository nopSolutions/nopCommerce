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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping method manager
    /// </summary>
    public partial class ShippingMethodManager : IShippingMethodManager
    {
        #region Constants
        private const string SHIPPINGMETHODS_BY_ID_KEY = "Nop.shippingMethod.id-{0}";
        private const string SHIPPINGMETHODS_PATTERN_KEY = "Nop.shippingMethod.";
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
        public ShippingMethodManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        public void DeleteShippingMethod(int shippingMethodId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;

            
            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            _context.DeleteObject(shippingMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>Shipping method</returns>
        public ShippingMethod GetShippingMethodById(int shippingMethodId)
        {
            if (shippingMethodId == 0)
                return null;

            string key = string.Format(SHIPPINGMETHODS_BY_ID_KEY, shippingMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingMethod)obj2;
            }

            
            var query = from sm in _context.ShippingMethods
                        where sm.ShippingMethodId == shippingMethodId
                        select sm;
            var shippingMethod = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingMethod);
            }
            return shippingMethod;
        }

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <returns>Shipping method collection</returns>
        public List<ShippingMethod> GetAllShippingMethods()
        {
            return GetAllShippingMethods(null);
        }

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country indentifier</param>
        /// <returns>Shipping method collection</returns>
        public List<ShippingMethod> GetAllShippingMethods(int? filterByCountryId)
        {
            
            var shippingMethods = _context.Sp_ShippingMethodLoadAll(filterByCountryId).ToList();
            return shippingMethods;
        }

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public void InsertShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            shippingMethod.Name = CommonHelper.EnsureNotNull(shippingMethod.Name);
            shippingMethod.Name = CommonHelper.EnsureMaximumLength(shippingMethod.Name, 100);
            shippingMethod.Description = CommonHelper.EnsureNotNull(shippingMethod.Description);
            shippingMethod.Description = CommonHelper.EnsureMaximumLength(shippingMethod.Description, 2000);

            

            _context.ShippingMethods.AddObject(shippingMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        public void UpdateShippingMethod(ShippingMethod shippingMethod)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException("shippingMethod");

            shippingMethod.Name = CommonHelper.EnsureNotNull(shippingMethod.Name);
            shippingMethod.Name = CommonHelper.EnsureMaximumLength(shippingMethod.Name, 100);
            shippingMethod.Description = CommonHelper.EnsureNotNull(shippingMethod.Description);
            shippingMethod.Description = CommonHelper.EnsureMaximumLength(shippingMethod.Description, 2000);

            
            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Creates the shipping method country mapping
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void CreateShippingMethodCountryMapping(int shippingMethodId, int countryId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;

            
            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedShippingMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedShippingMethods);

            country.NpRestrictedShippingMethods.Add(shippingMethod);
            _context.SaveChanges();
        }

        /// <summary>
        /// Checking whether the shipping method country mapping exists
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <returns>True if mapping exist, otherwise false</returns>
        public bool DoesShippingMethodCountryMappingExist(int shippingMethodId, int countryId)
        {
            

            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return false;

            //ensure that navigation property is loaded
            if (shippingMethod.NpRestrictedCountries == null)
                _context.LoadProperty(shippingMethod, sm => sm.NpRestrictedCountries);

            bool result = shippingMethod.NpRestrictedCountries.ToList().Find(c => c.CountryId == countryId) != null;
            return result;

            //var query = from sm in _context.ShippingMethods
            //            from c in sm.NpRestrictedCountries
            //            where sm.ShippingMethodId == shippingMethodId &&
            //            c.CountryId == countryId
            //            select sm;

            //bool result = query.Count() > 0;
            //return result;
        }

        /// <summary>
        /// Deletes the shipping method country mapping
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">The country identifier</param>
        public void DeleteShippingMethodCountryMapping(int shippingMethodId, int countryId)
        {
            var shippingMethod = GetShippingMethodById(shippingMethodId);
            if (shippingMethod == null)
                return;

            var country = IoCFactory.Resolve<ICountryManager>().GetCountryById(countryId);
            if (country == null)
                return;

            
            if (!_context.IsAttached(shippingMethod))
                _context.ShippingMethods.Attach(shippingMethod);
            if (!_context.IsAttached(country))
                _context.Countries.Attach(country);

            //ensure that navigation property is loaded
            if (country.NpRestrictedShippingMethods == null)
                _context.LoadProperty(country, c => c.NpRestrictedShippingMethods);

            country.NpRestrictedShippingMethods.Remove(shippingMethod);
            _context.SaveChanges();
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ShippingMethodManager.CacheEnabled");
            }
        }
        #endregion
    }
}
