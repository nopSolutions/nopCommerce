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
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Tax;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax provider service
    /// </summary>
    public partial class TaxProviderService : ITaxProviderService
    {
        #region Constants
        private const string TAXPROVIDERS_ALL_KEY = "Nop.taxprovider.all";
        private const string TAXPROVIDERS_BY_ID_KEY = "Nop.taxprovider.id-{0}";
        private const string TAXPROVIDERS_PATTERN_KEY = "Nop.taxprovider.";
        #endregion

        #region Fields

        private readonly IRepository<TaxProvider> _taxProviderRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxProviderRepository">Tax provider repository</param>
        public TaxProviderService(ICacheManager cacheManager,
            IRepository<TaxProvider> taxProviderRepository)
        {
            this._cacheManager = cacheManager;
            this._taxProviderRepository = taxProviderRepository;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Deletes a tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        public void DeleteTaxProvider(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                return;

            _taxProviderRepository.Delete(taxProvider);

            _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
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
            return _cacheManager.Get(key, () =>
            {
                var taxProvider = _taxProviderRepository.GetById(taxProviderId);
                return taxProvider;
            });
        }

        /// <summary>
        /// Gets all tax providers
        /// </summary>
        /// <returns>Shipping rate computation method collection</returns>
        public IList<TaxProvider> GetAllTaxProviders()
        {
            string key = string.Format(TAXPROVIDERS_ALL_KEY);

            return _cacheManager.Get(key, () =>
            {
                var query = from tp in _taxProviderRepository.Table
                            orderby tp.DisplayOrder
                            select tp;
                var taxProviders = query.ToList();
                return taxProviders;
            });
        }

        /// <summary>
        /// Inserts a tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        public void InsertTaxProvider(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException("taxProvider");

            _taxProviderRepository.Insert(taxProvider);

            _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the tax provider
        /// </summary>
        /// <param name="taxProvider">Tax provider</param>
        public void UpdateTaxProvider(TaxProvider taxProvider)
        {
            if (taxProvider == null)
                throw new ArgumentNullException("taxProvider");

            _taxProviderRepository.Update(taxProvider);

            _cacheManager.RemoveByPattern(TAXPROVIDERS_PATTERN_KEY);
        }
        #endregion
    }
}
