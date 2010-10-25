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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax category manager
    /// </summary>
    public partial class TaxCategoryManager : ITaxCategoryManager
    {
        #region Constants
        private const string TAXCATEGORIES_ALL_KEY = "Nop.taxcategory.all";
        private const string TAXCATEGORIES_BY_ID_KEY = "Nop.taxcategory.id-{0}";
        private const string TAXCATEGORIES_PATTERN_KEY = "Nop.taxcategory.";
        #endregion
        
        #region Methods
        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategoryId">The tax category identifier</param>
        public void DeleteTaxCategory(int taxCategoryId)
        {
            var taxCategory = GetTaxCategoryById(taxCategoryId);
            if (taxCategory == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(taxCategory))
                context.TaxCategories.Attach(taxCategory);
            context.DeleteObject(taxCategory);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>Tax category collection</returns>
        public List<TaxCategory> GetAllTaxCategories()
        {
            string key = string.Format(TAXCATEGORIES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<TaxCategory>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tc in context.TaxCategories
                        orderby tc.DisplayOrder
                        select tc;
            var taxCategories = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, taxCategories);
            }
            return taxCategories;
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
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (TaxCategory)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from tc in context.TaxCategories
                        where tc.TaxCategoryId == taxCategoryId
                        select tc;
            var taxCategory = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, taxCategory);
            }
            return taxCategory;
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public void InsertTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            taxCategory.Name = CommonHelper.EnsureNotNull(taxCategory.Name);
            taxCategory.Name = CommonHelper.EnsureMaximumLength(taxCategory.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;

            context.TaxCategories.AddObject(taxCategory);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        public void UpdateTaxCategory(TaxCategory taxCategory)
        {
            if (taxCategory == null)
                throw new ArgumentNullException("taxCategory");

            taxCategory.Name = CommonHelper.EnsureNotNull(taxCategory.Name);
            taxCategory.Name = CommonHelper.EnsureMaximumLength(taxCategory.Name, 100);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(taxCategory))
                context.TaxCategories.Attach(taxCategory);

            context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(TAXCATEGORIES_PATTERN_KEY);
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.TaxCategoryManager.CacheEnabled");
            }
        }
        #endregion
    }
}
