using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product tag service
    /// </summary>
    public partial class ProductTagService : IProductTagService
    {
        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductService _productService;
        private readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="productService">Product service</param>
        /// <param name="productProductTagMappingRepository">Product - product tag repository</param>
        /// <param name="productTagRepository">Product tag repository</param>
        /// <param name="staticCacheManager">Static cache manager</param>
        /// <param name="urlRecordService">Url record service</param>
        public ProductTagService(ICacheManager cacheManager,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            IProductService productService,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService)
        {
            this._cacheManager = cacheManager;
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
            this._productService = productService;
            this._productProductTagMappingRepository = productProductTagMappingRepository;
            this._productTagRepository = productTagRepository;
            this._staticCacheManager = staticCacheManager;
            this._urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        private Dictionary<int, int> GetProductCount(int storeId)
        {
            var key = string.Format(NopCatalogDefaults.ProductTagCountCacheKey, storeId);
            return _staticCacheManager.Get(key, () =>
            {
                return _dbContext.QueryFromSql<ProductTagWithCount>($"Exec ProductTagCountLoadAll {storeId}")
                    .ToDictionary(item => item.ProductTagId, item => item.ProductCount);

            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void DeleteProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            _productTagRepository.Delete(productTag);

            //cache
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);

            //event notification
            _eventPublisher.EntityDeleted(productTag);
        }

        /// <summary>
        /// Gets all product tags
        /// </summary>
        /// <returns>Product tags</returns>
        public virtual IList<ProductTag> GetAllProductTags()
        {
            var query = _productTagRepository.Table;
            var productTags = query.ToList();
            return productTags;
        }

        /// <summary>
        /// Gets all product tags by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product tags</returns>
        public virtual IList<ProductTag> GetAllProductTagsByProductId(int productId)
        {
            var key = string.Format(NopCatalogDefaults.ProductTagAllByProductIdCacheKey, productId);
            return _cacheManager.Get(key, () =>
            {
                var query = from pt in _productTagRepository.Table
                            join ppt in _productProductTagMappingRepository.Table on pt.Id equals ppt.ProductTagId
                            where ppt.ProductId == productId
                            orderby pt.Id
                            select pt;

                var productTags = query.ToList();
                return productTags;
            });
        }

        /// <summary>
        /// Gets product tag
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>Product tag</returns>
        public virtual ProductTag GetProductTagById(int productTagId)
        {
            if (productTagId == 0)
                return null;

            return _productTagRepository.GetById(productTagId);
        }

        /// <summary>
        /// Gets product tag by name
        /// </summary>
        /// <param name="name">Product tag name</param>
        /// <returns>Product tag</returns>
        public virtual ProductTag GetProductTagByName(string name)
        {
            var query = from pt in _productTagRepository.Table
                        where pt.Name == name
                        select pt;

            var productTag = query.FirstOrDefault();
            return productTag;
        }

        /// <summary>
        /// Inserts a product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void InsertProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            _productTagRepository.Insert(productTag);

            //cache
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);

            //event notification
            _eventPublisher.EntityInserted(productTag);
        }

        /// <summary>
        /// Updates the product tag
        /// </summary>
        /// <param name="productTag">Product tag</param>
        public virtual void UpdateProductTag(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException(nameof(productTag));

            _productTagRepository.Update(productTag);

            var seName = _urlRecordService.ValidateSeName(productTag, "", productTag.Name, true);
            _urlRecordService.SaveSlug(productTag, seName, 0);

            //cache
            _cacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);
            _staticCacheManager.RemoveByPattern(NopCatalogDefaults.ProductTagPatternCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(productTag);
        }

        /// <summary>
        /// Get number of products
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Number of products</returns>
        public virtual int GetProductCount(int productTagId, int storeId)
        {
            var dictionary = GetProductCount(storeId);
            if (dictionary.ContainsKey(productTagId))
                return dictionary[productTagId];

            return 0;
        }

        /// <summary>
        /// Update product tags
        /// </summary>
        /// <param name="product">Product for update</param>
        /// <param name="productTags">Product tags</param>
        public virtual void UpdateProductTags(Product product, string[] productTags)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //product tags
            var existingProductTags = GetAllProductTagsByProductId(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (var existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (var newProductTag in productTags)
                {
                    if (existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    productTagsToRemove.Add(existingProductTag);
                }
            }
            foreach (var productTag in productTagsToRemove)
            {
                //product.ProductTags.Remove(productTag);
                product.ProductProductTagMappings
                    .Remove(product.ProductProductTagMappings.FirstOrDefault(mapping => mapping.ProductTagId == productTag.Id));
                _productService.UpdateProduct(product);
            }
            foreach (var productTagName in productTags)
            {
                ProductTag productTag;
                var productTag2 = GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }
                if (!_productService.ProductTagExists(product, productTag.Id))
                {
                    //product.ProductTags.Add(productTag);
                    product.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productTag });
                    _productService.UpdateProduct(product);
                }

                var seName = _urlRecordService.ValidateSeName(productTag, "", productTag.Name, true);
                _urlRecordService.SaveSlug(productTag, seName, 0);

            }
        }

        #endregion
    }
}