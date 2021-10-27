using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Fields

        protected IAclService AclService { get; }
        protected ICustomerService CustomerService { get; }
        protected IRepository<Product> ProductRepository { get; }
        protected IRepository<ProductProductTagMapping> ProductProductTagMappingRepository { get; }
        protected IRepository<ProductTag> ProductTagRepository { get; }
        protected IStaticCacheManager StaticCacheManager { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public ProductTagService(
            IAclService aclService,
            ICustomerService customerService,
            IRepository<Product> productRepository,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            AclService = aclService;
            CustomerService = customerService;
            ProductRepository = productRepository;
            ProductProductTagMappingRepository = productProductTagMappingRepository;
            ProductTagRepository = productTagRepository;
            StaticCacheManager = staticCacheManager;
            StoreMappingService = storeMappingService;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Delete a product-product tag mapping
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task DeleteProductProductTagMappingAsync(int productId, int productTagId)
        {
            var mappingRecord = await ProductProductTagMappingRepository.Table
                .FirstOrDefaultAsync(pptm => pptm.ProductId == productId && pptm.ProductTagId == productTagId);

            if (mappingRecord is null)
                throw new Exception("Mapping record not found");

            await ProductProductTagMappingRepository.DeleteAsync(mappingRecord);
        }

        /// <summary>
        /// Indicates whether a product tag exists
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> ProductTagExistsAsync(Product product, int productTagId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return await ProductProductTagMappingRepository.Table
                .AnyAsync(pptm => pptm.ProductId == product.Id && pptm.ProductTagId == productTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        protected virtual async Task<ProductTag> GetProductTagByNameAsync(string name)
        {
            var query = from pt in ProductTagRepository.Table
                where pt.Name == name
                select pt;

            var productTag = await query.FirstOrDefaultAsync();
            return productTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task InsertProductTagAsync(ProductTag productTag)
        {
            await ProductTagRepository.InsertAsync(productTag);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductTagAsync(ProductTag productTag)
        {
            await ProductTagRepository.DeleteAsync(productTag);
        }

        /// <summary>
        /// Delete product tags
        /// </summary>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTagsAsync(string tagName = null)
        {
            var allProductTags = await ProductTagRepository.GetAllAsync(query => query, getCacheKey: cache => default);

            if (!string.IsNullOrEmpty(tagName))
                allProductTags = allProductTags.Where(tag => tag.Name.Contains(tagName)).ToList();

            return allProductTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ProductTag>> GetAllProductTagsByProductIdAsync(int productId)
        {
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagsByProductCacheKey, productId);

            return await StaticCacheManager.GetAsync(key, async () =>
            {
                var tagMapping = from ptm in ProductProductTagMappingRepository.Table
                                 join pt in ProductTagRepository.Table on ptm.ProductTagId equals pt.Id
                                 where ptm.ProductId == productId
                                 orderby pt.Id
                                 select pt;

                return await tagMapping.ToListAsync();
            });
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tag
        /// </returns>
        public virtual async Task<ProductTag> GetProductTagByIdAsync(int productTagId)
        {
            return await ProductTagRepository.GetByIdAsync(productTagId, cache => default);
        }

        /// <summary>
        /// Gets product tags
        /// </summary>
        /// <param name="productTagIds">Product tags identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product tags
        /// </returns>
        public virtual async Task<IList<ProductTag>> GetProductTagsByIdsAsync(int[] productTagIds)
        {
            return await ProductTagRepository.GetByIdsAsync(productTagIds);
        }
        
        /// <summary>
        /// Inserts a product-product tag mapping
        /// </summary>
        /// <param name="tagMapping">Product-product tag mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductProductTagMappingAsync(ProductProductTagMapping tagMapping)
        {
            await ProductProductTagMappingRepository.InsertAsync(tagMapping);
        }
        
        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductTagAsync(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            await ProductTagRepository.UpdateAsync(productTag);

            var seName = await UrlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
            await UrlRecordService.SaveSlugAsync(productTag, seName, 0);
        }

        /// <summary>
        /// Get products quantity linked to a passed tag identifier
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of products
        /// </returns>
        public virtual async Task<int> GetProductCountByProductTagIdAsync(int productTagId, int storeId, bool showHidden = false)
        {
            var dictionary = await GetProductCountAsync(storeId, showHidden);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        /// <summary>
        /// Get product count for every linked tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dictionary of "product tag ID : product count"
        /// </returns>
        public virtual async Task<Dictionary<int, int>> GetProductCountAsync(int storeId, bool showHidden = false)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var customerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer);

            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopCatalogDefaults.ProductTagCountCacheKey, storeId, customerRoleIds, showHidden);

            return await StaticCacheManager.GetAsync(key, async () =>
            {
                var query = ProductProductTagMappingRepository.Table;

                if (!showHidden)
                {
                    var productsQuery = ProductRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    productsQuery = await StoreMappingService.ApplyStoreMapping(productsQuery, storeId);

                    //apply ACL constraints
                    productsQuery = await AclService.ApplyAcl(productsQuery, customerRoleIds);

                    query = query.Where(pc => productsQuery.Any(p => !p.Deleted && pc.ProductId == p.Id));
                }

                var pTagCount = from pt in ProductTagRepository.Table
                                join ptm in query on pt.Id equals ptm.ProductTagId
                                group ptm by ptm.ProductTagId into ptmGrouped
                                select new
                                {
                                    ProductTagId = ptmGrouped.Key,
                                    ProductCount = ptmGrouped.Count()
                                };

                return pTagCount.ToDictionary(item => item.ProductTagId, item => item.ProductCount);
            });
        }
        
        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        /// <returns>A task that represents the asynchronous operation</returns>
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

                var seName = await UrlRecordService.ValidateSeNameAsync(productTag, string.Empty, productTag.Name, true);
                await UrlRecordService.SaveSlugAsync(productTag, seName, 0);
            }

            //cache
            await StaticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<ProductTag>.Prefix);
        }

        #endregion
    }
}