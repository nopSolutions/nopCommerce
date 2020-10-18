using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public virtual async Task DeleteProductProductTagMapping(int productId, int productTagId)
        {
            var mappingRecord = await _productProductTagMappingRepository.Table
                .ToAsyncEnumerable()
                .FirstOrDefaultAsync(pptm => pptm.ProductId == productId && pptm.ProductTagId == productTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await _productProductTagMappingRepository.Delete(mappingRecord);
        }

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private async Task<Dictionary<int, int>> GetProductCount(int storeId, bool showHidden)
        {
            var allowedCustomerRolesIds = string.Empty;
            if (!showHidden && !_catalogSettings.IgnoreAcl)
            {
                //Access control list. Allowed customer roles
                //pass customer role identifiers as comma-delimited string
                allowedCustomerRolesIds = string.Join(",", await _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()));
            }

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagCountCacheKey, storeId, 
                _customerService.GetCustomerRoleIds(await _workContext.GetCurrentCustomer()), 
                showHidden);
           
            return await _staticCacheManager.Get(key, async () =>
            {
                //prepare input parameters
                var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", storeId);
                var pAllowedCustomerRoleIds = SqlParameterHelper.GetStringParameter("AllowedCustomerRoleIds", allowedCustomerRolesIds);

                //invoke stored procedure
                return (await _dataProvider.QueryProc<ProductTagWithCount>("ProductTagCountLoadAll",
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
        public virtual async Task DeleteProductTag(ProductTag productTag)
        {
            await _productTagRepository.Delete(productTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        public virtual async Task DeleteProductTags(IList<ProductTag> productTags)
        {
            if (productTags == null)
                throw new ArgumentNullException(nameof(productTags));

            foreach (var productTag in productTags) 
                await DeleteProductTag(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <param name="tagName">Tag name</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTags(string tagName = null)
        {
            var allProductTags = await _productTagRepository.GetAll(getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName)) 
                allProductTags = allProductTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allProductTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTagsByProductId(int productId)
        {
            var productTags = await _productTagRepository.GetAll(query =>
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
        public virtual async Task<ProductTag> GetProductTagById(int productTagId)
        {
            return await _productTagRepository.GetById(productTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>Product tags</returns>
        public virtual async Task<IList<ProductTag>> GetProductTagsByIds(int[] productTagIds)
        {
            return await _productTagRepository.GetByIds(productTagIds);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public virtual async Task<ProductTag> GetProductTagByName(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = await query.ToAsyncEnumerable().FirstOrDefaultAsync();
            return productTag;
        }

        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        public virtual async Task InsertProductProductTagMapping(ProductProductTagMapping tagMapping)
        {
            await _productProductTagMappingRepository.Insert(tagMapping);
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual async Task InsertProductTag(ProductTag productTag)
        {
            await _productTagRepository.Insert(productTag);
        }

        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Result</returns>
        public virtual async Task<bool> ProductTagExists(Product product, int productTagId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return await _productProductTagMappingRepository.Table
                .ToAsyncEnumerable()
                .AnyAsync(pptm => pptm.ProductId == product.Id && pptm.ProductTagId == productTagId);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual async Task UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await _productTagRepository.Update(productTag);

            var seName = await _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
            await _urlRecordService.SaveSlug(productTag, seName, 0);
        }

        /// <summary>
        /// Get number of products
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Number of products</returns>
        public virtual async Task<int> GetProductCount(int productTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetProductCount(storeId, showHidden);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        public virtual async Task UpdateProductTags(Product product, string[] productTags)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //product tags
            var existingProductTags = await GetAllProductTagsByProductId(product.Id);
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
                await DeleteProductProductTagMapping(product.Id, productTag.Id);

            foreach (var productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = await GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    await InsertProductTag(productTag);
                }
                else
                    productTag = productTag2;

                if (!await ProductTagExists(product, productTag.Id)) 
                    await InsertProductProductTagMapping(new ProductProductTagMapping { ProductTagId = productTag.Id, ProductId = product.Id });

                var seName = await _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
                await _urlRecordService.SaveSlug(productTag, seName, 0);
            }

            //cache
            await _staticCacheManager.RemoveByPrefix(NopEntityCacheDefaults<ProductTag>.Prefix);
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