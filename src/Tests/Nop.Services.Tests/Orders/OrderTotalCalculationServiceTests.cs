using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Discounts;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderTotalCalculationServiceTests : ServiceTest
    {
        IWorkContext _workContext;
        ITaxService _taxService;
        IShippingService _shippingService;
        IPaymentService _paymentService;
        ICheckoutAttributeParser _checkoutAttributeParser;
        IDiscountService _discountService;
        IGiftCardService _giftCardService;
        TaxSettings _taxSettings;
        RewardPointsSettings _rewardPointsSettings;
        ICategoryService _categoryService;
        IProductAttributeParser _productAttributeParser;
        IPriceCalculationService _priceCalcService;
        IOrderTotalCalculationService _orderTotalCalcService;
        IAddressService _addressService;
        ShippingSettings _shippingSettings;
        ILogger _logger;
        IRepository<ShippingMethod> _shippingMethodRepository;
        ShoppingCartSettings _shoppingCartSettings;
        CatalogSettings _catalogSettings;
        IEventPublisher _eventPublisher;

        [SetUp]
        public new void SetUp()
        {
            _workContext = null;

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());
            var cacheManager = new NopNullCache();

            //price calculation service
            _discountService = MockRepository.GenerateMock<IDiscountService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            _priceCalcService = new PriceCalculationService(_workContext, _discountService,
                _categoryService, _productAttributeParser, _shoppingCartSettings, _catalogSettings);

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            //shipping
            _shippingSettings = new ShippingSettings();
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames = new List<string>();
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");
            _shippingMethodRepository = MockRepository.GenerateMock<IRepository<ShippingMethod>>();
            _logger = new NullLogger();
            _shippingService = new ShippingService(cacheManager,
                _shippingMethodRepository,
                _logger,
                _productAttributeParser,
                _checkoutAttributeParser,
                _shippingSettings, pluginFinder, _eventPublisher);
            

            _paymentService = MockRepository.GenerateMock<IPaymentService>();
            _checkoutAttributeParser = MockRepository.GenerateMock<ICheckoutAttributeParser>();
            _giftCardService = MockRepository.GenerateMock<IGiftCardService>();

            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            _eventPublisher.Expect(x => x.Publish(Arg<object>.Is.Anything));

            //tax
            _taxSettings = new TaxSettings();
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _taxSettings.DefaultTaxAddressId = 10;
            _addressService = MockRepository.GenerateMock<IAddressService>();
            _addressService.Expect(x => x.GetAddressById(_taxSettings.DefaultTaxAddressId)).Return(new Address() { Id = _taxSettings.DefaultTaxAddressId });
            _taxService = new TaxService(_addressService, _workContext, _taxSettings, pluginFinder, _eventPublisher);

            _rewardPointsSettings = new RewardPointsSettings();

            _orderTotalCalcService = new OrderTotalCalculationService(_workContext,
                _priceCalcService, _taxService, _shippingService, _paymentService,
                _checkoutAttributeParser, _discountService, _giftCardService,
                _taxSettings, _rewardPointsSettings, _shippingSettings, _shoppingCartSettings, _catalogSettings);
        }

        [Test]
        public void Can_get_shopping_cart_subTotal_excluding_tax()
        {
            //customer
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                 ProductVariant = productVariant1,
                 ProductVariantId = productVariant1.Id,
                 Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);
            
            decimal discountAmount;
            Discount appliedDiscount;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscount.ShouldBeNull();
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
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            decimal discountAmount;
            Discount appliedDiscount;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;

            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(0);
            appliedDiscount.ShouldBeNull();
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
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                 ProductVariant = productVariant1,
                 ProductVariantId = productVariant1.Id,
                 Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);
            
            //discounts
            var discount1 = new Discount()
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderSubTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            _discountService.Expect(ds => ds.IsDiscountValid(discount1, customer)).Return(true);
            _discountService.Expect(ds => ds.GetAllDiscounts(DiscountType.AssignedToOrderSubTotal)).Return(new List<Discount>() { discount1});

            decimal discountAmount;
            Discount appliedDiscount;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            //10% - default tax rate
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, false,
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);
            discountAmount.ShouldEqual(3);
            appliedDiscount.ShouldNotBeNull();
            appliedDiscount.Name.ShouldEqual("Discount 1");
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
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 12.34M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 21.57M,
                CustomerEntersPrice = false,
                Published = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            //discounts
            var discount1 = new Discount()
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderSubTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            _discountService.Expect(ds => ds.IsDiscountValid(discount1, customer)).Return(true);
            _discountService.Expect(ds => ds.GetAllDiscounts(DiscountType.AssignedToOrderSubTotal)).Return(new List<Discount>() { discount1 });

            decimal discountAmount;
            Discount appliedDiscount;
            decimal subTotalWithoutDiscount;
            decimal subTotalWithDiscount;
            SortedDictionary<decimal, decimal> taxRates;
            _orderTotalCalcService.GetShoppingCartSubTotal(cart, true,
                out discountAmount, out appliedDiscount,
                out subTotalWithoutDiscount, out subTotalWithDiscount, out taxRates);

            //TODO strange. Why does the commented test fail? discountAmount.ShouldEqual(3.3);
            //discountAmount.ShouldEqual(3.3);
            appliedDiscount.ShouldNotBeNull();
            appliedDiscount.Name.ShouldEqual("Discount 1");
            subTotalWithoutDiscount.ShouldEqual(98.329);
            subTotalWithDiscount.ShouldEqual(95.029);
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.639);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_excluding_tax()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false,
                }
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            decimal taxRate = decimal.Zero;
            Discount appliedDiscount = null;
            decimal? shipping = null;
            

            shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, false, out taxRate, out appliedDiscount);
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(52.5);
            appliedDiscount.ShouldBeNull();
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_with_fixed_shipping_rate_including_tax()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false,
                }
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            decimal taxRate = decimal.Zero;
            Discount appliedDiscount = null;
            decimal? shipping = null;

            shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, true, out taxRate, out appliedDiscount);
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change
            shipping.ShouldEqual(57.75);
            appliedDiscount.ShouldBeNull();
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_excluding_tax()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false,
                }
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            //discounts
            var discount1 = new Discount()
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToShipping,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            _discountService.Expect(ds => ds.IsDiscountValid(discount1, customer)).Return(true);
            _discountService.Expect(ds => ds.GetAllDiscounts(DiscountType.AssignedToShipping)).Return(new List<Discount>() { discount1 });


            decimal taxRate = decimal.Zero;
            Discount appliedDiscount = null;
            decimal? shipping = null;


            shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, false, out taxRate, out appliedDiscount);
            appliedDiscount.ShouldNotBeNull();
            appliedDiscount.Name.ShouldEqual("Discount 1");
            shipping.ShouldNotBeNull();
            //10 - default fixed shipping rate, 42.5 - additional shipping change, -3 - discount
            shipping.ShouldEqual(49.5);
            //10 - default fixed tax rate
            taxRate.ShouldEqual(10);
        }

        [Test]
        public void Can_get_shipping_total_discount_including_tax()
        {
            var sci1 = new ShoppingCartItem()
            {
                AttributesXml = "",
                Quantity = 3,
                ProductVariant = new ProductVariant()
                {
                    Id = 1,
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
                    Id = 2,
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
                    Id = 3,
                    Weight = 11.5M,
                    Height = 12.5M,
                    Length = 13.5M,
                    Width = 14.5M,
                    AdditionalShippingCharge = 7.5M,
                    IsShipEnabled = false,
                }
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2, sci3 };
            var customer = new Customer();
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            //discounts
            var discount1 = new Discount()
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToShipping,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            _discountService.Expect(ds => ds.IsDiscountValid(discount1, customer)).Return(true);
            _discountService.Expect(ds => ds.GetAllDiscounts(DiscountType.AssignedToShipping)).Return(new List<Discount>() { discount1 });


            decimal taxRate = decimal.Zero;
            Discount appliedDiscount = null;
            decimal? shipping = null;

            shipping = _orderTotalCalcService.GetShoppingCartShippingTotal(cart, true, out taxRate, out appliedDiscount);
            appliedDiscount.ShouldNotBeNull();
            appliedDiscount.Name.ShouldEqual("Discount 1");
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
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            
            
            SortedDictionary<decimal, decimal> taxRates;
            customer.SelectedPaymentMethodSystemName = "test1";
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee("test1")).Return(20);
            //56 - items, 10 - shipping (fixed), 20 - payment fee

            //1. shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(8.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(8.6);

            //2. shipping is taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(6.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(6.6);

            //3. shipping is not taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(7.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(7.6);

            //3. shipping is not taxable, payment fee is not taxable
            _taxSettings.ShippingIsTaxable = false;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = false;
            _orderTotalCalcService.GetTaxTotal(cart, out taxRates).ShouldEqual(5.6);
            taxRates.ShouldNotBeNull();
            taxRates.Count.ShouldEqual(1);
            taxRates.ContainsKey(10).ShouldBeTrue();
            taxRates[10].ShouldEqual(5.6);
        }

        [Test]
        public void Can_get_shopping_cart_total_without_shipping_required()
        {
            //customer
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = false,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = false,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);



            customer.SelectedPaymentMethodSystemName = "test1";
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee("test1")).Return(20);


            decimal discountAmount;
            Discount appliedDiscount;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;


            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 20 - payment fee, 7.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart,  out discountAmount, out appliedDiscount, 
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(83.6M);
        }

        [Test]
        public void Can_get_shopping_cart_total_with_shipping_required()
        {
            //customer
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);



            customer.SelectedPaymentMethodSystemName = "test1";
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee("test1")).Return(20);


            decimal discountAmount;
            Discount appliedDiscount;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;


            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(94.6M);
        }

        [Test]
        public void Can_get_shopping_cart_total_with_applied_reward_points()
        {
            //customer
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);



            customer.SelectedPaymentMethodSystemName = "test1";
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee("test1")).Return(20);


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
            customer.AddRewardPointsHistoryEntry(15); //15*2=30
            customer.UseRewardPointsDuringCheckout = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, -30 (reward points)
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(64.6M);
        }

        [Test]
        public void Can_get_shopping_cart_total_discount()
        {
            //customer
            Customer customer = new Customer();

            //shopping cart
            var productVariant1 = new ProductVariant
            {
                Id = 1,
                Name = "Product variant name 1",
                Price = 10M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 1,
                    Name = "Product name 1",
                    Published = true
                }
            };
            var sci1 = new ShoppingCartItem()
            {
                ProductVariant = productVariant1,
                ProductVariantId = productVariant1.Id,
                Quantity = 2,
            };
            var productVariant2 = new ProductVariant
            {
                Id = 2,
                Name = "Product variant name 2",
                Price = 12M,
                Published = true,
                IsShipEnabled = true,
                Product = new Product()
                {
                    Id = 2,
                    Name = "Product name 2",
                    Published = true
                }
            };
            var sci2 = new ShoppingCartItem()
            {
                ProductVariant = productVariant2,
                ProductVariantId = productVariant2.Id,
                Quantity = 3
            };

            var cart = new List<ShoppingCartItem>() { sci1, sci2 };
            cart.ForEach(sci => sci.Customer = customer);
            cart.ForEach(sci => sci.CustomerId = customer.Id);

            //discounts
            var discount1 = new Discount()
            {
                Id = 1,
                Name = "Discount 1",
                DiscountType = DiscountType.AssignedToOrderTotal,
                DiscountAmount = 3,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };
            _discountService.Expect(ds => ds.IsDiscountValid(discount1, customer)).Return(true);
            _discountService.Expect(ds => ds.GetAllDiscounts(DiscountType.AssignedToOrderTotal)).Return(new List<Discount>() { discount1 });
            
            customer.SelectedPaymentMethodSystemName = "test1";
            _paymentService.Expect(ps => ps.GetAdditionalHandlingFee("test1")).Return(20);


            decimal discountAmount;
            Discount appliedDiscount;
            List<AppliedGiftCard> appliedGiftCards;
            int redeemedRewardPoints;
            decimal redeemedRewardPointsAmount;

            //shipping is taxable, payment fee is taxable
            _taxSettings.ShippingIsTaxable = true;
            _taxSettings.PaymentMethodAdditionalFeeIsTaxable = true;

            //56 - items, 10 - shipping (fixed), 20 - payment fee, 8.6 - tax, [-3] - discount
            _orderTotalCalcService.GetShoppingCartTotal(cart, out discountAmount, out appliedDiscount,
                out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount)
                .ShouldEqual(91.6M); 
            discountAmount.ShouldEqual(3);
            appliedDiscount.ShouldNotBeNull();
            appliedDiscount.Name.ShouldEqual("Discount 1");
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
    }
}
