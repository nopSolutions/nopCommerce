using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute service
    /// </summary>
    public partial class ProductAttributeService : IProductAttributeService
    {
        #region Fields

        private readonly IRepository<PredefinedProductAttributeValue> _predefinedProductAttributeValueRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ProductAttributeService(IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            _predefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual async Task DeleteProductAttribute(ProductAttribute productAttribute)
        {
            await _productAttributeRepository.Delete(productAttribute);
        }

        /// <summary>
        /// Deletes product attributes
        /// </summary>
        /// <param name="productAttributes">Product attributes</param>
        public virtual async Task DeleteProductAttributes(IList<ProductAttribute> productAttributes)
        {
            if (productAttributes == null)
                throw new ArgumentNullException(nameof(productAttributes));

            foreach (var productAttribute in productAttributes) 
                await DeleteProductAttribute(productAttribute);
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Product attributes</returns>
        public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributes(int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var productAttributes = await _productAttributeRepository.GetAllPaged(query =>
            {
                return from pa in query
                    orderby pa.Name
                    select pa;
            }, pageIndex, pageSize);

            return productAttributes;
        }

        /// <summary>
        /// Gets a product attribute 
        /// </summary>
        /// <param name="productAttributeId">Product attribute identifier</param>
        /// <returns>Product attribute </returns>
        public virtual async Task<ProductAttribute> GetProductAttributeById(int productAttributeId)
        {
            return await _productAttributeRepository.GetById(productAttributeId, cache => default);
        }

        /// <summary>
        /// Gets product attributes 
        /// </summary>
        /// <param name="productAttributeIds">Product attribute identifiers</param>
        /// <returns>Product attributes </returns>
        public virtual async Task<IList<ProductAttribute>> GetProductAttributeByIds(int[] productAttributeIds)
        {
            return await _productAttributeRepository.GetByIds(productAttributeIds);
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual async Task InsertProductAttribute(ProductAttribute productAttribute)
        {
            await _productAttributeRepository.Insert(productAttribute);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        public virtual async Task UpdateProductAttribute(ProductAttribute productAttribute)
        {
            await _productAttributeRepository.Update(productAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>List of IDs not existing attributes</returns>
        public virtual async Task<int[]> GetNotExistingAttributes(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException(nameof(attributeId));

            var query = _productAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToAsyncEnumerable()
                .ToListAsync();
            
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        public virtual async Task DeleteProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            await _productAttributeMappingRepository.Delete(productAttributeMapping);
        }

        /// <summary>
        /// Gets product attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public virtual async Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductId(int productId)
        {
            var allCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, productId);

            var query = from pam in _productAttributeMappingRepository.Table
                orderby pam.DisplayOrder, pam.Id
                where pam.ProductId == productId
                select pam;

            var attributes = await _staticCacheManager.Get(allCacheKey, async () => await query.ToAsyncEnumerable().ToListAsync()) ?? new List<ProductAttributeMapping>();

            return attributes;
        }

        /// <summary>
        /// Gets a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>Product attribute mapping</returns>
        public virtual async Task<ProductAttributeMapping> GetProductAttributeMappingById(int productAttributeMappingId)
        {
            return await _productAttributeMappingRepository.GetById(productAttributeMappingId, cache => default);
        }

        /// <summary>
        /// Inserts a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        public virtual async Task InsertProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            await _productAttributeMappingRepository.Insert(productAttributeMapping);
        }

        /// <summary>
        /// Updates the product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        public virtual async Task UpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            await _productAttributeMappingRepository.Update(productAttributeMapping);
        }

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        public virtual async Task DeleteProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            await _productAttributeValueRepository.Delete(productAttributeValue);
        }

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public virtual async Task<IList<ProductAttributeValue>> GetProductAttributeValues(int productAttributeMappingId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, productAttributeMappingId);

            var query = from pav in _productAttributeValueRepository.Table
                orderby pav.DisplayOrder, pav.Id
                where pav.ProductAttributeMappingId == productAttributeMappingId
                select pav;
            var productAttributeValues = await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToListAsync());

            return productAttributeValues;
        }

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>Product attribute value</returns>
        public virtual async Task<ProductAttributeValue> GetProductAttributeValueById(int productAttributeValueId)
        {
            return await _productAttributeValueRepository.GetById(productAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public virtual async Task InsertProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            await _productAttributeValueRepository.Insert(productAttributeValue);
        }

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        public virtual async Task UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            await _productAttributeValueRepository.Update(productAttributeValue);
        }

        #endregion

        #region Predefined product attribute values

        /// <summary>
        /// Deletes a predefined product attribute value
        /// </summary>
        /// <param name="ppav">Predefined product attribute value</param>
        public virtual async Task DeletePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            await _predefinedProductAttributeValueRepository.Delete(ppav);
        }

        /// <summary>
        /// Gets predefined product attribute values by product attribute identifier
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>Product attribute mapping collection</returns>
        public virtual async Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValues(int productAttributeId)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, productAttributeId);

            var query = from ppav in _predefinedProductAttributeValueRepository.Table
                        orderby ppav.DisplayOrder, ppav.Id
                        where ppav.ProductAttributeId == productAttributeId
                        select ppav;

            var values = await _staticCacheManager.Get(key, async () => await query.ToAsyncEnumerable().ToListAsync());

            return values;
        }

        /// <summary>
        /// Gets a predefined product attribute value
        /// </summary>
        /// <param name="id">Predefined product attribute value identifier</param>
        /// <returns>Predefined product attribute value</returns>
        public virtual async Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueById(int id)
        {
            return await _predefinedProductAttributeValueRepository.GetById(id, cache => default);
        }

        /// <summary>
        /// Inserts a predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        public virtual async Task InsertPredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            await _predefinedProductAttributeValueRepository.Insert(ppav);
        }

        /// <summary>
        /// Updates the predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        public virtual async Task UpdatePredefinedProductAttributeValue(PredefinedProductAttributeValue ppav)
        {
            await _predefinedProductAttributeValueRepository.Update(ppav);
        }

        #endregion

        #region Product attribute combinations

        /// <summary>
        /// Deletes a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public virtual async Task DeleteProductAttributeCombination(ProductAttributeCombination combination)
        {
            await _productAttributeCombinationRepository.Delete(combination);
        }

        /// <summary>
        /// Gets all product attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product attribute combinations</returns>
        public virtual async Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinations(int productId)
        {
            if (productId == 0)
                return new List<ProductAttributeCombination>();

            var combinations = await _productAttributeCombinationRepository.GetAll(query =>
            {
                return from c in query
                       orderby c.Id
                    where c.ProductId == productId
                    select c;
            }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeCombinationsByProductCacheKey, productId));

            return combinations;
        }

        /// <summary>
        /// Gets a product attribute combination
        /// </summary>
        /// <param name="productAttributeCombinationId">Product attribute combination identifier</param>
        /// <returns>Product attribute combination</returns>
        public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationById(int productAttributeCombinationId)
        {
            return await _productAttributeCombinationRepository.GetById(productAttributeCombinationId, cache => default);
        }

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>Product attribute combination</returns>
        public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationBySku(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Id
                        where pac.Sku == sku
                        select pac;
            var combination = await query.ToAsyncEnumerable().FirstOrDefaultAsync();

            return combination;
        }

        /// <summary>
        /// Inserts a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public virtual async Task InsertProductAttributeCombination(ProductAttributeCombination combination)
        {
            await _productAttributeCombinationRepository.Insert(combination);
        }

        /// <summary>
        /// Updates a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        public virtual async Task UpdateProductAttributeCombination(ProductAttributeCombination combination)
        {
            await _productAttributeCombinationRepository.Update(combination);
        }

        #endregion

        #endregion
    }
}