using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Tests;

namespace Nop.Services.Tests
{
    public class FakeProductService : ProductService
    {
        public FakeProductService(CatalogSettings catalogSettings = null,
            CommonSettings commonSettings = null,
            IAclService aclService = null,
            ICacheKeyService cacheKeyService = null,
            ICustomerService customerService = null,
            INopDataProvider dataProvider = null,
            IDateRangeService dateRangeService = null,
            IEventPublisher eventPublisher = null,
            ILanguageService languageService = null,
            ILocalizationService localizationService = null,
            IProductAttributeParser productAttributeParser = null,
            IProductAttributeService productAttributeService = null,
            IRepository<AclRecord> aclRepository = null,
            IRepository<CrossSellProduct> crossSellProductRepository = null,
            IRepository<DiscountProductMapping> discountProductMappingRepository = null,
            IRepository<Product> productRepository = null,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository = null,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository = null,
            IRepository<ProductCategory> productCategoryRepository = null,
            IRepository<ProductPicture> productPictureRepository = null,
            IRepository<ProductReview> productReviewRepository = null,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository = null,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository = null,
            IRepository<RelatedProduct> relatedProductRepository = null,
            IRepository<Shipment> shipmentRepository = null,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository = null,
            IRepository<StoreMapping> storeMappingRepository = null,
            IRepository<TierPrice> tierPriceRepository = null,
            IRepository<Warehouse> warehouseRepository = null,
            IStaticCacheManager staticCacheManager = null,
            IStoreService storeService = null,
            IStoreMappingService storeMappingService = null,
            IWorkContext workContext = null,
            LocalizationSettings localizationSettings = null) : base(
                catalogSettings ?? new CatalogSettings(),
                commonSettings ?? new CommonSettings(),
                aclService ?? new Mock<IAclService>().Object,
                cacheKeyService ?? new FakeCacheKeyService(),
                customerService ?? new Mock<ICustomerService>().Object,
                dataProvider ?? new Mock<INopDataProvider>().Object,
                dateRangeService ?? new Mock<IDateRangeService>().Object,
                eventPublisher ?? new Mock<IEventPublisher>().Object,
                languageService ?? new Mock<ILanguageService>().Object,
                localizationService ?? new Mock<ILocalizationService>().Object,
                productAttributeParser ?? new Mock<IProductAttributeParser>().Object,
                productAttributeService ?? new Mock<IProductAttributeService>().Object,
                aclRepository.FakeRepoNullPropagation(),
                crossSellProductRepository.FakeRepoNullPropagation(),
                discountProductMappingRepository.FakeRepoNullPropagation(),
                productRepository.FakeRepoNullPropagation(),
                productAttributeCombinationRepository.FakeRepoNullPropagation(),
                productAttributeMappingRepository.FakeRepoNullPropagation(),
                productCategoryRepository.FakeRepoNullPropagation(),
                productPictureRepository.FakeRepoNullPropagation(),
                productReviewRepository.FakeRepoNullPropagation(),
                productReviewHelpfulnessRepository.FakeRepoNullPropagation(),
                productWarehouseInventoryRepository.FakeRepoNullPropagation(),
                relatedProductRepository.FakeRepoNullPropagation(),
                shipmentRepository.FakeRepoNullPropagation(),
                stockQuantityHistoryRepository.FakeRepoNullPropagation(),
                storeMappingRepository.FakeRepoNullPropagation(),
                tierPriceRepository.FakeRepoNullPropagation(),
                warehouseRepository.FakeRepoNullPropagation(),
                staticCacheManager ?? new TestCacheManager(),
                storeService ?? new Mock<IStoreService>().Object,
                storeMappingService ?? new Mock<IStoreMappingService>().Object,
                workContext ?? new Mock<IWorkContext>().Object,
                localizationSettings ?? new LocalizationSettings())
        {
        }
    }
}
