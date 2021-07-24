using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.MrPoly.Catalog
{
    public class PolyProductService : ProductService
    {
        private readonly IRepository<Product> _productRepository;
        public PolyProductService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IRepository<AclRecord> aclRepository,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<Product> productRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IRepository<TierPrice> tierPriceRepository,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)

            : base(
            catalogSettings,
            commonSettings,
            aclService,
            cacheManager,
            dataProvider,
            dateRangeService,
            dbContext,
            eventPublisher,
            languageService,
            localizationService,
            productAttributeParser,
            productAttributeService,
            aclRepository,
            crossSellProductRepository,
            productRepository,
            productPictureRepository,
            productReviewRepository,
            productWarehouseInventoryRepository,
            relatedProductRepository,
            stockQuantityHistoryRepository,
            storeMappingRepository,
            tierPriceRepository,
            storeService,
            storeMappingService,
            workContext,
            localizationSettings)
        {
            _productRepository = productRepository;
        }

        public override IList<Product> GetAllProductsDisplayedOnHomepage()
        {
            var query = from p in _productRepository.Table
                        orderby p.CreatedOnUtc descending
                        where p.Published &&
                        !p.Deleted
                        && p.CreatedOnUtc >= DateTime.UtcNow.AddDays(-14)
                        select p;
            var products = query.ToList();
            return products;
        }
    }
}
