using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Seo;

namespace Nop.Plugin.Data.MySQL.Services.Catalog
{
    public class ProductTagService : Nop.Services.Catalog.ProductTagService
    {
        #region Fileds

        private readonly IRepository<ProductProductTagMapping> _productProductTagMappingRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;

        #endregion

        #region Ctor

        public ProductTagService(ICacheManager cacheManager,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            IProductService productService,
            IRepository<ProductProductTagMapping> productProductTagMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IStaticCacheManager staticCacheManager,
            IUrlRecordService urlRecordService,
            IRepository<Product> productRepository,
            IRepository<StoreMapping> storeMappingRepository)
            : base(cacheManager,
                dbContext,
                eventPublisher,
                productService,
                productProductTagMappingRepository,
                productTagRepository,
                staticCacheManager,
                urlRecordService)
        {
            _productProductTagMappingRepository = productProductTagMappingRepository;
            _productTagRepository = productTagRepository;
            _staticCacheManager = staticCacheManager;
            _productRepository = productRepository;
            _storeMappingRepository = storeMappingRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get product count for each of existing product tag
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Dictionary of "product tag ID : product count"</returns>
        protected override Dictionary<int, int> GetProductCount(int storeId)
        {
            var key = string.Format(NopCatalogDefaults.ProductTagCountCacheKey, storeId);
            return _staticCacheManager.Get(key, () =>
            {
                return (from pt in _productTagRepository.Table
                        join ptm in _productProductTagMappingRepository.Table on pt.Id equals ptm.ProductTagId
                        join p in _productRepository.Table on ptm.ProductId equals p.Id
                        where !p.Deleted && p.Published &&
                              (storeId == 0 || !p.LimitedToStores || (from sm in _storeMappingRepository.Table
                                   where sm.EntityId == p.Id && sm.EntityName == "Product" && sm.StoreId == storeId
                                   select sm.EntityId).Any())
                        select new { ProductTagId = pt.Id, ProductId = p.Id }).GroupBy(arg => arg.ProductTagId)
                    .AsNoTracking().ToList()
                    .ToDictionary(item => item.Key, item => item.Count());
            });
        }

        #endregion
    }
}
