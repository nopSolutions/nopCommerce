using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.MrPoly.Catalog
{
    public class PolyProductService : ProductService
    {
        public PolyProductService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IRepository<CrossSellProduct> crossSellProductRepository,
            IRepository<DiscountProductMapping> discountProductMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductPicture> productPictureRepository,
            IRepository<ProductProductTagMapping> productTagMappingRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IRepository<ProductSpecificationAttribute> productSpecificationAttributeRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<TierPrice> tierPriceRepository,
            IRepository<Warehouse> warehouseRepository,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)

            : base(
            catalogSettings,
            commonSettings,
            aclService,
            customerService,
            dateRangeService,
            languageService,
             localizationService,
             productAttributeParser,
             productAttributeService,
            crossSellProductRepository,
            discountProductMappingRepository,
            localizedPropertyRepository,
            productRepository,
            productAttributeCombinationRepository,
            productAttributeMappingRepository,
            productCategoryRepository,
            productManufacturerRepository,
            productPictureRepository,
            productTagMappingRepository,
            productReviewRepository,
            productReviewHelpfulnessRepository,
            productSpecificationAttributeRepository,
            productTagRepository,
            productWarehouseInventoryRepository,
            relatedProductRepository,
            shipmentRepository,
            stockQuantityHistoryRepository,
            tierPriceRepository,
            warehouseRepository,
            staticCacheManager,
            storeService,
            storeMappingService,
            workContext,
            localizationSettings)
        {

        }

        public override async Task<IList<Product>> GetAllProductsDisplayedOnHomepageAsync()
        {
            var query = from p in _productRepository.Table
                        orderby p.CreatedOnUtc descending
                        where p.Published &&
                        !p.Deleted
                        && p.CreatedOnUtc >= DateTime.UtcNow.AddDays(-14)
                        select p;
            var products = await query.ToListAsync();
            return products;
        }
    }
}
