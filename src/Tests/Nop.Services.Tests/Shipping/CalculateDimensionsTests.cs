using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class CalculateDimensionsTests : ServiceTest
    {
        private IRepository<ShippingMethod> _shippingMethodRepository;
        private IRepository<DeliveryDate> _deliveryDateRepository;
        private IRepository<Warehouse> _warehouseRepository;
        private ILogger _logger;
        private IProductAttributeParser _productAttributeParser;
        private ICheckoutAttributeParser _checkoutAttributeParser;
        private ShippingSettings _shippingSettings;
        private IEventPublisher _eventPublisher;
        private ILocalizationService _localizationService;
        private IAddressService _addressService;
        private IGenericAttributeService _genericAttributeService;
        private IShippingService _shippingService;
        private ShoppingCartSettings _shoppingCartSettings;
        private IProductService _productService;
        private Store _store;
        private IStoreContext _storeContext;

        [SetUp]
        public new void SetUp()
        {
            _shippingSettings = new ShippingSettings();
            _shippingSettings.UseCubeRootMethod = true;

            _shippingMethodRepository = MockRepository.GenerateMock<IRepository<ShippingMethod>>();
            _deliveryDateRepository = MockRepository.GenerateMock<IRepository<DeliveryDate>>();
            _warehouseRepository = MockRepository.GenerateMock<IRepository<Warehouse>>();
            _logger = new NullLogger();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();
            _checkoutAttributeParser = MockRepository.GenerateMock<ICheckoutAttributeParser>();

            var cacheManager = new NopNullCache();

            var pluginFinder = new PluginFinder();
            _productService = MockRepository.GenerateMock<IProductService>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _localizationService = MockRepository.GenerateMock<ILocalizationService>();
            _addressService = MockRepository.GenerateMock<IAddressService>();
            _genericAttributeService = MockRepository.GenerateMock<IGenericAttributeService>();

            _store = new Store { Id = 1 };
            _storeContext = MockRepository.GenerateMock<IStoreContext>();
            _storeContext.Expect(x => x.CurrentStore).Return(_store);

            _shoppingCartSettings = new ShoppingCartSettings();
            _shippingService = new ShippingService(_shippingMethodRepository,
                _deliveryDateRepository,
                _warehouseRepository,
                _logger,
                _productService,
                _productAttributeParser,
                _checkoutAttributeParser,
                _genericAttributeService,
                _localizationService,
                _addressService,
                _shippingSettings,
                pluginFinder,
                _storeContext,
                _eventPublisher,
                _shoppingCartSettings,
                cacheManager);
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
                    }),
            };

            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
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
                    }),
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

            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
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

            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
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


            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
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

            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
            Math.Round(length, 2).ShouldEqual(3.78);
            Math.Round(width, 2).ShouldEqual(5);    //preserve max width
            Math.Round(height, 2).ShouldEqual(3.78);
        }

        [Test]
        public void can_calculate_with_multiple_items_2()
        {
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
            var items = new List<GetShippingOptionRequest.PackageItem>();
            for (int i = 0; i < 8; i++)
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

            decimal length, width, height;
            _shippingService.GetDimensions(items, out width, out length, out height);
            Math.Round(length, 2).ShouldEqual(2);
            Math.Round(width, 2).ShouldEqual(2);
            Math.Round(height, 2).ShouldEqual(2);
        }
    }
}
