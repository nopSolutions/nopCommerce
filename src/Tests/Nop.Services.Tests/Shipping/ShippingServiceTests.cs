using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
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

            var cacheManager = new NopNullCache();
            
            _productService = new Mock<IProductService>();

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var pluginFinder = new PluginFinder(_eventPublisher.Object);

            _localizationService = new Mock<ILocalizationService>();
            _addressService = new Mock<IAddressService>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _priceCalcService = new Mock<IPriceCalculationService>();

            _store = new Store { Id = 1 };
            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _shoppingCartSettings = new ShoppingCartSettings();

            _shippingService = new ShippingService(_shippingMethodRepository.Object,
                _warehouseRepository.Object,
                _logger,
                _productService.Object,
                _productAttributeParser.Object,
                _checkoutAttributeParser.Object,
                _genericAttributeService.Object,
                _localizationService.Object,
                _priceCalcService.Object,
                _addressService.Object,
                _shippingSettings, 
                pluginFinder, 
                _storeContext.Object,
                _eventPublisher.Object, 
                _shoppingCartSettings,
                cacheManager);
        }

        [Test]
        public void Can_load_shippingRateComputationMethods()
        {
            var srcm = _shippingService.LoadAllShippingRateComputationMethods();
            srcm.ShouldNotBeNull();
            srcm.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_load_shippingRateComputationMethod_by_systemKeyword()
        {
            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName("FixedRateTestShippingRateComputationMethod");
            srcm.ShouldNotBeNull();
        }

        [Test]
        public void Can_load_active_shippingRateComputationMethods()
        {
            var srcm = _shippingService.LoadActiveShippingRateComputationMethods();
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
