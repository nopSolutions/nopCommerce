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
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Shipping
{
    /// <summary>
    /// Shipping rate computation method manager
    /// </summary>
    public partial class ShippingRateComputationMethodManager : IShippingRateComputationMethodManager
    {
        #region Constants
        private const string SHIPPINGRATECOMPUTATIONMETHODS_ALL_KEY = "Nop.shippingratecomputationmethod.all-{0}";
        private const string SHIPPINGRATECOMPUTATIONMETHODS_BY_ID_KEY = "Nop.shippingratecomputationmethod.id-{0}";
        private const string SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY = "Nop.shippingratecomputationmethod.";
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
        public ShippingRateComputationMethodManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        public void DeleteShippingRateComputationMethod(int shippingRateComputationMethodId)
        {
            var shippingRateComputationMethod = GetShippingRateComputationMethodById(shippingRateComputationMethodId);
            if (shippingRateComputationMethod == null)
                return;

            
            if (!_context.IsAttached(shippingRateComputationMethod))
                _context.ShippingRateComputationMethods.Attach(shippingRateComputationMethod);
            _context.DeleteObject(shippingRateComputationMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethodId">Shipping rate computation method identifier</param>
        /// <returns>Shipping rate computation method</returns>
        public ShippingRateComputationMethod GetShippingRateComputationMethodById(int shippingRateComputationMethodId)
        {
            if (shippingRateComputationMethodId == 0)
                return null;

            string key = string.Format(SHIPPINGRATECOMPUTATIONMETHODS_BY_ID_KEY, shippingRateComputationMethodId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ShippingRateComputationMethod)obj2;
            }

            
            var query = from s in _context.ShippingRateComputationMethods
                        where s.ShippingRateComputationMethodId == shippingRateComputationMethodId
                        select s;
            var shippingRateComputationMethod = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingRateComputationMethod);
            }
            return shippingRateComputationMethod;
        }

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        public List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllShippingRateComputationMethods(showHidden);
        }

        /// <summary>
        /// Gets all shipping rate computation methods
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Shipping rate computation method collection</returns>
        public List<ShippingRateComputationMethod> GetAllShippingRateComputationMethods(bool showHidden)
        {
            string key = string.Format(SHIPPINGRATECOMPUTATIONMETHODS_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ShippingRateComputationMethod>)obj2;
            }

            
            var query = from s in _context.ShippingRateComputationMethods
                        orderby s.DisplayOrder
                        where showHidden || s.IsActive
                        select s;
            var shippingRateComputationMethods = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, shippingRateComputationMethods);
            }
            return shippingRateComputationMethods;
        }

        /// <summary>
        /// Inserts a shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        public void InsertShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod)
        {
            if (shippingRateComputationMethod == null)
                throw new ArgumentNullException("shippingRateComputationMethod");
                        
            shippingRateComputationMethod.Name = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Name);
            shippingRateComputationMethod.Name = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Name, 100);
            shippingRateComputationMethod.Description = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Description);
            shippingRateComputationMethod.Description = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Description, 4000);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ConfigureTemplatePath);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ConfigureTemplatePath, 500);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ClassName);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ClassName, 500);

            

            _context.ShippingRateComputationMethods.AddObject(shippingRateComputationMethod);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the shipping rate computation method
        /// </summary>
        /// <param name="shippingRateComputationMethod">Shipping rate computation method</param>
        public void UpdateShippingRateComputationMethod(ShippingRateComputationMethod shippingRateComputationMethod)
        {
            if (shippingRateComputationMethod == null)
                throw new ArgumentNullException("shippingRateComputationMethod");

            shippingRateComputationMethod.Name = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Name);
            shippingRateComputationMethod.Name = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Name, 100);
            shippingRateComputationMethod.Description = CommonHelper.EnsureNotNull(shippingRateComputationMethod.Description);
            shippingRateComputationMethod.Description = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.Description, 4000);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ConfigureTemplatePath);
            shippingRateComputationMethod.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ConfigureTemplatePath, 500);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureNotNull(shippingRateComputationMethod.ClassName);
            shippingRateComputationMethod.ClassName = CommonHelper.EnsureMaximumLength(shippingRateComputationMethod.ClassName, 500);

            
            if (!_context.IsAttached(shippingRateComputationMethod))
                _context.ShippingRateComputationMethods.Attach(shippingRateComputationMethod);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(SHIPPINGRATECOMPUTATIONMETHODS_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <param name="shippingRateComputationMethodId">The shipping rate computation method identifier</param>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum GetShippingRateComputationMethodTypeEnum(int shippingRateComputationMethodId)
        {
            var method = GetShippingRateComputationMethodById(shippingRateComputationMethodId);
            if (method == null)
                return ShippingRateComputationMethodTypeEnum.Unknown;
            var iMethod = Activator.CreateInstance(Type.GetType(method.ClassName)) as IShippingRateComputationMethod;
            return iMethod.ShippingRateComputationMethodType;
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ShippingRateComputationMethodManager.CacheEnabled");
            }
        }

        #endregion
    }
}