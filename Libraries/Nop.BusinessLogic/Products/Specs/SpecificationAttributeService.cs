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
using System.Linq;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Specs
{
    /// <summary>
    /// Specification attribute service
    /// </summary>
    public partial class SpecificationAttributeService : ISpecificationAttributeService
    {
        #region Constants
        private const string SPECIFICATIONATTRIBUTE_BY_ID_KEY = "Nop.specificationattributes.id-{0}";
        private const string SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY = "Nop.specificationattributeoptions.id-{0}";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY = "Nop.productspecificationattribute.allbyproductid-{0}-{1}-{2}";
        private const string SPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.specificationattributes.";
        private const string SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY = "Nop.specificationattributeoptions.";
        private const string PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY = "Nop.productspecificationattribute.";
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
        public SpecificationAttributeService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        #region Specification attribute
        
        /// <summary>
        /// Gets a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute</returns>
        public SpecificationAttribute GetSpecificationAttributeById(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTE_BY_ID_KEY, specificationAttributeId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (SpecificationAttribute)obj2;
            }

            
            var query = from sa in _context.SpecificationAttributes
                        where sa.SpecificationAttributeId == specificationAttributeId
                        select sa;
            var specificationAttribute = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, specificationAttribute);
            }
            return specificationAttribute;
        }

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <returns>Specification attribute collection</returns>
        public List<SpecificationAttribute> GetSpecificationAttributes()
        {
            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;
            return GetSpecificationAttributes(languageId);
        }

        /// <summary>
        /// Gets specification attribute collection
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute collection</returns>
        public List<SpecificationAttribute> GetSpecificationAttributes(int languageId)
        {
            
            var query = from sa in _context.SpecificationAttributes
                        orderby sa.DisplayOrder
                        select sa;
            var specificationAttributes = query.ToList();
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        public void DeleteSpecificationAttribute(int specificationAttributeId)
        {
            var specificationAttribute = GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
                return;

            
            if (!_context.IsAttached(specificationAttribute))
                _context.SpecificationAttributes.Attach(specificationAttribute);
            _context.DeleteObject(specificationAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("activityLogType");

            specificationAttribute.Name = CommonHelper.EnsureNotNull(specificationAttribute.Name);
            specificationAttribute.Name = CommonHelper.EnsureMaximumLength(specificationAttribute.Name, 100);

            

            _context.SpecificationAttributes.AddObject(specificationAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("activityLogType");

            specificationAttribute.Name = CommonHelper.EnsureNotNull(specificationAttribute.Name);
            specificationAttribute.Name = CommonHelper.EnsureMaximumLength(specificationAttribute.Name, 100);

            
            if (!_context.IsAttached(specificationAttribute))
                _context.SpecificationAttributes.Attach(specificationAttribute);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized specification attribute by id
        /// </summary>
        /// <param name="specificationAttributeLocalizedId">Localized specification identifier</param>
        /// <returns>Specification attribute content</returns>
        public SpecificationAttributeLocalized GetSpecificationAttributeLocalizedById(int specificationAttributeLocalizedId)
        {
            if (specificationAttributeLocalizedId == 0)
                return null;

            
            var query = from sal in _context.SpecificationAttributeLocalized
                        where sal.SpecificationAttributeLocalizedId == specificationAttributeLocalizedId
                        select sal;
            var specificationAttributeLocalized = query.SingleOrDefault();
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <returns>Secification attribute content</returns>
        public List<SpecificationAttributeLocalized> GetSpecificationAttributeLocalizedBySpecificationAttributeId(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return new List<SpecificationAttributeLocalized>();

            
            var query = from sal in _context.SpecificationAttributeLocalized
                        where sal.SpecificationAttributeId == specificationAttributeId
                        select sal;
            var content = query.ToList();
            return content;
        }
        
        /// <summary>
        /// Gets localized specification attribute by specification attribute id and language id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Specification attribute content</returns>
        public SpecificationAttributeLocalized GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(int specificationAttributeId, int languageId)
        {
            if (specificationAttributeId == 0 || languageId == 0)
                return null;

            
            var query = from sal in _context.SpecificationAttributeLocalized
                        orderby sal.SpecificationAttributeLocalizedId
                        where sal.SpecificationAttributeId == specificationAttributeId &&
                        sal.LanguageId == languageId
                        select sal;
            var specificationAttributeLocalized = query.FirstOrDefault();
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeLocalized">Localized specification attribute</param>
        public void InsertSpecificationAttributeLocalized(SpecificationAttributeLocalized specificationAttributeLocalized)
        {
            if (specificationAttributeLocalized == null)
                throw new ArgumentNullException("specificationAttributeLocalized");

            specificationAttributeLocalized.Name = CommonHelper.EnsureNotNull(specificationAttributeLocalized.Name);
            specificationAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(specificationAttributeLocalized.Name, 100);

            
            
            _context.SpecificationAttributeLocalized.AddObject(specificationAttributeLocalized);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized specification attribute
        /// </summary>
        /// <param name="specificationAttributeLocalized">Localized specification attribute</param>
        public void UpdateSpecificationAttributeLocalized(SpecificationAttributeLocalized specificationAttributeLocalized)
        {
            if (specificationAttributeLocalized == null)
                throw new ArgumentNullException("specificationAttributeLocalized");

            specificationAttributeLocalized.Name = CommonHelper.EnsureNotNull(specificationAttributeLocalized.Name);
            specificationAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(specificationAttributeLocalized.Name, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(specificationAttributeLocalized.Name);

            
            if (!_context.IsAttached(specificationAttributeLocalized))
                _context.SpecificationAttributeLocalized.Attach(specificationAttributeLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(specificationAttributeLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }
        
        #endregion

        #region Specification attribute option

        /// <summary>
        /// Gets a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        /// <returns>Specification attribute option</returns>
        public SpecificationAttributeOption GetSpecificationAttributeOptionById(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return null;

            string key = string.Format(SPECIFICATIONATTRIBUTEOPTION_BY_ID_KEY, specificationAttributeOptionId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (SpecificationAttributeOption)obj2;
            }

            
            var query = from sao in _context.SpecificationAttributeOptions
                        where sao.SpecificationAttributeOptionId == specificationAttributeOptionId
                        select sao;
            var specificationAttributeOption = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, specificationAttributeOption);
            }
            return specificationAttributeOption;
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public List<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            
            var query = from sao in _context.SpecificationAttributeOptions
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionId">The specification attribute option identifier</param>
        public void DeleteSpecificationAttributeOption(int specificationAttributeOptionId)
        {
            var specificationAttributeOption = GetSpecificationAttributeOptionById(specificationAttributeOptionId);
            if (specificationAttributeOption == null)
                return;

            
            if (!_context.IsAttached(specificationAttributeOption))
                _context.SpecificationAttributeOptions.Attach(specificationAttributeOption);
            _context.DeleteObject(specificationAttributeOption);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            specificationAttributeOption.Name = CommonHelper.EnsureNotNull(specificationAttributeOption.Name);
            specificationAttributeOption.Name = CommonHelper.EnsureMaximumLength(specificationAttributeOption.Name, 500);

            

            _context.SpecificationAttributeOptions.AddObject(specificationAttributeOption);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public void UpdateSpecificationAttributeOptions(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            specificationAttributeOption.Name = CommonHelper.EnsureNotNull(specificationAttributeOption.Name);
            specificationAttributeOption.Name = CommonHelper.EnsureMaximumLength(specificationAttributeOption.Name, 500);
            
            
            if (!_context.IsAttached(specificationAttributeOption))
                _context.SpecificationAttributeOptions.Attach(specificationAttributeOption);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized specification attribute option by id
        /// </summary>
        /// <param name="specificationAttributeOptionLocalizedId">Localized specification attribute option identifier</param>
        /// <returns>Localized specification attribute option</returns>
        public SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedById(int specificationAttributeOptionLocalizedId)
        {
            if (specificationAttributeOptionLocalizedId == 0)
                return null;

            
            var query = from saol in _context.SpecificationAttributeOptionLocalized
                        where saol.SpecificationAttributeOptionLocalizedId == specificationAttributeOptionLocalizedId
                        select saol;
            var specificationAttributeOptionLocalized = query.SingleOrDefault();
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute option by category id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <returns>Localized specification attribute option content</returns>
        public List<SpecificationAttributeOptionLocalized> GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionId(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return new List<SpecificationAttributeOptionLocalized>();

            
            var query = from saol in _context.SpecificationAttributeOptionLocalized
                        where saol.SpecificationAttributeOptionId == specificationAttributeOptionId
                        select saol;
            var content = query.ToList();
            return content;
        }


        /// <summary>
        /// Gets localized specification attribute option by specification attribute option id and language id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized specification attribute option</returns>
        public SpecificationAttributeOptionLocalized GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(int specificationAttributeOptionId, int languageId)
        {
            if (specificationAttributeOptionId == 0 || languageId == 0)
                return null;

            
            var query = from saol in _context.SpecificationAttributeOptionLocalized
                        orderby saol.SpecificationAttributeOptionLocalizedId
                        where saol.SpecificationAttributeOptionId == specificationAttributeOptionId &&
                        saol.LanguageId == languageId
                        select saol;
            var specificationAttributeOptionLocalized = query.FirstOrDefault();
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionLocalized">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        public void InsertSpecificationAttributeOptionLocalized(SpecificationAttributeOptionLocalized specificationAttributeOptionLocalized)
        {
            if (specificationAttributeOptionLocalized == null)
                throw new ArgumentNullException("specificationAttributeOptionLocalized");

            specificationAttributeOptionLocalized.Name = CommonHelper.EnsureNotNull(specificationAttributeOptionLocalized.Name);
            specificationAttributeOptionLocalized.Name = CommonHelper.EnsureMaximumLength(specificationAttributeOptionLocalized.Name, 500);

            

            _context.SpecificationAttributeOptionLocalized.AddObject(specificationAttributeOptionLocalized);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOptionLocalized">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        public void UpdateSpecificationAttributeOptionLocalized(SpecificationAttributeOptionLocalized specificationAttributeOptionLocalized)
        {
            if (specificationAttributeOptionLocalized == null)
                throw new ArgumentNullException("specificationAttributeOptionLocalized");

            specificationAttributeOptionLocalized.Name = CommonHelper.EnsureNotNull(specificationAttributeOptionLocalized.Name);
            specificationAttributeOptionLocalized.Name = CommonHelper.EnsureMaximumLength(specificationAttributeOptionLocalized.Name, 500);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(specificationAttributeOptionLocalized.Name);

            
            if (!_context.IsAttached(specificationAttributeOptionLocalized))
                _context.SpecificationAttributeOptionLocalized.Attach(specificationAttributeOptionLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(specificationAttributeOptionLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }
        
        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute identifier</param>
        public void DeleteProductSpecificationAttribute(int productSpecificationAttributeId)
        {
            var productSpecificationAttribute = GetProductSpecificationAttributeById(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
                return;

            
            if (!_context.IsAttached(productSpecificationAttribute))
                _context.ProductSpecificationAttributes.Attach(productSpecificationAttribute);
            _context.DeleteObject(productSpecificationAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId)
        {
            return GetProductSpecificationAttributesByProductId(productId, null, null);
        }

        /// <summary>
        /// Gets a product specification attribute mapping collection
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="allowFiltering">0 to load attributes with AllowFiltering set to false, 0 to load attributes with AllowFiltering set to true, null to load all attributes</param>
        /// <param name="showOnProductPage">0 to load attributes with ShowOnProductPage set to false, 0 to load attributes with ShowOnProductPage set to true, null to load all attributes</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public List<ProductSpecificationAttribute> GetProductSpecificationAttributesByProductId(int productId, 
            bool? allowFiltering, bool? showOnProductPage)
        {
            string allowFilteringCacheStr = "null";
            if (allowFiltering.HasValue)
                allowFilteringCacheStr = allowFiltering.ToString();
            string showOnProductPageCacheStr = "null";
            if (showOnProductPage.HasValue)
                showOnProductPageCacheStr = showOnProductPage.ToString();
            string key = string.Format(PRODUCTSPECIFICATIONATTRIBUTE_ALLBYPRODUCTID_KEY, productId, allowFilteringCacheStr, showOnProductPageCacheStr);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductSpecificationAttribute>)obj2;
            }

            
            var query = (IQueryable<ProductSpecificationAttribute>)_context.ProductSpecificationAttributes;
            query = query.Where(psa => psa.ProductId == productId);
            if (allowFiltering.HasValue)
                query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
            if (showOnProductPage.HasValue)
                query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
            query = query.OrderBy(psa => psa.DisplayOrder);

            var productSpecificationAttributes = query.ToList();
            
            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, productSpecificationAttributes);
            }
            return productSpecificationAttributes;
        }

        /// <summary>
        /// Gets a product specification attribute mapping 
        /// </summary>
        /// <param name="productSpecificationAttributeId">Product specification attribute mapping identifier</param>
        /// <returns>Product specification attribute mapping</returns>
        public ProductSpecificationAttribute GetProductSpecificationAttributeById(int productSpecificationAttributeId)
        {
            if (productSpecificationAttributeId == 0)
                return null;

            
            var query = from psa in _context.ProductSpecificationAttributes
                        where psa.ProductSpecificationAttributeId == productSpecificationAttributeId
                        select psa;
            var productSpecificationAttribute = query.SingleOrDefault();
            return productSpecificationAttribute;
        }

        /// <summary>
        /// Inserts a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public void InsertProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            
            
            _context.ProductSpecificationAttributes.AddObject(productSpecificationAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            
            if (!_context.IsAttached(productSpecificationAttribute))
                _context.ProductSpecificationAttributes.Attach(productSpecificationAttribute);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
                _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
            }
        }

        #endregion

        #region Specification attribute option filter
        
        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId)
        {
            int languageId = 0;
            if (NopContext.Current != null)
                languageId = NopContext.Current.WorkingLanguage.LanguageId;
            return GetSpecificationAttributeOptionFilter(categoryId, languageId);
        }

        /// <summary>
        /// Gets a filtered product specification attribute mapping collection by category id
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product specification attribute mapping collection</returns>
        public List<SpecificationAttributeOptionFilter> GetSpecificationAttributeOptionFilter(int categoryId, int languageId)
        {
            
            var result = _context.Sp_SpecificationAttributeOptionFilter_LoadByFilter(categoryId, languageId).ToList();
            return result;
        }
        
        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.SpecificationAttributeManager.CacheEnabled");
            }
        }
        #endregion
    }
}
