using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderTotalCalculationServiceTests : ServiceTest
    {
        private Mock<IWorkContext> _workContext;
        private Store _store;
        private Mock<IStoreContext> _storeContext;
        private Mock<IProductService> _productService;
        private IDiscountService _discountService;
        private Mock<ICategoryService> _categoryService;
        private Mock<IManufacturerService> _manufacturerService;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private CurrencySettings _currencySettings;
        private PriceCalculationService _priceCalculationService;
        private IPickupPluginManager _pickupPluginManager;
        private IShippingPluginManager _shippingPluginManager;
        private ITaxPluginManager _taxPluginManager;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IWebHelper> _webHelper;
        private ShippingSettings _shippingSettings;
        private Mock<IRepository<ShippingMethod>> _shippingMethodRepository;
        private Mock<IRepository<Warehouse>> _warehouseRepository;
        private NullLogger _logger;
        private ShippingService _shippingService;
        private Mock<IPaymentService> _paymentService;
        private Mock<ICheckoutAttributeParser> _checkoutAttributeParser;
        private Mock<IGiftCardService> _giftCardService;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<IGeoLookupService> _geoLookupService;
        private Mock<ICountryService> _countryService;
        private Mock<IStateProvinceService> _stateProvinceService;
        private CustomerSettings _customerSettings;
        private AddressSettings _addressSettings;
        private TaxSettings _taxSettings;
        private Mock<IAddressService> _addressService;
        private Mock<ICurrencyService> _currencyService;
        private Mock<IShoppingCartService> _shoppingCartService;
        private TaxService _taxService;
        private Mock<IRewardPointService> _rewardPointService;
        private RewardPointsSettings _rewardPointsSettings;
        private OrderTotalCalculationService _orderTotalCalcService;

        [SetUp]
        public new void SetUp()
        {
            _workContext = new Mock<IWorkContext>();
            _productService = new Mock<IProductService>();
            _storeContext = new Mock<IStoreContext>();
            _discountService = TestDiscountService.Init();
            _categoryService = new Mock<ICategoryService>();
            _manufacturerService = new Mock<IManufacturerService>();
            _productAttributeParser = new Mock<IProductAttributeParser>();
            _localizationService = new Mock<ILocalizationService>();
            _webHelper = new Mock<IWebHelper>();
            _shippingMethodRepository = new Mock<IRepository<ShippingMethod>>();
            _warehouseRepository = new Mock<IRepository<Warehouse>>();
            _eventPublisher = new Mock<IEventPublisher>();
            _paymentService = new Mock<IPaymentService>();
            _checkoutAttributeParser = new Mock<ICheckoutAttributeParser>();
            _giftCardService = new Mock<IGiftCardService>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _eventPublisher = new Mock<IEventPublisher>();
            _geoLookupService = new Mock<IGeoLookupService>();
            _countryService = new Mock<ICountryService>();
            _stateProvinceService = new Mock<IStateProvinceService>();
            _rewardPointService = new Mock<IRewardPointService>();
            _addressService = new Mock<IAddressService>();
            _currencyService = new Mock<ICurrencyService>();
            _shoppingCartService = new Mock<IShoppingCartService>();



            _store = new Store { Id = 1 };

            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            var cacheManager = new TestCacheManager();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();
            _currencySettings = new CurrencySettings();

            _priceCalculationService = new PriceCalculationService(_catalogSettings, _currencySettings, _categoryService.Object,
                _currencyService.Object, _discountService, _manufacturerService.Object,
                _productAttributeParser.Object, _productService.Object,
                cacheManager, _storeContext.Object,
                _workContext.Object, _shoppingCartSettings);

            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var customerService = new Mock<ICustomerService>();
            var loger = new Mock<ILogger>();

            var pluginService = new PluginService(_catalogSettings, customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, _webHelper.Object);

            _pickupPluginManager = new PickupPluginManager(pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(pluginService, _shippingSettings);
            _taxPluginManager = new TaxPluginManager(pluginService, _taxSettings);

            //shipping
            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string>(),
                AllowPickupInStore = true,
                IgnoreAdditionalShippingChargeForPickupInStore = false
            };
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");

            _logger = new NullLogger();

            _shippingService = new ShippingService(_addressService.Object,
                cacheManager,
                _checkoutAttributeParser.Object,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _localizationService.Object,
                _logger,
                _pickupPluginManager,
                _priceCalculationService,
                _productAttributeParser.Object,
                _productService.Object,
                _shippingMethodRepository.Object,
                _warehouseRepository.Object,
                _shippingPluginManager,
                _storeContext.Object,
                _shippingSettings,
                _shoppingCartSettings
            );

            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _customerSettings = new CustomerSettings();
            _addressSettings = new AddressSettings();

            //tax
            _taxSettings = new TaxSettings
            {
                ShippingIsTaxable = true,
                PaymentMethodAdditionalFeeIsTaxable = true,
                DefaultTaxAddressId = 10
            };

            _addressService.Setup(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Returns(new Address { Id = _taxSettings.DefaultTaxAddressId });

            _taxService = new TaxService(_addressSettings,
                _customerSettings,
                _addressService.Object,
                _countryService.Object,
                _genericAttributeService.Object,
                _geoLookupService.Object,
                _logger,
                _stateProvinceService.Object,
                new TestCacheManager(),
                _storeContext.Object,
                _taxPluginManager,
                _webHelper.Object,
                _workContext.Object,
                _shippingSettings,
                _taxSettings);

            _rewardPointsSettings = new RewardPointsSettings();

            _orderTotalCalcService = new OrderTotalCalculationService(_catalogSettings,
                _checkoutAttributeParser.Object,
                _discountService,
                _genericAttributeService.Object,
                _giftCardService.Object,
                _paymentService.Object,
                _priceCalculationService,
                _rewardPointService.Object,
                _shippingPluginManager,
                _shippingService,
                _shoppingCartService.Object,
                _storeContext.Object,
                _taxService,
                _workContext.Object,
                _rewardPointsSettings,
                _shippingSettings,
                _shoppingCartSettings,
                _taxSettings);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_excluding_tax()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out var discountAmount, out var appliedDiscounts,
                out var subTotalWithoutDiscount, out var subTotalWithDiscount, out var taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscounts.Count.ShouldEqual(0);
            subTotalWithoutDiscount.ShouldEqual(89.39);
            subTotalWithDiscount.ShouldEqual(89.39);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.939);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_including_tax()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out var discountAmount, out var appliedDiscounts,
                out var subTotalWithoutDiscount, out var subTotalWithDiscount, out var taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscounts.Count.ShouldEqual(0);
            subTotalWithoutDiscount.ShouldEqual(98.329);
            subTotalWithDiscount.ShouldEqual(98.329);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.939);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_discount_excluding_tax()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            (_discountService as TestDiscountService)?.AddDiscount(DiscountType.AssignedToOrderSubTotal);

            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out var discountAmount, out var appliedDiscounts,
                out var subTotalWithoutDiscount, out var subTotalWithDiscount, out var taxRates);
            discountAmount.ShouldEqual(3);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            subTotalWithoutDiscount.ShouldEqual(89.39);
            subTotalWithDiscount.ShouldEqual(86.39);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.639);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_discount_including_tax()
        {
            //customer
            var customer = new Customer();

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            (_discountService as TestDiscountService)?.AddDiscount(DiscountType.AssignedToOrderSubTotal);

            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out var discountAmount, out var appliedDiscounts,
                out var subTotalWithoutDiscount, out var subTotalWithDiscount,
                out var taxRates);

            //The comparison test failed before, because of a very tiny number difference.
            //discountAmount.ShouldEqual(3.3);
            (System.Math.Round(discountAmount, 10) == 3.3M).ShouldBeTrue();
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            subTotalWithoutDiscount.ShouldEqual(98.329);
            subTotalWithDiscount.ShouldEqual(95.029);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.639);
        }

        [Test]
        public void Can_get_shoppingCartItem_additional_shippingCharge()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                Product = new Product
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false
                }
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            _orderTotalCalcService.GetShoppingCartAdditionalShippingCharge(cart).ShouldEqual(42.5M);
        }

        [Test]
        public void Shipping_should_be_free_when_all_shoppingCartItems_are_marked_as_freeShipping()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true
                }
            };
            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Shipping_should_not_be_free_when_some_of_shoppingCartItems_are_not_marked_as_freeShipping()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = true,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true
                }
            };
            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(false);
        }

        [Test]
        public void Shipping_should_be_free_when_customer_is_in_role_with_free_shipping()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    IsFreeShipping = false,
                    IsShipEnabled = true
                }
            };
            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            var customer = new Customer();
            var customerRole1 = new CustomerRole
            {
                Active = true,
                FreeShipping = true
            };
            var customerRole2 = new CustomerRole
            {
                Active = true,
                FreeShipping = false
            };
            customer.CustomerRoles.Add(customerRole1);
            customer.CustomerRoles.Add(customerRole2);
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_excluding_tax()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                Product = new Product
                {
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false
                }
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, _store.Id, null))
                .Returns(new PickupPoint());

            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, _workContext.Object.CurrentCustomer, _storeContext.Object.CurrentStore.Id);

            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, false, shippingRateComputationMethods, out var taxRate, out var appliedDiscounts);
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(52.5);
            appliedDiscounts.Count.ShouldEqual(0);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_including_tax()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                Product = new Product
                {
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false
                }
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, _store.Id, null))
                .Returns(new PickupPoint());

            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, _workContext.Object.CurrentCustomer, _storeContext.Object.CurrentStore.Id);

            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, true, shippingRateComputationMethods, out var taxRate, out var appliedDiscounts);
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(57.75);
            appliedDiscounts.Count.ShouldEqual(0);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_excluding_tax()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                Product = new Product
                {
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false
                }
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, _store.Id, null))
                .Returns(new PickupPoint());

            (_discountService as TestDiscountService)?.AddDiscount(DiscountType.AssignedToShipping);

            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, _workContext.Object.CurrentCustomer, _storeContext.Object.CurrentStore.Id);

            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, false, shippingRateComputationMethods, out var taxRate, out var appliedDiscounts);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.ShouldEqual(49.5);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_including_tax()
        {
            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                Product = new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                }
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                Product = new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                }
            };

            //sci3 is not shippable
            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                Product = new Product
                {
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false
                }
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, _store.Id, null))
                .Returns(new PickupPoint());

            (_discountService as TestDiscountService)?.AddDiscount(DiscountType.AssignedToShipping);

            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, _workContext.Object.CurrentCustomer, _storeContext.Object.CurrentStore.Id);

            var shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, true, shippingRateComputationMethods, out var taxRate, out var appliedDiscounts);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.ShouldEqual(54.45);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_tax_total()
        {
            //customer
            var customer = new Customer
            {
                Id = 10
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _store.Id, null))
                .Returns("test1");

            _paymentService.Setup(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Returns(20);
            //_discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories, null, null, false)).Returns(new List<DiscountForCaching>());
            //_discountService.Setup(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers, null, null, false)).Returns(new List<DiscountForCaching>());

            //56 - items, 10 - shipping (fixed), 20 - payment fee

            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_shippingSettings.ActiveShippingRateComputationMethodSystemNames, _workContext.Object.CurrentCustomer, _storeContext.Object.CurrentStore.Id);

            //1. shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _orderTotalCalcService.GetTaxTotal(cart, shippingRateComputationMethods, out var taxRates).ShouldEqual(8.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.6);

            //2. shipping is taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, shippingRateComputationMethods, out taxRates).ShouldEqual(6.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(6.6);

            //3. shipping is not taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _orderTotalCalcService.GetTaxTotal(cart, shippingRateComputationMethods, out taxRates).ShouldEqual(7.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(7.6);

            //3. shipping is not taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, shippingRateComputationMethods, out taxRates).ShouldEqual(5.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(5.6);
        }

        [Test]
        public void Can_get_shopping_cart_total_without_shipping_required()
        {
            //customer
            var customer = new Customer
            {
                Id = 10
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = false
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = false
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _store.Id, null))
                .Returns("test1");

            _paymentService.Setup(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Returns(20);

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 20 - payment fee, 7.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out _, out _, out _, out _, out _)
                .ShouldEqual(83.6M);
        }

        [Test]
        public void Can_get_shopping_cart_total_with_shipping_required()
        {
            //customer
            var customer = new Customer
            {
                Id = 10
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _genericAttributeService.Setup(x => x.GetAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _store.Id, null))
                .Returns("test1");

            _paymentService.Setup(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Returns(20);

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out _, out _, out _, out _, out _)
                .ShouldEqual(94.6M);
        }

        /*TODO temporary disabled
        [Test]
        public void Can_get_shopping_cart_total_with_applied_reward_points()
        {
            //customer
            var customer = new Customer
            {
                Id = 10,
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2,
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);



            _genericAttributeService.Returns(x => x.GetAttributesForEntity(customer.Id, "Customer"))
                .Returns(new List<GenericAttribute>
                            {
                                new GenericAttribute
                                    {
                                        StoreId = _store.Id,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.SelectedPaymentMethod,
                                        KeyGroup = "Customer",
                                        Value = "test1"
                                    },
                                new GenericAttribute
                                        {
                                        StoreId = 1,
                                        EntityId = customer.Id,
                                        Key = SystemCustomerAttributeNames.UseRewardPointsDuringCheckout,
                                        KeyGroup = "Customer",
                                        Value = true.ToString()
                                        }
                            });
            _paymentService.Returns(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Returns(20);


            _discountService.Returns(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToCategories)).Returns(new List<DiscountForCaching>());
            _discountService.Returns(ds => ds.GetAllDiscountsForCaching(DiscountType.AssignedToManufacturers)).Returns(new List<DiscountForCaching>());

            decimal discountAmount;
            Discount appliedDiscount;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;


            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //reward points
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 2; //1 reward point = 2
            
            customer.AddRewardPointsHistoryEntry(15, 0); //15*2=30

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, -30 (reward points)
             _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(64.6M);
        }*/

        [Test]
        public void Can_get_shopping_cart_total_discount()
        {
            //customer
            var customer = new Customer
            {
                Id = 10
            };

            //shopping cart
            var product1 = new Product
            {
                Id = 1,
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };
            var sci1 = new ShoppingCartItem
            {
                Product = product1,
                ProductId = product1.Id,
                Quantity = 2
            };
            var product2 = new Product
            {
                Id = 2,
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };
            var sci2 = new ShoppingCartItem
            {
                Product = product2,
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            (_discountService as TestDiscountService)?.AddDiscount(DiscountType.AssignedToOrderTotal);

            _genericAttributeService.Setup(x =>
                    x.GetAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _store.Id, null))
                .Returns("test1");

            _paymentService.Setup(ps => ps.GetAdditionalHandlingFee(cart, "test1")).Returns(20);

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, [-3] - discount
            _orderTotalCalcService.GetShoppingCartTotal(cart, out var discountAmount, out var appliedDiscounts, out _, out _, out _)
                .ShouldEqual(91.6M);
            discountAmount.ShouldEqual(3);
            appliedDiscounts.Count.ShouldEqual(1);
            appliedDiscounts.First().Name.ShouldEqual("Discount 1");
        }

        [Test]
        public void Can_convert_reward_points_to_amount()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 15M;

            _orderTotalCalcService.ConvertRewardPointsToAmount(100).ShouldEqual(1500);
        }

        [Test]
        public void Can_convert_amount_to_reward_points()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.ExchangeRate = 15M;

            //we calculate ceiling for reward points
            _orderTotalCalcService.ConvertAmountToRewardPoints(100).ShouldEqual(7);
        }

        [Test]
        public void Can_check_minimum_reward_points_to_use_requirement()
        {
            _rewardPointsSettings.Enabled = true;
            _rewardPointsSettings.MinimumRewardPointsToUse = 0;

            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(0).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(1).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(10).ShouldEqual(true);

            _rewardPointsSettings.MinimumRewardPointsToUse = 2;
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(0).ShouldEqual(false);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(1).ShouldEqual(false);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(2).ShouldEqual(true);
            _orderTotalCalcService.CheckMinimumRewardPointsToUseRequirement(10).ShouldEqual(true);
        }
    }
}
