using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nop.Core;
using Nop.Data;
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
    public class ShippingServiceTests : ServiceTest
    {
        #region Fields

        private IPickupPluginManager _pickupPluginManager;
        private IProductService _productService;
        private IShippingPluginManager _shippingPluginManager;
        private IShippingService _shippingService;
        private IRepository<Product> _productRepository;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IStoreContext> _storeContext;
        private ShippingSettings _shippingSettings;

        #endregion

        #region Ctor

        public ShippingServiceTests()
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _productRepository = _fakeDataStore.RegRepository(new[] {
                new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M
                },
                new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M
                }
            });

            _productService = new FakeProductService(productRepository: _productRepository, eventPublisher: _eventPublisher.Object);

            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string> { "FixedRateTestShippingRateComputationMethod" }
            };

            var pluginService = new FakePluginService();

            _pickupPluginManager = new PickupPluginManager(new Mock<ICustomerService>().Object, pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(new Mock<ICustomerService>().Object, pluginService, _shippingSettings);

            _storeContext = new Mock<IStoreContext>();
            _storeContext.Setup(x => x.CurrentStore).Returns(new Store { Id = 1 });

            _shippingService = new FakeShippingService(eventPublisher: _eventPublisher.Object,
                pickupPluginManager: _pickupPluginManager,
                productService: _productService,
                shippingPluginManager: _shippingPluginManager,
                storeContext: _storeContext.Object,
                shippingSettings: _shippingSettings);

        }

        #endregion

        #region Setup

        [SetUp]
        public new void SetUp()
        {

        } 

        #endregion

        [Test]
        public void Can_load_shippingRateComputationMethods()
        {
            RunWithTestServiceProvider(() =>
            {
                var srcm = _shippingPluginManager.LoadAllPlugins();
                srcm.Should().NotBeNull();
                srcm.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_load_shippingRateComputationMethod_by_systemKeyword()
        {
            RunWithTestServiceProvider(() =>
            {
                var srcm = _shippingPluginManager.LoadPluginBySystemName("FixedRateTestShippingRateComputationMethod");
                srcm.Should().NotBeNull();
            });
        }

        [Test]
        public void Can_load_active_shippingRateComputationMethods()
        {
            RunWithTestServiceProvider(() =>
            {
                var srcm = _shippingPluginManager.LoadActivePlugins(_shippingSettings
                    .ActiveShippingRateComputationMethodSystemNames);
                srcm.Should().NotBeNull();
                srcm.Any().Should().BeTrue();
            });
        }

        [Test]
        public void Can_get_shoppingCart_totalWeight_without_attributes()
        {

            var product1 = _productRepository.GetById(1);
            var product2 = _productRepository.GetById(2);

            var request = new GetShippingOptionRequest
            {
                Items =
                {
                    new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        AttributesXml = "",
                        Quantity = 3,
                        ProductId = product1.Id
                    }, product1),
                    new GetShippingOptionRequest.PackageItem(new ShoppingCartItem
                    {
                        AttributesXml = "",
                        Quantity = 4,
                        ProductId = product2.Id
                    }, product2)
                }
            };
            RunWithTestServiceProvider(() =>
            {
                _shippingService.GetTotalWeight(request).Should().Be(50.5M);
            });
        }
    }
}
