using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcCategoryService : CategoryService, IAbcCategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public AbcCategoryService(
            IAclService aclService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<Category> categoryRepository,
            IRepository<DiscountCategoryMapping> discountCategoryMappingRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext
        ) : base(
            aclService,
            customerService,
            localizationService,
            categoryRepository,
            discountCategoryMappingRepository,
            productRepository,
            productCategoryRepository,
            staticCacheManager,
            storeContext,
            storeMappingService,
            workContext
        )
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            var query = from c in _categoryRepository.Table
                where c.Name == name
                select c;

            return await query.FirstOrDefaultAsync();
        }
    }
}