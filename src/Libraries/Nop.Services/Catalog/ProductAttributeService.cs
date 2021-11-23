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

        protected IRepository<PredefinedProductAttributeValue> PredefinedProductAttributeValueRepository { get; }
        private readonly IRepository<Product> _productRepository;
        protected IRepository<ProductAttribute> ProductAttributeRepository { get; }
        protected IRepository<ProductAttributeCombination> ProductAttributeCombinationRepository { get; }
        protected IRepository<ProductAttributeMapping> ProductAttributeMappingRepository { get; }
        protected IRepository<ProductAttributeValue> ProductAttributeValueRepository { get; }
        protected IRepository<Product> ProductRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }

        #endregion

        #region Ctor

        public ProductAttributeService(IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        {
            PredefinedProductAttributeValueRepository = predefinedProductAttributeValueRepository;
            _productRepository = productRepository;
            ProductAttributeRepository = productAttributeRepository;
            ProductAttributeCombinationRepository = productAttributeCombinationRepository;
            ProductAttributeMappingRepository = productAttributeMappingRepository;
            ProductAttributeValueRepository = productAttributeValueRepository;
            ProductRepository = productRepository;
            StaticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        #region Product attributes

        /// <summary>
        /// Deletes a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAttributeAsync(ProductAttribute productAttribute)
        {
            await ProductAttributeRepository.DeleteAsync(productAttribute);
        }

        /// <summary>
        /// Deletes product attributes
        /// </summary>
        /// <param name="productAttributes">Product attributes</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAttributesAsync(IList<ProductAttribute> productAttributes)
        {
            if (productAttributes == null)
                throw new ArgumentNullException(nameof(productAttributes));

            foreach (var productAttribute in productAttributes) 
                await DeleteProductAttributeAsync(productAttribute);
        }

        /// <summary>
        /// Gets all product attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attributes
        /// </returns>
        public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var productAttributes = await ProductAttributeRepository.GetAllPagedAsync(query =>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute 
        /// </returns>
        public virtual async Task<ProductAttribute> GetProductAttributeByIdAsync(int productAttributeId)
        {
            return await ProductAttributeRepository.GetByIdAsync(productAttributeId, cache => default);
        }

        /// <summary>
        /// Gets product attributes 
        /// </summary>
        /// <param name="productAttributeIds">Product attribute identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attributes 
        /// </returns>
        public virtual async Task<IList<ProductAttribute>> GetProductAttributeByIdsAsync(int[] productAttributeIds)
        {
            return await ProductAttributeRepository.GetByIdsAsync(productAttributeIds);
        }

        /// <summary>
        /// Inserts a product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAttributeAsync(ProductAttribute productAttribute)
        {
            await ProductAttributeRepository.InsertAsync(productAttribute);
        }

        /// <summary>
        /// Updates the product attribute
        /// </summary>
        /// <param name="productAttribute">Product attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAttributeAsync(ProductAttribute productAttribute)
        {
            await ProductAttributeRepository.UpdateAsync(productAttribute);
        }

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of IDs not existing attributes
        /// </returns>
        public virtual async Task<int[]> GetNotExistingAttributesAsync(int[] attributeId)
        {
            if (attributeId == null)
                throw new ArgumentNullException(nameof(attributeId));

            var query = ProductAttributeRepository.Table;
            var queryFilter = attributeId.Distinct().ToArray();
            var filter = await query.Select(a => a.Id)
                .Where(m => queryFilter.Contains(m))
                .ToListAsync();
            
            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Product attributes mappings

        /// <summary>
        /// Deletes a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            await ProductAttributeMappingRepository.DeleteAsync(productAttributeMapping);
        }

        /// <summary>
        /// Gets product attribute mappings by product identifier
        /// </summary>
        /// <param name="productId">The product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping collection
        /// </returns>
        public virtual async Task<IList<ProductAttributeMapping>> GetProductAttributeMappingsByProductIdAsync(int productId)
        {
            var allCacheKey = StaticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeMappingsByProductCacheKey, productId);

            var query = from pam in ProductAttributeMappingRepository.Table
                orderby pam.DisplayOrder, pam.Id
                where pam.ProductId == productId
                select pam;

            var attributes = await StaticCacheManager.GetAsync(allCacheKey, async () => await query.ToListAsync()) ?? new List<ProductAttributeMapping>();

            return attributes;
        }

        /// <summary>
        /// Gets a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMappingId">Product attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping
        /// </returns>
        public virtual async Task<ProductAttributeMapping> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
        {
            return await ProductAttributeMappingRepository.GetByIdAsync(productAttributeMappingId, cache => default);
        }

        /// <summary>
        /// Inserts a product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            await ProductAttributeMappingRepository.InsertAsync(productAttributeMapping);
        }

        /// <summary>
        /// Updates the product attribute mapping
        /// </summary>
        /// <param name="productAttributeMapping">The product attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            await ProductAttributeMappingRepository.UpdateAsync(productAttributeMapping);
        }

        #endregion

        #region Product attribute values

        /// <summary>
        /// Deletes a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">Product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await ProductAttributeValueRepository.DeleteAsync(productAttributeValue);
        }

        /// <summary>
        /// Gets product attribute values by product attribute mapping identifier
        /// </summary>
        /// <param name="productAttributeMappingId">The product attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping collection
        /// </returns>
        public virtual async Task<IList<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductAttributeValuesByAttributeCacheKey, productAttributeMappingId);

            var query = from pav in ProductAttributeValueRepository.Table
                orderby pav.DisplayOrder, pav.Id
                where pav.ProductAttributeMappingId == productAttributeMappingId
                select pav;
            var productAttributeValues = await StaticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return productAttributeValues;
        }

        /// <summary>
        /// Gets a product attribute value
        /// </summary>
        /// <param name="productAttributeValueId">Product attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute value
        /// </returns>
        public virtual async Task<ProductAttributeValue> GetProductAttributeValueByIdAsync(int productAttributeValueId)
        {
            return await ProductAttributeValueRepository.GetByIdAsync(productAttributeValueId, cache => default);
        }

        /// <summary>
        /// Inserts a product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await ProductAttributeValueRepository.InsertAsync(productAttributeValue);
        }

        /// <summary>
        /// Updates the product attribute value
        /// </summary>
        /// <param name="productAttributeValue">The product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await ProductAttributeValueRepository.UpdateAsync(productAttributeValue);
        }

        #endregion

        #region Predefined product attribute values

        /// <summary>
        /// Deletes a predefined product attribute value
        /// </summary>
        /// <param name="ppav">Predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeletePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
        {
            await PredefinedProductAttributeValueRepository.DeleteAsync(ppav);
        }

        /// <summary>
        /// Gets predefined product attribute values by product attribute identifier
        /// </summary>
        /// <param name="productAttributeId">The product attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute mapping collection
        /// </returns>
        public virtual async Task<IList<PredefinedProductAttributeValue>> GetPredefinedProductAttributeValuesAsync(int productAttributeId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.PredefinedProductAttributeValuesByAttributeCacheKey, productAttributeId);

            var query = from ppav in PredefinedProductAttributeValueRepository.Table
                        orderby ppav.DisplayOrder, ppav.Id
                        where ppav.ProductAttributeId == productAttributeId
                        select ppav;

            var values = await StaticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return values;
        }

        /// <summary>
        /// Gets a predefined product attribute value
        /// </summary>
        /// <param name="id">Predefined product attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined product attribute value
        /// </returns>
        public virtual async Task<PredefinedProductAttributeValue> GetPredefinedProductAttributeValueByIdAsync(int id)
        {
            return await PredefinedProductAttributeValueRepository.GetByIdAsync(id, cache => default);
        }

        /// <summary>
        /// Inserts a predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertPredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
        {
            await PredefinedProductAttributeValueRepository.InsertAsync(ppav);
        }

        /// <summary>
        /// Updates the predefined product attribute value
        /// </summary>
        /// <param name="ppav">The predefined product attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdatePredefinedProductAttributeValueAsync(PredefinedProductAttributeValue ppav)
        {
            await PredefinedProductAttributeValueRepository.UpdateAsync(ppav);
        }

        #endregion

        #region Product attribute combinations

        /// <summary>
        /// Deletes a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductAttributeCombinationAsync(ProductAttributeCombination combination)
        {
            await ProductAttributeCombinationRepository.DeleteAsync(combination);
        }

        /// <summary>
        /// Gets all product attribute combinations
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combinations
        /// </returns>
        public virtual async Task<IList<ProductAttributeCombination>> GetAllProductAttributeCombinationsAsync(int productId)
        {
            if (productId == 0)
                return new List<ProductAttributeCombination>();

            var combinations = await ProductAttributeCombinationRepository.GetAllAsync(query =>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination
        /// </returns>
        public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationByIdAsync(int productAttributeCombinationId)
        {
            return await ProductAttributeCombinationRepository.GetByIdAsync(productAttributeCombinationId, cache => default);
        }

        /// <summary>
        /// Gets a product attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product attribute combination
        /// </returns>
        public virtual async Task<ProductAttributeCombination> GetProductAttributeCombinationBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from pac in ProductAttributeCombinationRepository.Table
                        join p in ProductRepository.Table on pac.ProductId equals p.Id
                        orderby pac.Id
                        where !p.Deleted && pac.Sku == sku
                        select pac;
            var combination = await query.FirstOrDefaultAsync();

            return combination;
        }

        /// <summary>
        /// Inserts a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductAttributeCombinationAsync(ProductAttributeCombination combination)
        {
            await ProductAttributeCombinationRepository.InsertAsync(combination);
        }

        /// <summary>
        /// Updates a product attribute combination
        /// </summary>
        /// <param name="combination">Product attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductAttributeCombinationAsync(ProductAttributeCombination combination)
        {
            await ProductAttributeCombinationRepository.UpdateAsync(combination);
        }

        #endregion

        #endregion
    }
}