using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Tests;

namespace Nop.Services.Tests.FakeServices
{
    public class FakePriceCalculationService : PriceCalculationService
    {
        public FakePriceCalculationService(CatalogSettings catalogSettings = null,
            CurrencySettings currencySettings = null,
            ICacheKeyService cacheKeyService = null,
            ICategoryService categoryService = null,
            ICurrencyService currencyService = null,
            ICustomerService customerService = null,
            IDiscountService discountService = null,
            IManufacturerService manufacturerService = null,
            IProductAttributeParser productAttributeParser = null,
            IProductService productService = null,
            IStaticCacheManager staticCacheManager = null,
            IStoreContext storeContext = null,
            IWorkContext workContext = null) : base(
                catalogSettings ?? new CatalogSettings(),
                currencySettings ?? new CurrencySettings(),
                cacheKeyService ?? new FakeCacheKeyService(),
                categoryService ?? new Mock<ICategoryService>().Object,
                currencyService ?? new Mock<ICurrencyService>().Object,
                customerService ?? new Mock<ICustomerService>().Object,
                discountService ?? new Mock<IDiscountService>().Object,
                manufacturerService ?? new Mock<IManufacturerService>().Object,
                productAttributeParser ?? new Mock<IProductAttributeParser>().Object,
                productService ?? new Mock<IProductService>().Object,
                staticCacheManager ?? new TestCacheManager(),
                storeContext ?? new Mock<IStoreContext>().Object,
                workContext ?? new Mock<IWorkContext>().Object)
        {
        }
    }
}
