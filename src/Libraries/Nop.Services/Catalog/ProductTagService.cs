using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly INopDataProvider _dataProvider;
        private readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductTagService(CatalogSettings catalogSettings,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _productProductTagMappingRepository = productProductTagMappingRepository;
            _productTagRepository = productTagRepository;
            _staticCacheManager = staticCacheManager;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a product-product tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        public virtual async Task DeleteProductProductTagMappingAsync(int productId, int productTagId)
        {
            var mappingRecord = await _productProductTagMappingRepository.Table.FirstOrDefaultAsync(pptm => pptm.ProductId == productId && pptm.ProductTagId == productTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _productProductTagMappingRepository.DeleteAsync(mappingRecord);
        }

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private async Task<Dictionary<int, int>> GetProductCountAsync(int storeId, bool showHidden)
        {
            var allowedCustomerRolesIds = string.Empty;
            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                //pass customer role identifiers as comma-delimited string
                allowedCustomerRolesIds = string.Join(",", await _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()));
            }

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagCountCacheKey, storeId, 
                _customerService.GetCustomerRoleIdsAsync(await _workContext.GetCurrentCustomerAsync()), 
                showHidden);
           
            return await _staticCacheManager.GetAsync(key, async () =>
            {
                //prepare input parameters
                var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", storeId);
                var pAllowedCustomerRoleIds = SqlParameterHelper.GetStringParameter("AllowedCustomerRoleIds", allowedCustomerRolesIds);

                //invoke stored procedure
                return (await _dataProvider.QueryProcAsync<ProductTagWithCount>("ProductTagCountLoadAll",
                        pStoreId,
                        pAllowedCustomerRoleIds))
                    .ToDictionary(item => item.ProductTagId, item => item.ProductCount);
            });
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual async Task DeleteProductTagAsync(ProductTag productTag)
        {
            await _productTagRepository.DeleteAsync(productTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        public virtual async Task DeleteProductTagsAsync(IList<ProductTag> productTags)
        {
            if (productTags == null)
                throw new ArgumentNullException(nameof(productTags));

            foreach (var productTag in productTags) 
                await DeleteProductTagAsync(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTagsAsync(string tagName = null)
        {
            var allProductTags = await _productTagRepository.GetAllAsync(getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName)) 
                allProductTags = allProductTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allProductTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTagsByProductIdAsync(int productId)
        {
            var productTags = await _productTagRepository.GetAllAsync(query =>
            {
                return from pt in query
                    join ppt in _productProductTagMappingRepository.Table on pt.Id equals ppt.ProductTagId
                    where ppt.ProductId == productId
                    orderby pt.Id
                    select pt;
            }, cache => cache.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagsByProductCacheKey, productId));

            return productTags;
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual async Task<ProductTag> GetProductTagByIdAsync(int productTagId)
        {
            return await _productTagRepository.GetByIdAsync(productTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetProductTagsByIdsAsync(int[] productTagIds)
        {
            return await _productTagRepository.GetByIdsAsync(productTagIds);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public virtual async Task<ProductTag> GetProductTagByNameAsync(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = await query.FirstOrDefaultAsync();
            return productTag;
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        public virtual async Task InsertProductProductTagMappingAsync(ProductProductTagMapping tagMapping)
        {
            await _productProductTagMappingRepository.InsertAsync(tagMapping);
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual async Task InsertProductTagAsync(ProductTag productTag)
        {
            await _productTagRepository.InsertAsync(productTag);
        }

        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Result</returns>
        public virtual async Task<bool> ProductTagExistsAsync(Product product, int productTagId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return await _productProductTagMappingRepository.Table.AnyAsync(pptm => pptm.ProductId == product.Id && pptm.ProductTagId == productTagId);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual async Task UpdateProductTagAsync(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await _productTagRepository.UpdateAsync(productTag);

            var seName = await _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
            await _urlRecordService.SaveSlugAsync(productTag, seName, 0);
        }

        /// <summary>
        /// Get number of products
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Number of products</returns>
        public virtual async Task<int> GetProductCountAsync(int productTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetProductCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        public virtual async Task UpdateProductTagsAsync(Product product, string[] productTags)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //product tags
            var existingProductTags = await GetAllProductTagsByProductIdAsync(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (var newProductTag in productTags)
                {
                    if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found) 
                    productTagsToRemove.Add(existingProductTag);
            }

            foreach (var productTag in productTagsToRemove) 
                await DeleteProductProductTagMappingAsync(product.Id, productTag.Id);

            foreach (var productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = await GetProductTagByNameAsync(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    await InsertProductTagAsync(productTag);
                }
                else
                    productTag = productTag2;

                if (!await ProductTagExistsAsync(product, productTag.Id)) 
                    await InsertProductProductTagMappingAsync(new ProductProductTagMapping { ProductTagId = productTag.Id, ProductId = product.Id });

                var seName = await _urlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
                await _urlRecordService.SaveSlugAsync(productTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<ProductTag>.Prefix);
        }

        #endregion

        #region Nested class

        protected partial class ProductTagWithCount
        {
            /// <summary>
            /// Gets or sets the entity identifier
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the product tag ID
            /// </summary>
            public int ProductTagId { get; set; }

            /// <summary>
            /// Gets or sets the count
            /// </summary>
            public int ProductCount { get; set; }
        }

        #endregion
    }
}