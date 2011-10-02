using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Services.Tests.Orders
{
    [TestFixture]
    public class OrderProcessingServiceTests : ServiceTest
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
        IOrderService _orderService;
        IWebHelper _webHelper;
        ILocalizationService _localizationService;
        ILanguageService _languageService;
        IProductService _productService;
        IPriceFormatter _priceFormatter;
        IProductAttributeFormatter _productAttributeFormatter;
        IShoppingCartService _shoppingCartService;
        ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        ICustomerService _customerService;
        IEncryptionService _encryptionService;
        IWorkflowMessageService _workflowMessageService;
        ISmsService _smsService;
        ICustomerActivityService _customerActivityService;
        ICurrencyService _currencyService;
        PaymentSettings _paymentSettings;
        OrderSettings _orderSettings;
        LocalizationSettings _localizationSettings;
        ShoppingCartSettings _shoppingCartSettings;
        CatalogSettings _catalogSettings;
        IOrderProcessingService _orderProcessingService;
        IEventPublisher _eventPublisher;

        [SetUp]
        public new void SetUp()
        {
            _workContext = null;

            var pluginFinder = new PluginFinder(new AppDomainTypeFinder());
            var cacheManager = new NopNullCache();

            _shoppingCartSettings = new ShoppingCartSettings();
            _catalogSettings = new CatalogSettings();

            //price calculation service
            _discountService = MockRepository.GenerateMock<IDiscountService>();
            _categoryService = MockRepository.GenerateMock<ICategoryService>();
            _productAttributeParser = MockRepository.GenerateMock<IProductAttributeParser>();
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

            _orderService = MockRepository.GenerateMock<IOrderService>();
            _webHelper = MockRepository.GenerateMock<IWebHelper>();
            _localizationService= MockRepository.GenerateMock<ILocalizationService>();
            _languageService = MockRepository.GenerateMock<ILanguageService>();
            _productService = MockRepository.GenerateMock<IProductService>();
            _priceFormatter= MockRepository.GenerateMock<IPriceFormatter>();
            _productAttributeFormatter= MockRepository.GenerateMock<IProductAttributeFormatter>();
            _shoppingCartService= MockRepository.GenerateMock<IShoppingCartService>();
            _checkoutAttributeFormatter= MockRepository.GenerateMock<ICheckoutAttributeFormatter>();
            _customerService= MockRepository.GenerateMock<ICustomerService>();
            _encryptionService = MockRepository.GenerateMock<IEncryptionService>();
            _workflowMessageService = MockRepository.GenerateMock<IWorkflowMessageService>();
            _smsService = MockRepository.GenerateMock<ISmsService>();
            _customerActivityService = MockRepository.GenerateMock<ICustomerActivityService>();
            _currencyService = MockRepository.GenerateMock<ICurrencyService>();

            _paymentSettings = new PaymentSettings()
            {
                ActivePaymentMethodSystemNames = new List<string>()
                {
                    "Payments.TestMethod"
                }
            };
            _orderSettings = new OrderSettings();

            _localizationSettings = new LocalizationSettings();

            _orderProcessingService = new OrderProcessingService(_orderService, _webHelper,
                _localizationService, _languageService,
                _productService, _paymentService, _logger,
                _orderTotalCalcService, _priceCalcService, _priceFormatter,
                _productAttributeParser, _productAttributeFormatter,
                _giftCardService, _shoppingCartService, _checkoutAttributeFormatter,
                _shippingService, _taxService,
                _customerService, _discountService,
                _encryptionService, _workContext, _workflowMessageService,
                _smsService, _customerActivityService, _currencyService,
                _paymentSettings, _rewardPointsSettings,
                _orderSettings, _taxSettings, _localizationSettings);
        }

        [Test]
        public void Ensure_order_can_only_be_shipped_when_orderStatus_is_not_cancelled_and_its_not_shipped_yet()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled && ss == ShippingStatus.NotYetShipped)
                            _orderProcessingService.CanShip(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanShip(order).ShouldBeFalse();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_delivered_when_orderStatus_is_not_cancelled_and_its_already_shipped()
        {
            var order = new Order();
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;
                        if (os != OrderStatus.Cancelled && ss == ShippingStatus.Shipped)
                            _orderProcessingService.CanDeliver(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanDeliver(order).ShouldBeFalse();
                    }
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
            _paymentService.Expect(ps => ps.SupportCapture("paymentMethodSystemName_that_supports_capture")).Return(true);
            _paymentService.Expect(ps => ps.SupportCapture("paymentMethodSystemName_that_don't_support_capture")).Return(false);
            var order = new Order();


            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_capture";
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled && os != OrderStatus.Pending)
                            && (ps == PaymentStatus.Authorized))
                            _orderProcessingService.CanCapture(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanCapture(order).ShouldBeFalse();
                    }


            order.PaymentMethodSystemName = "paymentMethodSystemName_that_don't_support_capture";
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
                        if (os == OrderStatus.Cancelled
                            || (ps == PaymentStatus.Paid || ps == PaymentStatus.Refunded || ps == PaymentStatus.Voided))
                            _orderProcessingService.CanMarkOrderAsPaid(order).ShouldBeFalse();
                        else
                            _orderProcessingService.CanMarkOrderAsPaid(order).ShouldBeTrue();
                    }
        }

        [Test]
        public void Ensure_order_can_only_be_refunded_when_orderStatus_is_not_cancelled_and_paymentstatus_is_paid_and_paymentModule_supports_refund()
        {
            _paymentService.Expect(ps => ps.SupportRefund("paymentMethodSystemName_that_supports_refund")).Return(true);
            _paymentService.Expect(ps => ps.SupportRefund("paymentMethodSystemName_that_don't_support_refund")).Return(false);
            var order = new Order();
            order.OrderTotal = 1;
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_refund";

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled)
                            && (ps == PaymentStatus.Paid))
                            _orderProcessingService.CanRefund(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanRefund(order).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_don't_support_refund";
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
            _paymentService.Expect(ps => ps.SupportRefund("paymentMethodSystemName_that_supports_refund")).Return(true);
            var order = new Order();
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_refund";

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
        public void Ensure_order_can_only_be_refunded_offline_when_orderStatus_is_not_cancelled_and_paymentstatus_is_paid()
        {
            var order = new Order()
            {
                OrderTotal = 1,
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled)
                            && (ps == PaymentStatus.Paid))
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
        public void Ensure_order_can_only_be_voided_when_orderStatus_is_not_cancelled_and_paymentstatus_is_authorized_and_paymentModule_supports_void()
        {
            _paymentService.Expect(ps => ps.SupportVoid("paymentMethodSystemName_that_supports_void")).Return(true);
            _paymentService.Expect(ps => ps.SupportVoid("paymentMethodSystemName_that_don't_support_void")).Return(false);
            var order = new Order();
            order.OrderTotal = 1;
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_void";

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled)
                            && (ps == PaymentStatus.Authorized))
                            _orderProcessingService.CanVoid(order).ShouldBeTrue();
                        else
                            _orderProcessingService.CanVoid(order).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_don't_support_void";
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
            _paymentService.Expect(ps => ps.SupportVoid("paymentMethodSystemName_that_supports_void")).Return(true);
            var order = new Order();
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_void";

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
        public void Ensure_order_can_only_be_voided_offline_when_orderStatus_is_not_cancelled_and_paymentstatus_is_authorized()
        {
            var order = new Order()
            {
                OrderTotal = 1,
            };
            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled)
                            && (ps == PaymentStatus.Authorized))
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
        public void Ensure_order_can_only_be_partially_refunded_when_orderStatus_is_not_cancelled_and_paymentstatus_is_paid_or_partiallyRefunded_and_paymentModule_supports_partialRefund()
        {
            _paymentService.Expect(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_supports_partialrefund")).Return(true);
            _paymentService.Expect(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_don't_support_partialrefund")).Return(false);
            var order = new Order();
            order.OrderTotal = 100;
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_partialrefund";

            foreach (OrderStatus os in Enum.GetValues(typeof(OrderStatus)))
                foreach (PaymentStatus ps in Enum.GetValues(typeof(PaymentStatus)))
                    foreach (ShippingStatus ss in Enum.GetValues(typeof(ShippingStatus)))
                    {
                        order.OrderStatus = os;
                        order.PaymentStatus = ps;
                        order.ShippingStatus = ss;

                        if ((os != OrderStatus.Cancelled)
                            && (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded))
                            _orderProcessingService.CanPartiallyRefund(order, 10).ShouldBeTrue();
                        else
                            _orderProcessingService.CanPartiallyRefund(order, 10).ShouldBeFalse();
                    }



            order.PaymentMethodSystemName = "paymentMethodSystemName_that_don't_support_partialrefund";
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
            _paymentService.Expect(ps => ps.SupportPartiallyRefund("paymentMethodSystemName_that_supports_partialrefund")).Return(true);
            var order = new Order()
            {
                OrderTotal = 100,
                RefundedAmount = 30, //100-30=70 can be refunded
            };
            order.PaymentMethodSystemName = "paymentMethodSystemName_that_supports_partialrefund";

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
        public void Ensure_order_can_only_be_partially_refunded_offline_when_orderStatus_is_not_cancelled_and_paymentstatus_is_paid_or_partiallyRefunded()
        {
            var order = new Order();
            order.OrderTotal = 100;

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

                            if ((os != OrderStatus.Cancelled)
                                && (ps == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.PartiallyRefunded))
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).ShouldBeTrue();
                            else
                                _orderProcessingService.CanPartiallyRefundOffline(order, 10).ShouldBeFalse();
                        }
                    }
        }

        [Test]
        public void Ensure_order_cannot_be_partially_refunded_offline_when_amountToRefund_is_greater_than_amount_that_can_be_refunded()
        {
            var order = new Order()
            {
                OrderTotal = 100,
                RefundedAmount = 30, //100-30=70 can be refunded
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
