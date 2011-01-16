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
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Constants
        private const string TAXCATEGORIES_ALL_KEY = "Nop.taxcategory.all";
        private const string TAXCATEGORIES_BY_ID_KEY = "Nop.taxcategory.id-{0}";
        private const string TAXCATEGORIES_PATTERN_KEY = "Nop.taxcategory.";
        #endregion

        #region Fields

        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxCategoryRepository">Tax category repository</param>
        public TaxCategoryService(ICacheManager cacheManager,
            IRepository<TaxCategory> taxCategoryRepository)
        {
            this._cacheManager = cacheManager;
            this._taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public void DeleteTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                return;

            _taxCategoryRepository.Delete(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax category collection</returns>
        public IList<TaxCategory> GetAllTaxCategories()
        {
            string key = string.Format(TAXCATEGORIES_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from tc in _taxCategoryRepository.Table
                            orderby tc.DisplayOrder
                            select tc;
                var taxCategories = query.ToList();
                return taxCategories;
            });
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>Tax category</returns>
        public TaxCategory GetTaxCategoryById(int taxCategoryId)
        {
            if (taxCategoryId == 0)
                return null;

            string key = string.Format(TAXCATEGORIES_BY_ID_KEY, taxCategoryId);
            return _cacheManager.Get(key, () =>
            {
                var taxCategory = _taxCategoryRepository.GetById(taxCategoryId);
                return taxCategory;
            });
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public void InsertTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            _taxCategoryRepository.Insert(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public void UpdateTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            _taxCategoryRepository.Update(taxCategory);

            _cacheManager.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
        }
        #endregion
    }
}
