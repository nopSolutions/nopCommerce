using Moq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Tests;

namespace Nop.Services.Tests.FakeServices
{
    public class FakeShippingService : ShippingService
    {
        public FakeShippingService(
            IAddressService addressService = null,
            ICacheKeyService cacheKeyService = null,
            ICheckoutAttributeParser checkoutAttributeParser = null,
            ICountryService countryService = null,
            ICustomerService customerSerice = null,
            IEventPublisher eventPublisher = null,
            IGenericAttributeService genericAttributeService = null,
            ILocalizationService localizationService = null,
            ILogger logger = null,
            IPickupPluginManager pickupPluginManager = null,
            IPriceCalculationService priceCalculationService = null,
            IProductAttributeParser productAttributeParser = null,
            IProductService productService = null,
            IRepository<ShippingMethod> shippingMethodRepository = null,
            IRepository<ShippingMethodCountryMapping> shippingMethodCountryMappingRepository = null,
            IRepository<Warehouse> warehouseRepository = null,
            IShippingPluginManager shippingPluginManager = null,
            IStateProvinceService stateProvinceService = null,
            IStoreContext storeContext = null,
            ShippingSettings shippingSettings = null,
            ShoppingCartSettings shoppingCartSettings = null) : base(
                addressService ?? new Mock<IAddressService>().Object,
                cacheKeyService ?? new FakeCacheKeyService(),
                checkoutAttributeParser ?? new Mock<ICheckoutAttributeParser>().Object,
                countryService ?? new Mock<ICountryService>().Object,
                customerSerice ?? new Mock<ICustomerService>().Object,
                eventPublisher ?? new Mock<IEventPublisher>().Object,
                genericAttributeService ?? new Mock<IGenericAttributeService>().Object,
                localizationService ?? new Mock<ILocalizationService>().Object,
                logger ?? new NullLogger(),
                pickupPluginManager ?? new Mock<IPickupPluginManager>().Object,
                priceCalculationService ?? new Mock<IPriceCalculationService>().Object,
                productAttributeParser ?? new Mock<IProductAttributeParser>().Object,
                productService ?? new Mock<IProductService>().Object,
                shippingMethodRepository.FakeRepoNullPropagation(),
                shippingMethodCountryMappingRepository.FakeRepoNullPropagation(),
                warehouseRepository.FakeRepoNullPropagation(),
                shippingPluginManager ?? new Mock<IShippingPluginManager>().Object,
                stateProvinceService ?? new Mock<IStateProvinceService>().Object,
                storeContext ?? new Mock<IStoreContext>().Object,
                shippingSettings ?? new ShippingSettings(),
                shoppingCartSettings ?? new ShoppingCartSettings())
        {
        }
    }
}
