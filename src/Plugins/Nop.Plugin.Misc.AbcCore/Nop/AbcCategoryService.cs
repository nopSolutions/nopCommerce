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
using Nop.Services.Catalog;
using Nop.Services.Seo;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcCategoryService : CategoryService, IAbcCategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUrlRecordService _urlRecordService;

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
            IWorkContext workContext,
            IUrlRecordService urlRecordService
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
            _urlRecordService = urlRecordService;
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