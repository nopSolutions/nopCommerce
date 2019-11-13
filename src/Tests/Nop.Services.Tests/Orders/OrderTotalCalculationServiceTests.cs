using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Services.Tests.FakeServices;
using Nop.Services.Tests.FakeServices.Providers;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderTotalCalculationServiceTests : ServiceTest
    {

        private IDiscountService _discountService;
        private ICustomerService _customerService;
        private IPriceCalculationService _priceCalculationService;
        private IOrderTotalCalculationService _orderTotalCalcService;
        private IProductService _productService;
        private IRepository<Customer> _customerRepository;
        private IRepository<CustomerRole> _customerRoleRepository;
        private IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private IRepository<Discount> _discountRepository;
        private IRepository<Product> _productRepository;
        private IShippingService _shippingService;
        private IShoppingCartService _shoppingCartService;
        private ITaxService _taxService;

        private Mock<IAddressService> _addressService = new Mock<IAddressService>();
        private Mock<ICurrencyService> _currencyService = new Mock<ICurrencyService>();
        private Mock<IEventPublisher> _eventPublisher = new Mock<IEventPublisher>();
        private Mock<IGenericAttributeService> _genericAttributeService = new Mock<IGenericAttributeService>();
        private Mock<IPaymentService> _paymentService = new Mock<IPaymentService>();
        private Mock<IStoreContext> _storeContext = new Mock<IStoreContext>();
        private Mock<IWorkContext> _workContext = new Mock<IWorkContext>();

        private RewardPointsSettings _rewardPointsSettings = new RewardPointsSettings();
        private ShippingPluginManager _shippingPluginManager;
        private ShippingSettings _shippingSettings;
        private ShoppingCartSettings _shoppingCartSettings = new ShoppingCartSettings();
        private TaxSettings _taxSettings;
        private FakeServiceProvider _serviceProvider;

        public OrderTotalCalculationServiceTests()
        {

            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string> { "FixedRateTestShippingRateComputationMethod" },
                AllowPickupInStore = true,
                IgnoreAdditionalShippingChargeForPickupInStore = false
            };

            _taxSettings = new TaxSettings
            {
                ShippingIsTaxable = true,
                PaymentMethodAdditionalFeeIsTaxable = true,
                DefaultTaxAddressId = 10
            };

            var pluginService = new FakePluginService(fileProvider: CommonHelper.DefaultFileProvider);

            var pickupPluginManager = new PickupPluginManager(pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(pluginService, _shippingSettings);
            var taxPluginManager = new TaxPluginManager(pluginService, _taxSettings);
            var discountPluginManager = new DiscountPluginManager(pluginService);

            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Weight = 1.5M,
                    Height = 2.5M,
                    Length = 3.5M,
                    Width = 4.5M,
                    AdditionalShippingCharge = 5.5M,
                    IsShipEnabled = true
                },
                new Product
                {
                    Id = 2,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 6.5M,
                    IsShipEnabled = true
                },
                new Product
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

            _productRepository = _fakeDataStore.RegRepository(products);
            _productService = new FakeProductService(productRepository: _productRepository);

            var store = new Store { Id = 1 };

            _storeContext.Setup(x => x.CurrentStore).Returns(store);
            _currencyService.Setup(x => x.GetCurrencyById(1, true)).Returns(new Currency { Id = 1, RoundingTypeId = 0 });
            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));
            _addressService.Setup(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Returns(new Address { Id = _taxSettings.DefaultTaxAddressId });
            _paymentService.Setup(ps => ps.GetAdditionalHandlingFee(It.IsAny<IList<ShoppingCartItem>>(), "test1")).Returns(20);

            _genericAttributeService.Setup(x => 
                x.GetAttribute<PickupPoint>(It.IsAny<Customer>(), NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.Object.CurrentStore.Id, null))
                .Returns(new PickupPoint());
            _genericAttributeService.Setup(x => x.GetAttribute<string>(It.IsAny<Customer>(), NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.Object.CurrentStore.Id, null))
                .Returns("test1");

            _customerRoleRepository = _fakeDataStore.RegRepository(new[]
            {
                new CustomerRole
                {
                    Id = 1,
                    Active = true,
                    FreeShipping = true
                },
                new CustomerRole
                {
                    Id = 2,
                    Active = true,
                    FreeShipping = false
                }
            });

            _customerRepository = _fakeDataStore.RegRepository(new[] { new Customer() { Id = 1 } });

            _customerCustomerRoleMappingRepository = _fakeDataStore.RegRepository<CustomerCustomerRoleMapping>();

            _customerService = new FakeCustomerService(
                customerRepository: _customerRepository, 
                customerRoleRepository: _customerRoleRepository,
                customerCustomerRoleMappingRepository: _customerCustomerRoleMappingRepository);

            var currencySettings = new CurrencySettings { PrimaryStoreCurrencyId = 1 };

            _discountRepository = _fakeDataStore.RegRepository<Discount>();

            _discountService = new FakeDiscountService(
                customerService: _customerService,
                discountPluginManager: discountPluginManager,
                productService: _productService,
                discountRepository: _discountRepository,
                storeContext: _storeContext.Object);

            _priceCalculationService = new FakePriceCalculationService(
                currencySettings: currencySettings,
                currencyService: _currencyService.Object,
                customerService: _customerService,
                discountService: _discountService,
                productService: _productService,
                storeContext: _storeContext.Object);

            _shoppingCartService = new FakeShoppingCartService(
                productService: _productService,
                customerService: _customerService,
                genericAttributeService: _genericAttributeService.Object,
                priceCalculationService: _priceCalculationService,
                shoppingCartSettings: _shoppingCartSettings);


            _shippingService = new FakeShippingService(eventPublisher: _eventPublisher.Object,
                customerSerice: _customerService,
                genericAttributeService: _genericAttributeService.Object,                
                pickupPluginManager: pickupPluginManager,
                productService: _productService,
                shippingPluginManager: _shippingPluginManager,
                storeContext: _storeContext.Object,
                shippingSettings: _shippingSettings);

            _taxService = new FakeTaxService(
                            addressService: _addressService.Object,
                            customerService: _customerService,
                            genericAttributeService: _genericAttributeService.Object,
                            storeContext: _storeContext.Object,
                            taxPluginManager: taxPluginManager,
                            shippingSettings: _shippingSettings,
                            taxSettings: _taxSettings);

            _orderTotalCalcService = new FakeOrderTotalCalculationService(
                addressService: _addressService.Object,
                customerService: _customerService,
                discountService: _discountService,
                genericAttributeService: _genericAttributeService.Object,
                paymentService: _paymentService.Object,
                priceCalculationService: _priceCalculationService,
                productService: _productService,
                shippingPluginManager: _shippingPluginManager,
                shippingService: _shippingService,
                shoppingCartService: _shoppingCartService,
                storeContext: _storeContext.Object,
                taxService: _taxService,
                shippingSettings: _shippingSettings,
                taxSettings: _taxSettings,
                rewardPointsSettings: _rewardPointsSettings);

            _serviceProvider = new FakeServiceProvider(_shoppingCartService, _paymentService.Object);

            var nopEngine = new Mock<NopEngine>();

            nopEngine.Setup(x => x.ServiceProvider).Returns(_serviceProvider);
            nopEngine.Setup(x => x.ResolveUnregistered(It.IsAny<Type>())).Returns( (Type type ) => Activator.CreateInstance(type));
            EngineContext.Replace(nopEngine.Object);
        }

        [SetUp]
        public override void SetUp()
        {
            _fakeDataStore.ResetStore();
        }

        #region Utilities

        private ShoppingCartItem CreateTestShopCartItem(decimal productPrice, int quantity = 1)
        {
            //customer
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product = new Product
            {
                Name = "Product name 1",
                Price = productPrice,
                CustomerEntersPrice = false,
                Published = true,
                //set HasTierPrices property
                HasTierPrices = true
            };

            _productService.InsertProduct(product);

            var shoppingCartItem = new ShoppingCartItem
            {
                CustomerId = customer.Id,
                ProductId = product.Id,
                Quantity = quantity
            };

            return shoppingCartItem;
        }
        #endregion

        [Test]
        public void Can_get_shopping_cart_subTotal_excluding_tax()
        {
            //customer
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _discountService.InsertDiscount(new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderSubTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            });

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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _discountService.InsertDiscount(new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderSubTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            });

            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out var discountAmount, out var appliedDiscounts,
                out var subTotalWithoutDiscount, out var subTotalWithDiscount,
                out var taxRates);

            //The comparison test failed before, because of a very tiny number difference.
            //discountAmount.ShouldEqual(3.3);
            (Math.Round(discountAmount, 10) == 3.3M).ShouldBeTrue();
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
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                AdditionalShippingCharge = 5.5M,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 6.5M,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            //sci3 is not shippable

            var product3 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 7.5M,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product3);

            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                ProductId = product3.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            _orderTotalCalcService.GetShoppingCartAdditionalShippingCharge(cart).ShouldEqual(42.5M);
        }

        [Test]
        public void Shipping_should_be_free_when_all_shoppingCartItems_are_marked_as_freeShipping()
        {
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                IsFreeShipping = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                IsFreeShipping = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };

            var customer = _customerService.GetCustomerById(1);

            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Shipping_should_not_be_free_when_some_of_shoppingCartItems_are_not_marked_as_freeShipping()
        {
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                IsFreeShipping = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                IsFreeShipping = false,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var customer = _customerService.GetCustomerById(1);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id,
                CustomerId = customer.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id,
                CustomerId = customer.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(false);
        }

        [Test]
        public void Shipping_should_be_free_when_customer_is_in_role_with_free_shipping()
        {
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                IsFreeShipping = false,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                IsFreeShipping = false,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };
            var cart = new List<ShoppingCartItem> { sci1, sci2 };

            var customer = new Customer();

            _customerService.InsertCustomer(customer);

            var customerRole1 = _customerRoleRepository.GetById(1);
            var customerRole2 = _customerRoleRepository.GetById(2);

            _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole1.Id });
            _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole2.Id });

            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _orderTotalCalcService.IsFreeShipping(cart).ShouldEqual(true);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_excluding_tax()
        {

            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                AdditionalShippingCharge = 5.5M,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 6.5M,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            //sci3 is not shippable

            var product3 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 7.5M,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product3);

            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                ProductId = product3.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = _customerService.GetCustomerById(1);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

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
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                AdditionalShippingCharge = 5.5M,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 6.5M,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            //sci3 is not shippable

            var product3 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 7.5M,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product3);

            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                ProductId = product3.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = _customerService.GetCustomerById(1);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

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
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                AdditionalShippingCharge = 5.5M,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 6.5M,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            //sci3 is not shippable

            var product3 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 7.5M,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product3);

            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                ProductId = product3.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };

            var customer = new Customer();
            _customerService.InsertCustomer(customer);

            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _discountService.InsertDiscount(new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToShipping,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            });


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
            var product1 = new Product
            {
                Weight = 1.5M,
                Height = 2.5M,
                Length = 3.5M,
                Width = 4.5M,
                AdditionalShippingCharge = 5.5M,
                IsShipEnabled = true
            };
            _productService.InsertProduct(product1);

            var product2 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 6.5M,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci1 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 3,
                ProductId = product1.Id
            };
            var sci2 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 4,
                ProductId = product2.Id
            };

            //sci3 is not shippable

            var product3 = new Product
            {
                Weight = 11.5M,
                Height = 12.5M,
                Length = 13.5M,
                Width = 14.5M,
                AdditionalShippingCharge = 7.5M,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product3);

            var sci3 = new ShoppingCartItem
            {
                AttributesXml = "",
                Quantity = 5,
                ProductId = product3.Id
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2, sci3 };
            var customer = _customerService.GetCustomerById(1);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _discountService.InsertDiscount(new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToShipping,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            });

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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);
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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = false
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);

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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            
            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out _, out _, out _, out _, out _)
                .ShouldEqual(94.6M);
        }

        [Test]
        public void Can_get_shopping_cart_item_unitPrice()
        {
            //customer
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product = new Product
            {
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                //set HasTierPrices property
                HasTierPrices = true
            };

            _productService.InsertProduct(product);

            var sci1 = new ShoppingCartItem
            {
                CustomerId = customer.Id,
                ProductId = product.Id,
                Quantity = 2
            };

            _shoppingCartService.GetUnitPrice(sci1).ShouldEqual(12.34);
        }

        [Test]
        public void Can_get_shopping_cart_item_subTotal()
        {
            //customer
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product = new Product
            {
                Id = 111,
                Name = "Product name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                //set HasTierPrices property
                HasTierPrices = true
            };

            _productService.InsertProduct(product);

            var sci1 = new ShoppingCartItem
            {
                CustomerId = customer.Id,
                ProductId = product.Id,
                Quantity = 2
            };

            _shoppingCartService.GetSubTotal(sci1).ShouldEqual(24.68);
        }

        [Test]
        [TestCase(12.00009, 12.00)]
        [TestCase(12.119, 12.12)]
        [TestCase(12.115, 12.12)]
        [TestCase(12.114, 12.11)]
        public void Test_GetUnitPrice_WhenRoundPricesDuringCalculationIsTrue_PriceMustBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = true;
            var resultPrice = _shoppingCartService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
        }

        [Test]
        [TestCase(12.00009, 12.00009)]
        [TestCase(12.119, 12.119)]
        [TestCase(12.115, 12.115)]
        [TestCase(12.114, 12.114)]
        public void Test_GetUnitPrice_WhenNotRoundPricesDuringCalculationIsFalse_PriceMustNotBeRounded(decimal inputPrice, decimal expectedPrice)
        {
            // arrange            
            var shoppingCartItem = CreateTestShopCartItem(inputPrice);

            // act
            _shoppingCartSettings.RoundPricesDuringCalculation = false;
            var resultPrice = _shoppingCartService.GetUnitPrice(shoppingCartItem);

            // assert
            resultPrice.ShouldEqual(expectedPrice);
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
            var customer = _customerService.GetCustomerById(1);

            //shopping cart
            var product1 = new Product
            {
                Name = "Product name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product1);

            var sci1 = new ShoppingCartItem
            {
                ProductId = product1.Id,
                Quantity = 2
            };

            var product2 = new Product
            {
                Name = "Product name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true
            };

            _productService.InsertProduct(product2);

            var sci2 = new ShoppingCartItem
            {
                ProductId = product2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem> { sci1, sci2 };
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            _discountService.InsertDiscount(new Discount
            {
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited
            });

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
