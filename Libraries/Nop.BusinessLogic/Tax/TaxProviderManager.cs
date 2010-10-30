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

namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax provider manager
    /// </summary>
    public partial class TaxProviderManager : ITaxProviderManager
    {
        #region Constants
        private const string TAXPROVIDERS_ALL_KEY = "Nop.taxprovider.all";
        private const string TAXPROVIDERS_BY_ID_KEY = "Nop.taxprovider.id-{0}";
        private const string TAXPROVIDERS_PATTERN_KEY = "Nop.taxprovider.";
        #endregion

        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public TaxProviderManager(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a tax provider
        /// </summary>
        /// <param name="taxProviderId">Tax provider identifier</param>
        public void DeleteTaxProvider(int taxProviderId)
        {
            var taxProvider = GetTaxProviderById(taxProviderId);
            if (taxProvider == null)
                return;

            
            if (!_context.IsAttached(taxProvider))
                _context.TaxProviders.Attach(taxProvider);
            _context.DeleteObject(taxProvider);
            _context.SaveChanges();
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a tax provider
        /// </summary>
        /// <param name="taxProviderId">Tax provider identifier</param>
        /// <returns>Tax provider</returns>
        public TaxProvider GetTaxProviderById(int taxProviderId)
        {
            if (taxProviderId == 0)
                return null;

            string key = string.Format(TAXPROVIDERS_BY_ID_KEY, taxProviderId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (TaxProvider)obj2;
            }

            
            var query = from tp in _context.TaxProviders
                        where tp.TaxProviderId == taxProviderId
                        select tp;
            var taxProvider = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, taxProvider);
            }
            return taxProvider;
        }

        /// <summary>
        /// Gets all tax providers
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        public List<TaxProvider> GetAllTaxProviders()
        {
            string key = string.Format(TAXPROVIDERS_ALL_KEY);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<TaxProvider>)obj2;
            }

            
            var query = from tp in _context.TaxProviders
                        orderby tp.DisplayOrder
                        select tp;
            var taxProviders = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, taxProviders);
            }
            return taxProviders;
        }

        /// <summary>
        /// Inserts a tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        public void InsertTaxProvider(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException("taxProvider");
            
            taxProvider.Name = CommonHelper.EnsureNotNull(taxProvider.Name);
            taxProvider.Name = CommonHelper.EnsureMaximumLength(taxProvider.Name, 100);
            taxProvider.Description = CommonHelper.EnsureNotNull(taxProvider.Description);
            taxProvider.Description = CommonHelper.EnsureMaximumLength(taxProvider.Description, 4000);
            taxProvider.ConfigureTemplatePath = CommonHelper.EnsureNotNull(taxProvider.ConfigureTemplatePath);
            taxProvider.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(taxProvider.ConfigureTemplatePath, 500);
            taxProvider.ClassName = CommonHelper.EnsureNotNull(taxProvider.ClassName); 
            taxProvider.ClassName = CommonHelper.EnsureMaximumLength(taxProvider.ClassName, 500);

            
            
            _context.TaxProviders.AddObject(taxProvider);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        public void UpdateTaxProvider(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException("taxProvider");

            taxProvider.Name = CommonHelper.EnsureNotNull(taxProvider.Name);
            taxProvider.Name = CommonHelper.EnsureMaximumLength(taxProvider.Name, 100);
            taxProvider.Description = CommonHelper.EnsureNotNull(taxProvider.Description);
            taxProvider.Description = CommonHelper.EnsureMaximumLength(taxProvider.Description, 4000);
            taxProvider.ConfigureTemplatePath = CommonHelper.EnsureNotNull(taxProvider.ConfigureTemplatePath);
            taxProvider.ConfigureTemplatePath = CommonHelper.EnsureMaximumLength(taxProvider.ConfigureTemplatePath, 500);
            taxProvider.ClassName = CommonHelper.EnsureNotNull(taxProvider.ClassName);
            taxProvider.ClassName = CommonHelper.EnsureMaximumLength(taxProvider.ClassName, 500);

            
            if (!_context.IsAttached(taxProvider))
                _context.TaxProviders.Attach(taxProvider);

            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
            }
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.TaxProviderManager.CacheEnabled");
            }
        }
        #endregion
    }
}
