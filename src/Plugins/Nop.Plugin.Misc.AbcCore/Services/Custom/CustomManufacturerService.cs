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
    public class CustomManufacturerService : ManufacturerService, ICustomManufacturerService
    {
        private readonly IRepository<Manufacturer> _manufacturerRepository;

        public CustomManufacturerService(CatalogSettings catalogSettings,
            IAclService aclService,
            ICategoryService categoryService,
            ICustomerService customerService,
            IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Product> productRepository,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext
        ) : base(catalogSettings, aclService,
                categoryService, customerService,
                discountManufacturerMappingRepository, manufacturerRepository,
                productRepository,
                productManufacturerRepository, productCategoryRepository,
                staticCacheManager, storeContext,
                storeMappingService, workContext)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<IList<Manufacturer>> GetManufacturersByNameAsync(string name)
        {
            return await _manufacturerRepository.Table
                                          .Where(m => m.Name.ToLower() == name.ToLower())
                                          .ToListAsync();
        }
    }
}