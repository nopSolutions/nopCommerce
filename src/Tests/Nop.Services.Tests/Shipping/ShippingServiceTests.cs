using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class ShippingServiceTests : ServiceTest
    {
        private Mock<IRepository<ShippingMethod>> _shippingMethodRepository;
        private Mock<IRepository<Warehouse>> _warehouseRepository;
        private ILogger _logger;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private Mock<ICheckoutAttributeParser> _checkoutAttributeParser;
        private ShippingSettings _shippingSettings;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IAddressService> _addressService;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private IShippingService _shippingService;
        private ShoppingCartSettings _shoppingCartSettings;
        private Mock<IProductService> _productService;
        private Store _store;
        private Mock<IStoreContext> _storeContext;
        private Mock<IPriceCalculationService> _priceCalcService;
        private IPickupPluginManager _pickupPluginManager;
        private IShippingPluginManager _shippingPluginManager;
        private CatalogSettings _catalogSettings;

        [SetUp]
        public new void SetUp()
        {
            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string>()
            };
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");

            _shippingMethodRepository = new Mock<IRepository<ShippingMethod>>();
            _warehouseRepository = new Mock<IRepository<Warehouse>>();
            _logger = new NullLogger();
            _productAttributeParser = new Mock<IProductAttributeParser>();
            _checkoutAttributeParser = new Mock<ICheckoutAttributeParser>();

            var cacheManager = new TestCacheManager();
            
            _productService = new Mock<IProductService>();

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var customerService = new Mock<ICustomerService>();
            var loger = new Mock<ILogger>();
            var webHelper = new Mock<IWebHelper>();

            _catalogSettings = new CatalogSettings();
            var pluginService = new PluginService(_catalogSettings, customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, webHelper.Object);

            _pickupPluginManager = new PickupPluginManager(pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(pluginService, _shippingSettings);

            _localizationService = new Mock<ILocalizationService>();
            _addressService = new Mock<IAddressService>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _priceCalcService = new Mock<IPriceCalculationService>();

            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _shoppingCartSettings = new ShoppingCartSettings();

            _shippingService = new ShippingService(_addressService.Object,
                cacheManager,
                _checkoutAttributeParser.Object,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _localizationService.Object,
                _logger,
                _pickupPluginManager,
                _priceCalcService.Object,
                _productAttributeParser.Object,
                _productService.Object,
                _shippingMethodRepository.Object,
                _warehouseRepository.Object,
                _shippingPluginManager,
                _storeContext.Object,
                _shippingSettings,
                _shoppingCartSettings);
        }

        [Test]
        public void Can_load_shippingRateComputationMethods()
        {
            var srcm = _shippingPluginManager.LoadAllPlugins();
            srcm.ShouldNotBeNull();
            srcm.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_shippingRateComputationMethod_by_systemKeyword()
        {
            var srcm = _shippingPluginManager.LoadPluginBySystemName("FixedRateTestShippingRateComputationMethod");
            srcm.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_shippingRateComputationMethods()
        {
            var srcm = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames);
            srcm.ShouldNotBeNull();
            srcm.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_get_shoppingCart_totalWeight_without_attributes()
        {
            var request = new GetShippingOptionRequest
            {
                Items =
                {
                    new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        AttributesXml = "",
                        Quantity = 3,
                        Product = new Product
                        {
                            Weight = 1.5M,
                            Height = 2.5M,
                            Length = 3.5M,
                            Width = 4.5M
                        }
                    }),
                    new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        AttributesXml = "",
                        Quantity = 4,
                        Product = new Product
                        {
                            Weight = 11.5M,
                            Height = 12.5M,
                            Length = 13.5M,
                            Width = 14.5M
                        }
                    })
                }
            };
            _shippingService.GetTotalWeight(request).ShouldEqual(50.5M);
        }
    }
}
