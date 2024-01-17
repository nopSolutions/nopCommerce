using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.AbcCore.Services.Custom
{
    public class AbcProductService : ProductService, IAbcProductService
    {
        private readonly IPictureService _pictureService;

        public AbcProductService(
            CatalogSettings catalogSettings,
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
            LocalizationSettings localizationSettings,
            IPictureService pictureService
        ) : base(catalogSettings, commonSettings, aclService,
                customerService, dateRangeService,
                languageService, localizationService, productAttributeParser,
                productAttributeService, crossSellProductRepository,
                discountProductMappingRepository, localizedPropertyRepository,
                productRepository, productAttributeCombinationRepository,
                productAttributeMappingRepository, productCategoryRepository,
                productManufacturerRepository, productPictureRepository,
                productTagMappingRepository, productReviewRepository,
                productReviewHelpfulnessRepository,
                productSpecificationAttributeRepository, productTagRepository,
                productWarehouseInventoryRepository, relatedProductRepository,
                shipmentRepository, stockQuantityHistoryRepository,
                tierPriceRepository, warehouseRepository, staticCacheManager,
                storeService, storeMappingService, workContext, localizationSettings)
        {
            _pictureService = pictureService;
        }

        public async Task<IList<Product>> GetAllPublishedProductsAsync()
        {
            return await GetProductsByIdsAsync(GetAllPublishedProductsIds().ToArray());
        }

        public async Task<IList<Product>> GetProductsWithoutImagesAsync()
        {
            var publishedProductIds = GetAllPublishedProductsIds();
            var productIdsWithPicture = (await GetProductsImagesIdsAsync(publishedProductIds.ToArray())).Select(p => p.Key);

            return await GetProductsByIdsAsync(publishedProductIds.Except(productIdsWithPicture).ToArray());
        }

        public async Task<IList<Product>> GetNewProductsAsync()
        {
            return await GetProductsByIdsAsync(GetNewProductsIds().ToArray());
        }

        private IEnumerable<int> GetAllPublishedProductsIds()
        {
            return  _productRepository.Table.Where(
                        p => !p.Deleted &&
                        p.Published).ToList()
                    .Select(p => p.Id);
        }

        private IEnumerable<int> GetNewProductsIds()
        {
            return  _productRepository.Table.Where(
                        p => !p.Deleted && !p.Published &&
                        p.MarkAsNew).ToList()
                    .Select(p => p.Id);
        }
    }
}
