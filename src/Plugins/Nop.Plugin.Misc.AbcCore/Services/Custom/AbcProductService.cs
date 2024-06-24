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
        private readonly INopDataProvider _nopDataProvider;
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
            IPictureService pictureService,
            INopDataProvider nopDataProvider
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
            _nopDataProvider = nopDataProvider;
        }

        public async Task<IList<Product>> GetAllPublishedProductsAsync()
        {
            return await GetProductsByIdsAsync(GetAllPublishedProductsIds().ToArray());
        }

        public async Task<IList<Product>> GetProductsWithoutImagesAsync()
        {
            var productIds = await _nopDataProvider.QueryAsync<int>(@"
                IF OBJECT_ID('tempdb..#nonPromoPpmIds') IS NOT NULL
                DROP TABLE #nonPromoPpmIds;

                -- get non-promo picture mappings 
                select ppm.Id, ppm.ProductId
                INTO #nonPromoPpmIds
                from Product_Picture_Mapping ppm
                join Picture pi on pi.Id = ppm.PictureId
                where SeoFilename not like '%promo%'

                -- get products without images
                select pr.Id from Product pr
                left join #nonPromoPpmIds ppm on ppm.ProductId = pr.Id
                where Published = 1 and ppm.ProductId is null
            ");

            return await GetProductsByIdsAsync(productIds.ToArray());
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
                        p => !p.Deleted && p.MarkAsNew).ToList()
                    .Select(p => p.Id);
        }
    }
}
