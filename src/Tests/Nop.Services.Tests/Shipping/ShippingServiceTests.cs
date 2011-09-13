using System.Collections.Generic;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Shipping
{
    [TestFixture]
    public class ShippingServiceTests : ServiceTest
    {
        IRepository<ShippingMethod> _shippingMethodRepository;
        ILogger _logger;
        IProductAttributeParser _productAttributeParser;
        ICheckoutAttributeParser _checkoutAttributeParser;
        ShippingSettings _shippingSettings;
        IEventPublisher _eventPublisher;
        IShippingService _shippingService;

        [SetUp]
        public new void SetUp()
        {
            _shippingSettings = new ShippingSettings();
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames = new List<string>();
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");

            _shippingMethodRepository = MockRepository.GenerateMock<IRepository<ShippingMethod>>();
            _logger = new NullLogger();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();
            _checkoutAttributeParser = MockRepository.GenerateMock<ICheckoutAttributeParser>();

            var cacheManager = new NopNullCache();

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            _shippingService = new ShippingService(cacheManager, 
                _shippingMethodRepository, 
                _logger,
                _productAttributeParser,
                _checkoutAttributeParser,
                _shippingSettings, pluginFinder, _eventPublisher);
        }

        [Test]
        public void Can_load_shippingRateComputationMethods()
        {
            var srcm = _shippingService.LoadAllShippingRateComputationMethods();
            srcm.ShouldNotBeNull();
            (srcm.Count > 0).ShouldBeTrue();
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
            (srcm.Count > 0).ShouldBeTrue();
        }

        [Test]
        public void Can_get_shoppingCartItem_totalWeight_without_attributes()
        {
            var sci = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M
                }
            };
            _shippingService.GetShoppingCartItemTotalWeight(sci).ShouldEqual(4.5M);
        }

        [Test]
        public void Can_get_shoppingCart_totalWeight_without_attributes()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 4,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M
                }
            };
            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            _shippingService.GetShoppingCartTotalWeight(cart).ShouldEqual(50.5M);
        }

        [Test]
        public void Can_get_shoppingCartItem_additional_shippingCharge()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true,
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 4,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true,
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 5,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false,
                }
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2, sci3 };
            _shippingService.GetShoppingCartAdditionalShippingCharge(cart).ShouldEqual(42.5M);
        }

        [Test]
        public void Shipping_should_be_free_when_all_shoppingCartItems_are_marked_as_freeShipping()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true,
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 4,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true,
                }
            };
            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _shippingService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Shipping_should_not_be_free_when_some_of_shoppingCartItems_are_not_marked_as_freeShipping()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true,
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 4,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true,
                }
            };
            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _shippingService.IsFreeShipping(cart).ShouldEqual(false);
        }

        [Test]
        public void Shipping_should_be_free_when_customer_is_in_role_with_free_shipping()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true,
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 4,
                ProductVariant = new ProductVariant()
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true,
                }
            };
            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            var customer = new Customer();
            var customerRole1 = new CustomerRole()
            {
                Active = true,
                FreeShipping = true,
            };
            var customerRole2 = new CustomerRole()
            {
                Active = true,
                FreeShipping = false,
            };
            customer.CustomerRoles.Add(customerRole1);
            customer.CustomerRoles.Add(customerRole2);
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _shippingService.IsFreeShipping(cart).ShouldEqual(true);
        }
    }
}
