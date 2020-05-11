using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tests.FakeServices;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class CalculateDimensionsTests : ServiceTest
    {
        private IPickupPluginManager _pickupPluginManager;
        private IProductService _productService;

        private IShippingPluginManager _shippingPluginManager;
        private IShippingService _shippingService;

        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IStoreContext> _storeContext;

        private ShippingSettings _shippingSettings;

        public CalculateDimensionsTests()
        {
            _shippingSettings = new ShippingSettings
            {
                UseCubeRootMethod = true,
                ConsiderAssociatedProductsDimensions = true,
                ShipSeparatelyOneItemEach = false
            };

            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var pluginService = new FakePluginService();
            _pickupPluginManager = new PickupPluginManager(new Mock<ICustomerService>().Object, pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(new Mock<ICustomerService>().Object, pluginService, _shippingSettings);

            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(new Store { Id = 1 });

            _productService = new FakeProductService(
                productRepository: _fakeDataStore.RegRepository<Product>());

            _shippingService = new FakeShippingService(
                eventPublisher: _eventPublisher.Object,
                pickupPluginManager: _pickupPluginManager,
                shippingPluginManager: _shippingPluginManager,
                storeContext: _storeContext.Object,
                shippingSettings: _shippingSettings);
        }

        [Test]
        public void Should_return_zero_with_all_zero_dimensions()
        {
            var product1 = new Product
            {
                Length = 0,
                Width = 0,
                Height = 0
            };

            _productService.InsertProduct(product1);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 1,
                        ProductId = product1.Id
                    }, product1)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);

            var product2 = new Product
            {
                Length = 0,
                Width = 0,
                Height = 0
            };

            _productService.InsertProduct(product2);

            items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        Quantity = 2,
                        ProductId = product2.Id
                    }, product2)
            };

            _shippingService.GetDimensions(items, out width, out length, out height);

            length.Should().Be(0);
            width.Should().Be(0);
            height.Should().Be(0);
        }

        [Test]
        public void Can_calculate_with_single_item_and_qty_1_should_ignore_cubic_method()
        {
            var product1 = new Product
            {
                Length = 2,
                Width = 3,
                Height = 4
            };

            _productService.InsertProduct(product1);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    ProductId = product1.Id
                }, product1)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(2);
            width.Should().Be(3);
            height.Should().Be(4);
        }

        [Test]
        public void Can_calculate_with_single_item_and_qty_2()
        {
            var product1 = new Product
            {
                Length = 2,
                Width = 4,
                Height = 4
            };

            _productService.InsertProduct(product1);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 2,
                    ProductId = product1.Id
                }, product1)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            length.Should().Be(4);
            width.Should().Be(4);
            height.Should().Be(4);
        }

        [Test]
        public void Can_calculate_with_cubic_item_and_multiple_qty()
        {
            var product1 = new Product
            {
                Length = 2,
                Width = 2,
                Height = 2
            };

            _productService.InsertProduct(product1);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 3,
                    ProductId = product1.Id
                }, product1)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            Math.Round(length, 2).Should().Be(2.88M);
            Math.Round(width, 2).Should().Be(2.88M);
            Math.Round(height, 2).Should().Be(2.88M);
        }

        [Test]
        public void Can_calculate_with_multiple_items_1()
        {
            var product1 = new Product
            {
                Length = 2,
                Width = 2,
                Height = 2
            };

            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Length = 3,
                Width = 5,
                Height = 2
            };

            _productService.InsertProduct(product2);

            var items = new List<GetShippingOptionRequest.PackageItem>
            {
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 3,
                                    ProductId = product1.Id
                                }, product1),
                new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                                {
                                    Quantity = 1,
                                    ProductId = product2.Id
                                }, product2)
            };

            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            Math.Round(length, 2).Should().Be(3.78M);
            Math.Round(width, 2).Should().Be(5);    //preserve max width
            Math.Round(height, 2).Should().Be(3.78M);
        }

        [Test]
        public void Can_calculate_with_multiple_items_2()
        {
            //take 8 cubes of 1x1x1 which is "packed" as 2x2x2 
            var items = new List<GetShippingOptionRequest.PackageItem>();

            for (var i = 0; i < 8; i++)
            {
                var product = new Product
                {
                    Length = 1,
                    Width = 1,
                    Height = 1
                };

                _productService.InsertProduct(product);

                items.Add(new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                {
                    Quantity = 1,
                    ProductId = product.Id
                }, product));

            }
            _shippingService.GetDimensions(items, out var width, out var length, out var height);

            Math.Round(length, 2).Should().Be(2);
            Math.Round(width, 2).Should().Be(2);
            Math.Round(height, 2).Should().Be(2);
        }
    }
}
