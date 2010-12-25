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

        private readonly ILocalizedEntityService _leService;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRespository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRespository;
        private readonly IRepository<ProductSpecificationAttribute> _productSpecificationAttributeRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="leService">Localized entity service</param>
        /// <param name="specificationAttributeRespository">Specification attribute repository</param>
        /// <param name="specificationAttributeOptionRespository">Specification attribute option repository</param>
        /// <param name="productSpecificationAttributeRespository">Product specification attribute repository</param>
        public SpecificationAttributeService(ICacheManager cacheManager,
            ILocalizedEntityService leService,
            IRepository<SpecificationAttribute> specificationAttributeRespository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRespository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRespository)
        {
            this._cacheManager = cacheManager;
            this._leService = leService;
            this._specificationAttributeRespository = specificationAttributeRespository;
            this._specificationAttributeOptionRespository = specificationAttributeOptionRespository;
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
                var sa = _specificationAttributeRespository.GetById(specificationAttributeId);
                new DefaultPropertyLocalizer<SpecificationAttribute, LocalizedSpecificationAttribute>(_leService, sa).Localize();
                return sa;
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
            specificationAttributes.ForEach(sa => new DefaultPropertyLocalizer<SpecificationAttribute, LocalizedSpecificationAttribute>(_leService, sa).Localize());
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
                var sao = _specificationAttributeOptionRespository.GetById(specificationAttributeOptionId);
                new DefaultPropertyLocalizer<SpecificationAttributeOption, LocalizedSpecificationAttributeOption>(_leService, sao).Localize();
                return sao;
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
            specificationAttributeOptions.ForEach(sao => new DefaultPropertyLocalizer<SpecificationAttributeOption, LocalizedSpecificationAttributeOption>(_leService, sao).Localize());
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
