using System;
using System.Collections.Generic;
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
    public class CalculateDimensionsTests : ServiceTest
    {
        private ShippingSettings _shippingSettings;
        private Mock<IRepository<ShippingMethod>> _shippingMethodRepository;
        private Mock<IRepository<Warehouse>> _warehouseRepository;
        private NullLogger _logger;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private Mock<ICheckoutAttributeParser> _checkoutAttributeParser;
        private Mock<IProductService> _productService;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IAddressService> _addressService;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Store _store;
        private Mock<IStoreContext> _storeContext;
        private ShoppingCartSettings _shoppingCartSettings;
        private ShippingService _shippingService;
        private Mock<IPriceCalculationService> _priceCalcService;
        private IPickupPluginManager _pickupPluginManager;
        private IShippingPluginManager _shippingPluginManager;
        private CatalogSettings _catalogSettings;

        [SetUp]
        public new void SetUp()
        {
            _shippingSettings = new ShippingSettings
            {
                UseCubeRootMethod = true,
                ConsiderAssociatedProductsDimensions = true,
                ShipSeparatelyOneItemEach = false
            };

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
        public void should_return_zero_with_all_zero_dimensions()
        {
            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 1,
                        Product = new Product
                        {
                            Length = 0,
                            Width = 0,
                            Height = 0
                        }
                    })
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            length.ShouldEqual(0);
            width.ShouldEqual(0);
            height.ShouldEqual(0);

            items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 2,
                        Product = new Product
                        {
                            Length = 0,
                            Width = 0,
                            Height = 0
                        }
                    })
            };

            _shippingService.GetDimensions(items, out width, out length, out height);
            length.ShouldEqual(0);
            width.ShouldEqual(0);
            height.ShouldEqual(0);
        }

        [Test]
        public void can_calculate_with_single_item_and_qty_1_should_ignore_cubic_method()
        {
            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    Product = new Product
                    {
                        Length = 2,
                        Width = 3,
                        Height = 4
                    }
                })
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            length.ShouldEqual(2);
            width.ShouldEqual(3);
            height.ShouldEqual(4);
        }

        [Test]
        public void can_calculate_with_single_item_and_qty_2()
        {
            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 2,
                    Product = new Product
                    {
                        Length = 2,
                        Width = 4,
                        Height = 4
                    }
                })
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            length.ShouldEqual(4);
            width.ShouldEqual(4);
            height.ShouldEqual(4);
        }

        [Test]
        public void can_calculate_with_cubic_item_and_multiple_qty()
        {
            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 3,
                    Product = new Product
                    {
                        Length = 2,
                        Width = 2,
                        Height = 2
                    }
                })
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            Math.Round(length, 2).ShouldEqual(2.88);
            Math.Round(width, 2).ShouldEqual(2.88);
            Math.Round(height, 2).ShouldEqual(2.88);
        }

        [Test]
        public void can_calculate_with_multiple_items_1()
        {
            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 3,
                                    Product = new Product
                                    {
                                        Length = 2,
                                        Width = 2,
                                        Height = 2
                                    }
                                }),
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 1,
                                    Product = new Product
                                    {
                                        Length = 3,
                                        Width = 5,
                                        Height = 2
                                    }
                                })
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            Math.Round(length, 2).ShouldEqual(3.78);
            Math.Round(width, 2).ShouldEqual(5);    //preserve max width
            Math.Round(height, 2).ShouldEqual(3.78);
        }

        [Test]
        public void can_calculate_with_multiple_items_2()
        {
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
            var items = new List<GetShippingOptionRequest.PackageItem>();
            for (var i = 0; i < 8; i++)
                items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    Product = new Product
                    {
                        Length = 1,
                        Width = 1,
                        Height = 1
                    }
                }));

            _shippingService.GetDimensions(items, out var width, out var length, out var height);
            Math.Round(length, 2).ShouldEqual(2);
            Math.Round(width, 2).ShouldEqual(2);
            Math.Round(height, 2).ShouldEqual(2);
        }
    }
}
