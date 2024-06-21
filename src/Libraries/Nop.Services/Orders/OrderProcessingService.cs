using System.Globalization;
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
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Services.Orders;

/// <summary>
/// Order processing service
/// </summary>
public partial class OrderProcessingService : IOrderProcessingService
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly ICustomNumberFormatter _customNumberFormatter;
    protected readonly IDiscountService _discountService;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IOrderService _orderService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPdfService _pdfService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IVendorService _vendorService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public OrderProcessingService(CurrencySettings currencySettings,
        IAddressService addressService,
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
        IReturnRequestService returnRequestService,
        IRewardPointService rewardPointService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreMappingService storeMappingService,
        IStoreService storeService,
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
        _addressService = addressService;
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
        _returnRequestService = returnRequestService;
        _rewardPointService = rewardPointService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
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

    #region Utilities

    /// <summary>
    /// Books the inventory by specified shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task BookReservedInventoryAsync(Shipment shipment, string message)
    {
        foreach (var item in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
        {
            var product = await _orderService.GetProductByOrderItemIdAsync(item.OrderItemId);
            if (product is null)
                continue;

            await _productService.BookReservedInventoryAsync(product, item.WarehouseId, -item.Quantity, message);
        }
    }

    /// <summary>
    /// Reveres the booked inventory by specified order
    /// </summary>
    /// <param name="order"></param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ReverseBookedInventoryAsync(Order order, string message)
    {
        foreach (var shipment in await _shipmentService.GetShipmentsByOrderIdAsync(order.Id))
        {
            foreach (var shipmentItem in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
            {
                var product = await _orderService.GetProductByOrderItemIdAsync(shipmentItem.OrderItemId);
                if (product is null)
                    continue;

                await _productService.ReverseBookedInventoryAsync(product, shipmentItem, message);
            }
        }
    }

    /// <summary>
    /// Returns the stock by specified order
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="message">Message for the stock quantity history</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ReturnOrderStockAsync(Order order, string message)
    {
        foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
            if (product is null)
                continue;

            await _productService.AdjustInventoryAsync(product, orderItem.Quantity, orderItem.AttributesXml, message);
        }
    }

    /// <summary>
    /// Add order note
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="note">Note text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task AddOrderNoteAsync(Order order, string note)
    {
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = note,
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Prepare details to place an order. It also sets some properties to "processPaymentRequest"
    /// </summary>
    /// <param name="processPaymentRequest">Process payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the details
    /// </returns>
    protected virtual async Task<PlaceOrderContainer> PreparePlaceOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var details = new PlaceOrderContainer();

        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
        await PrepareAndValidateCustomerAsync(details, processPaymentRequest, currentCurrency);
        await PrepareAndValidateShoppingCartAndCheckoutAttributesAsync(details, processPaymentRequest, currentCurrency);
        await PrepareAndValidateBillingAddressAsync(details);
        await PrepareAndValidateShippingInfoAsync(details, processPaymentRequest);
        await PrepareAndValidateTotalsAsync(details, processPaymentRequest);

        //affiliate
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(details.Customer.AffiliateId);
        if (affiliate != null && affiliate.Active && !affiliate.Deleted)
            details.AffiliateId = affiliate.Id;

        //tax display type
        details.CustomerTaxDisplayType = await _customerService.GetCustomerTaxDisplayTypeAsync(details.Customer);

        //recurring or standard shopping cart?
        details.IsRecurringShoppingCart = await _shoppingCartService.ShoppingCartIsRecurringAsync(details.Cart);
        if (!details.IsRecurringShoppingCart)
            return details;

        await PrepareAndValidateRecurringShoppingAsync(details, processPaymentRequest);

        return details;
    }

    /// <summary>
    /// Prepare and validate recurring shopping cart
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <param name="processPaymentRequest">payment info holder</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateRecurringShoppingAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest)
    {
        var (recurringCyclesError, recurringCycleLength, recurringCyclePeriod, recurringTotalCycles) = await _shoppingCartService.GetRecurringCycleInfoAsync(details.Cart);

        if (!string.IsNullOrEmpty(recurringCyclesError))
            throw new NopException(recurringCyclesError);

        processPaymentRequest.RecurringCycleLength = recurringCycleLength;
        processPaymentRequest.RecurringCyclePeriod = recurringCyclePeriod;
        processPaymentRequest.RecurringTotalCycles = recurringTotalCycles;
    }

    /// <summary>
    /// Prepare and validate all totals
    ///
    /// sub total, shipping total, payment total, tax amount etc.
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <param name="processPaymentRequest">payment info holder</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateTotalsAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest)
    {
        var (discountAmountInclTax, discountAmountExclTax, appliedDiscounts, subTotalWithoutDiscountInclTax,
                subTotalWithoutDiscountExclTax, _, _, _) =
            await _orderTotalCalculationService.GetShoppingCartSubTotalsAsync(details.Cart);

        //sub total (incl tax)
        details.OrderSubTotalInclTax = subTotalWithoutDiscountInclTax;
        details.OrderSubTotalDiscountInclTax = discountAmountInclTax;

        //discount history
        foreach (var disc in appliedDiscounts)
            if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                details.AppliedDiscounts.Add(disc);

        //sub total (excl tax)
        details.OrderSubTotalExclTax = subTotalWithoutDiscountExclTax;
        details.OrderSubTotalDiscountExclTax = discountAmountExclTax;

        //shipping total
        var (orderShippingTotalInclTax, orderShippingTotalExclTax, _, shippingTotalDiscounts) = await _orderTotalCalculationService.GetShoppingCartShippingTotalsAsync(details.Cart);

        if (!orderShippingTotalInclTax.HasValue || !orderShippingTotalExclTax.HasValue)
            throw new NopException("Shipping total couldn't be calculated");

        details.OrderShippingTotalInclTax = orderShippingTotalInclTax.Value;
        details.OrderShippingTotalExclTax = orderShippingTotalExclTax.Value;

        foreach (var disc in shippingTotalDiscounts)
            if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                details.AppliedDiscounts.Add(disc);

        //payment total
        var paymentAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(details.Cart, processPaymentRequest.PaymentMethodSystemName);
        details.PaymentAdditionalFeeInclTax = (await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentAdditionalFee, true, details.Customer)).price;
        details.PaymentAdditionalFeeExclTax = (await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentAdditionalFee, false, details.Customer)).price;

        //tax amount
        SortedDictionary<decimal, decimal> taxRatesDictionary;
        (details.OrderTaxTotal, taxRatesDictionary) = await _orderTotalCalculationService.GetTaxTotalAsync(details.Cart);

        //VAT number
        if (_taxSettings.EuVatEnabled && details.Customer.VatNumberStatus == VatNumberStatus.Valid)
            details.VatNumber = details.Customer.VatNumber;

        //tax rates
        details.TaxRates = taxRatesDictionary.Aggregate(string.Empty, (current, next) =>
            $"{current}{next.Key.ToString(CultureInfo.InvariantCulture)}:{next.Value.ToString(CultureInfo.InvariantCulture)};   ");

        //order total (and applied discounts, gift cards, reward points)
        var (orderTotal, orderDiscountAmount, orderAppliedDiscounts, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(details.Cart);
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
    }

    /// <summary>
    /// Prepare and validate shipping info
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <param name="processPaymentRequest">payment info holder</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateShippingInfoAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest)
    {
        //shipping info
        if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(details.Cart))
        {
            var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(details.Customer,
                NopCustomerDefaults.SelectedPickupPointAttribute, processPaymentRequest.StoreId);
            if (_shippingSettings.AllowPickupInStore && pickupPoint != null)
            {
                var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
                var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

                details.PickupInStore = true;
                details.PickupAddress = new Address
                {
                    Address1 = pickupPoint.Address,
                    City = pickupPoint.City,
                    County = pickupPoint.County,
                    CountryId = country?.Id,
                    StateProvinceId = state?.Id,
                    ZipPostalCode = pickupPoint.ZipPostalCode,
                    CreatedOnUtc = DateTime.UtcNow
                };
            }
            else
            {
                if (details.Customer.ShippingAddressId == null)
                    throw new NopException("Shipping address is not provided");

                var shippingAddress = await _customerService.GetCustomerShippingAddressAsync(details.Customer);

                if (!CommonHelper.IsValidEmail(shippingAddress?.Email))
                    throw new NopException("Email is not valid");

                //clone shipping address
                details.ShippingAddress = _addressService.CloneAddress(shippingAddress);

                if (await _countryService.GetCountryByAddressAsync(details.ShippingAddress) is Country shippingCountry && !shippingCountry.AllowsShipping)
                    throw new NopException($"Country '{shippingCountry.Name}' is not allowed for shipping");
            }

            var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(details.Customer,
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
    }

    /// <summary>
    /// Prepare and validate shopping cart and checkout attributes
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <param name="processPaymentRequest">payment info holder</param>
    /// <param name="currentCurrency">The working currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateShoppingCartAndCheckoutAttributesAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest, Currency currentCurrency)
    {
        //checkout attributes
        details.CheckoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(details.Customer, NopCustomerDefaults.CheckoutAttributes, processPaymentRequest.StoreId);
        details.CheckoutAttributeDescription = await _checkoutAttributeFormatter.FormatAttributesAsync(details.CheckoutAttributesXml, details.Customer);

        //load shopping cart
        details.Cart = await _shoppingCartService.GetShoppingCartAsync(details.Customer, ShoppingCartType.ShoppingCart, processPaymentRequest.StoreId);

        if (!details.Cart.Any())
            throw new NopException("Cart is empty");

        //validate the entire shopping cart
        var warnings = await _shoppingCartService.GetShoppingCartWarningsAsync(details.Cart, details.CheckoutAttributesXml, true);
        if (warnings.Any())
            throw new NopException(warnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));

        //validate individual cart items
        foreach (var sci in details.Cart)
        {
            var product = await _productService.GetProductByIdAsync(sci.ProductId);

            var sciWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(details.Customer,
                sci.ShoppingCartType, product, processPaymentRequest.StoreId, sci.AttributesXml,
                sci.CustomerEnteredPrice, sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false, sci.Id);
            if (sciWarnings.Any())
                throw new NopException(sciWarnings.Aggregate(string.Empty, (current, next) => $"{current}{next};"));
        }

        //min totals validation
        if (!await ValidateMinOrderSubtotalAmountAsync(details.Cart))
        {
            var minOrderSubtotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderSubtotalAmount, currentCurrency);
            throw new NopException(string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"),
                await _priceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false)));
        }

        if (!await ValidateMinOrderTotalAmountAsync(details.Cart))
        {
            var minOrderTotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderTotalAmount, currentCurrency);
            throw new NopException(string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"),
                await _priceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false)));
        }
    }

    /// <summary>
    /// Prepare and validate billing address
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateBillingAddressAsync(PlaceOrderContainer details)
    {
        if (details.Customer.BillingAddressId is null)
            throw new NopException("Billing address is not provided");

        var billingAddress = await _customerService.GetCustomerBillingAddressAsync(details.Customer);

        if (!CommonHelper.IsValidEmail(billingAddress?.Email))
            throw new NopException("Email is not valid");

        details.BillingAddress = _addressService.CloneAddress(billingAddress);

        if (await _countryService.GetCountryByAddressAsync(details.BillingAddress) is Country billingCountry && !billingCountry.AllowsBilling)
            throw new NopException($"Country '{billingCountry.Name}' is not allowed for billing");
    }

    /// <summary>
    /// Prepare and validate customer
    /// </summary>
    /// <param name="details">PlaceOrder container</param>
    /// <param name="processPaymentRequest">payment info holder</param>
    /// <param name="currentCurrency">The working currency</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    /// <exception cref="NopException">Validation problems</exception>
    protected virtual async Task PrepareAndValidateCustomerAsync(PlaceOrderContainer details, ProcessPaymentRequest processPaymentRequest, Currency currentCurrency)
    {
        details.Customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);

        if (details.Customer == null)
            throw new ArgumentException("Customer is not set");

        //check whether customer is guest
        if (await _customerService.IsGuestAsync(details.Customer) && !_orderSettings.AnonymousCheckoutAllowed)
            throw new NopException("Anonymous checkout is not allowed");

        //customer currency
        var currencyTmp = await _currencyService.GetCurrencyByIdAsync(details.Customer.CurrencyId ?? 0);
        var customerCurrency = currencyTmp != null && currencyTmp.Published && await _storeMappingService.AuthorizeAsync(currencyTmp) ? currencyTmp : currentCurrency;
        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        details.CustomerCurrencyCode = customerCurrency.CurrencyCode;
        details.CustomerCurrencyRate = customerCurrency.Rate / primaryStoreCurrency.Rate;

        //customer language
        details.CustomerLanguage = await _languageService.GetLanguageByIdAsync(details.Customer.LanguageId ?? 0);
        if (details.CustomerLanguage == null || !details.CustomerLanguage.Published || !await _storeMappingService.AuthorizeAsync(details.CustomerLanguage))
            details.CustomerLanguage = await _workContext.GetWorkingLanguageAsync();
    }

    /// <summary>
    /// Prepare details to place order based on the recurring payment.
    /// </summary>
    /// <param name="processPaymentRequest">Process payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the details
    /// </returns>
    protected virtual async Task<PlaceOrderContainer> PrepareRecurringOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest)
    {
        var details = new PlaceOrderContainer
        {
            IsRecurringShoppingCart = true,
            //load initial order
            InitialOrder = processPaymentRequest.InitialOrder
        };

        if (details.InitialOrder == null)
            throw new ArgumentException("Initial order is not set for recurring payment");

        processPaymentRequest.PaymentMethodSystemName = details.InitialOrder.PaymentMethodSystemName;

        //customer
        details.Customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
        if (details.Customer == null)
            throw new ArgumentException("Customer is not set");

        //affiliate
        var affiliate = await _affiliateService.GetAffiliateByIdAsync(details.Customer.AffiliateId);
        if (affiliate != null && affiliate.Active && !affiliate.Deleted)
            details.AffiliateId = affiliate.Id;

        //check whether customer is guest
        if (await _customerService.IsGuestAsync(details.Customer) && !_orderSettings.AnonymousCheckoutAllowed)
            throw new NopException("Anonymous checkout is not allowed");

        //customer currency
        details.CustomerCurrencyCode = details.InitialOrder.CustomerCurrencyCode;
        details.CustomerCurrencyRate = details.InitialOrder.CurrencyRate;

        //customer language
        details.CustomerLanguage = await _languageService.GetLanguageByIdAsync(details.InitialOrder.CustomerLanguageId);
        if (details.CustomerLanguage == null || !details.CustomerLanguage.Published)
            details.CustomerLanguage = await _workContext.GetWorkingLanguageAsync();

        //billing address
        if (details.InitialOrder.BillingAddressId == 0)
            throw new NopException("Billing address is not available");

        var billingAddress = await _addressService.GetAddressByIdAsync(details.InitialOrder.BillingAddressId);

        details.BillingAddress = _addressService.CloneAddress(billingAddress);
        if (await _countryService.GetCountryByAddressAsync(billingAddress) is Country billingCountry && !billingCountry.AllowsBilling)
            throw new NopException($"Country '{billingCountry.Name}' is not allowed for billing");

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
                if (!details.InitialOrder.ShippingAddressId.HasValue || await _addressService.GetAddressByIdAsync(details.InitialOrder.ShippingAddressId.Value) is not Address shippingAddress)
                    throw new NopException("Shipping address is not available");

                //clone shipping address
                details.ShippingAddress = _addressService.CloneAddress(shippingAddress);
                if (await _countryService.GetCountryByAddressAsync(details.ShippingAddress) is Country shippingCountry && !shippingCountry.AllowsShipping)
                    throw new NopException($"Country '{shippingCountry.Name}' is not allowed for shipping");
            }
            else if (details.InitialOrder.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(details.InitialOrder.PickupAddressId.Value) is Address pickupAddress)
                details.PickupAddress = _addressService.CloneAddress(pickupAddress);

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
        foreach (var duh in await _discountService.GetAllDiscountUsageHistoryAsync(orderId: details.InitialOrder.Id))
        {
            var d = await _discountService.GetDiscountByIdAsync(duh.DiscountId);
            if (d != null)
                details.AppliedDiscounts.Add(d);
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order
    /// </returns>
    protected virtual async Task<Order> SaveOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest,
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
            PickupInStore = details.PickupInStore,
            ShippingStatus = details.ShippingStatus,
            ShippingMethod = details.ShippingMethodName,
            ShippingRateComputationMethodSystemName = details.ShippingRateComputationMethodSystemName,
            CustomValuesXml = _paymentService.SerializeCustomValues(processPaymentRequest),
            VatNumber = details.VatNumber,
            CreatedOnUtc = DateTime.UtcNow,
            CustomOrderNumber = string.Empty
        };

        if (details.BillingAddress is null)
            throw new NopException("Billing address is not provided");

        await _addressService.InsertAddressAsync(details.BillingAddress);
        order.BillingAddressId = details.BillingAddress.Id;

        if (details.PickupAddress != null)
        {
            await _addressService.InsertAddressAsync(details.PickupAddress);
            order.PickupAddressId = details.PickupAddress.Id;
        }

        if (details.ShippingAddress != null)
        {
            await _addressService.InsertAddressAsync(details.ShippingAddress);
            order.ShippingAddressId = details.ShippingAddress.Id;
        }

        await _orderService.InsertOrderAsync(order);

        //generate and set custom order number
        order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
        await _orderService.UpdateOrderAsync(order);

        //reward points history
        if (details.RedeemedRewardPointsAmount <= decimal.Zero)
            return order;

        order.RedeemedRewardPointsEntryId = await _rewardPointService.AddRewardPointsHistoryEntryAsync(details.Customer, -details.RedeemedRewardPoints, order.StoreId,
            string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.RedeemedForOrder", order.CustomerLanguageId), order.CustomOrderNumber),
            order, details.RedeemedRewardPointsAmount);

        await _orderService.UpdateOrderAsync(order);

        return order;
    }

    /// <summary>
    /// Send "order placed" notifications and save order notes
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SendNotificationsAndSaveNotesAsync(Order order)
    {
        //notes, messages
        await AddOrderNoteAsync(order, _workContext.OriginalCustomerIfImpersonated != null
            ? $"Order placed by a store owner ('{_workContext.OriginalCustomerIfImpersonated.Email}'. ID = {_workContext.OriginalCustomerIfImpersonated.Id}) impersonating the customer."
            : "Order placed");

        //send email notifications
        var orderPlacedStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPlacedStoreOwnerNotificationAsync(order, _localizationSettings.DefaultAdminLanguageId);
        if (orderPlacedStoreOwnerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order placed\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedStoreOwnerNotificationQueuedEmailIds)}.");

        var orderPlacedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
            (await _pdfService.SaveOrderPdfToDiskAsync(order)) : null;
        var orderPlacedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPlacedEmail ?
            (string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf") : null;
        var orderPlacedCustomerNotificationQueuedEmailIds = await _workflowMessageService
            .SendOrderPlacedCustomerNotificationAsync(order, order.CustomerLanguageId, orderPlacedAttachmentFilePath, orderPlacedAttachmentFileName);
        if (orderPlacedCustomerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order placed\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedCustomerNotificationQueuedEmailIds)}.");

        var vendors = await GetVendorsInOrderAsync(order);
        foreach (var vendor in vendors)
        {
            var orderPlacedVendorNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPlacedVendorNotificationAsync(order, vendor, _localizationSettings.DefaultAdminLanguageId);
            if (orderPlacedVendorNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order placed\" email (to vendor) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedVendorNotificationQueuedEmailIds)}.");
        }

        if (order.AffiliateId == 0)
            return;

        var orderPlacedAffiliateNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPlacedAffiliateNotificationAsync(order, _localizationSettings.DefaultAdminLanguageId);
        if (orderPlacedAffiliateNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order placed\" email (to affiliate) has been queued. Queued email identifiers: {string.Join(", ", orderPlacedAffiliateNotificationQueuedEmailIds)}.");
    }

    /// <summary>
    /// Award (earn) reward points (for placing a new order)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task AwardRewardPointsAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        var totalForRewardPoints = _orderTotalCalculationService
            .CalculateApplicableOrderTotalForRewardPoints(order.OrderShippingInclTax, order.OrderTotal);
        var points = totalForRewardPoints > decimal.Zero ?
            await _orderTotalCalculationService.CalculateRewardPointsAsync(customer, totalForRewardPoints) : 0;
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
        order.RewardPointsHistoryEntryId = await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, points, order.StoreId,
            string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.EarnedForOrder"), order.CustomOrderNumber),
            activatingDate: activatingDate, endDate: endDate);

        await _orderService.UpdateOrderAsync(order);
    }

    /// <summary>
    /// Reduce (cancel) reward points (previously awarded for placing an order)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ReduceRewardPointsAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        //ensure that reward points were already earned for this order before
        if (!order.RewardPointsHistoryEntryId.HasValue)
            return;

        //get appropriate history entry
        var rewardPointsHistoryEntry = await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RewardPointsHistoryEntryId.Value);
        if (rewardPointsHistoryEntry != null)
        {
            if (rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                await _rewardPointService.DeleteRewardPointsHistoryEntryAsync(rewardPointsHistoryEntry);
            }
            else
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                //or reduce reward points if the entry already exists
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -rewardPointsHistoryEntry.Points, order.StoreId,
                    string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.ReducedForOrder"), order.CustomOrderNumber));
            }
        }
    }

    /// <summary>
    /// Return back redeemed reward points to a customer (spent when placing an order)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ReturnBackRedeemedRewardPointsAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        //were some reward points spend on the order
        var allRewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(order.CustomerId, order.StoreId, orderGuid: order.OrderGuid);
        if (allRewardPoints?.Any() == true)
        {
            foreach (var rewardPoints in allRewardPoints)
            {
                //return back
                await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, -rewardPoints.Points, order.StoreId,
                    string.Format(await _localizationService.GetResourceAsync("RewardPoints.Message.ReturnedForOrder"), order.CustomOrderNumber));
            }
        }
    }

    /// <summary>
    /// Set IsActivated value for purchase gift cards for particular order
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="activate">A value indicating whether to activate gift cards; true - activate, false - deactivate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetActivatedValueForPurchasedGiftCardsAsync(Order order, bool activate)
    {
        var giftCards = await _giftCardService.GetAllGiftCardsAsync(order.Id, isGiftCardActivated: !activate);
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
                        var customerLang = (await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId) ??
                                            (await _languageService.GetAllLanguagesAsync()).FirstOrDefault())
                                           ?? throw new Exception("No languages could be loaded");
                        var queuedEmailIds = await _workflowMessageService.SendGiftCardNotificationAsync(gc, customerLang.Id);
                        if (queuedEmailIds.Any())
                            isRecipientNotified = true;
                    }
                }

                gc.IsGiftCardActivated = true;
                gc.IsRecipientNotified = isRecipientNotified;
                await _giftCardService.UpdateGiftCardAsync(gc);
            }
            else
            {
                //deactivate
                gc.IsGiftCardActivated = false;
                await _giftCardService.UpdateGiftCardAsync(gc);
            }
        }
    }

    /// <summary>
    /// Sets an order status
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="os">New order status</param>
    /// <param name="notifyCustomer">True to notify customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SetOrderStatusAsync(Order order, OrderStatus os, bool notifyCustomer)
    {
        ArgumentNullException.ThrowIfNull(order);

        var prevOrderStatus = order.OrderStatus;
        if (prevOrderStatus == os)
            return;

        //set and save new order status
        order.OrderStatusId = (int)os;
        await _orderService.UpdateOrderAsync(order);

        //notify and allow event handlers to change status
        await _eventPublisher.PublishAsync(new OrderStatusChangedEvent(order, prevOrderStatus));
        if (prevOrderStatus == order.OrderStatus)
            return;

        //order notes, notifications
        await AddOrderNoteAsync(order, $"Order status has been changed to {await _localizationService.GetLocalizedEnumAsync(os)}");

        if (prevOrderStatus != OrderStatus.Processing &&
            os == OrderStatus.Processing
            && notifyCustomer)
        {
            //notification
            var orderProcessingAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderProcessingEmail ?
                await _pdfService.SaveOrderPdfToDiskAsync(order) : null;
            var orderProcessingAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderProcessingEmail ?
                (string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf") : null;
            var orderProcessingCustomerNotificationQueuedEmailIds = await _workflowMessageService
                .SendOrderProcessingCustomerNotificationAsync(order, order.CustomerLanguageId, orderProcessingAttachmentFilePath,
                    orderProcessingAttachmentFileName);
            if (orderProcessingCustomerNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order processing\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderProcessingCustomerNotificationQueuedEmailIds)}.");
        }

        if (prevOrderStatus != OrderStatus.Complete &&
            os == OrderStatus.Complete
            && notifyCustomer)
        {
            //notification
            var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                await _pdfService.SaveOrderPdfToDiskAsync(order) : null;
            var orderCompletedAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                (string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf") : null;
            var orderCompletedCustomerNotificationQueuedEmailIds = await _workflowMessageService
                .SendOrderCompletedCustomerNotificationAsync(order, order.CustomerLanguageId, orderCompletedAttachmentFilePath,
                    orderCompletedAttachmentFileName);
            if (orderCompletedCustomerNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order completed\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderCompletedCustomerNotificationQueuedEmailIds)}.");
        }

        if (prevOrderStatus != OrderStatus.Cancelled &&
            os == OrderStatus.Cancelled
            && notifyCustomer)
        {
            //notification
            var orderCancelledCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderCancelledCustomerNotificationAsync(order, order.CustomerLanguageId);
            if (orderCancelledCustomerNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order cancelled\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderCancelledCustomerNotificationQueuedEmailIds)}.");

            var vendors = await GetVendorsInOrderAsync(order);
            foreach (var vendor in vendors)
            {
                var orderCancelVendorNotificationQueuedEmailIds = await _workflowMessageService.SendOrderCancelledVendorNotificationAsync(order, vendor, _localizationSettings.DefaultAdminLanguageId);

                if (orderCancelVendorNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order cancelled\" email (to vendor) has been queued. Queued email identifiers: {string.Join(", ", orderCancelVendorNotificationQueuedEmailIds)}.");
            }
        }

        //reward points
        if (order.OrderStatus == OrderStatus.Complete)
            await AwardRewardPointsAsync(order);

        if (order.OrderStatus == OrderStatus.Cancelled)
            await ReduceRewardPointsAsync(order);

        //gift cards activation
        if (_orderSettings.ActivateGiftCardsAfterCompletingOrder && order.OrderStatus == OrderStatus.Complete)
            await SetActivatedValueForPurchasedGiftCardsAsync(order, true);

        //gift cards deactivation
        if (_orderSettings.DeactivateGiftCardsAfterCancellingOrder && order.OrderStatus == OrderStatus.Cancelled)
            await SetActivatedValueForPurchasedGiftCardsAsync(order, false);
    }

    /// <summary>
    /// Process order paid status
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ProcessOrderPaidAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        //raise event
        await _eventPublisher.PublishAsync(new OrderPaidEvent(order));

        //order paid email notification
        if (order.OrderTotal != decimal.Zero)
        {
            //we should not send it for free ($0 total) orders?
            //remove this "if" statement if you want to send it in this case

            var orderPaidAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                await _pdfService.SaveOrderPdfToDiskAsync(order) : null;
            var orderPaidAttachmentFileName = _orderSettings.AttachPdfInvoiceToOrderPaidEmail ?
                (string.Format(await _localizationService.GetResourceAsync("PDFInvoice.FileName"), order.CustomOrderNumber) + ".pdf") : null;
            var orderPaidCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPaidCustomerNotificationAsync(order, order.CustomerLanguageId,
                orderPaidAttachmentFilePath, orderPaidAttachmentFileName);

            if (orderPaidCustomerNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order paid\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderPaidCustomerNotificationQueuedEmailIds)}.");

            var orderPaidStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPaidStoreOwnerNotificationAsync(order, _localizationSettings.DefaultAdminLanguageId);
            if (orderPaidStoreOwnerNotificationQueuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Order paid\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderPaidStoreOwnerNotificationQueuedEmailIds)}.");

            var vendors = await GetVendorsInOrderAsync(order);
            foreach (var vendor in vendors)
            {
                var orderPaidVendorNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPaidVendorNotificationAsync(order, vendor, _localizationSettings.DefaultAdminLanguageId);

                if (orderPaidVendorNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order paid\" email (to vendor) has been queued. Queued email identifiers: {string.Join(", ", orderPaidVendorNotificationQueuedEmailIds)}.");
            }

            if (order.AffiliateId != 0)
            {
                var orderPaidAffiliateNotificationQueuedEmailIds = await _workflowMessageService.SendOrderPaidAffiliateNotificationAsync(order,
                    _localizationSettings.DefaultAdminLanguageId);
                if (orderPaidAffiliateNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order paid\" email (to affiliate) has been queued. Queued email identifiers: {string.Join(", ", orderPaidAffiliateNotificationQueuedEmailIds)}.");
            }
        }

        //customer roles with "purchased with product" specified
        await ProcessCustomerRolesWithPurchasedProductSpecifiedAsync(order, true);
    }

    /// <summary>
    /// Process customer roles with "Purchased with Product" property configured
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="add">A value indicating whether to add configured customer role; true - add, false - remove</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task ProcessCustomerRolesWithPurchasedProductSpecifiedAsync(Order order, bool add)
    {
        ArgumentNullException.ThrowIfNull(order);

        //purchased product identifiers
        var purchasedProductIds = new List<int>();
        foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
        {
            //standard items
            purchasedProductIds.Add(orderItem.ProductId);

            //bundled (associated) products
            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(orderItem.AttributesXml);
            foreach (var attributeValue in attributeValues)
                if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    purchasedProductIds.Add(attributeValue.AssociatedProductId);
        }

        //list of customer roles
        var customerRoles = (await _customerService
                .GetAllCustomerRolesAsync(true))
            .Where(cr => purchasedProductIds.Contains(cr.PurchasedWithProductId))
            .ToList();

        if (!customerRoles.Any())
            return;

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        foreach (var customerRole in customerRoles)
            if (!await _customerService.IsInCustomerRoleAsync(customer, customerRole.SystemName))
            {
                //not in the list yet
                if (add)
                    //add
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
            }
            else
            {
                //already in the list
                if (!add)
                    //remove
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);
            }
    }

    /// <summary>
    /// Get a list of vendors in order (order items)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the vendors
    /// </returns>
    protected virtual async Task<IList<Vendor>> GetVendorsInOrderAsync(Order order)
    {
        var pIds = (await _orderService.GetOrderItemsAsync(order.Id)).Select(x => x.ProductId).ToArray();

        return await _vendorService.GetVendorsByProductIdsAsync(pIds);
    }

    /// <summary>
    /// Create recurring payment (the first payment)
    /// </summary>
    /// <param name="processPaymentRequest">Process payment request</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CreateFirstRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest, Order order)
    {
        var rp = new RecurringPayment
        {
            CycleLength = processPaymentRequest.RecurringCycleLength,
            CyclePeriod = processPaymentRequest.RecurringCyclePeriod,
            TotalCycles = processPaymentRequest.RecurringTotalCycles,
            StartDateUtc = DateTime.UtcNow,
            IsActive = true,
            CreatedOnUtc = DateTime.UtcNow,
            InitialOrderId = order.Id
        };
        await _orderService.InsertRecurringPaymentAsync(rp);

        switch (await _paymentService.GetRecurringPaymentTypeAsync(processPaymentRequest.PaymentMethodSystemName))
        {
            case RecurringPaymentType.NotSupported:
                //not supported
                break;
            case RecurringPaymentType.Manual:
                await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory
                {
                    RecurringPaymentId = rp.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id
                });
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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task MoveShoppingCartItemsToOrderItemsAsync(PlaceOrderContainer details, Order order)
    {
        foreach (var sc in details.Cart)
        {
            var product = await _productService.GetProductByIdAsync(sc.ProductId);

            //prices
            var scUnitPrice = (await _shoppingCartService.GetUnitPriceAsync(sc, true)).unitPrice;
            var (scSubTotal, discountAmount, scDiscounts, _) = await _shoppingCartService.GetSubTotalAsync(sc, true);
            var scUnitPriceInclTax = await _taxService.GetProductPriceAsync(product, scUnitPrice, true, details.Customer);
            var scUnitPriceExclTax = await _taxService.GetProductPriceAsync(product, scUnitPrice, false, details.Customer);
            var scSubTotalInclTax = await _taxService.GetProductPriceAsync(product, scSubTotal, true, details.Customer);
            var scSubTotalExclTax = await _taxService.GetProductPriceAsync(product, scSubTotal, false, details.Customer);
            var discountAmountInclTax = await _taxService.GetProductPriceAsync(product, discountAmount, true, details.Customer);
            var discountAmountExclTax = await _taxService.GetProductPriceAsync(product, discountAmount, false, details.Customer);
            foreach (var disc in scDiscounts)
                if (!_discountService.ContainsDiscount(details.AppliedDiscounts, disc))
                    details.AppliedDiscounts.Add(disc);

            //attributes
            var store = await _storeService.GetStoreByIdAsync(sc.StoreId);
            var attributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product, sc.AttributesXml, details.Customer, store);

            var itemWeight = await _shippingService.GetShoppingCartItemWeightAsync(sc);

            //save order item
            var orderItem = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                UnitPriceInclTax = scUnitPriceInclTax.price,
                UnitPriceExclTax = scUnitPriceExclTax.price,
                PriceInclTax = scSubTotalInclTax.price,
                PriceExclTax = scSubTotalExclTax.price,
                OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, sc.AttributesXml),
                AttributeDescription = attributeDescription,
                AttributesXml = sc.AttributesXml,
                Quantity = sc.Quantity,
                DiscountAmountInclTax = discountAmountInclTax.price,
                DiscountAmountExclTax = discountAmountExclTax.price,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = itemWeight,
                RentalStartDateUtc = sc.RentalStartDateUtc,
                RentalEndDateUtc = sc.RentalEndDateUtc
            };

            await _orderService.InsertOrderItemAsync(orderItem);

            //gift cards
            await AddGiftCardsAsync(product, sc.AttributesXml, sc.Quantity, orderItem, scUnitPriceExclTax.price);

            //inventory
            await _productService.AdjustInventoryAsync(product, -sc.Quantity, sc.AttributesXml,
                string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.PlaceOrder"), order.Id));
        }

        await _shoppingCartService.ClearShoppingCartAsync(details.Customer, order.StoreId);
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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task AddGiftCardsAsync(Product product, string attributesXml, int quantity, OrderItem orderItem, decimal? unitPriceExclTax = null, decimal? amount = null)
    {
        if (!product.IsGiftCard)
            return;

        _productAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

        for (var i = 0; i < quantity; i++)
        {
            await _giftCardService.InsertGiftCardAsync(new GiftCard
            {
                GiftCardType = product.GiftCardType,
                PurchasedWithOrderItemId = orderItem.Id,
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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the 
    /// </returns>
    protected virtual async Task<ProcessPaymentResult> GetProcessPaymentResultAsync(ProcessPaymentRequest processPaymentRequest, PlaceOrderContainer details)
    {
        //process payment
        ProcessPaymentResult processPaymentResult;
        //check if is payment workflow required
        if (await IsPaymentWorkflowRequiredAsync(details.Cart))
        {
            var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
            var paymentMethod = await _paymentPluginManager
                                    .LoadPluginBySystemNameAsync(processPaymentRequest.PaymentMethodSystemName, customer, processPaymentRequest.StoreId)
                                ?? throw new NopException("Payment method couldn't be loaded");

            //ensure that payment method is active
            if (!_paymentPluginManager.IsPluginActive(paymentMethod))
                throw new NopException("Payment method is not active");

            if (details.IsRecurringShoppingCart)
            {
                //recurring cart
                processPaymentResult = (await _paymentService.GetRecurringPaymentTypeAsync(processPaymentRequest.PaymentMethodSystemName)) switch
                {
                    RecurringPaymentType.NotSupported => throw new NopException("Recurring payments are not supported by selected payment method"),
                    RecurringPaymentType.Manual or
                        RecurringPaymentType.Automatic => await _paymentService.ProcessRecurringPaymentAsync(processPaymentRequest),
                    _ => throw new NopException("Not supported recurring payment type"),
                };
            }
            else
                //standard cart
                processPaymentResult = await _paymentService.ProcessPaymentAsync(processPaymentRequest);
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
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveGiftCardUsageHistoryAsync(PlaceOrderContainer details, Order order)
    {
        if (details.AppliedGiftCards == null || !details.AppliedGiftCards.Any())
            return;

        foreach (var agc in details.AppliedGiftCards)
            await _giftCardService.InsertGiftCardUsageHistoryAsync(new GiftCardUsageHistory
            {
                GiftCardId = agc.GiftCard.Id,
                UsedWithOrderId = order.Id,
                UsedValue = agc.AmountCanBeUsed,
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    /// <summary>
    /// Save discount usage history
    /// </summary>
    /// <param name="details">PlaceOrderContainer</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveDiscountUsageHistoryAsync(PlaceOrderContainer details, Order order)
    {
        if (details.AppliedDiscounts == null || !details.AppliedDiscounts.Any())
            return;

        foreach (var discount in details.AppliedDiscounts)
        {
            var d = await _discountService.GetDiscountByIdAsync(discount.Id);
            if (d == null)
                continue;

            await _discountService.InsertDiscountUsageHistoryAsync(new DiscountUsageHistory
            {
                DiscountId = d.Id,
                OrderId = order.Id,
                CreatedOnUtc = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Checks and save order status
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="needOrderSave">Indicate if we need save order if nothing changed on the order status</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task CheckAndSaveOrderStatusAsync(Order order, bool needOrderSave)
    {
        ArgumentNullException.ThrowIfNull(order);

        var completed = false;
        var isOrderSaved = !needOrderSave;

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            if (!order.PaidDateUtc.HasValue)
            {
                //ensure that paid date is set
                order.PaidDateUtc = DateTime.UtcNow;
                isOrderSaved = false;
            }

            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
                //shipping is not required
                completed = true;
            else
                //shipping is required
                completed = _orderSettings.CompleteOrderWhenDelivered
                    ? order.ShippingStatus == ShippingStatus.Delivered
                    : order.ShippingStatus == ShippingStatus.Shipped || order.ShippingStatus == ShippingStatus.Delivered;
        }

        switch (order.OrderStatus)
        {
            case OrderStatus.Pending:
                if (order.PaymentStatus == PaymentStatus.Authorized ||
                    order.PaymentStatus == PaymentStatus.Paid)
                {
                    await SetOrderStatusAsync(order, OrderStatus.Processing, !completed);
                    isOrderSaved = true;
                }

                if (order.ShippingStatus == ShippingStatus.PartiallyShipped ||
                    order.ShippingStatus == ShippingStatus.Shipped ||
                    order.ShippingStatus == ShippingStatus.Delivered)
                {
                    await SetOrderStatusAsync(order, OrderStatus.Processing, !completed);
                    isOrderSaved = true;
                }

                break;
            //is order complete?
            case OrderStatus.Cancelled:
            case OrderStatus.Complete:
                if (!isOrderSaved)
                    await _orderService.UpdateOrderAsync(order);
                return;
        }

        if (completed)
        {
            await SetOrderStatusAsync(order, OrderStatus.Complete, true);
            isOrderSaved = true;
        }

        if (!isOrderSaved)
            await _orderService.UpdateOrderAsync(order);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks order status
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task CheckOrderStatusAsync(Order order)
    {
        await CheckAndSaveOrderStatusAsync(order, false);
    }

    /// <summary>
    /// Places an order
    /// </summary>
    /// <param name="processPaymentRequest">Process payment request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the place order result
    /// </returns>
    public virtual async Task<PlaceOrderResult> PlaceOrderAsync(ProcessPaymentRequest processPaymentRequest)
    {
        ArgumentNullException.ThrowIfNull(processPaymentRequest);

        var result = new PlaceOrderResult();
        try
        {
            if (processPaymentRequest.OrderGuid == Guid.Empty)
                throw new Exception("Order GUID is not generated");

            //prepare order details
            var details = await PreparePlaceOrderDetailsAsync(processPaymentRequest);

            var processPaymentResult = await GetProcessPaymentResultAsync(processPaymentRequest, details)
                                       ?? throw new NopException("processPaymentResult is not available");

            if (processPaymentResult.Success)
            {
                var order = await SaveOrderDetailsAsync(processPaymentRequest, processPaymentResult, details);
                result.PlacedOrder = order;

                //move shopping cart items to order items
                await MoveShoppingCartItemsToOrderItemsAsync(details, order);

                //discount usage history
                await SaveDiscountUsageHistoryAsync(details, order);

                //gift card usage history
                await SaveGiftCardUsageHistoryAsync(details, order);

                //recurring orders
                if (details.IsRecurringShoppingCart)
                    await CreateFirstRecurringPaymentAsync(processPaymentRequest, order);

                //notifications
                await SendNotificationsAndSaveNotesAsync(order);

                //reset checkout data
                await _customerService.ResetCheckoutDataAsync(details.Customer, processPaymentRequest.StoreId, clearCouponCodes: true, clearCheckoutAttributes: true);
                await _customerActivityService.InsertActivityAsync("PublicStore.PlaceOrder",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.PlaceOrder"), order.Id), order);

                //raise event       
                await _eventPublisher.PublishAsync(new OrderPlacedEvent(order));

                //check order status
                await CheckOrderStatusAsync(order);

                if (order.PaymentStatus == PaymentStatus.Paid)
                    await ProcessOrderPaidAsync(order);
            }
            else
                foreach (var paymentError in processPaymentResult.Errors)
                    result.AddError(string.Format(await _localizationService.GetResourceAsync("Checkout.PaymentError"), paymentError));
        }
        catch (Exception exc)
        {
            await _logger.ErrorAsync(exc.Message, exc);
            result.AddError(exc.Message);
        }

        if (result.Success)
            return result;

        //log errors
        var logError = result.Errors.Aggregate("Error while placing order. ",
            (current, next) => $"{current}Error {result.Errors.IndexOf(next) + 1}: {next}. ");
        var customer = await _customerService.GetCustomerByIdAsync(processPaymentRequest.CustomerId);
        await _logger.ErrorAsync(logError, customer: customer);

        return result;
    }

    /// <summary>
    /// Update order totals
    /// </summary>
    /// <param name="updateOrderParameters">Parameters for the updating order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateOrderTotalsAsync(UpdateOrderParameters updateOrderParameters)
    {
        if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
            return;

        var updatedOrder = updateOrderParameters.UpdatedOrder;
        var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

        //restore shopping cart from order items
        var (restoredCart, updatedShoppingCartItem) = await restoreShoppingCartAsync(updatedOrder, updatedOrderItem.Id);

        var itemDeleted = updatedShoppingCartItem is null;

        //validate shopping cart for warnings
        updateOrderParameters.Warnings.AddRange(await _shoppingCartService.GetShoppingCartWarningsAsync(restoredCart, string.Empty, false));

        var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);

        if (!itemDeleted)
        {
            var product = await _productService.GetProductByIdAsync(updatedShoppingCartItem.ProductId);
            var store = await _storeService.GetStoreByIdAsync(updatedShoppingCartItem.StoreId);

            updateOrderParameters.Warnings.AddRange(await _shoppingCartService.GetShoppingCartItemWarningsAsync(customer, updatedShoppingCartItem.ShoppingCartType,
                product, updatedOrder.StoreId, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.CustomerEnteredPrice,
                updatedShoppingCartItem.RentalStartDateUtc, updatedShoppingCartItem.RentalEndDateUtc, updatedShoppingCartItem.Quantity, false, updatedShoppingCartItem.Id));

            updatedOrderItem.ItemWeight = await _shippingService.GetShoppingCartItemWeightAsync(updatedShoppingCartItem);
            updatedOrderItem.OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, updatedShoppingCartItem.AttributesXml);
            updatedOrderItem.AttributeDescription = await _productAttributeFormatter.FormatAttributesAsync(product,
                updatedShoppingCartItem.AttributesXml, customer, store);

            //gift cards
            await AddGiftCardsAsync(product, updatedShoppingCartItem.AttributesXml, updatedShoppingCartItem.Quantity, updatedOrderItem, updatedOrderItem.UnitPriceExclTax);
        }

        await _orderTotalCalculationService.UpdateOrderTotalsAsync(updateOrderParameters, restoredCart);

        if (!itemDeleted)
            await _orderService.UpdateOrderItemAsync(updatedOrderItem);

        if (updateOrderParameters.PickupPoint != null)
        {
            updatedOrder.PickupInStore = true;

            var pickupAddress = new Address
            {
                Address1 = updateOrderParameters.PickupPoint.Address,
                City = updateOrderParameters.PickupPoint.City,
                County = updateOrderParameters.PickupPoint.County,
                CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(updateOrderParameters.PickupPoint.CountryCode))?.Id,
                ZipPostalCode = updateOrderParameters.PickupPoint.ZipPostalCode,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _addressService.InsertAddressAsync(pickupAddress);

            updatedOrder.PickupAddressId = pickupAddress.Id;
            var shippingMethod = !string.IsNullOrEmpty(updateOrderParameters.PickupPoint.Name) ?
                string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), updateOrderParameters.PickupPoint.Name) :
                await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName");
            updatedOrder.ShippingMethod = shippingMethod;
            updatedOrder.ShippingRateComputationMethodSystemName = updateOrderParameters.PickupPoint.ProviderSystemName;
        }

        await CheckAndSaveOrderStatusAsync(updatedOrder, true);

        //discount usage history
        var discountUsageHistoryForOrder = await _discountService.GetAllDiscountUsageHistoryAsync(null, customer.Id, updatedOrder.Id);
        foreach (var discount in updateOrderParameters.AppliedDiscounts)
        {
            if (discountUsageHistoryForOrder.Any(history => history.DiscountId == discount.Id))
                continue;

            var d = await _discountService.GetDiscountByIdAsync(discount.Id);
            if (d != null)
                await _discountService.InsertDiscountUsageHistoryAsync(new DiscountUsageHistory
                {
                    DiscountId = d.Id,
                    OrderId = updatedOrder.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });
        }

        async Task<(List<ShoppingCartItem> restoredCart, ShoppingCartItem updatedShoppingCartItem)> restoreShoppingCartAsync(Order order, int updatedOrderItemId)
        {
            ArgumentNullException.ThrowIfNull(order);

            var cart = (await _orderService.GetOrderItemsAsync(order.Id)).Select(item => new ShoppingCartItem
            {
                Id = item.Id,
                AttributesXml = item.AttributesXml,
                CustomerId = order.CustomerId,
                ProductId = item.ProductId,
                Quantity = item.Id == updatedOrderItemId ? updateOrderParameters.Quantity : item.Quantity,
                RentalEndDateUtc = item.RentalEndDateUtc,
                RentalStartDateUtc = item.RentalStartDateUtc,
                ShoppingCartType = ShoppingCartType.ShoppingCart,
                StoreId = order.StoreId
            }).ToList();

            //get shopping cart item which has been updated
            var cartItem = cart.FirstOrDefault(shoppingCartItem => shoppingCartItem.Id == updatedOrderItemId);

            return (cart, cartItem);
        }
    }

    /// <summary>
    /// Deletes an order
    /// </summary>
    /// <param name="order">The order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        //check whether the order wasn't cancelled before
        //if it already was cancelled, then there's no need to make the following adjustments
        //(such as reward points, inventory, recurring payments)
        //they already was done when cancelling the order
        if (order.OrderStatus != OrderStatus.Cancelled)
        {
            //return (add) back redeemded reward points
            await ReturnBackRedeemedRewardPointsAsync(order);
            //reduce (cancel) back reward points (previously awarded for this order)
            await ReduceRewardPointsAsync(order);

            //cancel recurring payments
            var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id);
            foreach (var rp in recurringPayments)
                await CancelRecurringPaymentAsync(rp);

            //Adjust inventory for already shipped shipments
            //only products with "use multiple warehouses"
            await ReverseBookedInventoryAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));

            //Adjust inventory
            await ReturnOrderStockAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.DeleteOrder"), order.Id));
        }

        //deactivate gift cards
        if (_orderSettings.DeactivateGiftCardsAfterDeletingOrder)
            await SetActivatedValueForPurchasedGiftCardsAsync(order, false);

        //add a note
        await AddOrderNoteAsync(order, "Order has been deleted");

        //now delete an order
        await _orderService.DeleteOrderAsync(order);
    }

    /// <summary>
    /// Process next recurring payment
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <param name="paymentResult">Process payment result (info about last payment for automatic recurring payments)</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the collection of errors
    /// </returns>
    public virtual async Task<IEnumerable<string>> ProcessNextRecurringPaymentAsync(RecurringPayment recurringPayment, ProcessPaymentResult paymentResult = null)
    {
        ArgumentNullException.ThrowIfNull(recurringPayment);

        try
        {
            if (!recurringPayment.IsActive)
                throw new NopException("Recurring payment is not active");

            var initialOrder = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId)
                               ?? throw new NopException("Initial order could not be loaded");

            var customer = await _customerService.GetCustomerByIdAsync(initialOrder.CustomerId)
                           ?? throw new NopException("Customer could not be loaded");

            if (await GetNextPaymentDateAsync(recurringPayment) is null)
                throw new NopException("Next payment date could not be calculated");

            //payment info
            var processPaymentRequest = new ProcessPaymentRequest
            {
                StoreId = initialOrder.StoreId,
                CustomerId = customer.Id,
                OrderGuid = Guid.NewGuid(),
                InitialOrder = initialOrder,
                RecurringCycleLength = recurringPayment.CycleLength,
                RecurringCyclePeriod = recurringPayment.CyclePeriod,
                RecurringTotalCycles = recurringPayment.TotalCycles,
                CustomValues = _paymentService.DeserializeCustomValues(initialOrder)
            };

            //prepare order details
            var details = await PrepareRecurringOrderDetailsAsync(processPaymentRequest);

            ProcessPaymentResult processPaymentResult;
            //skip payment workflow if order total equals zero
            var skipPaymentWorkflow = details.OrderTotal == decimal.Zero;
            if (!skipPaymentWorkflow)
            {
                var paymentMethod = await _paymentPluginManager
                                        .LoadPluginBySystemNameAsync(processPaymentRequest.PaymentMethodSystemName, customer, initialOrder.StoreId)
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
                processPaymentResult = (await _paymentService.GetRecurringPaymentTypeAsync(processPaymentRequest.PaymentMethodSystemName)) switch
                {
                    RecurringPaymentType.NotSupported => throw new NopException("Recurring payments are not supported by selected payment method"),
                    RecurringPaymentType.Manual => await _paymentService.ProcessRecurringPaymentAsync(processPaymentRequest),
                    //payment is processed on payment gateway site, info about last transaction in paymentResult parameter
                    RecurringPaymentType.Automatic => paymentResult ?? new ProcessPaymentResult(),
                    _ => throw new NopException("Not supported recurring payment type"),
                };
            }
            else
                processPaymentResult = paymentResult ?? new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Paid };

            if (processPaymentResult == null)
                throw new NopException("processPaymentResult is not available");

            if (processPaymentResult.Success)
            {
                //save order details
                var order = await SaveOrderDetailsAsync(processPaymentRequest, processPaymentResult, details);

                foreach (var orderItem in await _orderService.GetOrderItemsAsync(details.InitialOrder.Id))
                {
                    //save item
                    var newOrderItem = new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        OrderId = order.Id,
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

                    await _orderService.InsertOrderItemAsync(newOrderItem);

                    var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                    //gift cards
                    await AddGiftCardsAsync(product, orderItem.AttributesXml, orderItem.Quantity, newOrderItem, amount: orderItem.UnitPriceExclTax);

                    //inventory
                    await _productService.AdjustInventoryAsync(product, -orderItem.Quantity, orderItem.AttributesXml,
                        string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.PlaceOrder"), order.Id));
                }

                //discount usage history
                await SaveDiscountUsageHistoryAsync(details, order);

                //notifications
                await SendNotificationsAndSaveNotesAsync(order);

                //raise event       
                await _eventPublisher.PublishAsync(new OrderPlacedEvent(order));

                //check order status
                await CheckOrderStatusAsync(order);

                if (order.PaymentStatus == PaymentStatus.Paid)
                    await ProcessOrderPaidAsync(order);

                //last payment succeeded
                recurringPayment.LastPaymentFailed = false;

                //next recurring payment
                await _orderService.InsertRecurringPaymentHistoryAsync(new RecurringPaymentHistory
                {
                    RecurringPaymentId = recurringPayment.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id
                });

                await _orderService.UpdateRecurringPaymentAsync(recurringPayment);

                return new List<string>();
            }

            //log errors
            var logError = processPaymentResult.Errors.Aggregate("Error while processing recurring order. ",
                (current, next) => $"{current}Error {processPaymentResult.Errors.IndexOf(next) + 1}: {next}. ");
            await _logger.ErrorAsync(logError, customer: customer);

            if (!processPaymentResult.RecurringPaymentFailed)
                return processPaymentResult.Errors;

            //set flag that last payment failed
            recurringPayment.LastPaymentFailed = true;
            await _orderService.UpdateRecurringPaymentAsync(recurringPayment);

            if (_paymentSettings.CancelRecurringPaymentsAfterFailedPayment)
            {
                //cancel recurring payment
                var errors = (await CancelRecurringPaymentAsync(recurringPayment)).ToList();
                foreach (var error in errors)
                {
                    await _logger.ErrorAsync(error);
                }

                //notify a customer about cancelled payment
                await _workflowMessageService.SendRecurringPaymentCancelledCustomerNotificationAsync(recurringPayment, initialOrder.CustomerLanguageId);
            }
            else
                //notify a customer about failed payment
                await _workflowMessageService.SendRecurringPaymentFailedCustomerNotificationAsync(recurringPayment, initialOrder.CustomerLanguageId);

            return processPaymentResult.Errors;
        }
        catch (Exception exc)
        {
            await _logger.ErrorAsync($"Error while processing recurring order. {exc.Message}", exc);
            throw;
        }
    }

    /// <summary>
    /// Cancels a recurring payment
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<IList<string>> CancelRecurringPaymentAsync(RecurringPayment recurringPayment)
    {
        ArgumentNullException.ThrowIfNull(recurringPayment);

        var initialOrder = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
        if (initialOrder == null)
            return new List<string> { "Initial order could not be loaded" };

        var request = new CancelRecurringPaymentRequest();
        CancelRecurringPaymentResult result = null;
        try
        {
            request.Order = initialOrder;
            result = await _paymentService.CancelRecurringPaymentAsync(request);
            if (result.Success)
            {
                //update recurring payment
                recurringPayment.IsActive = false;
                await _orderService.UpdateRecurringPaymentAsync(recurringPayment);

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = initialOrder.Id,
                    Note = "Recurring payment has been cancelled",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                //notify a store owner
                await _workflowMessageService
                    .SendRecurringPaymentCancelledStoreOwnerNotificationAsync(recurringPayment,
                        _localizationSettings.DefaultAdminLanguageId);
            }
        }
        catch (Exception exc)
        {
            result ??= new CancelRecurringPaymentResult();
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
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = initialOrder.Id,
            Note = $"Unable to cancel recurring payment. {error}",
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });

        //log it
        var logError = $"Error cancelling recurring payment. Order #{initialOrder.Id}. Error: {error}";
        await _logger.InsertLogAsync(LogLevel.Error, logError, logError);
        return result.Errors;
    }

    /// <summary>
    /// Gets a value indicating whether a customer can cancel recurring payment
    /// </summary>
    /// <param name="customerToValidate">Customer</param>
    /// <param name="recurringPayment">Recurring Payment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value indicating whether a customer can cancel recurring payment
    /// </returns>
    public virtual async Task<bool> CanCancelRecurringPaymentAsync(Customer customerToValidate, RecurringPayment recurringPayment)
    {
        if (recurringPayment is null)
            return false;

        if (customerToValidate is null)
            return false;

        var initialOrder = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);
        if (initialOrder is null)
            return false;

        var customer = await _customerService.GetCustomerByIdAsync(initialOrder.CustomerId);
        if (customer is null)
            return false;

        if (initialOrder.OrderStatus == OrderStatus.Cancelled)
            return false;

        if (!await _customerService.IsAdminAsync(customerToValidate))
            if (customer.Id != customerToValidate.Id)
                return false;

        if (await GetNextPaymentDateAsync(recurringPayment) is null)
            return false;

        return true;
    }

    /// <summary>
    /// Gets a value indicating whether a customer can retry last failed recurring payment
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="recurringPayment">Recurring Payment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true if a customer can retry payment; otherwise false
    /// </returns>
    public virtual async Task<bool> CanRetryLastRecurringPaymentAsync(Customer customer, RecurringPayment recurringPayment)
    {
        if (recurringPayment == null || customer == null)
            return false;

        var order = await _orderService.GetOrderByIdAsync(recurringPayment.InitialOrderId);

        if (order is null)
            return false;

        var orderCustomer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        if (order.OrderStatus == OrderStatus.Cancelled)
            return false;

        if (!recurringPayment.LastPaymentFailed || await _paymentService.GetRecurringPaymentTypeAsync(order.PaymentMethodSystemName) != RecurringPaymentType.Manual)
            return false;

        if (orderCustomer == null || (!await _customerService.IsAdminAsync(customer) && orderCustomer.Id != customer.Id))
            return false;

        return true;
    }

    /// <summary>
    /// Send a shipment
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="notifyCustomer">True to notify customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ShipAsync(Shipment shipment, bool notifyCustomer)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

        if (order.PickupInStore)
            throw new Exception("This shipment is can't be shipped. The order has been placed with 'pickup in store' shipping option.");

        if (shipment.ShippedDateUtc.HasValue)
            throw new Exception("This shipment is already shipped");

        shipment.ShippedDateUtc = DateTime.UtcNow;
        await _shipmentService.UpdateShipmentAsync(shipment);

        //process products with "Multiple warehouse" support enabled
        await BookReservedInventoryAsync(shipment, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Ship"), shipment.OrderId));

        //check whether we have more items to ship
        if (await _orderService.HasItemsToAddToShipmentAsync(order) || await _orderService.HasItemsToShipAsync(order))
            order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
        else
            order.ShippingStatusId = (int)ShippingStatus.Shipped;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, $"Shipment# {shipment.Id} has been sent");

        if (notifyCustomer)
        {
            //notify customer
            var queuedEmailIds = await _workflowMessageService.SendShipmentSentCustomerNotificationAsync(shipment, order.CustomerLanguageId);
            if (queuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Shipped\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", queuedEmailIds)}.");
        }

        //event
        await _eventPublisher.PublishShipmentSentAsync(shipment);

        //check order status
        await CheckOrderStatusAsync(order);
    }

    /// <summary>
    /// Marks a shipment as ready for pickup
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="notifyCustomer">True to notify customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ReadyForPickupAsync(Shipment shipment, bool notifyCustomer)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

        if (!order.PickupInStore)
            throw new Exception("This shipment is can't be marked as 'ready for pickup'. The order has been placed without 'pickup in store' shipping option.");

        if (shipment.ReadyForPickupDateUtc.HasValue)
            throw new Exception("This shipment is already marked as 'ready for pickup'");

        shipment.ReadyForPickupDateUtc = DateTime.UtcNow;
        await _shipmentService.UpdateShipmentAsync(shipment);

        await AddOrderNoteAsync(order, $"Shipment# {shipment.Id} has been ready for pickup");

        if (notifyCustomer)
        {
            var queuedEmailIds = await _workflowMessageService.SendShipmentReadyForPickupNotificationAsync(shipment, order.CustomerLanguageId);
            if (queuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Ready for pickup\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", queuedEmailIds)}.");
        }

        await _eventPublisher.PublishShipmentReadyForPickupAsync(shipment);
    }

    /// <summary>
    /// Marks a shipment as delivered
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="notifyCustomer">True to notify customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeliverAsync(Shipment shipment, bool notifyCustomer)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId) ?? throw new Exception("Order cannot be loaded");

        if (!order.PickupInStore && !shipment.ShippedDateUtc.HasValue)
            throw new Exception("This shipment is not shipped yet");

        if (order.PickupInStore && !shipment.ReadyForPickupDateUtc.HasValue)
            throw new Exception("This shipment is not yet ready for pickup");

        if (shipment.DeliveryDateUtc.HasValue)
            throw new Exception("This shipment is already delivered");

        shipment.DeliveryDateUtc = DateTime.UtcNow;
        await _shipmentService.UpdateShipmentAsync(shipment);

        if (!await _orderService.HasItemsToAddToShipmentAsync(order) &&
            !await _orderService.HasItemsToShipAsync(order) &&
            !await _orderService.HasItemsToReadyForPickupAsync(order) &&
            !await _orderService.HasItemsToDeliverAsync(order))
        {
            order.ShippingStatusId = (int)ShippingStatus.Delivered;
            await _orderService.UpdateOrderAsync(order);
        }

        //add a note
        await AddOrderNoteAsync(order, $"Shipment# {shipment.Id} has been delivered");

        if (order.PickupInStore)
        {
            // Shipment has been collected by customer.
            // We must process products with "Multiple warehouse" support enabled.
            await BookReservedInventoryAsync(shipment, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ReadyForPickupByCustomer"), shipment.OrderId));
        }

        if (notifyCustomer)
        {
            //send email notification
            var queuedEmailIds = await _workflowMessageService.SendShipmentDeliveredCustomerNotificationAsync(shipment, order.CustomerLanguageId);
            if (queuedEmailIds.Any())
                await AddOrderNoteAsync(order, $"\"Delivered\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", queuedEmailIds)}.");
        }

        //event
        await _eventPublisher.PublishShipmentDeliveredAsync(shipment);

        //check order status
        await CheckOrderStatusAsync(order);
    }

    /// <summary>
    /// Gets a value indicating whether cancel is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A value indicating whether cancel is allowed</returns>
    public virtual bool CanCancelOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.OrderStatus == OrderStatus.Cancelled)
            return false;

        return true;
    }

    /// <summary>
    /// Cancels order
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="notifyCustomer">True to notify customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task CancelOrderAsync(Order order, bool notifyCustomer)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!CanCancelOrder(order))
            throw new NopException("Cannot do cancel for order.");

        //cancel order
        await SetOrderStatusAsync(order, OrderStatus.Cancelled, notifyCustomer);

        //add a note
        await AddOrderNoteAsync(order, "Order has been cancelled");

        //return (add) back redeemded reward points
        await ReturnBackRedeemedRewardPointsAsync(order);

        //delete gift card usage history
        if (_orderSettings.DeleteGiftCardUsageHistory)
            await _giftCardService.DeleteGiftCardUsageHistoryAsync(order);

        //cancel recurring payments
        var recurringPayments = await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id);
        foreach (var rp in recurringPayments)
            await CancelRecurringPaymentAsync(rp);

        //Adjust inventory for already shipped shipments
        //only products with "use multiple warehouses"
        await ReverseBookedInventoryAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));

        //Adjust inventory
        await ReturnOrderStockAsync(order, string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.CancelOrder"), order.Id));
    }

    /// <summary>
    /// Gets a value indicating whether order can be marked as authorized
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A value indicating whether order can be marked as authorized</returns>
    public virtual bool CanMarkOrderAsAuthorized(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MarkAsAuthorizedAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        order.PaymentStatusId = (int)PaymentStatus.Authorized;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, "Order has been marked as authorized");

        await _eventPublisher.PublishAsync(new OrderAuthorizedEvent(order));

        //check order status
        await CheckOrderStatusAsync(order);
    }

    /// <summary>
    /// Gets a value indicating whether capture from admin panel is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether capture from admin panel is allowed
    /// </returns>
    public virtual async Task<bool> CanCaptureAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.OrderStatus == OrderStatus.Cancelled ||
            order.OrderStatus == OrderStatus.Pending)
            return false;

        if (order.PaymentStatus == PaymentStatus.Authorized &&
            await _paymentService.SupportCaptureAsync(order.PaymentMethodSystemName))
            return true;

        return false;
    }

    /// <summary>
    /// Capture an order (from admin panel)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of errors; empty list if no errors
    /// </returns>
    public virtual async Task<IList<string>> CaptureAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!await CanCaptureAsync(order))
            throw new NopException("Cannot do capture for order.");

        var request = new CapturePaymentRequest();
        CapturePaymentResult result = null;
        try
        {
            //old info from placing order
            request.Order = order;
            result = await _paymentService.CaptureAsync(request);

            if (result.Success)
            {
                var paidDate = order.PaidDateUtc;
                if (result.NewPaymentStatus == PaymentStatus.Paid)
                    paidDate = DateTime.UtcNow;

                order.CaptureTransactionId = result.CaptureTransactionId;
                order.CaptureTransactionResult = result.CaptureTransactionResult;
                order.PaymentStatus = result.NewPaymentStatus;
                order.PaidDateUtc = paidDate;
                await _orderService.UpdateOrderAsync(order);

                //add a note
                await AddOrderNoteAsync(order, "Order has been captured");

                await CheckOrderStatusAsync(order);

                if (order.PaymentStatus == PaymentStatus.Paid)
                    await ProcessOrderPaidAsync(order);
            }
        }
        catch (Exception exc)
        {
            result ??= new CapturePaymentResult();
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
        await AddOrderNoteAsync(order, $"Unable to capture order. {error}");

        //log it
        var logError = $"Error capturing order #{order.Id}. Error: {error}";
        await _logger.InsertLogAsync(LogLevel.Error, logError, logError);
        return result.Errors;
    }

    /// <summary>
    /// Gets a value indicating whether order can be marked as paid
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A value indicating whether order can be marked as paid</returns>
    public virtual bool CanMarkOrderAsPaid(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task MarkOrderAsPaidAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!CanMarkOrderAsPaid(order))
            throw new NopException("You can't mark this order as paid");

        order.PaymentStatusId = (int)PaymentStatus.Paid;
        order.PaidDateUtc = DateTime.UtcNow;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, "Order has been marked as paid");

        await CheckOrderStatusAsync(order);

        if (order.PaymentStatus == PaymentStatus.Paid)
            await ProcessOrderPaidAsync(order);
    }

    /// <summary>
    /// Gets a value indicating whether refund from admin panel is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether refund from admin panel is allowed
    /// </returns>
    public virtual async Task<bool> CanRefundAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.OrderTotal == decimal.Zero)
            return false;

        //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
        if (order.RefundedAmount > decimal.Zero)
            return false;

        //uncomment the lines below in order to disallow this operation for cancelled orders
        //if (order.OrderStatus == OrderStatus.Cancelled)
        //    return false;

        if (order.PaymentStatus == PaymentStatus.Paid &&
            await _paymentService.SupportRefundAsync(order.PaymentMethodSystemName))
            return true;

        return false;
    }

    /// <summary>
    /// Refunds an order (from admin panel)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of errors; empty list if no errors
    /// </returns>
    public virtual async Task<IList<string>> RefundAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!await CanRefundAsync(order))
            throw new NopException("Cannot do refund for order.");

        var request = new RefundPaymentRequest();
        RefundPaymentResult result = null;
        try
        {
            request.Order = order;
            request.AmountToRefund = order.OrderTotal;
            request.IsPartialRefund = false;
            result = await _paymentService.RefundAsync(request);
            if (result.Success)
            {
                //total amount refunded
                var totalAmountRefunded = order.RefundedAmount + request.AmountToRefund;

                //update order info
                order.RefundedAmount = totalAmountRefunded;
                order.PaymentStatus = result.NewPaymentStatus;
                await _orderService.UpdateOrderAsync(order);

                //add a note
                await AddOrderNoteAsync(order, $"Order has been refunded. Amount = {request.AmountToRefund}");

                //raise event       
                await _eventPublisher.PublishAsync(new OrderRefundedEvent(order, request.AmountToRefund));

                //check order status
                await CheckOrderStatusAsync(order);

                //notifications
                var orderRefundedStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(order, request.AmountToRefund, _localizationSettings.DefaultAdminLanguageId);
                if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

                var orderRefundedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(order, request.AmountToRefund, order.CustomerLanguageId);
                if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");
            }
        }
        catch (Exception exc)
        {
            result ??= new RefundPaymentResult();
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
        await AddOrderNoteAsync(order, $"Unable to refund order. {error}");

        //log it
        var logError = $"Error refunding order #{order.Id}. Error: {error}";
        await _logger.InsertLogAsync(LogLevel.Error, logError, logError);

        return result.Errors;
    }

    /// <summary>
    /// Gets a value indicating whether order can be marked as refunded
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A value indicating whether order can be marked as refunded</returns>
    public virtual bool CanRefundOffline(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task RefundOfflineAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!CanRefundOffline(order))
            throw new NopException("You can't refund this order");

        //amout to refund
        var amountToRefund = order.OrderTotal;

        //total amount refunded
        var totalAmountRefunded = order.RefundedAmount + amountToRefund;

        //update order info
        order.RefundedAmount = totalAmountRefunded;
        order.PaymentStatus = PaymentStatus.Refunded;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, $"Order has been marked as refunded. Amount = {amountToRefund}");

        //raise event       
        await _eventPublisher.PublishAsync(new OrderRefundedEvent(order, amountToRefund));

        //check order status
        await CheckOrderStatusAsync(order);

        //notifications
        var orderRefundedStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
        if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

        var orderRefundedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(order, amountToRefund, order.CustomerLanguageId);
        if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");
    }

    /// <summary>
    /// Gets a value indicating whether partial refund from admin panel is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="amountToRefund">Amount to refund</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether refund from admin panel is allowed
    /// </returns>
    public virtual async Task<bool> CanPartiallyRefundAsync(Order order, decimal amountToRefund)
    {
        ArgumentNullException.ThrowIfNull(order);

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
            await _paymentService.SupportPartiallyRefundAsync(order.PaymentMethodSystemName))
            return true;

        return false;
    }

    /// <summary>
    /// Partially refunds an order (from admin panel)
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="amountToRefund">Amount to refund</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a list of errors; empty list if no errors
    /// </returns>
    public virtual async Task<IList<string>> PartiallyRefundAsync(Order order, decimal amountToRefund)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!await CanPartiallyRefundAsync(order, amountToRefund))
            throw new NopException("Cannot do partial refund for order.");

        var request = new RefundPaymentRequest();
        RefundPaymentResult result = null;
        try
        {
            request.Order = order;
            request.AmountToRefund = amountToRefund;
            request.IsPartialRefund = true;

            result = await _paymentService.RefundAsync(request);

            if (result.Success)
            {
                //total amount refunded
                var totalAmountRefunded = order.RefundedAmount + amountToRefund;

                //update order info
                order.RefundedAmount = totalAmountRefunded;
                //mark payment status as 'Refunded' if the order total amount is fully refunded
                order.PaymentStatus = order.OrderTotal == totalAmountRefunded && result.NewPaymentStatus == PaymentStatus.PartiallyRefunded ? PaymentStatus.Refunded : result.NewPaymentStatus;
                await _orderService.UpdateOrderAsync(order);

                //add a note
                await AddOrderNoteAsync(order, $"Order has been partially refunded. Amount = {amountToRefund}");

                //raise event       
                await _eventPublisher.PublishAsync(new OrderRefundedEvent(order, amountToRefund));

                //check order status
                await CheckOrderStatusAsync(order);

                //notifications
                var orderRefundedStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
                if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

                var orderRefundedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(order, amountToRefund, order.CustomerLanguageId);
                if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
                    await AddOrderNoteAsync(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");
            }
        }
        catch (Exception exc)
        {
            result ??= new RefundPaymentResult();
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
        await AddOrderNoteAsync(order, $"Unable to partially refund order. {error}");

        //log it
        var logError = $"Error refunding order #{order.Id}. Error: {error}";
        await _logger.InsertLogAsync(LogLevel.Error, logError, logError);
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
        ArgumentNullException.ThrowIfNull(order);

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
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PartiallyRefundOfflineAsync(Order order, decimal amountToRefund)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!CanPartiallyRefundOffline(order, amountToRefund))
            throw new NopException("You can't partially refund (offline) this order");

        //total amount refunded
        var totalAmountRefunded = order.RefundedAmount + amountToRefund;

        //update order info
        order.RefundedAmount = totalAmountRefunded;
        //mark payment status as 'Refunded' if the order total amount is fully refunded
        order.PaymentStatus = order.OrderTotal == totalAmountRefunded ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, $"Order has been marked as partially refunded. Amount = {amountToRefund}");

        //raise event       
        await _eventPublisher.PublishAsync(new OrderRefundedEvent(order, amountToRefund));

        //check order status
        await CheckOrderStatusAsync(order);

        //notifications
        var orderRefundedStoreOwnerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(order, amountToRefund, _localizationSettings.DefaultAdminLanguageId);
        if (orderRefundedStoreOwnerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order refunded\" email (to store owner) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedStoreOwnerNotificationQueuedEmailIds)}.");

        var orderRefundedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendOrderRefundedCustomerNotificationAsync(order, amountToRefund, order.CustomerLanguageId);
        if (orderRefundedCustomerNotificationQueuedEmailIds.Any())
            await AddOrderNoteAsync(order, $"\"Order refunded\" email (to customer) has been queued. Queued email identifiers: {string.Join(", ", orderRefundedCustomerNotificationQueuedEmailIds)}.");
    }

    /// <summary>
    /// Gets a value indicating whether void from admin panel is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value indicating whether void from admin panel is allowed
    /// </returns>
    public virtual async Task<bool> CanVoidAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (order.OrderTotal == decimal.Zero)
            return false;

        //uncomment the lines below in order to allow this operation for cancelled orders
        //if (order.OrderStatus == OrderStatus.Cancelled)
        //    return false;

        if (order.PaymentStatus == PaymentStatus.Authorized &&
            await _paymentService.SupportVoidAsync(order.PaymentMethodSystemName))
            return true;

        return false;
    }

    /// <summary>
    /// Voids order (from admin panel)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the voided orders
    /// </returns>
    public virtual async Task<IList<string>> VoidAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!await CanVoidAsync(order))
            throw new NopException("Cannot do void for order.");

        var request = new VoidPaymentRequest();
        VoidPaymentResult result = null;
        try
        {
            request.Order = order;
            result = await _paymentService.VoidAsync(request);

            if (result.Success)
            {
                //update order info
                order.PaymentStatus = result.NewPaymentStatus;
                await _orderService.UpdateOrderAsync(order);

                //add a note
                await AddOrderNoteAsync(order, "Order has been voided");

                //raise event       
                await _eventPublisher.PublishAsync(new OrderVoidedEvent(order));

                //check order status
                await CheckOrderStatusAsync(order);
            }
        }
        catch (Exception exc)
        {
            result ??= new VoidPaymentResult();
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
        await AddOrderNoteAsync(order, $"Unable to voiding order. {error}");

        //log it
        var logError = $"Error voiding order #{order.Id}. Error: {error}";
        await _logger.InsertLogAsync(LogLevel.Error, logError, logError);
        return result.Errors;
    }

    /// <summary>
    /// Gets a value indicating whether order can be marked as voided
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A value indicating whether order can be marked as voided</returns>
    public virtual bool CanVoidOffline(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

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
    /// Void order (offline)
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task VoidOfflineAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!CanVoidOffline(order))
            throw new NopException("You can't void this order");

        order.PaymentStatusId = (int)PaymentStatus.Voided;
        await _orderService.UpdateOrderAsync(order);

        //add a note
        await AddOrderNoteAsync(order, "Order has been marked as voided");

        //raise event       
        await _eventPublisher.PublishAsync(new OrderVoidedEvent(order));

        //check order status
        await CheckOrderStatusAsync(order);
    }

    /// <summary>
    /// Place order items in current user shopping cart.
    /// </summary>
    /// <param name="order">The order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the warnings
    /// </returns>
    public virtual async Task<IList<string>> ReOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

        var warnings = new List<string>();

        //move shopping cart items (if possible)
        foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            warnings.AddRange(await _shoppingCartService.AddToCartAsync(customer, product,
                ShoppingCartType.ShoppingCart, order.StoreId,
                orderItem.AttributesXml, orderItem.UnitPriceExclTax,
                orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                orderItem.Quantity, false));
        }

        //set checkout attributes
        //comment the code below if you want to disable this functionality
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CheckoutAttributes, order.CheckoutAttributesXml, order.StoreId);

        return warnings;
    }

    /// <summary>
    /// Check whether return request is allowed
    /// </summary>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual async Task<bool> IsReturnRequestAllowedAsync(Order order)
    {
        if (!_orderSettings.ReturnRequestsEnabled)
            return false;

        if (order == null || order.Deleted)
            return false;

        //status should be complete
        if (order.OrderStatus != OrderStatus.Complete)
            return false;

        //validate allowed number of days
        if (_orderSettings.NumberOfDaysReturnRequestAvailable > 0)
        {
            var daysPassed = (DateTime.UtcNow - order.CreatedOnUtc).TotalDays;
            if (daysPassed >= _orderSettings.NumberOfDaysReturnRequestAvailable)
                return false;
        }

        var returnRequestAvailability = await _returnRequestService.GetReturnRequestAvailabilityAsync(order.Id);
        if (returnRequestAvailability == null)
            return false;

        return returnRequestAvailability.IsAllowed;
    }

    /// <summary>
    /// Validate minimum order sub-total amount
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - OK; false - minimum order sub-total amount is not reached
    /// </returns>
    public virtual async Task<bool> ValidateMinOrderSubtotalAmountAsync(IList<ShoppingCartItem> cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        //min order amount sub-total validation
        if (!cart.Any() || _orderSettings.MinOrderSubtotalAmount <= decimal.Zero)
            return true;

        //subtotal
        var (_, _, subTotalWithoutDiscountBase, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, _orderSettings.MinOrderSubtotalAmountIncludingTax);

        if (subTotalWithoutDiscountBase < _orderSettings.MinOrderSubtotalAmount)
            return false;

        return true;
    }

    /// <summary>
    /// Validate minimum order total amount
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains true - OK; false - minimum order total amount is not reached
    /// </returns>
    public virtual async Task<bool> ValidateMinOrderTotalAmountAsync(IList<ShoppingCartItem> cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        if (!cart.Any() || _orderSettings.MinOrderTotalAmount <= decimal.Zero)
            return true;

        var shoppingCartTotalBase = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal;

        if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value < _orderSettings.MinOrderTotalAmount)
            return false;

        return true;
    }

    /// <summary>
    /// Gets a value indicating whether payment workflow is required
    /// </summary>
    /// <param name="cart">Shopping cart</param>
    /// <param name="useRewardPoints">A value indicating reward points should be used; null to detect current choice of the customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the value indicating whether payment workflow is required
    /// </returns>
    public virtual async Task<bool> IsPaymentWorkflowRequiredAsync(IList<ShoppingCartItem> cart, bool? useRewardPoints = null)
    {
        ArgumentNullException.ThrowIfNull(cart);

        var result = true;

        //check whether order total equals zero
        var shoppingCartTotalBase = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, useRewardPoints: useRewardPoints, usePaymentMethodAdditionalFee: false)).shoppingCartTotal;

        if (shoppingCartTotalBase is decimal.Zero)
            result = false;

        return result;
    }

    /// <summary>
    /// Gets the next payment date
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<DateTime?> GetNextPaymentDateAsync(RecurringPayment recurringPayment)
    {
        ArgumentNullException.ThrowIfNull(recurringPayment);

        if (!recurringPayment.IsActive)
            return null;

        var historyCollection = await _orderService.GetRecurringPaymentHistoryAsync(recurringPayment);
        if (historyCollection.Count >= recurringPayment.TotalCycles)
            return null;

        //result
        DateTime? result = null;

        //calculate next payment date
        if (historyCollection.Any())
        {
            result = recurringPayment.CyclePeriod switch
            {
                RecurringProductCyclePeriod.Days => recurringPayment.StartDateUtc.AddDays((double)recurringPayment.CycleLength * historyCollection.Count),
                RecurringProductCyclePeriod.Weeks => recurringPayment.StartDateUtc.AddDays((double)(7 * recurringPayment.CycleLength) * historyCollection.Count),
                RecurringProductCyclePeriod.Months => recurringPayment.StartDateUtc.AddMonths(recurringPayment.CycleLength * historyCollection.Count),
                RecurringProductCyclePeriod.Years => recurringPayment.StartDateUtc.AddYears(recurringPayment.CycleLength * historyCollection.Count),
                _ => throw new NopException("Not supported cycle period"),
            };
        }
        else
        {
            if (recurringPayment.TotalCycles > 0)
                result = recurringPayment.StartDateUtc;
        }

        return result;
    }

    /// <summary>
    /// Gets the cycles remaining
    /// </summary>
    /// <param name="recurringPayment">Recurring payment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task<int> GetCyclesRemainingAsync(RecurringPayment recurringPayment)
    {
        ArgumentNullException.ThrowIfNull(recurringPayment);

        var historyCollection = await _orderService.GetRecurringPaymentHistoryAsync(recurringPayment);

        var result = recurringPayment.TotalCycles - historyCollection.Count;
        if (result < 0)
            result = 0;

        return result;
    }

    #endregion

    #region Nested class

    /// <summary>
    /// PlaceOrder container
    /// </summary>
    protected partial class PlaceOrderContainer
    {
        public PlaceOrderContainer()
        {
            Cart = new List<ShoppingCartItem>();
            AppliedDiscounts = new List<Discount>();
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
        public List<Discount> AppliedDiscounts { get; set; }

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
}