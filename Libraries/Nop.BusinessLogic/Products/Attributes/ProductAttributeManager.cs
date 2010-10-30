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

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Product attribute manager
    /// </summary>
    public partial class ProductAttributeManager : IProductAttributeManager
    {
        #region Constants
        private const string PRODUCTATTRIBUTES_ALL_KEY = "Nop.productattribute.all";
        private const string PRODUCTATTRIBUTES_BY_ID_KEY = "Nop.productattribute.id-{0}";
        private const string PRODUCTVARIANTATTRIBUTES_ALL_KEY = "Nop.productvariantattribute.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTES_BY_ID_KEY = "Nop.productvariantattribute.id-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY = "Nop.productvariantattributevalue.all-{0}";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY = "Nop.productvariantattributevalue.id-{0}";
        private const string PRODUCTATTRIBUTES_PATTERN_KEY = "Nop.productattribute.";
        private const string PRODUCTVARIANTATTRIBUTES_PATTERN_KEY = "Nop.productvariantattribute.";
        private const string PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY = "Nop.productvariantattributevalue.";
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
        public ProductAttributeManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        public void DeleteProductAttribute(int productAttributeId)
        {
            var productAttribute = GetProductAttributeById(productAttributeId);
            if (productAttribute == null)
                return;

            
            if (!_context.IsAttached(productAttribute))
                _context.ProductAttributes.Attach(productAttribute);
            _context.DeleteObject(productAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <returns>Product attribute collection</returns>
        public List<ProductAttribute> GetAllProductAttributes()
        {
            string key = string.Format(PRODUCTATTRIBUTES_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductAttribute>)obj2;
            }

            
            var query = from pa in _context.ProductAttributes
                        orderby pa.Name
                        select pa;
            var productAttributes = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productAttributes);
            }
            return productAttributes;
        }
        
        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        public ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTES_BY_ID_KEY, productAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ProductAttribute)obj2;
            }

            
            var query = from pa in _context.ProductAttributes
                        where pa.ProductAttributeId == productAttributeId
                        select pa;
            var productAttribute = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productAttribute);
            }
            return productAttribute;
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            productAttribute.Name = CommonHelper.EnsureNotNull(productAttribute.Name);
            productAttribute.Name = CommonHelper.EnsureMaximumLength(productAttribute.Name, 100);
            productAttribute.Description = CommonHelper.EnsureNotNull(productAttribute.Description);
            productAttribute.Description = CommonHelper.EnsureMaximumLength(productAttribute.Description, 400);

            
            
            _context.ProductAttributes.AddObject(productAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            productAttribute.Name = CommonHelper.EnsureNotNull(productAttribute.Name);
            productAttribute.Name = CommonHelper.EnsureMaximumLength(productAttribute.Name, 100);
            productAttribute.Description = CommonHelper.EnsureNotNull(productAttribute.Description);
            productAttribute.Description = CommonHelper.EnsureMaximumLength(productAttribute.Description, 400);

            
            if (!_context.IsAttached(productAttribute))
                _context.ProductAttributes.Attach(productAttribute);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized product attribute by id
        /// </summary>
        /// <param name="productAttributeLocalizedId">Localized product attribute identifier</param>
        /// <returns>Product attribute content</returns>
        public ProductAttributeLocalized GetProductAttributeLocalizedById(int productAttributeLocalizedId)
        {
            if (productAttributeLocalizedId == 0)
                return null;

            
            var query = from pal in _context.ProductAttributeLocalized
                        where pal.ProductAttributeLocalizedId == productAttributeLocalizedId
                        select pal;
            var productAttributeLocalized = query.SingleOrDefault();
            return productAttributeLocalized;
        }

        /// <summary>
        /// Gets localized product attribute by product attribute id
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute content</returns>
        public List<ProductAttributeLocalized> GetProductAttributeLocalizedByProductAttributeId(int productAttributeId)
        {
            if (productAttributeId == 0)
                return new List<ProductAttributeLocalized>();

            
            var query = from pal in _context.ProductAttributeLocalized
                        where pal.ProductAttributeId == productAttributeId
                        select pal;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized product attribute by product attribute id and language id
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Product attribute content</returns>
        public ProductAttributeLocalized GetProductAttributeLocalizedByProductAttributeIdAndLanguageId(int productAttributeId, int languageId)
        {
            if (productAttributeId == 0 || languageId == 0)
                return null;

            
            var query = from pal in _context.ProductAttributeLocalized
                        orderby pal.ProductAttributeLocalizedId
                        where pal.ProductAttributeId == productAttributeId &&
                        pal.LanguageId == languageId
                        select pal;
            var productAttributeLocalized = query.FirstOrDefault();
            return productAttributeLocalized;
        }

        /// <summary>
        /// Inserts a localized product attribute
        /// </summary>
        /// <param name="productAttributeLocalized">Localized product attribute</param>
        public void InsertProductAttributeLocalized(ProductAttributeLocalized productAttributeLocalized)
        {
            if (productAttributeLocalized == null)
                throw new ArgumentNullException("productAttributeLocalized");

            productAttributeLocalized.Name = CommonHelper.EnsureNotNull(productAttributeLocalized.Name);
            productAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(productAttributeLocalized.Name, 100);
            productAttributeLocalized.Description = CommonHelper.EnsureNotNull(productAttributeLocalized.Description);
            productAttributeLocalized.Description = CommonHelper.EnsureMaximumLength(productAttributeLocalized.Description, 400);

            
            
            _context.ProductAttributeLocalized.AddObject(productAttributeLocalized);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized product attribute
        /// </summary>
        /// <param name="productAttributeLocalized">Localized product attribute</param>
        public void UpdateProductAttributeLocalized(ProductAttributeLocalized productAttributeLocalized)
        {
            if (productAttributeLocalized == null)
                throw new ArgumentNullException("productAttributeLocalized");

            productAttributeLocalized.Name = CommonHelper.EnsureNotNull(productAttributeLocalized.Name);
            productAttributeLocalized.Name = CommonHelper.EnsureMaximumLength(productAttributeLocalized.Name, 100);
            productAttributeLocalized.Description = CommonHelper.EnsureNotNull(productAttributeLocalized.Description);
            productAttributeLocalized.Description = CommonHelper.EnsureMaximumLength(productAttributeLocalized.Description, 400);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(productAttributeLocalized.Name) &&
                string.IsNullOrEmpty(productAttributeLocalized.Description);

            
            if (!_context.IsAttached(productAttributeLocalized))
                _context.ProductAttributeLocalized.Attach(productAttributeLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(productAttributeLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }
        
        #endregion

        #region Product variant attributes mappings (ProductVariantAttribute)

        /// <summary>
        /// Deletes a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        public void DeleteProductVariantAttribute(int productVariantAttributeId)
        {
            var productVariantAttribute = GetProductVariantAttributeById(productVariantAttributeId);
            if (productVariantAttribute == null)
                return;

            
            if (!_context.IsAttached(productVariantAttribute))
                _context.ProductVariantAttributes.Attach(productVariantAttribute);
            _context.DeleteObject(productVariantAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets product variant attribute mappings by product identifier
        /// </summary>
        /// <param name="productVariantId">The product variant identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public List<ProductVariantAttribute> GetProductVariantAttributesByProductVariantId(int productVariantId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTES_ALL_KEY, productVariantId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductVariantAttribute>)obj2;
            }

            
            var query = from pva in _context.ProductVariantAttributes
                        orderby pva.DisplayOrder
                        where pva.ProductVariantId == productVariantId
                        select pva;
            var productVariantAttributes = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariantAttributes);
            }
            return productVariantAttributes;
        }

        /// <summary>
        /// Gets a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping</returns>
        public ProductVariantAttribute GetProductVariantAttributeById(int productVariantAttributeId)
        {
            if (productVariantAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTES_BY_ID_KEY, productVariantAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ProductVariantAttribute)obj2;
            }

            
            var query = from pva in _context.ProductVariantAttributes
                        where pva.ProductVariantAttributeId == productVariantAttributeId
                        select pva;
            var productVariantAttribute = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariantAttribute);
            }
            return productVariantAttribute;
        }

        /// <summary>
        /// Inserts a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public void InsertProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            productVariantAttribute.TextPrompt = CommonHelper.EnsureNotNull(productVariantAttribute.TextPrompt);
            productVariantAttribute.TextPrompt = CommonHelper.EnsureMaximumLength(productVariantAttribute.TextPrompt, 200);

            
            
            _context.ProductVariantAttributes.AddObject(productVariantAttribute);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public void UpdateProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            productVariantAttribute.TextPrompt = CommonHelper.EnsureNotNull(productVariantAttribute.TextPrompt);
            productVariantAttribute.TextPrompt = CommonHelper.EnsureMaximumLength(productVariantAttribute.TextPrompt, 200);

            
            if (!_context.IsAttached(productVariantAttribute))
                _context.ProductVariantAttributes.Attach(productVariantAttribute);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        #endregion

        #region Product variant attribute values (ProductVariantAttributeValue)

        /// <summary>
        /// Deletes a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        public void DeleteProductVariantAttributeValue(int productVariantAttributeValueId)
        {
            var productVariantAttributeValue = GetProductVariantAttributeValueById(productVariantAttributeValueId);
            if (productVariantAttributeValue == null)
                return;

            
            if (!_context.IsAttached(productVariantAttributeValue))
                _context.ProductVariantAttributeValues.Attach(productVariantAttributeValue);
            _context.DeleteObject(productVariantAttributeValue);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }
        
        /// <summary>
        /// Gets product variant attribute values by product identifier
        /// </summary>
        /// <param name="productVariantAttributeId">The product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public List<ProductVariantAttributeValue> GetProductVariantAttributeValues(int productVariantAttributeId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY, productVariantAttributeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<ProductVariantAttributeValue>)obj2;
            }

            
            var query = from pvav in _context.ProductVariantAttributeValues
                        orderby pvav.DisplayOrder
                        where pvav.ProductVariantAttributeId == productVariantAttributeId
                        select pvav;
            var productVariantAttributeValues = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariantAttributeValues);
            }
            return productVariantAttributeValues;
        }
        
        /// <summary>
        /// Gets a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Product variant attribute value</returns>
        public ProductVariantAttributeValue GetProductVariantAttributeValueById(int productVariantAttributeValueId)
        {
            if (productVariantAttributeValueId == 0)
                return null;

            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY, productVariantAttributeValueId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (ProductVariantAttributeValue)obj2;
            }

            
            var query = from pvav in _context.ProductVariantAttributeValues
                        where pvav.ProductVariantAttributeValueId == productVariantAttributeValueId
                        select pvav;
            var productVariantAttributeValue = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, productVariantAttributeValue);
            }
            return productVariantAttributeValue;
        }

        /// <summary>
        /// Inserts a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public void InsertProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            productVariantAttributeValue.Name = CommonHelper.EnsureNotNull(productVariantAttributeValue.Name);
            productVariantAttributeValue.Name = CommonHelper.EnsureMaximumLength(productVariantAttributeValue.Name, 100);

            

            _context.ProductVariantAttributeValues.AddObject(productVariantAttributeValue);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public void UpdateProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            productVariantAttributeValue.Name = CommonHelper.EnsureNotNull(productVariantAttributeValue.Name);
            productVariantAttributeValue.Name = CommonHelper.EnsureMaximumLength(productVariantAttributeValue.Name, 100);

            
            if (!_context.IsAttached(productVariantAttributeValue))
                _context.ProductVariantAttributeValues.Attach(productVariantAttributeValue);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets localized product variant attribute value by id
        /// </summary>
        /// <param name="productVariantAttributeValueLocalizedId">Localized product variant attribute value identifier</param>
        /// <returns>Localized product variant attribute value</returns>
        public ProductVariantAttributeValueLocalized GetProductVariantAttributeValueLocalizedById(int productVariantAttributeValueLocalizedId)
        {
            if (productVariantAttributeValueLocalizedId == 0)
                return null;

            
            var query = from pvavl in _context.ProductVariantAttributeValueLocalized
                        where pvavl.ProductVariantAttributeValueLocalizedId == productVariantAttributeValueLocalizedId
                        select pvavl;
            var productVariantAttributeValueLocalized = query.SingleOrDefault();
            return productVariantAttributeValueLocalized;
        }

        /// <summary>
        /// Gets localized  product variant attribute value by id
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Content</returns>
        public List<ProductVariantAttributeValueLocalized> GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueId(int productVariantAttributeValueId)
        {
            if (productVariantAttributeValueId == 0)
                return new List<ProductVariantAttributeValueLocalized>();

            
            var query = from pvavl in _context.ProductVariantAttributeValueLocalized
                        where pvavl.ProductVariantAttributeValueId == productVariantAttributeValueId
                        select pvavl;
            var content = query.ToList();
            return content;
        }

        /// <summary>
        /// Gets localized product variant attribute value by product variant attribute value id and language id
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized product variant attribute value</returns>
        public ProductVariantAttributeValueLocalized GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueIdAndLanguageId(int productVariantAttributeValueId, int languageId)
        {
            if (productVariantAttributeValueId == 0 || languageId == 0)
                return null;

            
            var query = from pvavl in _context.ProductVariantAttributeValueLocalized
                        orderby pvavl.ProductVariantAttributeValueLocalizedId
                        where pvavl.ProductVariantAttributeValueId == productVariantAttributeValueId &&
                        pvavl.LanguageId == languageId
                        select pvavl;
            var productVariantAttributeValueLocalized = query.FirstOrDefault();
            return productVariantAttributeValueLocalized;
        }

        /// <summary>
        /// Inserts a localized product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueLocalized">Localized product variant attribute value</param>
        public void InsertProductVariantAttributeValueLocalized(ProductVariantAttributeValueLocalized productVariantAttributeValueLocalized)
        {
            if (productVariantAttributeValueLocalized == null)
                throw new ArgumentNullException("productVariantAttributeValueLocalized");

            productVariantAttributeValueLocalized.Name = CommonHelper.EnsureNotNull(productVariantAttributeValueLocalized.Name);
            productVariantAttributeValueLocalized.Name = CommonHelper.EnsureMaximumLength(productVariantAttributeValueLocalized.Name, 100);

            
            
            _context.ProductVariantAttributeValueLocalized.AddObject(productVariantAttributeValueLocalized);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Update a localized product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueLocalized">Localized product variant attribute value</param>
        public void UpdateProductVariantAttributeValueLocalized(ProductVariantAttributeValueLocalized productVariantAttributeValueLocalized)
        {
            if (productVariantAttributeValueLocalized == null)
                throw new ArgumentNullException("productVariantAttributeValueLocalized");

            productVariantAttributeValueLocalized.Name = CommonHelper.EnsureNotNull(productVariantAttributeValueLocalized.Name);
            productVariantAttributeValueLocalized.Name = CommonHelper.EnsureMaximumLength(productVariantAttributeValueLocalized.Name, 100);

            bool allFieldsAreEmpty = string.IsNullOrEmpty(productVariantAttributeValueLocalized.Name);

            
            if (!_context.IsAttached(productVariantAttributeValueLocalized))
                _context.ProductVariantAttributeValueLocalized.Attach(productVariantAttributeValueLocalized);

            if (allFieldsAreEmpty)
            {
                //delete if all fields are empty
                _context.DeleteObject(productVariantAttributeValueLocalized);
                _context.SaveChanges();
            }
            else
            {
                _context.SaveChanges();
            }

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
                NopRequestCache.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            }
        }
        
        #endregion

        #region Product variant attribute compinations (ProductVariantAttributeCombination)

        /// <summary>
        /// Deletes a product variant attribute combination
        /// </summary>
        /// <param name="productVariantAttributeCombinationId">Product variant attribute combination identifier</param>
        public void DeleteProductVariantAttributeCombination(int productVariantAttributeCombinationId)
        {
            var combination = GetProductVariantAttributeCombinationById(productVariantAttributeCombinationId);
            if (combination == null)
                return;

            
            if (!_context.IsAttached(combination))
                _context.ProductVariantAttributeCombinations.Attach(combination);
            _context.DeleteObject(combination);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all product variant attribute combinations
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <returns>Product variant attribute combination collection</returns>
        public List<ProductVariantAttributeCombination> GetAllProductVariantAttributeCombinations(int productVariantId)
        {
            if (productVariantId == 0)
                return new List<ProductVariantAttributeCombination>();
            
            var query = from pvac in _context.ProductVariantAttributeCombinations
                        orderby pvac.ProductVariantAttributeCombinationId
                        where pvac.ProductVariantId == productVariantId
                        select pvac;
            var combinations = query.ToList();
            return combinations;
        }

        /// <summary>
        /// Gets a product variant attribute combination
        /// </summary>
        /// <param name="productVariantAttributeCombinationId">Product variant attribute combination identifier</param>
        /// <returns>Product variant attribute combination</returns>
        public ProductVariantAttributeCombination GetProductVariantAttributeCombinationById(int productVariantAttributeCombinationId)
        {
            if (productVariantAttributeCombinationId == 0)
                return null;

            
            var query = from pvac in _context.ProductVariantAttributeCombinations
                        where pvac.ProductVariantAttributeCombinationId == productVariantAttributeCombinationId
                        select pvac;
            var combination = query.SingleOrDefault();

            return combination;
        }

        /// <summary>
        /// Inserts a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public void InsertProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            

            _context.ProductVariantAttributeCombinations.AddObject(combination);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a product variant attribute combination
        /// </summary>
        /// <param name="productVariantAttributeCombination">Product variant attribute combination</param>
        public void UpdateProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            
            if (!_context.IsAttached(combination))
                _context.ProductVariantAttributeCombinations.Attach(combination);

            _context.SaveChanges();
        }

        /// <summary>
        /// Finds a product variant attribute combination by attributes stored in XML 
        /// </summary>
        /// <param name="productVariantId">Product variant identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Found product variant attribute combination</returns>
        public ProductVariantAttributeCombination FindProductVariantAttributeCombination(int productVariantId, string attributesXml)
        {
            //existing combinations
            var combinations = this.GetAllProductVariantAttributeCombinations(productVariantId);
            if (combinations.Count == 0)
                return null;

            foreach (var combination in combinations)
            {
                bool attributesEqual = ProductAttributeHelper.AreProductAttributesEqual(combination.AttributesXml, attributesXml);
                if (attributesEqual)
                {
                    return combination;
                }
            }

            return null;
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
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.ProductAttributeManager.CacheEnabled");
            }
        }
        #endregion
    }
}
