using System;
using System.Collections.Generic;
using Moq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderProcessingServiceTests : ServiceTest
    {
        private TaxSettings _taxSettings;
        private RewardPointsSettings _rewardPointsSettings;
        private ShippingSettings _shippingSettings;
        private PaymentSettings _paymentSettings;
        private OrderSettings _orderSettings;
        private LocalizationSettings _localizationSettings;
        private ShoppingCartSettings _shoppingCartSettings;
        private CatalogSettings _catalogSettings;
        private CurrencySettings _currencySettings;
        private CustomerSettings _customerSettings;
        private AddressSettings _addressSettings;
        private IWorkContext _workContext;
        private Store _store;
        private Mock<IStoreContext> _storeContext;
        private Mock<IProductService> _productService;
        private Mock<IDiscountService> _discountService;
        private Mock<ICategoryService> _categoryService;
        private Mock<IManufacturerService> _manufacturerService;
        private Mock<IProductAttributeParser> _productAttributeParser;
        private PriceCalculationService _priceCalcService;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<ILocalizationService> _localizationService;
        private Mock<IRepository<ShippingMethod>> _shippingMethodRepository;
        private Mock<IRepository<Warehouse>> _warehouseRepository;
        private NullLogger _logger;
        private ShippingService _shippingService;
        private Mock<IShipmentService> _shipmentService;
        private Mock<IPaymentService> _paymentService;
        private Mock<ICheckoutAttributeParser> _checkoutAttributeParser;
        private Mock<IGiftCardService> _giftCardService;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<IGeoLookupService> _geoLookupService;
        private Mock<ICountryService> _countryService;
        private Mock<IStateProvinceService> _stateProvinceService;
        private Mock<IAddressService> _addressService;
        private TaxService _taxService;
        private Mock<IRewardPointService> _rewardPointService;
        private OrderTotalCalculationService _orderTotalCalcService;
        private Mock<IOrderService> _orderService;
        private Mock<IWebHelper> _webHelper;
        private Mock<ILanguageService> _languageService;
        private Mock<IPriceFormatter> _priceFormatter;
        private Mock<IProductAttributeFormatter> _productAttributeFormatter;
        private Mock<IShoppingCartService> _shoppingCartService;
        private Mock<ICheckoutAttributeFormatter> _checkoutAttributeFormatter;
        private Mock<ICustomerService> _customerService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<IWorkflowMessageService> _workflowMessageService;
        private Mock<ICustomerActivityService> _customerActivityService;
        private Mock<ICurrencyService> _currencyService;
        private Mock<IAffiliateService> _affiliateService;
        private Mock<IVendorService> _vendorService;
        private Mock<IPdfService> _pdfService;
        private Mock<ICustomNumberFormatter> _customNumberFormatter;
        private OrderProcessingService _orderProcessingService;
        private PaymentPluginManager _paymentPluginManager;
        private IPickupPluginManager _pickupPluginManager;
        private IShippingPluginManager _shippingPluginManager;
        private ITaxPluginManager _taxPluginManager;

        [SetUp]
        public new void SetUp()
        {
            _productService = new Mock<IProductService>();
            _storeContext = new Mock<IStoreContext>();
            _discountService = new Mock<IDiscountService>();
            _categoryService = new Mock<ICategoryService>();
            _manufacturerService = new Mock<IManufacturerService>();
            _productAttributeParser = new Mock<IProductAttributeParser>();
            _eventPublisher = new Mock<IEventPublisher>();
            _localizationService = new Mock<ILocalizationService>();
            _shippingMethodRepository = new Mock<IRepository<ShippingMethod>>();
            _warehouseRepository = new Mock<IRepository<Warehouse>>();
            _shipmentService = new Mock<IShipmentService>();
            _paymentService = new Mock<IPaymentService>();
            _checkoutAttributeParser = new Mock<ICheckoutAttributeParser>();
            _giftCardService = new Mock<IGiftCardService>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _geoLookupService = new Mock<IGeoLookupService>();
            _countryService = new Mock<ICountryService>();
            _stateProvinceService = new Mock<IStateProvinceService>();
            _eventPublisher = new Mock<IEventPublisher>();
            _addressService = new Mock<IAddressService>();
            _rewardPointService = new Mock<IRewardPointService>();
            _orderService = new Mock<IOrderService>();
            _webHelper = new Mock<IWebHelper>();
            _languageService = new Mock<ILanguageService>();
            _priceFormatter = new Mock<IPriceFormatter>();
            _productAttributeFormatter = new Mock<IProductAttributeFormatter>();
            _shoppingCartService = new Mock<IShoppingCartService>();
            _checkoutAttributeFormatter = new Mock<ICheckoutAttributeFormatter>();
            _customerService = new Mock<ICustomerService>();
            _encryptionService = new Mock<IEncryptionService>();
            _workflowMessageService = new Mock<IWorkflowMessageService>();
            _customerActivityService = new Mock<ICustomerActivityService>();
            _currencyService = new Mock<ICurrencyService>();
            _affiliateService = new Mock<IAffiliateService>();
            _vendorService = new Mock<IVendorService>();
            _pdfService = new Mock<IPdfService>();
            _customNumberFormatter = new Mock<ICustomNumberFormatter>();
            _rewardPointService = new Mock<IRewardPointService>();

            _workContext = null;

            _store = new Store { Id = 1 };

            _storeContext.Setup(x => x.CurrentStore).Returns(_store);

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            var cacheManager = new TestCacheManager();

            _currencySettings = new CurrencySettings();

            //price calculation service
            _priceCalcService = new PriceCalculationService(_catalogSettings, _currencySettings, _categoryService.Object,
                _currencyService.Object, _discountService.Object, _manufacturerService.Object, _productAttributeParser.Object,
                _productService.Object, cacheManager, _storeContext.Object, _workContext, _shoppingCartSettings);

            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            var loger = new Mock<ILogger>();

            var pluginService = new PluginService(_catalogSettings, _customerService.Object, loger.Object, CommonHelper.DefaultFileProvider, _webHelper.Object);
            _paymentPluginManager = new PaymentPluginManager(pluginService, null, _paymentSettings);
            _pickupPluginManager = new PickupPluginManager(pluginService, _shippingSettings);
            _shippingPluginManager = new ShippingPluginManager(pluginService, _shippingSettings);
            _taxPluginManager = new TaxPluginManager(pluginService, _taxSettings);

            //shipping
            _shippingSettings = new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string>()
            };
            _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add("FixedRateTestShippingRateComputationMethod");

            _logger = new NullLogger();
            _customerSettings = new CustomerSettings();
            _addressSettings = new AddressSettings();

            _shippingService = new ShippingService(_addressService.Object,
                cacheManager,
                _checkoutAttributeParser.Object,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _localizationService.Object,
                _logger,
                _pickupPluginManager,
                _priceCalcService,
                _productAttributeParser.Object,
                _productService.Object,
                _shippingMethodRepository.Object,
                _warehouseRepository.Object,
                _shippingPluginManager,
                _storeContext.Object,
                _shippingSettings,
                _shoppingCartSettings);

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
                cacheManager,
                _storeContext.Object,
                _taxPluginManager,
                _webHelper.Object,
                _workContext,
                _shippingSettings,
                _taxSettings);

            _rewardPointsSettings = new RewardPointsSettings();

            _orderTotalCalcService = new OrderTotalCalculationService(_catalogSettings,
                _checkoutAttributeParser.Object,
                _discountService.Object,
                _genericAttributeService.Object,
                _giftCardService.Object,
                _paymentService.Object,
                _priceCalcService,
                _rewardPointService.Object,
                _shippingPluginManager,
                _shippingService,
                _shoppingCartService.Object,
                _storeContext.Object,
                _taxService,
                _workContext,
                _rewardPointsSettings,
                _shippingSettings,
                _shoppingCartSettings,
                _taxSettings);

            _paymentSettings = new PaymentSettings
            {
                ActivePaymentMethodSystemNames = new List<string>
                {
                    "Payments.TestMethod"
                }
            };
            _orderSettings = new OrderSettings();

            _localizationSettings = new LocalizationSettings();

            _eventPublisher.Setup(x => x.Publish(It.IsAny<object>()));

            _orderProcessingService = new OrderProcessingService(_currencySettings,
                _affiliateService.Object,
                _checkoutAttributeFormatter.Object,
                _countryService.Object,
                _currencyService.Object,
                _customerActivityService.Object,
                _customerService.Object,
                _customNumberFormatter.Object,
                _discountService.Object,
                _encryptionService.Object,
                _eventPublisher.Object,
                _genericAttributeService.Object,
                _giftCardService.Object,
                _languageService.Object,
                _localizationService.Object,
                _logger,
                _orderService.Object,
                _orderTotalCalcService,
                _paymentPluginManager,
                _paymentService.Object,
                _pdfService.Object,
                _priceCalcService,
                _priceFormatter.Object,
                _productAttributeFormatter.Object,
                _productAttributeParser.Object,
                _productService.Object,
                _rewardPointService.Object,
                _shipmentService.Object,
                _shippingPluginManager,
                _shippingService,
                _shoppingCartService.Object,
                _stateProvinceService.Object,
                _storeContext.Object,
                _taxService,
                _vendorService.Object,
                _webHelper.Object,
                _workContext,
                _workflowMessageService.Object,
                _localizationSettings,
                _orderSettings,
                _paymentSettings,
                _rewardPointsSettings,
                _shippingSettings,
                _taxSettings);
        }

        [Test]
        public void Ensure_order_can_only_be_cancelled_when_orderStatus_is_not_cancelled_yet()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled)
                            _orderProcessingService.CanCancelOrder(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanCancelOrder(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_marked_as_authorized_when_orderStatus_is_not_cancelled_and_paymentStatus_is_pending()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled && ps == PaymentStatus.Pending)
                            _orderProcessingService.CanMarkOrderAsAuthorized(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanMarkOrderAsAuthorized(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_captured_when_orderStatus_is_not_cancelled_or_pending_and_paymentstatus_is_authorized_and_paymentModule_supports_capture()
        {
            _paymentService.Setup(ps => ps.SupportCapture("paymentMethodSystemName_that_supports_capture")).Returns(true);
            _paymentService.Setup(ps => ps.SupportCapture("paymentMethodSystemName_that_doesn't_support_capture")).Returns(false);
            var order = new Order
            {
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_capture"
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (os != OrderStatus.Cancelled && os != OrderStatus.Pending
                            && ps == PaymentStatus.Authorized)
                            _orderProcessingService.CanCapture(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanCapture(order).ShouldBeFalse();
                    }


            order.PaymentMethodSystemName = "paymentMethodSystemName_that_doesn't_support_capture";
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanCapture(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_marked_as_paid_when_orderStatus_is_cancelled_or_paymentStatus_is_paid_or_refunded_or_voided()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os == OrderStatus.Cancelled || ps == PaymentStatus.Paid || ps == PaymentStatus.Refunded || ps == PaymentStatus.Voided)
                            _orderProcessingService.CanMarkOrderAsPaid(order).ShouldBeFalse();
                        else
                            _orderProcessingService.CanMarkOrderAsPaid(order).ShouldBeTrue();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_refunded_when_paymentstatus_is_paid_and_paymentModule_supports_refund()
        {
            _paymentService.Setup(ps => ps.SupportRefund("paymentMethodSystemName_that_supports_refund")).Returns(true);
            _paymentService.Setup(ps => ps.SupportRefund("paymentMethodSystemName_that_doesn't_support_refund")).Returns(false);
            var order = new Order
            {
                OrderTotal = 1,
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_refund"
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid)
                            _orderProcessingService.CanRefund(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanRefund(order).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_doesn't_support_refund";
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanRefund(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_refunded_when_orderTotal_is_zero()
        {
            _paymentService.Setup(ps => ps.SupportRefund("paymentMethodSystemName_that_supports_refund")).Returns(true);
            var order = new Order
            {
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_refund"
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanRefund(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_refunded_offline_when_paymentstatus_is_paid()
        {
            var order = new Order
            {
                OrderTotal = 1
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid)
                            _orderProcessingService.CanRefundOffline(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanRefundOffline(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_refunded_offline_when_orderTotal_is_zero()
        {
            var order = new Order();

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanRefundOffline(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_voided_when_paymentstatus_is_authorized_and_paymentModule_supports_void()
        {
            _paymentService.Setup(ps => ps.SupportVoid("paymentMethodSystemName_that_supports_void")).Returns(true);
            _paymentService.Setup(ps => ps.SupportVoid("paymentMethodSystemName_that_doesn't_support_void")).Returns(false);
            var order = new Order
            {
                OrderTotal = 1,
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_void"
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Authorized)
                            _orderProcessingService.CanVoid(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanVoid(order).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_doesn't_support_void";
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanVoid(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_voided_when_orderTotal_is_zero()
        {
            _paymentService.Setup(ps => ps.SupportVoid("paymentMethodSystemName_that_supports_void")).Returns(true);
            var order = new Order
            {
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_void"
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanVoid(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_voided_offline_when_paymentstatus_is_authorized()
        {
            var order = new Order
            {
                OrderTotal = 1
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Authorized)
                            _orderProcessingService.CanVoidOffline(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanVoidOffline(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_voided_offline_when_orderTotal_is_zero()
        {
            var order = new Order();

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanVoidOffline(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_partially_refunded_when_paymentstatus_is_paid_or_partiallyRefunded_and_paymentModule_supports_partialRefund()
        {
            _paymentService.Setup(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_supports_partialrefund")).Returns(true);
            _paymentService.Setup(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_doesn't_support_partialrefund")).Returns(false);
            var order = new Order
            {
                OrderTotal = 100,
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_partialrefund"
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                            _orderProcessingService.CanPartiallyRefund(order, 10).ShouldBeTrue();
                        else
                            _orderProcessingService.CanPartiallyRefund(order, 10).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_doesn't_support_partialrefund";
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanPartiallyRefund(order, 10).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_partially_refunded_when_amountToRefund_is_greater_than_amount_that_can_be_refunded()
        {
            _paymentService.Setup(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_supports_partialrefund")).Returns(true);
            var order = new Order
            {
                OrderTotal = 100,
                RefundedAmount = 30, //100-30=70 can be refunded
                PaymentMethodSystemName = "paymentMethodSystemName_that_supports_partialrefund"

            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanPartiallyRefund(order, 80).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_partially_refunded_offline_when_paymentstatus_is_paid_or_partiallyRefunded()
        {
            var order = new Order
            {
                OrderTotal = 100
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        {
                            order.OrderStatus = os;
                            order.PaymentStatus = ps;
                            order.ShippingStatus = ss;

                            if (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).ShouldBeTrue();
                            else
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).ShouldBeFalse();
                        }
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_partially_refunded_offline_when_amountToRefund_is_greater_than_amount_that_can_be_refunded()
        {
            var order = new Order
            {
                OrderTotal = 100,
                RefundedAmount = 30 //100-30=70 can be refunded
            };

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        _orderProcessingService.CanPartiallyRefundOffline(order, 80).ShouldBeFalse();
                    }
        }

        //TODO write unit tests for the following methods:
        //PlaceOrder
        //CanCancelRecurringPayment, ProcessNextRecurringPayment, CancelRecurringPayment
    }
}
