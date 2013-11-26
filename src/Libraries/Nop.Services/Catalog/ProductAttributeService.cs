using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class ProductAttributeService : IProductAttributeService
    {
        #region Constants
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        private const string PRODUCTATTRIBUTES_ALL_KEY = "Nop.productattribute.all-{0}-{1}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute ID
        /// </remarks>
        private const string PRODUCTATTRIBUTES_BY_ID_KEY = "Nop.productattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTVARIANTATTRIBUTES_ALL_KEY = "Nop.productvariantattribute.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product variant attribute ID
        /// </remarks>
        private const string PRODUCTVARIANTATTRIBUTES_BY_ID_KEY = "Nop.productvariantattribute.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product variant attribute ID
        /// </remarks>
        private const string PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY = "Nop.productvariantattributevalue.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product variant attribute value ID
        /// </remarks>
        private const string PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY = "Nop.productvariantattributevalue.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTVARIANTATTRIBUTECOMBINATIONS_ALL_KEY = "Nop.productvariantattributecombination.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTATTRIBUTES_PATTERN_KEY = "Nop.productattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTVARIANTATTRIBUTES_PATTERN_KEY = "Nop.productvariantattribute.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY = "Nop.productvariantattributevalue.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY = "Nop.productvariantattributecombination.";

        #endregion

        #region Fields

        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductVariantAttribute> _productVariantAttributeRepository;
        private readonly IRepository<ProductVariantAttributeCombination> _productVariantAttributeCombinationRepository;
        private readonly IRepository<ProductVariantAttributeValue> _productVariantAttributeValueRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;


        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="productAttributeRepository">Product attribute repository</param>
        /// <param name="productVariantAttributeRepository">Product variant attribute mapping repository</param>
        /// <param name="productVariantAttributeCombinationRepository">Product variant attribute combination repository</param>
        /// <param name="productVariantAttributeValueRepository">Product variant attribute value repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ProductAttributeService(ICacheManager cacheManager,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductVariantAttribute> productVariantAttributeRepository,
            IRepository<ProductVariantAttributeCombination> productVariantAttributeCombinationRepository,
            IRepository<ProductVariantAttributeValue> productVariantAttributeValueRepository,
            IEventPublisher eventPublisher
            )
        {
            _cacheManager = cacheManager;
            _productAttributeRepository = productAttributeRepository;
            _productVariantAttributeRepository = productVariantAttributeRepository;
            _productVariantAttributeCombinationRepository = productVariantAttributeCombinationRepository;
            _productVariantAttributeValueRepository = productVariantAttributeValueRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual void DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Delete(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productAttribute);
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Product attribute collection</returns>
        public virtual IPagedList<ProductAttribute> GetAllProductAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(PRODUCTATTRIBUTES_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from pa in _productAttributeRepository.Table
                            orderby pa.Name
                            select pa;
                var productAttributes = new PagedList<ProductAttribute>(query, pageIndex, pageSize);
                return productAttributes;
            });
        }

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        public virtual ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0)
                return null;

            string key = string.Format(PRODUCTATTRIBUTES_BY_ID_KEY, productAttributeId);
            return _cacheManager.Get(key, () => { return _productAttributeRepository.GetById(productAttributeId); });
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Insert(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productAttribute);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Update(productAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productAttribute);
        }

        #endregion

        #region Product variant attributes mappings (ProductVariantAttribute)

        /// <summary>
        /// Deletes a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">Product variant attribute mapping</param>
        public virtual void DeleteProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Delete(productVariantAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productVariantAttribute);
        }

        /// <summary>
        /// Gets product variant attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public virtual IList<ProductVariantAttribute> GetProductVariantAttributesByProductId(int productId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTES_ALL_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pva in _productVariantAttributeRepository.Table
                            orderby pva.DisplayOrder
                            where pva.ProductId == productId
                            select pva;
                var productVariantAttributes = query.ToList();
                return productVariantAttributes;
            });
        }

        /// <summary>
        /// Gets a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttributeId">Product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping</returns>
        public virtual ProductVariantAttribute GetProductVariantAttributeById(int productVariantAttributeId)
        {
            if (productVariantAttributeId == 0)
                return null;
            
            string key = string.Format(PRODUCTVARIANTATTRIBUTES_BY_ID_KEY, productVariantAttributeId);
            return _cacheManager.Get(key, () => { return _productVariantAttributeRepository.GetById(productVariantAttributeId); });
        }

        /// <summary>
        /// Inserts a product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public virtual void InsertProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Insert(productVariantAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productVariantAttribute);
        }

        /// <summary>
        /// Updates the product variant attribute mapping
        /// </summary>
        /// <param name="productVariantAttribute">The product variant attribute mapping</param>
        public virtual void UpdateProductVariantAttribute(ProductVariantAttribute productVariantAttribute)
        {
            if (productVariantAttribute == null)
                throw new ArgumentNullException("productVariantAttribute");

            _productVariantAttributeRepository.Update(productVariantAttribute);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productVariantAttribute);
        }

        #endregion

        #region Product variant attribute values (ProductVariantAttributeValue)

        /// <summary>
        /// Deletes a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">Product variant attribute value</param>
        public virtual void DeleteProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Delete(productVariantAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(productVariantAttributeValue);
        }

        /// <summary>
        /// Gets product variant attribute values by product identifier
        /// </summary>
        /// <param name="productVariantAttributeId">The product variant attribute mapping identifier</param>
        /// <returns>Product variant attribute mapping collection</returns>
        public virtual IList<ProductVariantAttributeValue> GetProductVariantAttributeValues(int productVariantAttributeId)
        {
            string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_ALL_KEY, productVariantAttributeId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pvav in _productVariantAttributeValueRepository.Table
                            orderby pvav.DisplayOrder
                            where pvav.ProductVariantAttributeId == productVariantAttributeId
                            select pvav;
                var productVariantAttributeValues = query.ToList();
                return productVariantAttributeValues;
            });
        }

        /// <summary>
        /// Gets a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValueId">Product variant attribute value identifier</param>
        /// <returns>Product variant attribute value</returns>
        public virtual ProductVariantAttributeValue GetProductVariantAttributeValueById(int productVariantAttributeValueId)
        {
            if (productVariantAttributeValueId == 0)
                return null;
            
           string key = string.Format(PRODUCTVARIANTATTRIBUTEVALUES_BY_ID_KEY, productVariantAttributeValueId);
           return _cacheManager.Get(key, () => { return _productVariantAttributeValueRepository.GetById(productVariantAttributeValueId); });
        }

        /// <summary>
        /// Inserts a product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public virtual void InsertProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Insert(productVariantAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(productVariantAttributeValue);
        }

        /// <summary>
        /// Updates the product variant attribute value
        /// </summary>
        /// <param name="productVariantAttributeValue">The product variant attribute value</param>
        public virtual void UpdateProductVariantAttributeValue(ProductVariantAttributeValue productVariantAttributeValue)
        {
            if (productVariantAttributeValue == null)
                throw new ArgumentNullException("productVariantAttributeValue");

            _productVariantAttributeValueRepository.Update(productVariantAttributeValue);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(productVariantAttributeValue);
        }

        #endregion

        #region Product variant attribute combinations (ProductVariantAttributeCombination)

        /// <summary>
        /// Deletes a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public virtual void DeleteProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productVariantAttributeCombinationRepository.Delete(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(combination);
        }

        /// <summary>
        /// Gets all product variant attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product variant attribute combination collection</returns>
        public virtual IList<ProductVariantAttributeCombination> GetAllProductVariantAttributeCombinations(int productId)
        {
            if (productId == 0)
                return new List<ProductVariantAttributeCombination>();

            string key = string.Format(PRODUCTVARIANTATTRIBUTECOMBINATIONS_ALL_KEY, productId);

            return _cacheManager.Get(key, () =>
            {
                var query = from pvac in _productVariantAttributeCombinationRepository.Table
                            orderby pvac.Id
                            where pvac.ProductId == productId
                            select pvac;
                var combinations = query.ToList();
                return combinations;
            });
        }

        /// <summary>
        /// Gets a product variant attribute combination
        /// </summary>
        /// <param name="productVariantAttributeCombinationId">Product variant attribute combination identifier</param>
        /// <returns>Product variant attribute combination</returns>
        public virtual ProductVariantAttributeCombination GetProductVariantAttributeCombinationById(int productVariantAttributeCombinationId)
        {
            if (productVariantAttributeCombinationId == 0)
                return null;
            
            return _productVariantAttributeCombinationRepository.GetById(productVariantAttributeCombinationId);
        }

        /// <summary>
        /// Inserts a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public virtual void InsertProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productVariantAttributeCombinationRepository.Insert(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(combination);
        }

        /// <summary>
        /// Updates a product variant attribute combination
        /// </summary>
        /// <param name="combination">Product variant attribute combination</param>
        public virtual void UpdateProductVariantAttributeCombination(ProductVariantAttributeCombination combination)
        {
            if (combination == null)
                throw new ArgumentNullException("combination");

            _productVariantAttributeCombinationRepository.Update(combination);

            //cache
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTEVALUES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTVARIANTATTRIBUTECOMBINATIONS_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(combination);
        }

        #endregion

        #endregion
    }
}
