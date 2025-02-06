using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Core;
using Nop.Core.Domain.Discounts;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Seo;
using Nop.Core.Domain.Vendors;
using SevenSpikes.Nop.Services.Catalog;
using Nop.Services.Configuration;
using SevenSpikes.Nop.Services.Helpers;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcManufacturerService7Spikes : ManufacturerService7Spikes, IManufacturerService7Spikes
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        public AbcManufacturerService7Spikes(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            ICategoryService7Spikes categoryService7Spikes,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IAclHelper aclHelper,
            IStoreHelper storeHelper,
            IStoreContext storeContext,
            // custom
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICategoryService categoryService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext
        ) : base(
            productRepository,
            categoryRepository,
            manufacturerRepository,
            vendorRepository,
            productCategoryRepository,
            productManufacturerRepository,
            categoryService7Spikes,
            settingService,
            staticCacheManager,
            aclHelper,
            storeHelper,
            storeContext
        )
        {
            _aclService = aclService;
            _catalogSettings = catalogSettings;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            // custom
            _aclService = aclService;
            _categoryService = categoryService;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        // Needs "new" since base class doesn't provide virtual
        // Pulled this rationale from ManufacturerService but changed out the cache key
        public new async Task<IList<Manufacturer>> GetManufacturersByCategoryIdAsync(
            int categoryId,
            bool includeProductsInSubcategories = false,
            bool showHiddenProducts = false
        )
        {
            if (categoryId <= 0)
                return new List<Manufacturer>();

            // get available products in category
            var productsQuery = 
                from p in _productRepository.Table
                where !p.Deleted && p.Published &&
                      (p.ParentGroupedProductId == 0 || p.VisibleIndividually) &&
                      (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc <= DateTime.UtcNow) &&
                      (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc >= DateTime.UtcNow)
                select p;

            var store = await _storeContext.GetCurrentStoreAsync();
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            //apply store mapping constraints
            productsQuery = await _storeMappingService.ApplyStoreMapping(productsQuery, store.Id);

            //apply ACL constraints
            productsQuery = await _aclService.ApplyAcl(productsQuery, currentCustomer);

            var subCategoryIds = _catalogSettings.ShowProductsFromSubcategories
                ? await _categoryService.GetChildCategoryIdsAsync(categoryId, store.Id)
                : null;

            var productCategoryQuery = 
                from pc in _productCategoryRepository.Table
                where (pc.CategoryId == categoryId || (_catalogSettings.ShowProductsFromSubcategories && subCategoryIds.Contains(pc.CategoryId))) &&
                      (_catalogSettings.IncludeFeaturedProductsInNormalLists || !pc.IsFeaturedProduct)
                select pc;

            // get manufacturers of the products
            var manufacturersQuery =
                from m in _manufacturerRepository.Table
                join pm in _productManufacturerRepository.Table on m.Id equals pm.ManufacturerId
                join p in productsQuery on pm.ProductId equals p.Id
                join pc in productCategoryQuery on p.Id equals pc.ProductId
                where !m.Deleted
                orderby
                   m.DisplayOrder, m.Name
                select m;

            var key = _staticCacheManager
                .PrepareKeyForDefaultCache(
                    new CacheKey("Nop.manufacturer.bycategory.{0}.{1}", "Nop.manufacturer.bycategory."),
                    categoryId.ToString(),
                    store.Id);

            return await _staticCacheManager.GetAsync(key, async () => await manufacturersQuery.Distinct().ToListAsync());
        }
    }
}