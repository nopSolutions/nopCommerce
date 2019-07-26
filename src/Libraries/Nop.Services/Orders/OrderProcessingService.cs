using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
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
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order processing service
    /// </summary>
    public partial class OrderProcessingService : IOrderProcessingService
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IAffiliateService _affiliateService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IDiscountService _discountService;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OrderProcessingService(CurrencySettings currencySettings,
            IAffiliateService affiliateService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ICustomNumberFormatter customNumberFormatter,
            IDiscountService discountService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPdfService pdfService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IShippingPluginManager shippingPluginManager,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings)
        {
            _currencySettings = currencySettings;
            _affiliateService = affiliateService;
            _checkoutAttributeFormatter = checkoutAttributeFormatter;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _customNumberFormatter = customNumberFormatter;
            _discountService = discountService;
            _encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _logger = logger;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _shippingPluginManager = shippingPluginManager;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Nested classes

        /// <summary>
        /// PlaceOrder container
        /// </summary>
        protected class PlaceOrderContainer
        {
            public PlaceOrderContainer()
            {
                Cart = new List<ShoppingCartItem>();
                AppliedDiscounts = new List<DiscountForCaching>();
                AppliedGiftCards = new List<AppliedGiftCard>();
            }

            /// <summary>
            /// Customer
            /// </summary>
            public Customer Customer { get; set; }

            /// <summary>
            /// Customer language
            /// </summary>
            public Language CustomerLanguage { get; set; }

            /// <summary>
            /// Affiliate identifier
            /// </summary>
            public int AffiliateId { get; set; }

            /// <summary>
            /// TAx display type
            /// </summary>
            public TaxDisplayType CustomerTaxDisplayType { get; set; }

            /// <summary>
            /// Selected currency
            /// </summary>
            public string CustomerCurrencyCode { get; set; }

            /// <summary>
            /// Customer currency rate
            /// </summary>
            public decimal CustomerCurrencyRate { get; set; }

            /// <summary>
            /// Billing address
            /// </summary>
            public Address BillingAddress { get; set; }

            /// <summary>
            /// Shipping address
            /// </summary>
            public Address ShippingAddress { get; set; }

            /// <summary>
            /// Shipping status
            /// </summary>
            public ShippingStatus ShippingStatus { get; set; }

            /// <summary>
            /// Selected shipping method
            /// </summary>
            public string ShippingMethodName { get; set; }

            /// <summary>
            /// Shipping rate computation method system name
            /// </summary>
            public string ShippingRateComputationMethodSystemName { get; set; }

            /// <summary>
            /// Is pickup in store selected?
            /// </summary>
            public bool PickupInStore { get; set; }

            /// <summary>
            /// Selected pickup address
            /// </summary>
            public Address PickupAddress { get; set; }

            /// <summary>
            /// Is recurring shopping cart
            /// </summary>
            public bool IsRecurringShoppingCart { get; set; }

            /// <summary>
            /// Initial order (used with recurring payments)
            /// </summary>
            public Order InitialOrder { get; set; }

            /// <summary>
            /// Checkout attributes
            /// </summary>
            public string CheckoutAttributeDescription { get; set; }

            /// <summary>
            /// Shopping cart
            /// </summary>
            public string CheckoutAttributesXml { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public IList<ShoppingCartItem> Cart { get; set; }

            /// <summary>
            /// Applied discounts
            /// </summary>
            public List<DiscountForCaching> AppliedDiscounts { get; set; }

            /// <summary>
            /// Applied gift cards
            /// </summary>
            public List<AppliedGiftCard> AppliedGiftCards { get; set; }

            /// <summary>
            /// Order subtotal (incl tax)
            /// </summary>
            public decimal OrderSubTotalInclTax { get; set; }

            /// <summary>
            /// Order subtotal (excl tax)
            /// </summary>
            public decimal OrderSubTotalExclTax { get; set; }

            /// <summary>
            /// Subtotal discount (incl tax)
            /// </summary>
            public decimal OrderSubTotalDiscountInclTax { get; set; }

            /// <summary>
            /// Subtotal discount (excl tax)
            /// </summary>
            public decimal OrderSubTotalDiscountExclTax { get; set; }

            /// <summary>
            /// Shipping (incl tax)
            /// </summary>
            public decimal OrderShippingTotalInclTax { get; set; }

            /// <summary>
            /// Shipping (excl tax)
            /// </summary>
            public decimal OrderShippingTotalExclTax { get; set; }

            /// <summary>
            /// Payment additional fee (incl tax)
            /// </summary>
            public decimal PaymentAdditionalFeeInclTax { get; set; }

            /// <summary>
            /// Payment additional fee (excl tax)
            /// </summary>
            public decimal PaymentAdditionalFeeExclTax { get; set; }

            /// <summary>
            /// Tax
            /// </summary>
            public decimal OrderTaxTotal { get; set; }

            /// <summary>
            /// VAT number
            /// </summary>
            public string VatNumber { get; set; }

            /// <summary>
            /// Tax rates
            /// </summary>
            public string TaxRates { get; set; }

            /// <summary>
            /// Order total discount amount
            /// </summary>
            public decimal OrderDiscountAmount { get; set; }

            /// <summary>
            /// Redeemed reward points
            /// </summary>
            public int RedeemedRewardPoints { get; set; }

            /// <summary>
            /// Redeemed reward points amount
            /// </summary>
            public decimal RedeemedRewardPointsAmount { get; set; }

            /// <summary>
            /// Order total
            /// </summary>
            public decimal OrderTotal { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Add order note
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="note">Note text</param>
        protected virtual void AddOrderNote(Order order, string note)
        {
            order.OrderNotes.Add(new OrderNote
            {
                Note = note,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Prepare details to place an order. It also sets some properties to "processPaymentRequest"
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceOrderContainer PreparePlaceOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceOrderContainer
            {
                //customer
                Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId)
            };
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                throw new NopException("Anonymous checkout is not allowed");

            //customer currency
            var currencyTmp = _currencyService.GetCurrencyById(
                _genericAttributeService.GetAttribute<int>(details.Customer, NopCustomerDefaults.CurrencyIdAttribute, processPaymentRequest.StoreId));
            var customerCurrency = currencyTmp != null && currencyTmp.Published ? currencyTmp : _workContext.WorkingCurrency;
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
            details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(
                _genericAttributeService.GetAttribute<int>(details.Customer, NopCustomerDefaults.LanguageIdAttribute, processPaymentRequest.StoreId));
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //billing address
            if (details.Customer.BillingAddress == null)
                throw new NopException("Billing address is not provided");

            if (!CommonHelper.IsValidEmail(details.Customer.BillingAddress.Email))
                throw new NopException("Email is not valid");

            details.BillingAddress = (Address)details.Customer.BillingAddress.Clone();
            if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                throw new NopException($"Country '{details.BillingAddress.Country.Name}' is not allowed for billing");

            //checkout attributes
            details.CheckoutAttributesXml = _genericAttributeService.GetAttribute<string>(details.Customer, NopCustomerDefaults.CheckoutAttributes, processPaymentRequest.StoreId);
            details.CheckoutAttributeDescription = _checkoutAttributeFormatter.FormatAttributes(details.CheckoutAttributesXml, details.Customer);

            //load shopping cart
            details.Cart = _shoppingCartService.GetShoppingCart(details.Customer, ShoppingCartType.ShoppingCart, processPaymentRequest.StoreId);

            if (!details.Cart.Any())
                throw new NopException("Cart is empty");

            //validate the entire shopping cart
            var warnings = _shoppingCartService.GetShoppingCartWarnings(details.Cart, details.CheckoutAttributesXml, true);
            if (warnings.Any())
                throw new NopException(warnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));

            //validate individual cart items
            foreach (var sci in details.Cart)
            {
                var sciWarnings = _shoppingCartService.GetShoppingCartItemWarnings(details.Customer,
                    sci.ShoppingCartType, sci.Product, processPaymentRequest.StoreId, sci.AttributesXml,
                    sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false, sci.Id);
                if (sciWarnings.Any())
                    throw new NopException(sciWarnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));
            }

            //min totals validation
            if (!ValidateMinOrderSubtotalAmount(details.Cart))
            {
                var minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"),
                    _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false)));
            }

            if (!ValidateMinOrderTotalAmount(details.Cart))
            {
                var minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                throw new NopException(string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"),
                    _priceFormatter.FormatPrice(minOrderTotalAmount, true, false)));
            }

            //tax display type
            if (_taxSettings.AllowCustomersToSelectTaxDisplayType)
                details.CustomerTaxDisplayType = (TaxDisplayType)_genericAttributeService.GetAttribute<int>(details.Customer, NopCustomerDefaults.TaxDisplayTypeIdAttribute, processPaymentRequest.StoreId);
            else
                details.CustomerTaxDisplayType = _taxSettings.TaxDisplayType;

            //sub total (incl tax)
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, true, out var orderSubTotalDiscountAmount, out var orderSubTotalAppliedDiscounts, out var subTotalWithoutDiscountBase, out var _);
            details.OrderSubTotalInclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountInclTax = orderSubTotalDiscountAmount;

            //discount history
            foreach (var disc in orderSubTotalAppliedDiscounts)
                if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                    details.AppliedDiscounts.Add(disc);

            //sub total (excl tax)
            _orderTotalCalculationService.GetShoppingCartSubTotal(details.Cart, false, out orderSubTotalDiscountAmount,
                out orderSubTotalAppliedDiscounts, out subTotalWithoutDiscountBase, out _);
            details.OrderSubTotalExclTax = subTotalWithoutDiscountBase;
            details.OrderSubTotalDiscountExclTax = orderSubTotalDiscountAmount;

            //shipping info
            if (_shoppingCartService.ShoppingCartRequiresShipping(details.Cart))
            {
                var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(details.Customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, processPaymentRequest.StoreId);
                if (_shippingSettings.AllowPickupInStore && pickupPoint != null)
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);

                    details.PickupInStore = true;
                    details.PickupAddress = new Address
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        County = pickupPoint.County,
                        Country = country,
                        StateProvince = state,
                        ZipPostalCode = pickupPoint.ZipPostalCode,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                }
                else
                {
                    if (details.Customer.ShippingAddress == null)
                        throw new NopException("Shipping address is not provided");

                    if (!CommonHelper.IsValidEmail(details.Customer.ShippingAddress.Email))
                        throw new NopException("Email is not valid");

                    //clone shipping address
                    details.ShippingAddress = (Address)details.Customer.ShippingAddress.Clone();
                    if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        throw new NopException($"Country '{details.ShippingAddress.Country.Name}' is not allowed for shipping");
                }

                var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(details.Customer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, processPaymentRequest.StoreId);
                if (shippingOption != null)
                {
                    details.ShippingMethodName = shippingOption.Name;
                    details.ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName;
                }

                details.ShippingStatus = ShippingStatus.NotYetShipped;
            }
            else
                details.ShippingStatus = ShippingStatus.ShippingNotRequired;

            //LoadAllShippingRateComputationMethods
            var shippingRateComputationMethods = _shippingPluginManager.LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            //shipping total
            var orderShippingTotalInclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(details.Cart, true, shippingRateComputationMethods, out var _, out var shippingTotalDiscounts);
            var orderShippingTotalExclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(details.Cart, false, shippingRateComputationMethods);
            if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
                throw new NopException("Shipping total couldn't be calculated");

            details.OrderShippingTotalInclTax = orderShippingTotalInclTax.Value;
            details.OrderShippingTotalExclTax = orderShippingTotalExclTax.Value;

            foreach (var disc in shippingTotalDiscounts)
                if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                    details.AppliedDiscounts.Add(disc);

            //payment total
            var paymentAdditionalFee = _paymentService.GetAdditionalHandlingFee(details.Cart, processPaymentRequest.PaymentMethodSystemName);
            details.PaymentAdditionalFeeInclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, true, details.Customer);
            details.PaymentAdditionalFeeExclTax = _taxService.GetPaymentMethodAdditionalFee(paymentAdditionalFee, false, details.Customer);

            //tax amount
            details.OrderTaxTotal = _orderTotalCalculationService.GetTaxTotal(details.Cart, shippingRateComputationMethods, out var taxRatesDictionary);

            //VAT number
            var customerVatStatus = (VatNumberStatus)_genericAttributeService.GetAttribute<int>(details.Customer, NopCustomerDefaults.VatNumberStatusIdAttribute);
            if (_taxSettings.EuVatEnabled && customerVatStatus == VatNumberStatus.Valid)
                details.VatNumber = _genericAttributeService.GetAttribute<string>(details.Customer, NopCustomerDefaults.VatNumberAttribute);

            //tax rates
            details.TaxRates = taxRatesDictionary.Aggregate(string.Empty, (current, next) =>
                $"{current}{next.Key.ToString(CultureInfo.InvariantCulture)}:{next.Value.ToString(CultureInfo.InvariantCulture)};   ");

            //order total (and applied discounts, gift cards, reward points)
            var orderTotal = _orderTotalCalculationService.GetShoppingCartTotal(details.Cart, out var orderDiscountAmount, out var orderAppliedDiscounts, out var appliedGiftCards, out var redeemedRewardPoints, out var redeemedRewardPointsAmount);
            if (!orderTotal.HasValue)
                throw new NopException("Order total couldn't be calculated");

            details.OrderDiscountAmount = orderDiscountAmount;
            details.RedeemedRewardPoints = redeemedRewardPoints;
            details.RedeemedRewardPointsAmount = redeemedRewardPointsAmount;
            details.AppliedGiftCards = appliedGiftCards;
            details.OrderTotal = orderTotal.Value;

            //discount history
            foreach (var disc in orderAppliedDiscounts)
                if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                    details.AppliedDiscounts.Add(disc);

            processPaymentRequest.OrderTotal = details.OrderTotal;

            //recurring or standard shopping cart?
            details.IsRecurringShoppingCart = _shoppingCartService.ShoppingCartIsRecurring(details.Cart);
            if (!details.IsRecurringShoppingCart)
                return details;

            var recurringCyclesError = _shoppingCartService.GetRecurringCycleInfo(details.Cart,
                out var recurringCycleLength, out var recurringCyclePeriod, out var recurringTotalCycles);
            if (!string.IsNullOrEmpty(recurringCyclesError))
                throw new NopException(recurringCyclesError);

            processPaymentRequest.RecurringCycleLength = recurringCycleLength;
            processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
            processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;

            return details;
        }

        /// <summary>
        /// Prepare details to place order based on the recurring payment.
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Details</returns>
        protected virtual PlaceOrderContainer PrepareRecurringOrderDetails(ProcessPaymentRequest processPaymentRequest)
        {
            var details = new PlaceOrderContainer
            {
                IsRecurringShoppingCart = true,
                //load initial order
                InitialOrder = _orderService.GetOrderById(processPaymentRequest.InitialOrderId)
            };

            if (details.InitialOrder == null)
                throw new ArgumentException("Initial order is not set for recurring payment");

            processPaymentRequest.PaymentMethodSystemName = details.InitialOrder.PaymentMethodSystemName;

            //customer
            details.Customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (details.Customer == null)
                throw new ArgumentException("Customer is not set");

            //affiliate
            var affiliate = _affiliateService.GetAffiliateById(details.Customer.AffiliateId);
            if (affiliate != null && affiliate.Active && !affiliate.Deleted)
                details.AffiliateId = affiliate.Id;

            //check whether customer is guest
            if (details.Customer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                throw new NopException("Anonymous checkout is not allowed");

            //customer currency
            details.CustomerCurrencyCode = details.InitialOrder.CustomerCurrencyCode;
            details.CustomerCurrencyRate = details.InitialOrder.CurrencyRate;

            //customer language
            details.CustomerLanguage = _languageService.GetLanguageById(details.InitialOrder.CustomerLanguageId);
            if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
                details.CustomerLanguage = _workContext.WorkingLanguage;

            //billing address
            if (details.InitialOrder.BillingAddress == null)
                throw new NopException("Billing address is not available");

            details.BillingAddress = (Address)details.InitialOrder.BillingAddress.Clone();
            if (details.BillingAddress.Country != null && !details.BillingAddress.Country.AllowsBilling)
                throw new NopException($"Country '{details.BillingAddress.Country.Name}' is not allowed for billing");

            //checkout attributes
            details.CheckoutAttributesXml = details.InitialOrder.CheckoutAttributesXml;
            details.CheckoutAttributeDescription = details.InitialOrder.CheckoutAttributeDescription;

            //tax display type
            details.CustomerTaxDisplayType = details.InitialOrder.CustomerTaxDisplayType;

            //sub total
            details.OrderSubTotalInclTax = details.InitialOrder.OrderSubtotalInclTax;
            details.OrderSubTotalExclTax = details.InitialOrder.OrderSubtotalExclTax;
            details.OrderSubTotalDiscountExclTax = details.InitialOrder.OrderSubTotalDiscountExclTax;
            details.OrderSubTotalDiscountInclTax = details.InitialOrder.OrderSubTotalDiscountInclTax;

            //shipping info
            if (details.InitialOrder.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                details.PickupInStore = details.InitialOrder.PickupInStore;
                if (!details.PickupInStore)
                {
                    if (details.InitialOrder.ShippingAddress == null)
                        throw new NopException("Shipping address is not available");

                    //clone shipping address
                    details.ShippingAddress = (Address)details.InitialOrder.ShippingAddress.Clone();
                    if (details.ShippingAddress.Country != null && !details.ShippingAddress.Country.AllowsShipping)
                        throw new NopException($"Country '{details.ShippingAddress.Country.Name}' is not allowed for shipping");
                }
                else
                    if (details.InitialOrder.PickupAddress != null)
                    details.PickupAddress = (Address)details.InitialOrder.PickupAddress.Clone();
                details.ShippingMethodName = details.InitialOrder.ShippingMethod;
                details.ShippingRateComputationMethodSystemName = details.InitialOrder.ShippingRateComputationMethodSystemName;
                details.ShippingStatus = ShippingStatus.NotYetShipped;
            }
            else
                details.ShippingStatus = ShippingStatus.ShippingNotRequired;

            //shipping total
            details.OrderShippingTotalInclTax = details.InitialOrder.OrderShippingInclTax;
            details.OrderShippingTotalExclTax = details.InitialOrder.OrderShippingExclTax;

            //payment total
            details.PaymentAdditionalFeeInclTax = details.InitialOrder.PaymentMethodAdditionalFeeInclTax;
            details.PaymentAdditionalFeeExclTax = details.InitialOrder.PaymentMethodAdditionalFeeExclTax;

            //tax total
            details.OrderTaxTotal = details.InitialOrder.OrderTax;

            //tax rates
            details.TaxRates = details.InitialOrder.TaxRates;

            //VAT number
            details.VatNumber = details.InitialOrder.VatNumber;

            //discount history (the same)
            foreach (var duh in details.InitialOrder.DiscountUsageHistory)
            {
                var d = _discountService.GetDiscountById(duh.DiscountId);
                if (d != null)
                    details.AppliedDiscounts.Add(_discountService.MapDiscount(d));
            }

            //order total
            details.OrderDiscountAmount = details.InitialOrder.OrderDiscount;
            details.OrderTotal = details.InitialOrder.OrderTotal;
            processPaymentRequest.OrderTotal = details.OrderTotal;

            return details;
        }

        /// <summary>
        /// Save order and add order notes
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="processPaymentResult">Process payment result</param>
        /// <param name="details">Details</param>
        /// <returns>Order</returns>
        protected virtual Order SaveOrderDetails(ProcessPaymentRequest processPaymentRequest,
            ProcessPaymentResult processPaymentResult, PlaceOrderContainer details)
        {
            var order = new Order
            {
                StoreId = processPaymentRequest.StoreId,
                OrderGuid = processPaymentRequest.OrderGuid,
                CustomerId = details.Customer.Id,
                CustomerLanguageId = details.CustomerLanguage.Id,
                CustomerTaxDisplayType = details.CustomerTaxDisplayType,
                CustomerIp = _webHelper.GetCurrentIpAddress(),
                OrderSubtotalInclTax = details.OrderSubTotalInclTax,
                OrderSubtotalExclTax = details.OrderSubTotalExclTax,
                OrderSubTotalDiscountInclTax = details.OrderSubTotalDiscountInclTax,
                OrderSubTotalDiscountExclTax = details.OrderSubTotalDiscountExclTax,
                OrderShippingInclTax = details.OrderShippingTotalInclTax,
                OrderShippingExclTax = details.OrderShippingTotalExclTax,
                PaymentMethodAdditionalFeeInclTax = details.PaymentAdditionalFeeInclTax,
                PaymentMethodAdditionalFeeExclTax = details.PaymentAdditionalFeeExclTax,
                TaxRates = details.TaxRates,
                OrderTax = details.OrderTaxTotal,
                OrderTotal = details.OrderTotal,
                RefundedAmount = decimal.Zero,
                OrderDiscount = details.OrderDiscountAmount,
                CheckoutAttributeDescription = details.CheckoutAttributeDescription,
                CheckoutAttributesXml = details.CheckoutAttributesXml,
                CustomerCurrencyCode = details.CustomerCurrencyCode,
                CurrencyRate = details.CustomerCurrencyRate,
                AffiliateId = details.AffiliateId,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = processPaymentResult.AllowStoringCreditCardNumber,
                CardType = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardType) : string.Empty,
                CardName = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardName) : string.Empty,
                CardNumber = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardNumber) : string.Empty,
                MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(processPaymentRequest.CreditCardNumber)),
                CardCvv2 = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardCvv2) : string.Empty,
                CardExpirationMonth = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireMonth.ToString()) : string.Empty,
                CardExpirationYear = processPaymentResult.AllowStoringCreditCardNumber ? _encryptionService.EncryptText(processPaymentRequest.CreditCardExpireYear.ToString()) : string.Empty,
                PaymentMethodSystemName = processPaymentRequest.PaymentMethodSystemName,
                AuthorizationTransactionId = processPaymentResult.AuthorizationTransactionId,
                AuthorizationTransactionCode = processPaymentResult.AuthorizationTransactionCode,
                AuthorizationTransactionResult = processPaymentResult.AuthorizationTransactionResult,
                CaptureTransactionId = processPaymentResult.CaptureTransactionId,
                CaptureTransactionResult = processPaymentResult.CaptureTransactionResult,
                SubscriptionTransactionId = processPaymentResult.SubscriptionTransactionId,
                PaymentStatus = processPaymentResult.NewPaymentStatus,
                PaidDateUtc = null,
                BillingAddress = details.BillingAddress,
                ShippingAddress = details.ShippingAddress,
                ShippingStatus = details.ShippingStatus,
                ShippingMethod = details.ShippingMethodName,
                PickupInStore = details.PickupInStore,
                PickupAddress = details.PickupAddress,
                ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
                CustomValuesXml = _paymentService.SerializeCustomValues(processPaymentRequest),
                VatNumber = details.VatNumber,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            _orderService.InsertOrder(order);

            //generate and set custom order number
            order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
            _orderService.UpdateOrder(order);

            //reward points history
            if (details.RedeemedRewardPointsAmount <= decimal.Zero)
                return order;

            _rewardPointService.AddRewardPointsHistoryEntry(details.Customer, -details.RedeemedRewardPoints, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
                order, details.RedeemedRewardPointsAmount);
            _customerService.UpdateCustomer(details.Customer);

            return order;
        }

        /// <summary>
        /// Send "order placed" notifications and save order notes
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void SendNotificationsAndSaveNotes(Order order)
        {
            //notes, messages
            AddOrderNote(order, _workContext.OriginalCustomerIfImpersonated != null
                ? $"Order placed by a store owner ('{_workContext.OriginalCustomerIfImpersonated.Email}'. ID = {_workContext.OriginalCustomerIfImpersonated.Id}) impersonating the customer."
                : "Order placed");

            //send email notifications
            var orderPlacedStoreOwnerNotificationQueuedEmailIds = _workflowMessageService.SendOrderPlacedStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
            if (orderPlacedStoreOwnerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order placed\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedStoreOwnerNotificationQueuedEmailIds)}.");

            var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                _pdfService.PrintOrderToPdf(order) : null;
            var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
                "order.pdf" : null;
            var orderPlacedCustomerNotificationQueuedEmailIds = _workflowMessageService
                .SendOrderPlacedCustomerNotification(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
            if (orderPlacedCustomerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order placed\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedCustomerNotificationQueuedEmailIds)}.");

            var vendors = GetVendorsInOrder(order);
            foreach (var vendor in vendors)
            {
                var orderPlacedVendorNotificationQueuedEmailIds = _workflowMessageService.SendOrderPlacedVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                if (orderPlacedVendorNotificationQueuedEmailIds.Any())
                    AddOrderNote(order, $"\"Order placed\" email (to vendor) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedVendorNotificationQueuedEmailIds)}.");
            }

            if (order.AffiliateId == 0)
                return;

            var orderPlacedAffiliateNotificationQueuedEmailIds = _workflowMessageService.SendOrderPlacedAffiliateNotification(order, _localizationSettings.DefaultAdminLanguageId);
            if (orderPlacedAffiliateNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order placed\" email (to affiliate) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedAffiliateNotificationQueuedEmailIds)}.");
        }

        /// <summary>
        /// Award (earn) reward points (for placing a new order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void AwardRewardPoints(Order order)
        {
            var totalForRewardPoints = _orderTotalCalculationService
                .CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
            var points = totalForRewardPoints > decimal.Zero ?
                _orderTotalCalculationService.CalculateRewardPoints(order.Customer, totalForRewardPoints) : 0;
            if (points == 0)
                return;

            //Ensure that reward points were not added (earned) before. We should not add reward points if they were already earned for this order
            if (order.RewardPointsHistoryEntryId.HasValue)
                return;

            //check whether delay is set
            DateTime? activatingDate = null;
            if (_rewardPointsSettings.ActivationDelay > 0)
            {
                var delayPeriod = (RewardPointsActivatingDelayPeriod)_rewardPointsSettings.ActivationDelayPeriodId;
                var delayInHours = delayPeriod.ToHours(_rewardPointsSettings.ActivationDelay);
                activatingDate = DateTime.UtcNow.AddHours(delayInHours);
            }

            //whether points validity is set
            DateTime? endDate = null;
            if (_rewardPointsSettings.PurchasesPointsValidity > 0)
                endDate = (activatingDate ?? DateTime.UtcNow).AddDays(_rewardPointsSettings.PurchasesPointsValidity.Value);

            //add reward points
            order.RewardPointsHistoryEntryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForOrder"), order.CustomOrderNumber),
                activatingDate: activatingDate, endDate: endDate);

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Reduce (cancel) reward points (previously awarded for placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ReduceRewardPoints(Order order)
        {
            var totalForRewardPoints = _orderTotalCalculationService
                .CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
            var points = totalForRewardPoints > decimal.Zero ?
                _orderTotalCalculationService.CalculateRewardPoints(order.Customer, totalForRewardPoints) : 0;
            if (points == 0)
                return;

            //ensure that reward points were already earned for this order before
            if (!order.RewardPointsHistoryEntryId.HasValue)
                return;

            //get appropriate history entry
            var rewardPointsHistoryEntry = _rewardPointService.GetRewardPointsHistoryEntryById(order.RewardPointsHistoryEntryId.Value);
            if (rewardPointsHistoryEntry != null && rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                _rewardPointService.DeleteRewardPointsHistoryEntry(rewardPointsHistoryEntry);
            }
            else
            {
                //or reduce reward points if the entry already exists
                _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -points, order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForOrder"), order.CustomOrderNumber));
            }

            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Return back redeemed reward points to a customer (spent when placing an order)
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ReturnBackRedeemedRewardPoints(Order order)
        {
            //were some points redeemed when placing an order?
            if (order.RedeemedRewardPointsEntry == null)
                return;

            //return back
            _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, -order.RedeemedRewardPointsEntry.Points, order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
            _orderService.UpdateOrder(order);
        }

        /// <summary>
        /// Set IsActivated value for purchase gift cards for particular order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="activate">A value indicating whether to activate gift cards; true - activate, false - deactivate</param>
        protected virtual void SetActivatedValueForPurchasedGiftCards(Order order, bool activate)
        {
            var giftCards = _giftCardService.GetAllGiftCards(order.Id, isGiftCardActivated: !activate);
            foreach (var gc in giftCards)
            {
                if (activate)
                {
                    //activate
                    var isRecipientNotified = gc.IsRecipientNotified;
                    if (gc.GiftCardType == GiftCardType.Virtual)
                    {
                        //send email for virtual gift card
                        if (!string.IsNullOrEmpty(gc.RecipientEmail) &&
                            !string.IsNullOrEmpty(gc.SenderEmail))
                        {
                            var customerLang = _languageService.GetLanguageById(order.CustomerLanguageId) ??
                                               _languageService.GetAllLanguages().FirstOrDefault();
                            if (customerLang == null)
                                throw new Exception("No languages could be loaded");
                            var queuedEmailIds = _workflowMessageService.SendGiftCardNotification(gc, customerLang.Id);
                            if (queuedEmailIds.Any())
                                isRecipientNotified = true;
                        }
                    }

                    gc.IsGiftCardActivated = true;
                    gc.IsRecipientNotified = isRecipientNotified;
                    _giftCardService.UpdateGiftCard(gc);
                }
                else
                {
                    //deactivate
                    gc.IsGiftCardActivated = false;
                    _giftCardService.UpdateGiftCard(gc);
                }
            }
        }

        /// <summary>
        /// Sets an order status
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="os">New order status</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        protected virtual void SetOrderStatus(Order order, OrderStatus os, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var prevOrderStatus = order.OrderStatus;
            if (prevOrderStatus == os)
                return;

            //set and save new order status
            order.OrderStatusId = (int)os;
            _orderService.UpdateOrder(order);

            //order notes, notifications
            AddOrderNote(order, $"Order status has been changed to {os.ToString()}");

            if (prevOrderStatus != OrderStatus.Complete &&
                os == OrderStatus.Complete
                && notifyCustomer)
            {
                //notification
                var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    _pdfService.PrintOrderToPdf(order) : null;
                var orderCompletedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                    "order.pdf" : null;
                var orderCompletedCustomerNotificationQueuedEmailIds = _workflowMessageService
                    .SendOrderCompletedCustomerNotification(order, order.CustomerLanguageId, orderCompletedAttachmentFilePath,
                    orderCompletedAttachmentFileName);
                if (orderCompletedCustomerNotificationQueuedEmailIds.Any())
                    AddOrderNote(order, $"\"Order completed\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderCompletedCustomerNotificationQueuedEmailIds)}.");
            }

            if (prevOrderStatus != OrderStatus.Cancelled &&
                os == OrderStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                var orderCancelledCustomerNotificationQueuedEmailIds = _workflowMessageService.SendOrderCancelledCustomerNotification(order, order.CustomerLanguageId);
                if (orderCancelledCustomerNotificationQueuedEmailIds.Any())
                    AddOrderNote(order, $"\"Order cancelled\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderCancelledCustomerNotificationQueuedEmailIds)}.");
            }

            //reward points
            if (order.OrderStatus == OrderStatus.Complete)
            {
                AwardRewardPoints(order);
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                ReduceRewardPoints(order);
            }

            //gift cards activation
            if (_orderSettings.ActivateGiftCardsAfterCompletingOrder && order.OrderStatus == OrderStatus.Complete)
            {
                SetActivatedValueForPurchasedGiftCards(order, true);
            }

            //gift cards deactivation
            if (_orderSettings.DeactivateGiftCardsAfterCancellingOrder && order.OrderStatus == OrderStatus.Cancelled)
            {
                SetActivatedValueForPurchasedGiftCards(order, false);
            }
        }

        /// <summary>
        /// Process order paid status
        /// </summary>
        /// <param name="order">Order</param>
        protected virtual void ProcessOrderPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //raise event
            _eventPublisher.Publish(new OrderPaidEvent(order));

            //order paid email notification
            if (order.OrderTotal != decimal.Zero)
            {
                //we should not send it for free ($0 total) orders?
                //remove this "if" statement if you want to send it in this case

                var orderPaidAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    _pdfService.PrintOrderToPdf(order) : null;
                var orderPaidAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                    "order.pdf" : null;
                _workflowMessageService.SendOrderPaidCustomerNotification(order, order.CustomerLanguageId,
                    orderPaidAttachmentFilePath, orderPaidAttachmentFileName);

                _workflowMessageService.SendOrderPaidStoreOwnerNotification(order, _localizationSettings.DefaultAdminLanguageId);
                var vendors = GetVendorsInOrder(order);
                foreach (var vendor in vendors)
                {
                    _workflowMessageService.SendOrderPaidVendorNotification(order, vendor, _localizationSettings.DefaultAdminLanguageId);
                }

                if (order.AffiliateId != 0)
                    _workflowMessageService.SendOrderPaidAffiliateNotification(order, _localizationSettings.DefaultAdminLanguageId);

                //TODO add "order paid email sent" order note
            }

            //customer roles with "purchased with product" specified
            ProcessCustomerRolesWithPurchasedProductSpecified(order, true);
        }

        /// <summary>
        /// Process customer roles with "Purchased with Product" property configured
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
        protected virtual void ProcessCustomerRolesWithPurchasedProductSpecified(Order order, bool add)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //purchased product identifiers
            var purchasedProductIds = new List<int>();
            foreach (var orderItem in order.OrderItems)
            {
                //standard items
                purchasedProductIds.Add(orderItem.ProductId);

                //bundled (associated) products
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(orderItem.AttributesXml);
                foreach (var attributeValue in attributeValues)
                {
                    if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                        purchasedProductIds.Add(attributeValue.AssociatedProductId);
                    }
                }
            }

            //list of customer roles
            var customerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Where(cr => purchasedProductIds.Contains(cr.PurchasedWithProductId))
                .ToList();

            if (!customerRoles.Any())
                return;

            var customer = order.Customer;
            foreach (var customerRole in customerRoles)
            {
                if (customer.CustomerCustomerRoleMappings.Count(mapping => mapping.CustomerRoleId == customerRole.Id) == 0)
                {
                    //not in the list yet
                    if (add)
                    {
                        //add
                        customer.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = customerRole });
                    }
                }
                else
                {
                    //already in the list
                    if (!add)
                    {
                        //remove
                        customer.RemoveCustomerRoleMapping(
                            customer.CustomerCustomerRoleMappings.FirstOrDefault(mapping => mapping.CustomerRoleId == customerRole.Id));
                    }
                }
            }

            _customerService.UpdateCustomer(customer);
        }

        /// <summary>
        /// Get a list of vendors in order (order items)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Vendors</returns>
        protected virtual IList<Vendor> GetVendorsInOrder(Order order)
        {
            var vendors = new List<Vendor>();
            foreach (var orderItem in order.OrderItems)
            {
                var vendorId = orderItem.Product.VendorId;
                //find existing
                var vendor = vendors.FirstOrDefault(v => v.Id == vendorId);
                if (vendor != null)
                    continue;

                //not found. load by Id
                vendor = _vendorService.GetVendorById(vendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    vendors.Add(vendor);
                }
            }

            return vendors;
        }

        /// <summary>
        /// Create recurring payment (the first payment)
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="order">Order</param>
        protected virtual void CreateFirstRecurringPayment(ProcessPaymentRequest processPaymentRequest, Order order)
        {
            var rp = new RecurringPayment
            {
                CycleLength = processPaymentRequest.RecurringCycleLength,
                CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
                TotalCycles = processPaymentRequest.RecurringTotalCycles,
                StartDateUtc = DateTime.UtcNow,
                IsActive = true,
                CreatedOnUtc = DateTime.UtcNow,
                InitialOrder = order
            };
            _orderService.InsertRecurringPayment(rp);

            switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
            {
                case RecurringPaymentType.NotSupported:
                    //not supported
                    break;
                case RecurringPaymentType.Manual:
                    rp.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                    {
                        RecurringPayment = rp,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id
                    });
                    _orderService.UpdateRecurringPayment(rp);
                    break;
                case RecurringPaymentType.Automatic:
                    //will be created later (process is automated)
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Move shopping cart items to order items
        /// </summary>
        /// <param name="details">Place order container</param>
        /// <param name="order">Order</param>
        protected virtual void MoveShoppingCartItemsToOrderItems(PlaceOrderContainer details, Order order)
        {
            foreach (var sc in details.Cart)
            {
                //prices
                var scUnitPrice = _priceCalculationService.GetUnitPrice(sc);
                var scSubTotal = _priceCalculationService.GetSubTotal(sc, true, out var discountAmount,
                    out var scDiscounts, out _);
                var scUnitPriceInclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, true, details.Customer, out var _);
                var scUnitPriceExclTax =
                    _taxService.GetProductPrice(sc.Product, scUnitPrice, false, details.Customer, out _);
                var scSubTotalInclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, true, details.Customer, out _);
                var scSubTotalExclTax =
                    _taxService.GetProductPrice(sc.Product, scSubTotal, false, details.Customer, out _);
                var discountAmountInclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, true, details.Customer, out _);
                var discountAmountExclTax =
                    _taxService.GetProductPrice(sc.Product, discountAmount, false, details.Customer, out _);
                foreach (var disc in scDiscounts)
                    if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                        details.AppliedDiscounts.Add(disc);

                //attributes
                var attributeDescription =
                    _productAttributeFormatter.FormatAttributes(sc.Product, sc.AttributesXml, details.Customer);

                var itemWeight = _shippingService.GetShoppingCartItemWeight(sc);

                //save order item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    Order = order,
                    ProductId = sc.ProductId,
                    UnitPriceInclTax = scUnitPriceInclTax,
                    UnitPriceExclTax = scUnitPriceExclTax,
                    PriceInclTax = scSubTotalInclTax,
                    PriceExclTax = scSubTotalExclTax,
                    OriginalProductCost = _priceCalculationService.GetProductCost(sc.Product, sc.AttributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = sc.AttributesXml,
                    Quantity = sc.Quantity,
                    DiscountAmountInclTax = discountAmountInclTax,
                    DiscountAmountExclTax = discountAmountExclTax,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = sc.RentalStartDateUtc,
                    RentalEndDateUtc = sc.RentalEndDateUtc
                };
                order.OrderItems.Add(orderItem);
                _orderService.UpdateOrder(order);

                //gift cards
                AddGiftCards(sc.Product, sc.AttributesXml, sc.Quantity, orderItem, scUnitPriceExclTax);

                //inventory
                _productService.AdjustInventory(sc.Product, -sc.Quantity, sc.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.PlaceOrder"),
                        order.Id));
            }

            //clear shopping cart
            details.Cart.ToList().ForEach(sci => _shoppingCartService.DeleteShoppingCartItem(sci, false));
        }

        /// <summary>
        /// Add gift cards
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">attributes XML</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="unitPriceExclTax">Unit price exclude tax, it set as amount if not set specific amount and product.OverriddenGiftCardAmount isn't set to</param>
        /// <param name="amount">Amount</param>
        protected virtual void AddGiftCards(Product product, string attributesXml, int quantity, OrderItem orderItem, decimal? unitPriceExclTax = null, decimal? amount = null)
        {
            if (!product.IsGiftCard)
                return;

            _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

            for (var i = 0; i < quantity; i++)
            {
                _giftCardService.InsertGiftCard(new GiftCard
                {
                    GiftCardType = product.GiftCardType,
                    PurchasedWithOrderItem = orderItem,
                    Amount = amount ?? product.OverriddenGiftCardAmount ?? unitPriceExclTax ?? 0,
                    IsGiftCardActivated = false,
                    GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                    RecipientName = giftCardRecipientName,
                    RecipientEmail = giftCardRecipientEmail,
                    SenderName = giftCardSenderName,
                    SenderEmail = giftCardSenderEmail,
                    Message = giftCardMessage,
                    IsRecipientNotified = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get process payment result
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <param name="details">Place order container</param>
        /// <returns></returns>
        protected virtual ProcessPaymentResult GetProcessPaymentResult(ProcessPaymentRequest processPaymentRequest, PlaceOrderContainer details)
        {
            //process payment
            ProcessPaymentResult processPaymentResult;
            //skip payment workflow if order total equals zero
            var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
            if (!skipPaymentWorkflow)
            {
                var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName)
                    ?? throw new NopException("Payment method couldn't be loaded");

                //ensure that payment method is active
                if (!_paymentPluginManager.IsPluginActive(paymentMethod))
                    throw new NopException("Payment method is not active");

                if (details.IsRecurringShoppingCart)
                {
                    //recurring cart
                    switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                    {
                        case RecurringPaymentType.NotSupported:
                            throw new NopException("Recurring payments are not supported by selected payment method");
                        case RecurringPaymentType.Manual:
                        case RecurringPaymentType.Automatic:
                            processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                            break;
                        default:
                            throw new NopException("Not supported recurring payment type");
                    }
                }
                else
                    //standard cart
                    processPaymentResult = _paymentService.ProcessPayment(processPaymentRequest);
            }
            else
                //payment is not required
                processPaymentResult = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };
            return processPaymentResult;
        }

        /// <summary>
        /// Save gift card usage history
        /// </summary>
        /// <param name="details">Place order container</param>
        /// <param name="order">Order</param>
        protected virtual void SaveGiftCardUsageHistory(PlaceOrderContainer details, Order order)
        {
            if (details.AppliedGiftCards == null || !details.AppliedGiftCards.Any())
                return;

            foreach (var agc in details.AppliedGiftCards)
            {
                agc.GiftCard.GiftCardUsageHistory.Add(new GiftCardUsageHistory
                {
                    GiftCard = agc.GiftCard,
                    UsedWithOrder = order,
                    UsedValue = agc.AmountCanBeUsed,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _giftCardService.UpdateGiftCard(agc.GiftCard);
            }
        }

        /// <summary>
        /// Save discount usage history
        /// </summary>
        /// <param name="details">PlaceOrderContainer</param>
        /// <param name="order">Order</param>
        protected virtual void SaveDiscountUsageHistory(PlaceOrderContainer details, Order order)
        {
            if (details.AppliedDiscounts == null || !details.AppliedDiscounts.Any())
                return;

            foreach (var discount in details.AppliedDiscounts)
            {
                var d = _discountService.GetDiscountById(discount.Id);
                if (d != null)
                {
                    _discountService.InsertDiscountUsageHistory(new DiscountUsageHistory
                    {
                        Discount = d,
                        Order = order,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks order status
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void CheckOrderStatus(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.PaymentStatus == PaymentStatus.Paid && !order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);
            }

            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    if (order.PaymentStatus == PaymentStatus.Authorized ||
                        order.PaymentStatus == PaymentStatus.Paid)
                    {
                        SetOrderStatus(order, OrderStatus.Processing, false);
                    }

                    if (order.ShippingStatus == ShippingStatus.PartiallyShipped ||
                        order.ShippingStatus == ShippingStatus.Shipped ||
                        order.ShippingStatus == ShippingStatus.Delivered)
                    {
                        SetOrderStatus(order, OrderStatus.Processing, false);
                    }

                    break;
                //is order complete?
                case OrderStatus.Cancelled:
                case OrderStatus.Complete:
                    return;
            }

            if (order.PaymentStatus != PaymentStatus.Paid)
                return;

            bool completed;

            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
            {
                //shipping is not required
                completed = true;
            }
            else
            {
                //shipping is required
                if (_orderSettings.CompleteOrderWhenDelivered)
                {
                    completed = order.ShippingStatus == ShippingStatus.Delivered;
                }
                else
                {
                    completed = order.ShippingStatus == ShippingStatus.Shipped ||
                                order.ShippingStatus == ShippingStatus.Delivered;
                }
            }

            if (completed)
            {
                SetOrderStatus(order, OrderStatus.Complete, true);
            }
        }

        /// <summary>
        /// Places an order
        /// </summary>
        /// <param name="processPaymentRequest">Process payment request</param>
        /// <returns>Place order result</returns>
        public virtual PlaceOrderResult PlaceOrder(ProcessPaymentRequest processPaymentRequest)
        {
            if (processPaymentRequest == null)
                throw new ArgumentNullException(nameof(processPaymentRequest));

            var result = new PlaceOrderResult();
            try
            {
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    throw new Exception("Order GUID is not generated");

                //prepare order details
                var details = PreparePlaceOrderDetails(processPaymentRequest);

                var processPaymentResult = GetProcessPaymentResult(processPaymentRequest, details);

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);
                    result.PlacedOrder = order;

                    //move shopping cart items to order items
                    MoveShoppingCartItemsToOrderItems(details, order);

                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //gift card usage history
                    SaveGiftCardUsageHistory(details, order);

                    //recurring orders
                    if (details.IsRecurringShoppingCart)
                    {
                        CreateFirstRecurringPayment(processPaymentRequest, order);
                    }

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //reset checkout data
                    _customerService.ResetCheckoutData(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                    _customerActivityService.InsertActivity("PublicStore.PlaceOrder",
                        string.Format(_localizationService.GetResource("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);
                }
                else
                    foreach (var paymentError in processPaymentResult.Errors)
                        result.AddError(string.Format(_localizationService.GetResource("Checkout.PaymentError"), paymentError));
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
                result.AddError(exc.Message);
            }

            if (result.Success)
                return result;

            //log errors
            var logError = result.Errors.Aggregate("Error while placing order. ",
                (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            _logger.Error(logError, customer: customer);

            return result;
        }

        /// <summary>
        /// Update order totals
        /// </summary>
        /// <param name="updateOrderParameters">Parameters for the updating order</param>
        public virtual void UpdateOrderTotals(UpdateOrderParameters updateOrderParameters)
        {
            if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                return;

            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

            //restore shopping cart from order items
            var restoredCart = updatedOrder.OrderItems.Select(orderItem => new ShoppingCartItem
            {
                Id = orderItem.Id,
                AttributesXml = orderItem.AttributesXml,
                Customer = updatedOrder.Customer,
                Product = orderItem.Product,
                Quantity = orderItem.Id == updatedOrderItem.Id ? updateOrderParameters.Quantity : orderItem.Quantity,
                RentalEndDateUtc = orderItem.RentalEndDateUtc,
                RentalStartDateUtc = orderItem.RentalStartDateUtc,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = updatedOrder.StoreId
            }).ToList();

            //get shopping cart item which has been updated
            var updatedShoppingCartItem = restoredCart.FirstOrDefault(shoppingCartItem => shoppingCartItem.Id == updatedOrderItem.Id);
            var itemDeleted = updatedShoppingCartItem == null;

            //validate shopping cart for warnings
            updateOrderParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartWarnings(restoredCart, string.Empty, false));
            if (!itemDeleted)
                updateOrderParameters.Warnings.AddRange(_shoppingCartService.GetShoppingCartItemWarnings(updatedOrder.Customer, updatedShoppingCartItem.ShoppingCartType,
                    updatedShoppingCartItem.Product, updatedOrder.StoreId, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.CustomerEnteredPrice,
                    updatedShoppingCartItem.RentalStartDateUtc, updatedShoppingCartItem.RentalEndDateUtc, updatedShoppingCartItem.Quantity, false, updatedShoppingCartItem.Id));

            _orderTotalCalculationService.UpdateOrderTotals(updateOrderParameters, restoredCart);

            if (updateOrderParameters.PickupPoint != null)
            {
                updatedOrder.PickupInStore = true;
                updatedOrder.PickupAddress = new Address
                {
                    Address1 = updateOrderParameters.PickupPoint.Address,
                    City = updateOrderParameters.PickupPoint.City,
                    County = updateOrderParameters.PickupPoint.County,
                    Country = _countryService.GetCountryByTwoLetterIsoCode(updateOrderParameters.PickupPoint.CountryCode),
                    ZipPostalCode = updateOrderParameters.PickupPoint.ZipPostalCode,
                    CreatedOnUtc = DateTime.UtcNow
                };
                updatedOrder.ShippingMethod = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), updateOrderParameters.PickupPoint.Name);
                updatedOrder.ShippingRateComputationMethodSystemName = updateOrderParameters.PickupPoint.ProviderSystemName;
            }

            if (!itemDeleted)
            {
                updatedOrderItem.ItemWeight = _shippingService.GetShoppingCartItemWeight(updatedShoppingCartItem);
                updatedOrderItem.OriginalProductCost = _priceCalculationService.GetProductCost(updatedShoppingCartItem.Product, updatedShoppingCartItem.AttributesXml);
                updatedOrderItem.AttributeDescription = _productAttributeFormatter.FormatAttributes(updatedShoppingCartItem.Product,
                    updatedShoppingCartItem.AttributesXml, updatedOrder.Customer);

                //gift cards
                AddGiftCards(updatedShoppingCartItem.Product, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.Quantity, updatedOrderItem, updatedOrderItem.UnitPriceExclTax);
            }

            _orderService.UpdateOrder(updatedOrder);

            //discount usage history
            var discountUsageHistoryForOrder = _discountService.GetAllDiscountUsageHistory(null, updatedOrder.Customer.Id, updatedOrder.Id);
            foreach (var discount in updateOrderParameters.AppliedDiscounts)
            {
                if (discountUsageHistoryForOrder.Any(history => history.DiscountId == discount.Id))
                    continue;

                var d = _discountService.GetDiscountById(discount.Id);
                if (d != null)
                {
                    _discountService.InsertDiscountUsageHistory(new DiscountUsageHistory
                    {
                        Discount = d,
                        Order = updatedOrder,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                }
            }

            CheckOrderStatus(updatedOrder);
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void DeleteOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //check whether the order wasn't cancelled before
            //if it already was cancelled, then there's no need to make the following adjustments
            //(such as reward points, inventory, recurring payments)
            //they already was done when cancelling the order
            if (order.OrderStatus != OrderStatus.Cancelled)
            {
                //return (add) back redeemded reward points
                ReturnBackRedeemedRewardPoints(order);
                //reduce (cancel) back reward points (previously awarded for this order)
                ReduceRewardPoints(order);

                //cancel recurring payments
                var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);
                foreach (var rp in recurringPayments)
                {
                    CancelRecurringPayment(rp);
                }

                //Adjust inventory for already shipped shipments
                //only products with "use multiple warehouses"
                foreach (var shipment in order.Shipments)
                {
                    foreach (var shipmentItem in shipment.ShipmentItems)
                    {
                        var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                        if (orderItem == null)
                            continue;

                        _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                            string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                    }
                }

                //Adjust inventory
                foreach (var orderItem in order.OrderItems)
                {
                    _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                        string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
                }
            }

            //deactivate gift cards
            if (_orderSettings.DeactivateGiftCardsAfterDeletingOrder)
                SetActivatedValueForPurchasedGiftCards(order, false);

            //add a note
            AddOrderNote(order, "Order has been deleted");

            //now delete an order
            _orderService.DeleteOrder(order);
        }

        /// <summary>
        /// Process next recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
        /// <returns>Collection of errors</returns>
        public virtual IEnumerable<string> ProcessNextRecurringPayment(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            try
            {
                if (!recurringPayment.IsActive)
                    throw new NopException("Recurring payment is not active");

                var initialOrder = recurringPayment.InitialOrder;
                if (initialOrder == null)
                    throw new NopException("Initial order could not be loaded");

                var customer = initialOrder.Customer;
                if (customer == null)
                    throw new NopException("Customer could not be loaded");

                var nextPaymentDate = recurringPayment.NextPaymentDate;
                if (!nextPaymentDate.HasValue)
                    throw new NopException("Next payment date could not be calculated");

                //payment info
                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = initialOrder.StoreId,
                    CustomerId = customer.Id,
                    OrderGuid = Guid.NewGuid(),
                    InitialOrderId = initialOrder.Id,
                    RecurringCycleLength = recurringPayment.CycleLength,
                    RecurringCyclePeriod = recurringPayment.CyclePeriod,
                    RecurringTotalCycles = recurringPayment.TotalCycles,
                    CustomValues = _paymentService.DeserializeCustomValues(initialOrder)
                };

                //prepare order details
                var details = PrepareRecurringOrderDetails(processPaymentRequest);

                ProcessPaymentResult processPaymentResult;
                //skip payment workflow if order total equals zero
                var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
                if (!skipPaymentWorkflow)
                {
                    var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(processPaymentRequest.PaymentMethodSystemName)
                        ?? throw new NopException("Payment method couldn't be loaded");

                    if (!_paymentPluginManager.IsPluginActive(paymentMethod))
                        throw new NopException("Payment method is not active");

                    //Old credit card info
                    if (details.InitialOrder.AllowStoringCreditCardNumber)
                    {
                        processPaymentRequest.CreditCardType = _encryptionService.DecryptText(details.InitialOrder.CardType);
                        processPaymentRequest.CreditCardName = _encryptionService.DecryptText(details.InitialOrder.CardName);
                        processPaymentRequest.CreditCardNumber = _encryptionService.DecryptText(details.InitialOrder.CardNumber);
                        processPaymentRequest.CreditCardCvv2 = _encryptionService.DecryptText(details.InitialOrder.CardCvv2);
                        try
                        {
                            processPaymentRequest.CreditCardExpireMonth = Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationMonth));
                            processPaymentRequest.CreditCardExpireYear = Convert.ToInt32(_encryptionService.DecryptText(details.InitialOrder.CardExpirationYear));
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    //payment type
                    switch (_paymentService.GetRecurringPaymentType(processPaymentRequest.PaymentMethodSystemName))
                    {
                        case RecurringPaymentType.NotSupported:
                            throw new NopException("Recurring payments are not supported by selected payment method");
                        case RecurringPaymentType.Manual:
                            processPaymentResult = _paymentService.ProcessRecurringPayment(processPaymentRequest);
                            break;
                        case RecurringPaymentType.Automatic:
                            //payment is processed on payment gateway site, info about last transaction in paymentResult parameter
                            processPaymentResult = paymentResult ?? new ProcessPaymentResult();
                            break;
                        default:
                            throw new NopException("Not supported recurring payment type");
                    }
                }
                else
                    processPaymentResult = paymentResult ?? new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

                if (processPaymentResult == null)
                    throw new NopException("processPaymentResult is not available");

                if (processPaymentResult.Success)
                {
                    //save order details
                    var order = SaveOrderDetails(processPaymentRequest, processPaymentResult, details);

                    foreach (var orderItem in details.InitialOrder.OrderItems)
                    {
                        //save item
                        var newOrderItem = new OrderItem
                        {
                            OrderItemGuid = Guid.NewGuid(),
                            Order = order,
                            ProductId = orderItem.ProductId,
                            UnitPriceInclTax = orderItem.UnitPriceInclTax,
                            UnitPriceExclTax = orderItem.UnitPriceExclTax,
                            PriceInclTax = orderItem.PriceInclTax,
                            PriceExclTax = orderItem.PriceExclTax,
                            OriginalProductCost = orderItem.OriginalProductCost,
                            AttributeDescription = orderItem.AttributeDescription,
                            AttributesXml = orderItem.AttributesXml,
                            Quantity = orderItem.Quantity,
                            DiscountAmountInclTax = orderItem.DiscountAmountInclTax,
                            DiscountAmountExclTax = orderItem.DiscountAmountExclTax,
                            DownloadCount = 0,
                            IsDownloadActivated = false,
                            LicenseDownloadId = 0,
                            ItemWeight = orderItem.ItemWeight,
                            RentalStartDateUtc = orderItem.RentalStartDateUtc,
                            RentalEndDateUtc = orderItem.RentalEndDateUtc
                        };
                        order.OrderItems.Add(newOrderItem);
                        _orderService.UpdateOrder(order);

                        //gift cards
                        AddGiftCards(orderItem.Product, orderItem.AttributesXml, orderItem.Quantity, newOrderItem, amount: orderItem.UnitPriceExclTax);

                        //inventory
                        _productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml,
                            string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.PlaceOrder"), order.Id));
                    }

                    //discount usage history
                    SaveDiscountUsageHistory(details, order);

                    //notifications
                    SendNotificationsAndSaveNotes(order);

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderPlacedEvent(order));

                    if (order.PaymentStatus == PaymentStatus.Paid)
                        ProcessOrderPaid(order);

                    //last payment succeeded
                    recurringPayment.LastPaymentFailed = false;

                    //next recurring payment
                    recurringPayment.RecurringPaymentHistory.Add(new RecurringPaymentHistory
                    {
                        RecurringPayment = recurringPayment,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = order.Id
                    });
                    _orderService.UpdateRecurringPayment(recurringPayment);

                    return new List<string>();
                }

                //log errors
                var logError = processPaymentResult.Errors.Aggregate("Error while processing recurring order. ",
                    (current, next) => $"{current}Error {processPaymentResult.Errors.IndexOf(next) + 1}: {next}. ");
                _logger.Error(logError, customer: customer);

                if (!processPaymentResult.RecurringPaymentFailed)
                    return processPaymentResult.Errors;

                //set flag that last payment failed
                recurringPayment.LastPaymentFailed = true;
                _orderService.UpdateRecurringPayment(recurringPayment);

                if (_paymentSettings.CancelRecurringPaymentsAfterFailedPayment)
                {
                    //cancel recurring payment
                    CancelRecurringPayment(recurringPayment).ToList().ForEach(error => _logger.Error(error));

                    //notify a customer about cancelled payment
                    _workflowMessageService.SendRecurringPaymentCancelledCustomerNotification(recurringPayment, initialOrder.CustomerLanguageId);
                }
                else
                    //notify a customer about failed payment
                    _workflowMessageService.SendRecurringPaymentFailedCustomerNotification(recurringPayment, initialOrder.CustomerLanguageId);

                return processPaymentResult.Errors;
            }
            catch (Exception exc)
            {
                _logger.Error($"Error while processing recurring order. {exc.Message}", exc);
                throw;
            }
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        public virtual IList<string> CancelRecurringPayment(RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return new List<string> { "Initial order could not be loaded" };

            var request = new CancelRecurringPaymentRequest();
            CancelRecurringPaymentResult result = null;
            try
            {
                request.Order = initialOrder;
                result = _paymentService.CancelRecurringPayment(request);
                if (result.Success)
                {
                    //update recurring payment
                    recurringPayment.IsActive = false;
                    _orderService.UpdateRecurringPayment(recurringPayment);

                    //add a note
                    initialOrder.OrderNotes.Add(new OrderNote
                    {
                        Note = "Recurring payment has been cancelled",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(initialOrder);

                    //notify a store owner
                    _workflowMessageService
                        .SendRecurringPaymentCancelledStoreOwnerNotification(recurringPayment,
                        _localizationSettings.DefaultAdminLanguageId);
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CancelRecurringPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            initialOrder.OrderNotes.Add(new OrderNote
            {
                Note = $"Unable to cancel recurring payment. {error}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(initialOrder);

            //log it
            var logError = $"Error cancelling recurring payment. Order #{initialOrder.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can cancel recurring payment
        /// </summary>
        /// <param name="customerToValidate">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>value indicating whether a customer can cancel recurring payment</returns>
        public virtual bool CanCancelRecurringPayment(Customer customerToValidate, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null)
                return false;

            if (customerToValidate == null)
                return false;

            var initialOrder = recurringPayment.InitialOrder;
            if (initialOrder == null)
                return false;

            var customer = recurringPayment.InitialOrder.Customer;
            if (customer == null)
                return false;

            if (initialOrder.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (!customerToValidate.IsAdmin())
            {
                if (customer.Id != customerToValidate.Id)
                    return false;
            }

            if (!recurringPayment.NextPaymentDate.HasValue)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a customer can retry last failed recurring payment
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="recurringPayment">Recurring Payment</param>
        /// <returns>True if a customer can retry payment; otherwise false</returns>
        public virtual bool CanRetryLastRecurringPayment(Customer customer, RecurringPayment recurringPayment)
        {
            if (recurringPayment == null || customer == null)
                return false;

            if (recurringPayment.InitialOrder == null || recurringPayment.InitialOrder.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (!recurringPayment.LastPaymentFailed || _paymentService.GetRecurringPaymentType(recurringPayment.InitialOrder.PaymentMethodSystemName) != RecurringPaymentType.Manual)
                return false;

            if (recurringPayment.InitialOrder.Customer == null || (!customer.IsAdmin() && recurringPayment.InitialOrder.Customer.Id != customer.Id))
                return false;

            return true;
        }

        /// <summary>
        /// Send a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Ship(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is already shipped");

            shipment.ShippedDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            //process products with "Multiple warehouse" support enabled
            foreach (var item in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                _productService.BookReservedInventory(orderItem.Product, item.WarehouseId, -item.Quantity,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.Ship"), shipment.OrderId));
            }

            //check whether we have more items to ship
            if (_orderService.HasItemsToAddToShipment(order) || _orderService.HasItemsToShip(order))
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Shipment# {shipment.Id} has been sent");

            if (notifyCustomer)
            {
                //notify customer
                var queuedEmailIds = _workflowMessageService.SendShipmentSentCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailIds.Any())
                    AddOrderNote(order, $"\"Shipped\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", queuedEmailIds)}.");
            }

            //event
            _eventPublisher.PublishShipmentSent(shipment);

            //check order status
            CheckOrderStatus(order);
        }

        /// <summary>
        /// Marks a shipment as delivered
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void Deliver(Shipment shipment, bool notifyCustomer)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = shipment.Order;
            if (order == null)
                throw new Exception("Order cannot be loaded");

            if (!shipment.ShippedDateUtc.HasValue)
                throw new Exception("This shipment is not shipped yet");

            if (shipment.DeliveryDateUtc.HasValue)
                throw new Exception("This shipment is already delivered");

            shipment.DeliveryDateUtc = DateTime.UtcNow;
            _shipmentService.UpdateShipment(shipment);

            if (!_orderService.HasItemsToAddToShipment(order) && !_orderService.HasItemsToShip(order) && !_orderService.HasItemsToDeliver(order))
                order.ShippingStatusId = (int)ShippingStatus.Delivered;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Shipment# {shipment.Id} has been delivered");

            if (notifyCustomer)
            {
                //send email notification
                var queuedEmailIds = _workflowMessageService.SendShipmentDeliveredCustomerNotification(shipment, order.CustomerLanguageId);
                if (queuedEmailIds.Any())
                    AddOrderNote(order, $"\"Delivered\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", queuedEmailIds)}.");
            }

            //event
            _eventPublisher.PublishShipmentDelivered(shipment);

            //check order status
            CheckOrderStatus(order);
        }

        /// <summary>
        /// Gets a value indicating whether cancel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether cancel is allowed</returns>
        public virtual bool CanCancelOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            return true;
        }

        /// <summary>
        /// Cancels order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="notifyCustomer">True to notify customer</param>
        public virtual void CancelOrder(Order order, bool notifyCustomer)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCancelOrder(order))
                throw new NopException("Cannot do cancel for order.");

            //cancel order
            SetOrderStatus(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            AddOrderNote(order, "Order has been cancelled");

            //return (add) back redeemded reward points
            ReturnBackRedeemedRewardPoints(order);

            //delete gift card usage history
            if (_orderSettings.DeleteGiftCardUsageHistory)
            {
                _giftCardService.DeleteGiftCardUsageHistory(order);
            }

            //cancel recurring payments
            var recurringPayments = _orderService.SearchRecurringPayments(initialOrderId: order.Id);
            foreach (var rp in recurringPayments)
            {
                CancelRecurringPayment(rp);
            }

            //Adjust inventory for already shipped shipments
            //only products with "use multiple warehouses"
            foreach (var shipment in order.Shipments)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                        string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
                }
            }
            //Adjust inventory
            foreach (var orderItem in order.OrderItems)
            {
                _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
            }

            _eventPublisher.Publish(new OrderCancelledEvent(order));
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as authorized
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as authorized</returns>
        public virtual bool CanMarkOrderAsAuthorized(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Pending)
                return true;

            return false;
        }

        /// <summary>
        /// Marks order as authorized
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void MarkAsAuthorized(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            order.PaymentStatusId = (int)PaymentStatus.Authorized;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as authorized");

            //check order status
            CheckOrderStatus(order);
        }

        /// <summary>
        /// Gets a value indicating whether capture from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether capture from admin panel is allowed</returns>
        public virtual bool CanCapture(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportCapture(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Capture an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Capture(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanCapture(order))
                throw new NopException("Cannot do capture for order.");

            var request = new CapturePaymentRequest();
            CapturePaymentResult result = null;
            try
            {
                //old info from placing order
                request.Order = order;
                result = _paymentService.Capture(request);

                if (result.Success)
                {
                    var paidDate = order.PaidDateUtc;
                    if (result.NewPaymentStatus == PaymentStatus.Paid)
                        paidDate = DateTime.UtcNow;

                    order.CaptureTransactionId = result.CaptureTransactionId;
                    order.CaptureTransactionResult = result.CaptureTransactionResult;
                    order.PaymentStatus = result.NewPaymentStatus;
                    order.PaidDateUtc = paidDate;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, "Order has been captured");

                    CheckOrderStatus(order);

                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        ProcessOrderPaid(order);
                    }
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new CapturePaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to capture order. {error}");

            //log it
            var logError = $"Error capturing order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as paid
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as paid</returns>
        public virtual bool CanMarkOrderAsPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded ||
                order.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        /// <summary>
        /// Marks order as paid
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void MarkOrderAsPaid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanMarkOrderAsPaid(order))
                throw new NopException("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as paid");

            CheckOrderStatus(order);

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                ProcessOrderPaid(order);
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanRefund(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.SupportRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> Refund(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanRefund(order))
                throw new NopException("Cannot do refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = order.OrderTotal;
                request.IsPartialRefund = false;
                result = _paymentService.Refund(request);
                if (result.Success)
                {
                    //total amount refunded
                    var totalAmountRefunded = order.RefundedAmount + request.AmountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    order.PaymentStatus = result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, $"Order has been refunded. Amount = {request.AmountToRefund}");

                    //check order status
                    CheckOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, request.AmountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                        AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

                    var orderRefundedCustomerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedCustomerNotification(order, request.AmountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                        AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");

                    //raise event       
                    _eventPublisher.Publish(new OrderRefundedEvent(order, request.AmountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to refund order. {error}");

            //log it
            var logError = $"Error refunding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as refunded</returns>
        public virtual bool CanRefundOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //     return false;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        /// <summary>
        /// Refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void RefundOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanRefundOffline(order))
                throw new NopException("You can't refund this order");

            //amout to refund
            var amountToRefund = order.OrderTotal;

            //total amount refunded
            var totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            order.PaymentStatus = PaymentStatus.Refunded;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Order has been marked as refunded. Amount = {amountToRefund}");

            //check order status
            CheckOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

            var orderRefundedCustomerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");

            //raise event       
            _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether partial refund from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether refund from admin panel is allowed</returns>
        public virtual bool CanPartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.SupportPartiallyRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A list of errors; empty list if no errors</returns>
        public virtual IList<string> PartiallyRefund(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanPartiallyRefund(order, amountToRefund))
                throw new NopException("Cannot do partial refund for order.");

            var request = new RefundPaymentRequest();
            RefundPaymentResult result = null;
            try
            {
                request.Order = order;
                request.AmountToRefund = amountToRefund;
                request.IsPartialRefund = true;

                result = _paymentService.Refund(request);

                if (result.Success)
                {
                    //total amount refunded
                    var totalAmountRefunded = order.RefundedAmount + amountToRefund;

                    //update order info
                    order.RefundedAmount = totalAmountRefunded;
                    //mark payment status as 'Refunded' if the order total amount is fully refunded
                    order.PaymentStatus = order.OrderTotal == totalAmountRefunded && result.NewPaymentStatus == PaymentStatus.PartiallyRefunded ? PaymentStatus.Refunded : result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, $"Order has been partially refunded. Amount = {amountToRefund}");

                    //check order status
                    CheckOrderStatus(order);

                    //notifications
                    var orderRefundedStoreOwnerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
                    if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                        AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

                    var orderRefundedCustomerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
                    if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                        AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");

                    //raise event       
                    _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new RefundPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to partially refund order. {error}");

            //log it
            var logError = $"Error refunding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as partially refunded
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A value indicating whether order can be marked as partially refunded</returns>
        public virtual bool CanPartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        /// <summary>
        /// Partially refunds an order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public virtual void PartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanPartiallyRefundOffline(order, amountToRefund))
                throw new NopException("You can't partially refund (offline) this order");

            //total amount refunded
            var totalAmountRefunded = order.RefundedAmount + amountToRefund;

            //update order info
            order.RefundedAmount = totalAmountRefunded;
            //mark payment status as 'Refunded' if the order total amount is fully refunded
            order.PaymentStatus = order.OrderTotal == totalAmountRefunded ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, $"Order has been marked as partially refunded. Amount = {amountToRefund}");

            //check order status
            CheckOrderStatus(order);

            //notifications
            var orderRefundedStoreOwnerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedStoreOwnerNotification(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
            if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

            var orderRefundedCustomerNotificationQueuedEmailIds = _workflowMessageService.SendOrderRefundedCustomerNotification(order, amountToRefund, order.CustomerLanguageId);
            if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                AddOrderNote(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");

            //raise event       
            _eventPublisher.Publish(new OrderRefundedEvent(order, amountToRefund));
        }

        /// <summary>
        /// Gets a value indicating whether void from admin panel is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether void from admin panel is allowed</returns>
        public virtual bool CanVoid(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportVoid(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (from admin panel)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Voided order</returns>
        public virtual IList<string> Void(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanVoid(order))
                throw new NopException("Cannot do void for order.");

            var request = new VoidPaymentRequest();
            VoidPaymentResult result = null;
            try
            {
                request.Order = order;
                result = _paymentService.Void(request);

                if (result.Success)
                {
                    //update order info
                    order.PaymentStatus = result.NewPaymentStatus;
                    _orderService.UpdateOrder(order);

                    //add a note
                    AddOrderNote(order, "Order has been voided");

                    //check order status
                    CheckOrderStatus(order);

                    //raise event       
                    _eventPublisher.Publish(new OrderVoidedEvent(order));
                }
            }
            catch (Exception exc)
            {
                if (result == null)
                    result = new VoidPaymentResult();
                result.AddError($"Error: {exc.Message}. Full exception: {exc}");
            }

            //process errors
            var error = string.Empty;
            for (var i = 0; i < result.Errors.Count; i++)
            {
                error += $"Error {i}: {result.Errors[i]}";
                if (i != result.Errors.Count - 1)
                    error += ". ";
            }

            if (string.IsNullOrEmpty(error))
                return result.Errors;

            //add a note
            AddOrderNote(order, $"Unable to voiding order. {error}");

            //log it
            var logError = $"Error voiding order #{order.Id}. Error: {error}";
            _logger.InsertLog(LogLevel.Error, logError, logError);
            return result.Errors;
        }

        /// <summary>
        /// Gets a value indicating whether order can be marked as voided
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A value indicating whether order can be marked as voided</returns>
        public virtual bool CanVoidOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        /// <summary>
        /// Voids order (offline)
        /// </summary>
        /// <param name="order">Order</param>
        public virtual void VoidOffline(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!CanVoidOffline(order))
                throw new NopException("You can't void this order");

            order.PaymentStatusId = (int)PaymentStatus.Voided;
            _orderService.UpdateOrder(order);

            //add a note
            AddOrderNote(order, "Order has been marked as voided");

            //check order status
            CheckOrderStatus(order);

            //raise event       
            _eventPublisher.Publish(new OrderVoidedEvent(order));
        }

        /// <summary>
        /// Place order items in current user shopping cart.
        /// </summary>
        /// <param name="order">The order</param>
        public virtual void ReOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //move shopping cart items (if possible)
            foreach (var orderItem in order.OrderItems)
            {
                _shoppingCartService.AddToCart(order.Customer, orderItem.Product,
                    ShoppingCartType.ShoppingCart, order.StoreId,
                    orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                    orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                    orderItem.Quantity, false);
            }

            //set checkout attributes
            //comment the code below if you want to disable this functionality
            _genericAttributeService.SaveAttribute(order.Customer, NopCustomerDefaults.CheckoutAttributes, order.CheckoutAttributesXml, order.StoreId);
        }

        /// <summary>
        /// Check whether return request is allowed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public virtual bool IsReturnRequestAllowed(Order order)
        {
            if (!_orderSettings.ReturnRequestsEnabled)
                return false;

            if (order == null || order.Deleted)
                return false;

            //status should be complete
            if (order.OrderStatus != OrderStatus.Complete)
                return false;

            //validate allowed number of days
            if (_orderSettings.NumberOfDaysReturnRequestAvailable <= 0)
                return order.OrderItems.Any(oi => !oi.Product.NotReturnable);

            var daysPassed = (DateTime.UtcNow - order.CreatedOnUtc).TotalDays;

            if (daysPassed >= _orderSettings.NumberOfDaysReturnRequestAvailable)
                return false;

            //ensure that we have at least one returnable product
            return order.OrderItems.Any(oi => !oi.Product.NotReturnable);
        }

        /// <summary>
        /// Validate minimum order sub-total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order sub-total amount is not reached</returns>
        public virtual bool ValidateMinOrderSubtotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            //min order amount sub-total validation
            if (!cart.Any() || _orderSettings.MinOrderSubtotalAmount <= decimal.Zero)
                return true;

            //subtotal
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax, out var _, out var _, out var subTotalWithoutDiscountBase, out var _);

            if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
                return false;

            return true;
        }

        /// <summary>
        /// Validate minimum order total amount
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool ValidateMinOrderTotalAmount(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (!cart.Any() || _orderSettings.MinOrderTotalAmount <= decimal.Zero)
                return true;

            var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart);

            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether payment workflow is required
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
        /// <returns>true - OK; false - minimum order total amount is not reached</returns>
        public virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool? useRewardPoints = null)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var result = true;

            //check whether order total equals zero
            var shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, useRewardPoints: useRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        #endregion
    }
}