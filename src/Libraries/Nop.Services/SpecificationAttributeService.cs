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
using Nop.Core.Domain;
using Nop.Data;

namespace Nop.Services
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

        private readonly IRepository<SpecificationAttribute> _specificationAttributeRespository;
        private readonly IRepository<LocalizedSpecificationAttribute> _localizedSpecificationAttributeRespository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRespository;
        private readonly IRepository<LocalizedSpecificationAttributeOption> _localizedSpecificationAttributeOptionRespository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="specificationAttributeRespository">Specification attribute repository</param>
        /// <param name="localizedSpecificationAttributeRespository">localized specification attribute repository</param>
        /// <param name="specificationAttributeOptionRespository">Specification attribute option repository</param>
        /// <param name="localizedSpecificationAttributeOptionRespository">Localized specification attribute option repository</param>
        /// <param name="productSpecificationAttributeRespository">Product specification attribute repository</param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            IRepository<SpecificationAttribute> specificationAttributeRespository,
            IRepository<LocalizedSpecificationAttribute> localizedSpecificationAttributeRespository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRespository,
            IRepository<LocalizedSpecificationAttributeOption> localizedSpecificationAttributeOptionRespository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRespository)
        {
            this._cacheManager = cacheManager;
            this._specificationAttributeRespository = specificationAttributeRespository;
            this._localizedSpecificationAttributeRespository = localizedSpecificationAttributeRespository;
            this._specificationAttributeOptionRespository = specificationAttributeOptionRespository;
            this._localizedSpecificationAttributeOptionRespository = localizedSpecificationAttributeOptionRespository;
            this._productSpecificationAttributeRespository = productSpecificationAttributeRespository;
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
            return _cacheManager.Get(key, () =>
            {
                return _specificationAttributeRespository.GetById(specificationAttributeId);
            });
        }

        /// <summary>
        /// Gets specification attributes
        /// </summary>
        /// <returns>Specification attributes</returns>
        public List<SpecificationAttribute> GetSpecificationAttributes()
        {
            var query = from sa in _specificationAttributeRespository.Table
                        orderby sa.DisplayOrder
                        select sa;
            var specificationAttributes = query.ToList();
            return specificationAttributes;
        }

        /// <summary>
        /// Deletes a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public void DeleteSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                return;

            _specificationAttributeRespository.Delete(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts a specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public void InsertSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRespository.Insert(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttribute">The specification attribute</param>
        public void UpdateSpecificationAttribute(SpecificationAttribute specificationAttribute)
        {
            if (specificationAttribute == null)
                throw new ArgumentNullException("specificationAttribute");

            _specificationAttributeRespository.Update(specificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Gets localized specification attribute by id
        /// </summary>
        /// <param name="localizedSpecificationAttributeId">Localized specification identifier</param>
        /// <returns>Specification attribute content</returns>
        public LocalizedSpecificationAttribute GetSpecificationAttributeLocalizedById(int localizedSpecificationAttributeId)
        {
            if (localizedSpecificationAttributeId == 0)
                return null;

            var specificationAttributeLocalized = _localizedSpecificationAttributeRespository.GetById(localizedSpecificationAttributeId);
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">Specification attribute identifier</param>
        /// <returns>Secification attribute content</returns>
        public List<LocalizedSpecificationAttribute> GetSpecificationAttributeLocalizedBySpecificationAttributeId(int specificationAttributeId)
        {
            if (specificationAttributeId == 0)
                return new List<LocalizedSpecificationAttribute>();

            var query = from sal in _localizedSpecificationAttributeRespository.Table
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
        public LocalizedSpecificationAttribute GetSpecificationAttributeLocalizedBySpecificationAttributeIdAndLanguageId(int specificationAttributeId, int languageId)
        {
            if (specificationAttributeId == 0 || languageId == 0)
                return null;

            var query = from sal in _localizedSpecificationAttributeRespository.Table
                        orderby sal.Id
                        where sal.SpecificationAttributeId == specificationAttributeId &&
                        sal.LanguageId == languageId
                        select sal;
            var specificationAttributeLocalized = query.FirstOrDefault();
            return specificationAttributeLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute
        /// </summary>
        /// <param name="localizedSpecificationAttribute">Localized specification attribute</param>
        public void InsertSpecificationAttributeLocalized(LocalizedSpecificationAttribute localizedSpecificationAttribute)
        {
            if (localizedSpecificationAttribute == null)
                throw new ArgumentNullException("localizedSpecificationAttribute");

            _localizedSpecificationAttributeRespository.Insert(localizedSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Update a localized specification attribute
        /// </summary>
        /// <param name="localizedSpecificationAttribute">Localized specification attribute</param>
        public void UpdateSpecificationAttributeLocalized(LocalizedSpecificationAttribute localizedSpecificationAttribute)
        {
            if (localizedSpecificationAttribute == null)
                throw new ArgumentNullException("localizedSpecificationAttribute");

            bool allFieldsAreEmpty = string.IsNullOrEmpty(localizedSpecificationAttribute.Name);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _localizedSpecificationAttributeRespository.Delete(localizedSpecificationAttribute);
            }
            else
            {
                _localizedSpecificationAttributeRespository.Update(localizedSpecificationAttribute);
            }

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
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
            return _cacheManager.Get(key, () =>
            {
                return _specificationAttributeOptionRespository.GetById(specificationAttributeOptionId);
            });
        }

        /// <summary>
        /// Gets a specification attribute option by specification attribute id
        /// </summary>
        /// <param name="specificationAttributeId">The specification attribute identifier</param>
        /// <returns>Specification attribute option</returns>
        public List<SpecificationAttributeOption> GetSpecificationAttributeOptionsBySpecificationAttribute(int specificationAttributeId)
        {
            var query = from sao in _specificationAttributeOptionRespository.Table
                        orderby sao.DisplayOrder
                        where sao.SpecificationAttributeId == specificationAttributeId
                        select sao;
            var specificationAttributeOptions = query.ToList();

            return specificationAttributeOptions;
        }

        /// <summary>
        /// Deletes a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public void DeleteSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                return;

            _specificationAttributeOptionRespository.Delete(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Inserts a specification attribute option
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public void InsertSpecificationAttributeOption(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRespository.Insert(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the specification attribute
        /// </summary>
        /// <param name="specificationAttributeOption">The specification attribute option</param>
        public void UpdateSpecificationAttributeOptions(SpecificationAttributeOption specificationAttributeOption)
        {
            if (specificationAttributeOption == null)
                throw new ArgumentNullException("specificationAttributeOption");

            _specificationAttributeOptionRespository.Update(specificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Gets localized specification attribute option by id
        /// </summary>
        /// <param name="localizedSpecificationAttributeOptionId">Localized specification attribute option identifier</param>
        /// <returns>Localized specification attribute option</returns>
        public LocalizedSpecificationAttributeOption GetSpecificationAttributeOptionLocalizedById(int localizedSpecificationAttributeOptionId)
        {
            if (localizedSpecificationAttributeOptionId == 0)
                return null;

            var specificationAttributeOptionLocalized = _localizedSpecificationAttributeOptionRespository.GetById(localizedSpecificationAttributeOptionId);
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Gets localized specification attribute option by category id
        /// </summary>
        /// <param name="specificationAttributeOptionId">Specification attribute option identifier</param>
        /// <returns>Localized specification attribute option content</returns>
        public List<LocalizedSpecificationAttributeOption> GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionId(int specificationAttributeOptionId)
        {
            if (specificationAttributeOptionId == 0)
                return new List<LocalizedSpecificationAttributeOption>();
            
            var query = from saol in _localizedSpecificationAttributeOptionRespository.Table
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
        public LocalizedSpecificationAttributeOption GetSpecificationAttributeOptionLocalizedBySpecificationAttributeOptionIdAndLanguageId(int specificationAttributeOptionId, int languageId)
        {
            if (specificationAttributeOptionId == 0 || languageId == 0)
                return null;

            var query = from saol in _localizedSpecificationAttributeOptionRespository.Table
                        orderby saol.Id
                        where saol.SpecificationAttributeOptionId == specificationAttributeOptionId &&
                        saol.LanguageId == languageId
                        select saol;
            var specificationAttributeOptionLocalized = query.FirstOrDefault();
            return specificationAttributeOptionLocalized;
        }

        /// <summary>
        /// Inserts a localized specification attribute option
        /// </summary>
        /// <param name="localizedSpecificationAttributeOption">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        public void InsertSpecificationAttributeOptionLocalized(LocalizedSpecificationAttributeOption localizedSpecificationAttributeOption)
        {
            if (localizedSpecificationAttributeOption == null)
                throw new ArgumentNullException("localizedSpecificationAttributeOption");

            _localizedSpecificationAttributeOptionRespository.Insert(localizedSpecificationAttributeOption);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Update a localized specification attribute option
        /// </summary>
        /// <param name="localizedSpecificationAttributeOption">Localized specification attribute option</param>
        /// <returns>Localized specification attribute option</returns>
        public void UpdateSpecificationAttributeOptionLocalized(LocalizedSpecificationAttributeOption localizedSpecificationAttributeOption)
        {
            if (localizedSpecificationAttributeOption == null)
                throw new ArgumentNullException("localizedSpecificationAttributeOption");

            bool allFieldsAreEmpty = string.IsNullOrEmpty(localizedSpecificationAttributeOption.Name);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _localizedSpecificationAttributeOptionRespository.Delete(localizedSpecificationAttributeOption);
            }
            else
            {
                _localizedSpecificationAttributeOptionRespository.Update(localizedSpecificationAttributeOption);
            }

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }
        
        #endregion

        #region Product specification attribute

        /// <summary>
        /// Deletes a product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute</param>
        public void DeleteProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                return;

            _productSpecificationAttributeRespository.Delete(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
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
            
            return _cacheManager.Get(key, () =>
            {
                var query = (IQueryable<ProductSpecificationAttribute>)_productSpecificationAttributeRespository.Table;
                query = query.Where(psa => psa.ProductId == productId);
                if (allowFiltering.HasValue)
                    query = query.Where(psa => psa.AllowFiltering == allowFiltering.Value);
                if (showOnProductPage.HasValue)
                    query = query.Where(psa => psa.ShowOnProductPage == showOnProductPage.Value);
                query = query.OrderBy(psa => psa.DisplayOrder);

                var productSpecificationAttributes = query.ToList();
                return productSpecificationAttributes;
            });
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
            
            var productSpecificationAttribute = _productSpecificationAttributeRespository.GetById(productSpecificationAttributeId);
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

            _productSpecificationAttributeRespository.Insert(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the product specification attribute mapping
        /// </summary>
        /// <param name="productSpecificationAttribute">Product specification attribute mapping</param>
        public void UpdateProductSpecificationAttribute(ProductSpecificationAttribute productSpecificationAttribute)
        {
            if (productSpecificationAttribute == null)
                throw new ArgumentNullException("productSpecificationAttribute");

            _productSpecificationAttributeRespository.Update(productSpecificationAttribute);

            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SPECIFICATIONATTRIBUTEOPTION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTSPECIFICATIONATTRIBUTE_PATTERN_KEY);
        }

        #endregion

        #endregion
    }
}
