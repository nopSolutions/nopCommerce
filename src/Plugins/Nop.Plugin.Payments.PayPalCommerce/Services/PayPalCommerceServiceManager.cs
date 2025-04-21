using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Payments.PayPalCommerce.Domain;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Authentication;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Identity;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Onboarding;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;
using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Mvc.Routing;
using Address = Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Address;
using NopAddress = Nop.Core.Domain.Common.Address;
using NopOrder = Nop.Core.Domain.Orders.Order;
using NopShippingOption = Nop.Core.Domain.Shipping.ShippingOption;
using Order = Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Order;
using ShippingOption = Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.ShippingOption;

namespace Nop.Plugin.Payments.PayPalCommerce.Services;

/// <summary>
/// Represents the plugin service manager
/// </summary>
public class PayPalCommerceServiceManager
{
    #region Fields

    private readonly CurrencySettings _currencySettings;
    private readonly CustomerSettings _customerSettings;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IAddressService _addressService;
    private readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    private readonly ICountryService _countryService;
    private readonly ICurrencyService _currencyService;
    private readonly ICustomerService _customerService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger _logger;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly IPaymentService _paymentService;
    private readonly IPickupPluginManager _pickupPluginManager;
    private readonly IPictureService _pictureService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IProductService _productService;
    private readonly IShipmentService _shipmentService;
    private readonly IShippingService _shippingService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IShortTermCacheManager _shortTermCacheManager;
    private readonly IStateProvinceService _stateProvinceService;
    private readonly IStoreContext _storeContext;
    private readonly IStoreService _storeService;
    private readonly ITaxService _taxService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IUrlRecordService _urlRecordService;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly OrderSettings _orderSettings;
    private readonly PayPalCommerceHttpClient _httpClient;
    private readonly PayPalTokenService _tokenService;
    private readonly ShippingSettings _shippingSettings;
    private readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public PayPalCommerceServiceManager(CurrencySettings currencySettings,
        CustomerSettings customerSettings,
        IActionContextAccessor actionContextAccessor,
        IAddressService addressService,
        IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        INopUrlHelper nopUrlHelper,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPickupPluginManager pickupPluginManager,
        IPictureService pictureService,
        IPriceCalculationService priceCalculationService,
        IProductService productService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        IShortTermCacheManager shortTermCacheManager,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreService storeService,
        ITaxService taxService,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext,
        OrderSettings orderSettings,
        PayPalCommerceHttpClient httpClient,
        PayPalTokenService tokenService,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings)
    {
        _currencySettings = currencySettings;
        _customerSettings = customerSettings;
        _actionContextAccessor = actionContextAccessor;
        _addressService = addressService;
        _checkoutAttributeParser = checkoutAttributeParser;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _nopUrlHelper = nopUrlHelper;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pickupPluginManager = pickupPluginManager;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _productService = productService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _shortTermCacheManager = shortTermCacheManager;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeService = storeService;
        _taxService = taxService;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _httpClient = httpClient;
        _tokenService = tokenService;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error message if exists
    /// </returns>
    private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //invoke function
            return (await function(), default);
        }
        catch (Exception exception)
        {
            //log errors
            if (logErrors)
            {
                var logMessage = $"{PayPalCommerceDefaults.SystemName} error:{Environment.NewLine}{exception.Message}";
                var exceptionToLog = exception is NopException nopException ? nopException.InnerException ?? nopException : exception;
                var customer = await _workContext.GetCurrentCustomerAsync();
                await _logger.ErrorAsync(logMessage, exceptionToLog, customer);
            }

            return (default, exception.Message);
        }
    }

    #region Components

    /// <summary>
    /// Prepare amount value for Pay Later messages
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="customer">Customer</param>
    /// <param name="currencyCode">Currency code</param>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the amount value
    /// </returns>
    private async Task<string> PrepareMessagesAmountAsync(ButtonPlacement placement, Customer customer, string currencyCode, int? productId)
    {
        //cache result during HTTP request
        return await _shortTermCacheManager.GetAsync(async () =>
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var product = await _productService.GetProductByIdAsync(productId ?? 0);
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            var (_, _, _, subTotal, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, true);

            var amount = placement switch
            {
                ButtonPlacement.Cart => subTotal,
                ButtonPlacement.Product when product is not null
                    => (await _priceCalculationService.GetFinalPriceAsync(product, customer, store)).finalPrice, //+ subTotal,
                ButtonPlacement.PaymentMethod
                    => (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, null, usePaymentMethodAdditionalFee: false))
                        .shoppingCartTotal ?? subTotal,
                _ => (decimal?)null
            };

            return amount is not null ? PrepareMoney(amount.Value, currencyCode).Value : null;
        }, new($"{PayPalCommerceDefaults.SystemName}.Messages.Amount-{productId ?? 0}"));
    }

    #endregion

    #region Orders

    /// <summary>
    /// Prepare money object
    /// </summary>
    /// <param name="value">Amount value</param>
    /// <param name="currencyCode">Currency code</param>
    /// <returns>Money object</returns>
    private static Money PrepareMoney(decimal value, string currencyCode)
    {
        var format = PayPalCommerceDefaults.CurrenciesWithoutDecimals.Contains(currencyCode.ToUpper()) ? "0" : "0.00";
        return new()
        {
            CurrencyCode = currencyCode,
            Value = value.ToString(format, CultureInfo.InvariantCulture)
        };
    }

    /// <summary>
    /// Convert money object to decimal value
    /// </summary>
    /// <param name="value">Amount value</param>
    /// <returns>Decimal value</returns>
    private static decimal ConvertMoney(Money amount)
    {
        return decimal.TryParse(amount?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : decimal.Zero;
    }

    /// <summary>
    /// Prepare order context
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="details">Shopping cart details</param>
    /// <param name="orderGuid">Order internal id</param>
    /// <param name="isApplePay">Apple Pay payment</param>
    /// <returns>Order context</returns>
    private ExperienceContext PrepareOrderContext(PayPalCommerceSettings settings, CartDetails details, string orderGuid, bool isApplePay = false)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var protocol = _webHelper.GetCurrentRequestProtocol();

        var shippingPreference = ShippingPreferenceType.NO_SHIPPING.ToString().ToUpper();
        if (details.ShippingIsRequired)
        {
            shippingPreference = details.Placement == ButtonPlacement.PaymentMethod && !isApplePay
                ? ShippingPreferenceType.SET_PROVIDED_ADDRESS.ToString().ToUpper()
                : ShippingPreferenceType.GET_FROM_FILE.ToString().ToUpper();
        }

        return new()
        {
            //Locale = null, //PayPal auto detects this
            BrandName = CommonHelper.EnsureMaximumLength(details.Store.Name, 127),
            LandingPage = LandingPageType.NO_PREFERENCE.ToString().ToUpper(),
            UserAction = details.Placement == ButtonPlacement.PaymentMethod && settings.SkipOrderConfirmPage
                ? UserActionType.PAY_NOW.ToString().ToUpper()
                : UserActionType.CONTINUE.ToString().ToUpper(),
            CancelUrl = details.Placement switch
            {
                ButtonPlacement.PaymentMethod => urlHelper.RouteUrl(PayPalCommerceDefaults.Route.PaymentInfo, null, protocol),
                ButtonPlacement.Cart or ButtonPlacement.Product => urlHelper.RouteUrl(PayPalCommerceDefaults.Route.ShoppingCart, null, protocol),
                _ => null
            },
            ReturnUrl = urlHelper.RouteUrl(PayPalCommerceDefaults.Route.ConfirmOrder, new { token = orderGuid, approve = true }, protocol),
            PaymentMethodPreference = settings.ImmediatePaymentRequired
                ? PaymentMethodPreferenceType.IMMEDIATE_PAYMENT_REQUIRED.ToString().ToUpper()
                : PaymentMethodPreferenceType.UNRESTRICTED.ToString().ToUpper(),
            ShippingPreference = shippingPreference
        };
    }

    /// <summary>
    /// Prepare order billing details
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the payer with the billing details
    /// </returns>
    private async Task<Payer> PrepareBillingDetailsAsync(CartDetails details)
    {
        var customer = details.Customer;
        var address = details.BillingAddress;
        var isPaymentMethodPage = details.Placement == ButtonPlacement.PaymentMethod;

        var email = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.Email : customer.Email, 254);
        var name = new Name
        {
            GivenName = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.FirstName : customer.FirstName, 140),
            Surname = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.LastName : customer.LastName, 140)
        };
        //phone number format is unpredictable
        //var phone = isPaymentMethodPage
        //    ? (!string.IsNullOrEmpty(address.PhoneNumber) ? new Phone { PhoneNumber = new() { NationalNumber = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(address.PhoneNumber), 14) } } : null)
        //    : !string.IsNullOrEmpty(customer.Phone) ? new Phone { PhoneNumber = new() { NationalNumber = CommonHelper.EnsureMaximumLength(CommonHelper.EnsureNumericOnly(customer.Phone), 14) } } : null;
        var birthDate = customer.DateOfBirth?.ToString("yyyy-MM-dd");
        var country = await _countryService.GetCountryByIdAsync(isPaymentMethodPage ? address.CountryId ?? 0 : customer.CountryId);
        var state = await _stateProvinceService
            .GetStateProvinceByIdAsync(isPaymentMethodPage ? address.StateProvinceId ?? 0 : customer.StateProvinceId);
        var billingAddress = new Address
        {
            AddressLine1 = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.Address1 : customer.StreetAddress, 300),
            AddressLine2 = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.Address2 : customer.StreetAddress2, 300),
            AdminArea2 = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.City : customer.City, 120),
            AdminArea1 = CommonHelper.EnsureMaximumLength(state?.Abbreviation, 300),
            CountryCode = CommonHelper.EnsureMaximumLength(country?.TwoLetterIsoCode, 2),
            PostalCode = CommonHelper.EnsureMaximumLength(isPaymentMethodPage ? address.ZipPostalCode : customer.ZipPostalCode, 60)
        };

        //country is required
        if (string.IsNullOrEmpty(billingAddress.CountryCode))
            billingAddress = null;

        return new() { EmailAddress = email, Name = name, BirthDate = birthDate, Address = billingAddress };
    }

    /// <summary>
    /// Prepare order items
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of purchase items
    /// </returns>
    private async Task<List<Item>> PrepareOrderItemsAsync(CartDetails details)
    {
        //cart items
        var items = await details.Cart.SelectAwait(async item =>
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            if (product is null)
                return null;

            var sku = await _productService.FormatSkuAsync(product, item.AttributesXml);
            var seName = await _urlRecordService.GetSeNameAsync(product);
            var url = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());

            var picture = await _pictureService.GetProductPictureAsync(product, item.AttributesXml);
            //PayPal doesn't currently support WebP images
            var ext = await _pictureService.GetFileExtensionFromMimeTypeAsync(picture?.MimeType);
            var (imageUrl, _) = ext != "webp" ? await _pictureService.GetPictureUrlAsync(picture) : default;

            var (itemSubTotal, itemDiscount, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
            var unitPrice = itemSubTotal / item.Quantity;
            var (unitPriceExclTax, _) = await _taxService.GetProductPriceAsync(product, unitPrice, false, details.Customer);

            return new Item
            {
                Name = CommonHelper.EnsureMaximumLength(product.Name, 127),
                Description = CommonHelper.EnsureMaximumLength(product.ShortDescription, 127),
                Sku = CommonHelper.EnsureMaximumLength(sku, 127),
                Quantity = item.Quantity.ToString(),
                Category = product.IsDownload
                    ? CategoryType.DIGITAL_GOODS.ToString().ToUpper()
                    : CategoryType.PHYSICAL_GOODS.ToString().ToUpper(),
                Url = url,
                ImageUrl = imageUrl,
                UnitAmount = PrepareMoney(unitPriceExclTax, details.CurrencyCode)
            };
        }).Where(item => item is not null).ToListAsync();

        //and checkout attributes
        var checkoutAttributes = await _genericAttributeService
            .GetAttributeAsync<string>(details.Customer, NopCustomerDefaults.CheckoutAttributes, details.Store.Id);
        var checkoutAttributeValues = _checkoutAttributeParser.ParseAttributeValues(checkoutAttributes);
        await foreach (var (attribute, values) in checkoutAttributeValues)
        {
            await foreach (var attributeValue in values)
            {
                var (attributePriceExclTax, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, false, details.Customer);

                items.Add(new()
                {
                    Name = CommonHelper.EnsureMaximumLength(attribute.Name, 127),
                    Description = CommonHelper.EnsureMaximumLength($"{attribute.Name} - {attributeValue.Name}", 127),
                    Quantity = 1.ToString(),
                    UnitAmount = PrepareMoney(attributePriceExclTax, details.CurrencyCode)
                });
            }
        }

        return items;
    }

    /// <summary>
    /// Prepare order amount with breakdown
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <param name="items">Purchase items</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order amount with breakdown
    /// </returns>
    private async Task<OrderMoney> PrepareOrderMoneyAsync(CartDetails details, List<Item> items)
    {
        //in some rare cases we need an additional item to adjust the order total
        //this can happen due to complex discounts or a large order and related to rounding in calculations
        //PayPal uses two decimal places, while nopCommerce can use more complex types of rounding (configured for each currency separately) 
        var adjustmentName = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Adjustment.Name");
        var adjustmentDescription = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Adjustment.Description");
        if (items.FirstOrDefault(item => adjustmentName.Equals(item.Name) && adjustmentDescription.Equals(item.Description)) is Item adjustmentItem)
            items.Remove(adjustmentItem);

        var (total, _, _, _, _, _) = await _orderTotalCalculationService
            .GetShoppingCartTotalAsync(details.Cart, usePaymentMethodAdditionalFee: false);
        if (total is null)
        {
            if (details.Placement == ButtonPlacement.PaymentMethod)
                throw new NopException("Shopping cart total couldn't be calculated now");

            //on product and cart pages the total is not yet calculated, so use subtotal here
            var (_, _, subTotal, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(details.Cart, includingTax: false);
            total = subTotal;
        }
        var orderTotal = PrepareMoney(total.Value, details.CurrencyCode);

        var (shippingTotal, _, _) = await _orderTotalCalculationService
            .GetShoppingCartShippingTotalAsync(details.Cart, includingTax: _taxSettings.ShippingPriceIncludesTax);
        var orderShippingTotal = PrepareMoney(shippingTotal ?? decimal.Zero, details.CurrencyCode);

        var (taxTotal, _) = await _orderTotalCalculationService.GetTaxTotalAsync(details.Cart, usePaymentMethodAdditionalFee: false);
        var orderTaxTotal = PrepareMoney(taxTotal, details.CurrencyCode);

        var itemAdjustment = decimal.Zero;
        var itemTotal = items.Sum(item => ConvertMoney(item.UnitAmount) * int.Parse(item.Quantity));
        var discountTotal = itemTotal + ConvertMoney(orderTaxTotal) + ConvertMoney(orderShippingTotal) - ConvertMoney(orderTotal);
        if (discountTotal < decimal.Zero)
        {
            itemAdjustment = -discountTotal;
            itemTotal += itemAdjustment;
            discountTotal = decimal.Zero;
        }
        var orderItemTotal = PrepareMoney(itemTotal, details.CurrencyCode);
        var orderDiscount = PrepareMoney(discountTotal, details.CurrencyCode);

        //set adjustment item if needed
        if (itemAdjustment > decimal.Zero)
        {
            var unitAmount = PrepareMoney(itemAdjustment, details.CurrencyCode);
            if (ConvertMoney(unitAmount) > decimal.Zero)
            {
                items.Add(new()
                {
                    Name = adjustmentName,
                    Description = adjustmentDescription,
                    Quantity = 1.ToString(),
                    UnitAmount = unitAmount
                });
            }
        }

        return new()
        {
            CurrencyCode = details.CurrencyCode,
            Value = orderTotal.Value,
            Breakdown = new()
            {
                ItemTotal = orderItemTotal,
                TaxTotal = orderTaxTotal,
                Shipping = orderShippingTotal,
                Discount = orderDiscount
            }
        };
    }

    /// <summary>
    /// Prepare order shipping details
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <param name="selectedOptionId">Selected shipping option</param>
    /// <param name="isApplePay">Apple Pay payment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping details
    /// </returns>
    private async Task<Shipping> PrepareShippingDetailsAsync(CartDetails details, string selectedOptionId, bool isApplePay = false)
    {
        if (!details.ShippingIsRequired)
            return null;

        var shippingAddress = details.ShippingAddress;
        var fullName = shippingAddress is not null && !details.IsPickup
            ? await _customerService.GetCustomerFullNameAsync(new() { FirstName = shippingAddress.FirstName, LastName = shippingAddress.LastName })
            : null;
        if (string.IsNullOrEmpty(fullName))
            fullName = await _customerService.GetCustomerFullNameAsync(details.Customer);

        //if the shipping option type is set to PICKUP, then the full name should start with S2S meaning ship to store (for example, S2S My Store)
        if (details.IsPickup && details.PickupPoint is not null)
            fullName = $"S2S {details.PickupPoint.Name}";

        var address = shippingAddress is not null ? new Address
        {
            AddressLine1 = CommonHelper.EnsureMaximumLength(shippingAddress.Address1, 300),
            AddressLine2 = CommonHelper.EnsureMaximumLength(shippingAddress.Address2, 300),
            AdminArea2 = CommonHelper.EnsureMaximumLength(shippingAddress.City, 120),
            AdminArea1 = CommonHelper.EnsureMaximumLength((await _stateProvinceService
                .GetStateProvinceByAddressAsync(shippingAddress))?.Abbreviation, 300),
            CountryCode = (await _countryService.GetCountryByIdAsync(shippingAddress.CountryId ?? 0))?.TwoLetterIsoCode,
            PostalCode = CommonHelper.EnsureMaximumLength(shippingAddress.ZipPostalCode, 60)
        } : null;

        var shipping = new Shipping
        {
            Name = new() { FullName = CommonHelper.EnsureMaximumLength(fullName, 300), },
            Address = address
        };

        if (details.Placement == ButtonPlacement.PaymentMethod && !isApplePay)
        {
            shipping.Type = details.IsPickup
                ? ShippingType.SHIPPING.ToString().ToUpper() //PICKUP_IN_STORE option doesn't work for some reason
                : ShippingType.SHIPPING.ToString().ToUpper();

            return shipping;
        }

        var (shippingOptions, pickupPoints) = await PrepareShippingOptionsAsync(details);
        if (!shippingOptions?.Any() ?? true)
            throw new NopException("No available shipping options");

        var selectedShippingOption = shippingOptions.FirstOrDefault();
        if (!string.IsNullOrEmpty(selectedOptionId))
        {
            var existingOption = shippingOptions
                .FirstOrDefault(option => string.Equals(option.Name, selectedOptionId, StringComparison.InvariantCultureIgnoreCase))
                ?? throw new NopException("Selected shipping option is unavailable");

            selectedShippingOption = existingOption;
        }

        if (selectedShippingOption is null)
            throw new NopException("Selected shipping option is unavailable");

        PickupPoint pickupPoint = null;
        if (selectedShippingOption.IsPickupInStore)
        {
            pickupPoint = await pickupPoints.FirstOrDefaultAwaitAsync(async point =>
                string.Equals(await GetShippingOptionNameAsync(new() { Name = point.Name, IsPickupInStore = true }), selectedShippingOption.Name) &&
                string.Equals(point.ProviderSystemName, selectedShippingOption.ShippingRateComputationMethodSystemName));

            details.IsPickup = true;
            details.PickupPoint = pickupPoint;

            //if the shipping option type is set to PICKUP, then the full name should start with S2S meaning ship to store (for example, S2S My Store)
            if (details.IsPickup && details.PickupPoint is not null)
                shipping.Name.FullName = $"S2S {details.PickupPoint.Name}";
        }
        details.ShippingOption = selectedShippingOption;

        //save selected options in attributes
        await _genericAttributeService
            .SaveAttributeAsync(details.Customer, NopCustomerDefaults.SelectedShippingOptionAttribute, selectedShippingOption, details.Store.Id);
        await _genericAttributeService
            .SaveAttributeAsync(details.Customer, NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, details.Store.Id);

        async Task<ShippingOption> convertOptionAsync(NopShippingOption option)
        {
            var (adjustedShippingRate, _) = await _orderTotalCalculationService
                .AdjustShippingRateAsync(option.Rate, details.Cart, option.IsPickupInStore);
            //var (rate, _) = await _taxService.GetShippingPriceAsync(adjustedShippingRate, details.Customer);
            //PayPal currently handles taxable shipping incorrectly, so we display shipping rates without tax, but it'll be included to tax total
            var rate = adjustedShippingRate;

            return new ShippingOption
            {
                Id = CommonHelper.EnsureMaximumLength(option.Name, 127),
                Label = CommonHelper.EnsureMaximumLength(option.Name, 127),
                Selected = false,
                Type = option.IsPickupInStore
                    ? ShippingType.PICKUP.ToString().ToUpper()
                    : ShippingType.SHIPPING.ToString().ToUpper(),
                Amount = PrepareMoney(rate, details.CurrencyCode)
            };
        }

        shipping.Options = details.Placement == ButtonPlacement.PaymentMethod
            ? [await convertOptionAsync(selectedShippingOption)]
            : await shippingOptions.SelectAwait(async option => await convertOptionAsync(option)).ToListAsync();

        //set default shipping option
        (shipping.Options
            .FirstOrDefault(option => string.Equals(option.Id, details.ShippingOption?.Name, StringComparison.InvariantCultureIgnoreCase))
            ?? shipping.Options.First())
            .Selected = true;

        return shipping;
    }

    /// <summary>
    /// Prepare available shipping options
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of shipping options; list of pickup points
    /// </returns>
    private async Task<(List<NopShippingOption> ShippingOptions, List<PickupPoint> PickupPoints)> PrepareShippingOptionsAsync(CartDetails details)
    {
        if (!details.ShippingIsRequired)
            return (null, null);

        if (details.ShippingAddress is null && !_shippingSettings.AllowPickupInStore)
            return (null, null);

        var shippingOptions = new List<NopShippingOption>();
        var pickupPoints = new List<PickupPoint>();

        //pickup points
        if (_shippingSettings.AllowPickupInStore)
        {
            var pickupPointProviders = await _pickupPluginManager.LoadActivePluginsAsync(details.Customer, details.Store.Id);
            if (pickupPointProviders.Any())
            {
                var pickupPointsResponse = await _shippingService
                    .GetPickupPointsAsync(details.Cart, details.BillingAddress, details.Customer, storeId: details.Store.Id);
                if (pickupPointsResponse.Success)
                {
                    shippingOptions.AddRange(await pickupPointsResponse.PickupPoints.SelectAwait(async point => new NopShippingOption
                    {
                        Name = await GetShippingOptionNameAsync(new() { Name = point.Name, IsPickupInStore = true }),
                        Rate = point.PickupFee,
                        Description = point.Description,
                        ShippingRateComputationMethodSystemName = point.ProviderSystemName,
                        IsPickupInStore = true,
                        DisplayOrder = point.DisplayOrder,
                        TransitDays = point.TransitDays
                    }).ToListAsync());
                    pickupPoints.AddRange(pickupPointsResponse.PickupPoints);
                }
            }
        }

        //and shipping options
        if (details.ShippingAddress is not null)
        {
            var shippingOptionResponse = await _shippingService
                .GetShippingOptionsAsync(details.Cart, details.ShippingAddress, details.Customer, storeId: details.Store.Id);
            if (shippingOptionResponse.Success)
                shippingOptions.AddRange(shippingOptionResponse.ShippingOptions);
        }

        //sort options
        shippingOptions = (_shippingSettings.ShippingSorting switch
        {
            ShippingSortingEnum.ShippingCost => shippingOptions.OrderBy(option => option.Rate),
            _ => shippingOptions.OrderBy(option => option.DisplayOrder)
        }).ToList();

        return (shippingOptions, pickupPoints);
    }

    /// <summary>
    /// Prepare the updated shipping details
    /// </summary>
    /// <param name="details">Shopping cart details</param>
    /// <param name="email">Customer email</param>
    /// <param name="selectedAddress">Selected shipping address</param>
    /// <param name="selectedOption">Selected shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping details
    /// </returns>
    private async Task<Shipping> PrepareUpdatedShippingAsync(CartDetails details, string email,
        (string City, string State, string Country, string PostalCode) selectedAddress,
        (string Id, string Type) selectedOption)
    {
        //change shipping address when customer selects another one
        if (!string.IsNullOrEmpty(selectedAddress.City) && !string.IsNullOrEmpty(selectedAddress.State) &&
            !string.IsNullOrEmpty(selectedAddress.Country) && !string.IsNullOrEmpty(selectedAddress.PostalCode))
        {
            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(selectedAddress.Country);
            var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(selectedAddress.State, country?.Id);
            var newShippingAddress = await PrepareCustomerAddressAsync(details.Customer, new()
            {
                Email = email ?? details.Customer.Email,
                City = selectedAddress.City,
                StateProvinceId = state?.Id,
                CountryId = country?.Id,
                ZipPostalCode = selectedAddress.PostalCode
            });
            if (newShippingAddress.Id != details.Customer.ShippingAddressId)
            {
                details.Customer.ShippingAddressId = newShippingAddress.Id;
                await _customerService.UpdateCustomerAsync(details.Customer);
            }
        }

        //change shipping option when customer selects another one
        if (string.IsNullOrEmpty(selectedOption.Id))
        {
            var shippingOption = await _genericAttributeService
                .GetAttributeAsync<NopShippingOption>(details.Customer, NopCustomerDefaults.SelectedShippingOptionAttribute, details.Store.Id);
            if (shippingOption is not null)
            {
                var type = shippingOption.IsPickupInStore ? ShippingType.PICKUP.ToString() : ShippingType.SHIPPING.ToString();
                selectedOption = (shippingOption.Name, type);
            }
        }

        //set new parameters to update shipping details
        details.BillingAddress = await _customerService.GetCustomerBillingAddressAsync(details.Customer);
        details.ShippingAddress = await _customerService.GetCustomerShippingAddressAsync(details.Customer);
        details.IsPickup =
            selectedOption.Type?.ToUpper() == ShippingType.PICKUP.ToString() ||
            selectedOption.Type?.ToUpper() == ShippingType.PICKUP_IN_STORE.ToString() ||
            selectedOption.Type?.ToUpper() == ShippingType.PICKUP_FROM_PERSON.ToString();
        if (details.ShippingAddress is null && !details.IsPickup)
            return null;

        var shipping = await PrepareShippingDetailsAsync(details, selectedOption.Id);

        return shipping;
    }

    /// <summary>
    /// Prepare patches to update an order
    /// </summary>
    /// <param name="purchaseUnit">Purchase unit details</param>
    /// <returns>List of patch objects</returns>
    private static List<Patch<object>> PreparePatches(PurchaseUnit purchaseUnit)
    {
        var patches = new List<Patch<object>>
        {
            new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/amount",
                Value = purchaseUnit.Amount
            },
            new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/items",
                Value = purchaseUnit.Items
            },
            new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/supplementary_data/card",
                Value = purchaseUnit.SupplementaryData.Card
            }
        };

        if (purchaseUnit.Shipping?.Name is not null)
        {
            patches.Add(new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/shipping/name",
                Value = purchaseUnit.Shipping.Name
            });
        }
        if (purchaseUnit.Shipping?.Address is not null)
        {
            patches.Add(new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/shipping/address",
                Value = purchaseUnit.Shipping.Address
            });
        }
        if (purchaseUnit.Shipping?.Options is not null)
        {
            patches.Add(new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/shipping/options",
                Value = purchaseUnit.Shipping.Options
            });
        }
        if (!string.IsNullOrEmpty(purchaseUnit.Shipping?.Type))
        {
            patches.Add(new()
            {
                Op = PatchOpType.REPLACE.ToString().ToLower(),
                Path = "/purchase_units/@reference_id=='default'/shipping/type",
                Value = purchaseUnit.Shipping.Type
            });
        }

        return patches;
    }

    /// <summary>
    /// Get customer's address (existing or a new one)
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="newAddress">Address to check</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address
    /// </returns>
    private async Task<NopAddress> PrepareCustomerAddressAsync(Customer customer, NopAddress newAddress)
    {
        var customerAddresses = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
        var query = customerAddresses.AsQueryable();
        if (!string.IsNullOrEmpty(newAddress.Email))
            query = query.Where(address => string.Equals(address.Email, newAddress.Email));
        if (!string.IsNullOrEmpty(newAddress.FirstName))
            query = query.Where(address => string.Equals(address.FirstName, newAddress.FirstName));
        if (!string.IsNullOrEmpty(newAddress.LastName))
            query = query.Where(address => string.Equals(address.LastName, newAddress.LastName));
        if (!string.IsNullOrEmpty(newAddress.Address1))
            query = query.Where(address => string.Equals(address.Address1, newAddress.Address1));
        if (!string.IsNullOrEmpty(newAddress.Address2))
            query = query.Where(address => string.Equals(address.Address2, newAddress.Address2));
        if (!string.IsNullOrEmpty(newAddress.City))
            query = query.Where(address => string.Equals(address.City, newAddress.City));
        if (newAddress.StateProvinceId > 0)
            query = query.Where(address => address.StateProvinceId == newAddress.StateProvinceId);
        if (newAddress.CountryId > 0)
            query = query.Where(address => address.CountryId == newAddress.CountryId);
        if (!string.IsNullOrEmpty(newAddress.ZipPostalCode))
            query = query.Where(address => string.Equals(address.ZipPostalCode, newAddress.ZipPostalCode));

        var existingAddress = query.FirstOrDefault();
        if (existingAddress is not null)
            return existingAddress;

        await _addressService.InsertAddressAsync(newAddress);
        await _customerService.InsertCustomerAddressAsync(customer, newAddress);

        return newAddress;
    }

    /// <summary>
    /// Get shipping option name
    /// </summary>
    /// <param name="option">Shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping option name
    /// </returns>
    private async Task<string> GetShippingOptionNameAsync(NopShippingOption option)
    {
        return option.IsPickupInStore
            ? (string.IsNullOrEmpty(option.Name)
            ? await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName")
            : string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), option.Name))
            : option.Name;
    }

    #endregion

    #region Payment tokens

    /// <summary>
    /// Prepare payment tokens with additional details
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="tokens">Payment tokens</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of payment tokens
    /// </returns>
    private async Task<List<PayPalToken>> PreparePaymentTokensAsync(PayPalCommerceSettings settings, IList<PayPalToken> tokens)
    {
        var paymentTokens = new List<PaymentToken>();

        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token.VaultCustomerId))
                continue;

            //try to get payment tokens from the vault
            var response = await _httpClient
                .RequestAsync<GetPaymentTokensRequest, GetPaymentTokensResponse>(new() { VaultCustomerId = token.VaultCustomerId }, settings);
            paymentTokens.AddRange(response?.PaymentTokens ?? new());
        }
        if (paymentTokens?.Any() != true)
            return new List<PayPalToken>();

        return tokens.OrderBy(token => token.IsPrimaryMethod ? 0 : 1).ThenBy(token => token.Id).Select(paymentToken =>
        {
            var existingToken = paymentTokens
                .FirstOrDefault(token => string.Equals(token.Id, paymentToken.VaultId, StringComparison.InvariantCultureIgnoreCase));
            if (existingToken is null)
                return null;

            //set title and expiration date
            paymentToken.Title = existingToken.PaymentSource?.Card is not null
                ? $"{existingToken.PaymentSource.Card.Brand} *{existingToken.PaymentSource.Card.LastDigits}"
                : (existingToken.PaymentSource?.Venmo is not null
                ? existingToken.PaymentSource.Venmo.UserName
                : (existingToken.PaymentSource?.PayPal is not null
                ? existingToken.PaymentSource.PayPal.EmailAddress
                : "N/A"));
            paymentToken.Expiration = existingToken.PaymentSource?.Card is not null ? existingToken.PaymentSource.Card.Expiry : "N/A";

            return paymentToken;
        }).Where(token => token is not null).ToList();
    }

    #endregion

    #region Common

    /// <summary>
    /// Get calculated SHA256 hash for the input string
    /// </summary>
    /// <param name="stringToHash">Input string for the hash</param>
    /// <returns>SHA256 hash</returns>
    private static string GetSha256Hash(string stringToHash)
    {
        return SHA256.HashData(Encoding.Default.GetBytes(stringToHash)).Aggregate(string.Empty, (current, next) => $"{current}{next:x2}");
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Convert object properties to a dictionary
    /// </summary>
    /// <param name="data">Object to convert</param>
    /// <returns>Dictionary of properties names and values</returns>
    public static Dictionary<string, string> ObjectToDictionary(object data)
    {
        return new Dictionary<string, string>(data.GetType().GetProperties().Select(property =>
        {
            var key = property
                ?.GetCustomAttributes(typeof(JsonPropertyAttribute), false).OfType<JsonPropertyAttribute>().FirstOrDefault()
                ?.PropertyName ?? string.Empty;
            var value = property.GetValue(data) is bool boolValue
                ? boolValue.ToString().ToLower()
                : property.GetValue(data)?.ToString();
            return new KeyValuePair<string, string>(key, value);
        }).Where(pair => !string.IsNullOrEmpty(pair.Key) && !string.IsNullOrEmpty(pair.Value)));
    }

    #region Configuration

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(PayPalCommerceSettings settings)
    {
        //client id and secret are required to request remote services
        return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings.SecretKey);
    }

    /// <summary>
    /// Check whether the plugin is configured and connected
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConnected(PayPalCommerceSettings settings)
    {
        //webhook is required to accept notifications
        return IsConfigured(settings) && !string.IsNullOrEmpty(settings.WebhookUrl);
    }

    /// <summary>
    /// Check whether the plugin is configured, connected and active
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; plugin instance
    /// </returns>
    public async Task<(bool Active, IPaymentMethod paymentMethod)> IsActiveAsync(PayPalCommerceSettings settings)
    {
        if (!IsConnected(settings))
            return (false, null);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var plugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(PayPalCommerceDefaults.SystemName, customer, store.Id);
        if (!_paymentPluginManager.IsPluginActive(plugin))
            return (false, plugin);

        return (true, plugin);
    }

    /// <summary>
    /// Get access token
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the access token; error message if exists
    /// </returns>
    public async Task<(AccessToken AccessToken, string Error)> GetAccessTokenAsync(PayPalCommerceSettings settings)
    {
        return await HandleFunctionAsync(async () =>
        {
            return await _httpClient.RequestAsync<GetAccessTokenRequest, GetAccessTokenResponse>(new()
            {
                ClientId = settings.ClientId,
                Secret = settings.SecretKey,
                GrantType = "client_credentials"
            }, settings);
        });
    }

    #endregion

    #region Components

    /// <summary>
    /// Prepare details to render payment buttons/messages
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="placement">Button placement</param>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the script details, customer details, messages details; cart details; error message if exists
    /// </returns>
    public async Task<(((string ScriptUrl, string ClientToken, string UserToken),
        (string Email, string Name),
        (string MessageConfig, string Amount),
        (bool? IsRecurring, bool IsShippable)),
        string Error)>
        PreparePaymentDetailsAsync(PayPalCommerceSettings settings, ButtonPlacement placement, int? productId)
    {
        return await HandleFunctionAsync(async () =>
        {
            //get the primary store currency
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            //customer details
            var customer = await _workContext.GetCurrentCustomerAsync();
            var isGuest = await _customerService.IsGuestAsync(customer);
            var address = await _customerService.GetCustomerBillingAddressAsync(customer);
            var email = address is not null ? address.Email : customer.Email;
            var fullName = address is not null
                ? await _customerService.GetCustomerFullNameAsync(new() { FirstName = address.FirstName, LastName = address.LastName })
                : await _customerService.GetCustomerFullNameAsync(customer);

            //prepare script components
            var components = new List<string>() { "buttons", "funding-eligibility" };
            if (placement == ButtonPlacement.PaymentMethod && settings.UseCardFields)
                components.Add("card-fields");
            if (placement == ButtonPlacement.PaymentMethod && settings.UseAlternativePayments)
                components.Add("payment-fields");
            if (settings.UseApplePay && placement != ButtonPlacement.Product)
                components.Add("applepay");
            if (settings.UseGooglePay)
                components.Add("googlepay");
            if (settings.UseSandbox || settings.ConfiguratorSupported)
                components.Add("messages");

            var script = new Script
            {
                ClientId = settings.ClientId,
                Currency = currencyCode.ToUpper(),
                Intent = settings.PaymentType.ToString().ToLower(),
                Commit = placement == ButtonPlacement.PaymentMethod && settings.SkipOrderConfirmPage,
                Components = string.Join(',', components),
                EnableFunding = settings.EnabledFunding,
                DisableFunding = settings.DisabledFunding,
                Vault = settings.UseVault && !isGuest,
                Debug = false,
                //BuyerCountry = null,    //PayPal auto detects this
                //Locale = null,          //PayPal auto detects this
                //IntegrationDate = null  //defaults to the date when client ID was created
            };

            var scriptUrl = QueryHelpers.AddQueryString(PayPalCommerceDefaults.ServiceScriptUrl, ObjectToDictionary(script));

            //client token is required for Advanced Credit and Debit Card Payments
            string clientToken = null;
            if (placement == ButtonPlacement.PaymentMethod && settings.UseCardFields)
            {
                var identityToken = await _httpClient.RequestAsync<CreateIdentityTokenRequest, CreateIdentityTokenResponse>(new()
                {
                    CustomerId = CommonHelper.EnsureMaximumLength(GetSha256Hash(customer.CustomerGuid.ToString()), 22)
                }, settings);
                clientToken = identityToken?.ClientToken;
            }

            //user ID token is required for Vault feature
            string userToken = null;
            if (settings.UseVault && !isGuest)
            {
                var tokens = await _tokenService.GetAllTokensAsync(settings.ClientId, customer.Id);
                var vaultCustomerId = tokens
                    .OrderBy(token => token.IsPrimaryMethod ? 0 : 1)
                    .ThenBy(token => token.Id)
                    .FirstOrDefault()
                    ?.VaultCustomerId;
                var accessToken = await _httpClient.RequestAsync<GetAccessTokenRequest, GetAccessTokenResponse>(new()
                {
                    ClientId = settings.ClientId,
                    Secret = settings.SecretKey,
                    GrantType = "client_credentials",
                    ResponseType = "id_token",
                    TargetCustomerId = vaultCustomerId
                }, settings);
                userToken = accessToken?.UserIdToken;
            }

            //Pay Later details
            var amount = await PrepareMessagesAmountAsync(placement, customer, currencyCode, productId);
            var payLaterConfig = new
            {
                cart = new MessageConfiguration(),
                product = new MessageConfiguration(),
                checkout = new MessageConfiguration()
            };
            payLaterConfig = JsonConvert.DeserializeAnonymousType(settings.PayLaterConfig ?? string.Empty, payLaterConfig);
            var config = placement switch
            {
                ButtonPlacement.Cart => payLaterConfig?.cart,
                ButtonPlacement.Product => payLaterConfig?.product,
                ButtonPlacement.PaymentMethod => payLaterConfig?.checkout,
                _ => null
            };
            var messageConfig = !string.IsNullOrEmpty(config?.Status) ? JsonConvert.SerializeObject(config, Formatting.Indented) : "{}";

            //cart details
            var (isRecurring, _) = await CheckShoppingCartIsRecurringAsync(placement, productId);
            var (isShippable, _) = await CheckShippingIsRequiredAsync(productId);

            return ((scriptUrl, clientToken, userToken), (email, fullName), (messageConfig, amount), (isRecurring, isShippable));
        });
    }

    /// <summary>
    /// Prepare details to render Pay Later messages
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the message configuration, amount value, currency code; error message if exists
    /// </returns>
    public async Task<((string Config, string Amount, string CurrencyCode), string Error)>
        PrepareMessagesAsync(PayPalCommerceSettings settings, ButtonPlacement placement)
    {
        return await HandleFunctionAsync(async () =>
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var amount = await PrepareMessagesAmountAsync(placement, customer, currencyCode, null);

            var payLaterConfig = new
            {
                cart = new MessageConfiguration(),
                product = new MessageConfiguration(),
                checkout = new MessageConfiguration()
            };
            payLaterConfig = JsonConvert.DeserializeAnonymousType(settings.PayLaterConfig ?? string.Empty, payLaterConfig);
            var config = placement switch
            {
                ButtonPlacement.Cart => payLaterConfig?.cart,
                ButtonPlacement.Product => payLaterConfig?.product,
                ButtonPlacement.PaymentMethod => payLaterConfig?.checkout,
                _ => null
            };
            var messageConfig = !string.IsNullOrEmpty(config?.Status) ? JsonConvert.SerializeObject(config, Formatting.Indented) : "{}";

            return (messageConfig, amount, currencyCode);
        });
    }

    #endregion

    #region Checkout

    /// <summary>
    /// Check whether the checkout is enabled for the customer
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check results; shopping cart
    /// </returns>
    public async Task<(bool IsEnabled, bool LoginIsRequired, IList<ShoppingCartItem> Cart)> CheckoutIsEnabledAsync()
    {
        return (await HandleFunctionAsync(async () =>
        {
            if (_orderSettings.CheckoutDisabled)
                return (false, false, null);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                return (false, false, null);

            if (await _customerService.IsGuestAsync(customer))
            {
                if (!_orderSettings.AnonymousCheckoutAllowed)
                    return (true, true, cart);

                var downloadableProductsRequireRegistration = _customerSettings.RequireRegistrationForDownloadableProducts &&
                    await _productService.HasAnyDownloadableProductAsync(cart.Select(item => item.ProductId).ToArray());
                if (downloadableProductsRequireRegistration)
                    return (true, true, cart);
            }

            return (true, false, cart);
        }, false)).Result;
    }

    /// <summary>
    /// Check whether the shopping cart is valid
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validation warnings; error message if exists
    /// </returns>
    public async Task<(IList<string> Warnings, string Error)> ValidateShoppingCartAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            await _customerService.ResetCheckoutDataAsync(customer, store.Id, clearShippingMethod: false);

            var checkoutAttributesXml = await _genericAttributeService
                .GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
            var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, true);
            if (cartWarnings.Any())
                return cartWarnings;

            foreach (var item in cart)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                var itemWarnings = await _shoppingCartService
                    .GetShoppingCartItemWarningsAsync(customer, item.ShoppingCartType, product, item.StoreId, item.AttributesXml,
                    item.CustomerEnteredPrice, item.RentalStartDateUtc, item.RentalEndDateUtc, item.Quantity, false, item.Id);
                if (itemWarnings.Any())
                    return itemWarnings;
            }

            return null;
        });
    }

    /// <summary>
    /// Check whether the shipping is required for the current cart/product
    /// </summary>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; error message if exists
    /// </returns>
    public async Task<(bool ShippingIsRequired, string Error)> CheckShippingIsRequiredAsync(int? productId)
    {
        return await HandleFunctionAsync(async () =>
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);

            if (!shippingIsRequired && await _productService.GetProductByIdAsync(productId ?? 0) is Product product)
                shippingIsRequired = product.IsShipEnabled;

            return shippingIsRequired;
        }, false);
    }

    /// <summary>
    /// Check whether the current cart/product is recurring
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="productId">Product id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check result; error message if exists
    /// </returns>
    public async Task<(bool? IsRecurring, string Error)> CheckShoppingCartIsRecurringAsync(ButtonPlacement placement, int? productId = null)
    {
        return await HandleFunctionAsync(async () =>
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            var isRecurring = await _shoppingCartService.ShoppingCartIsRecurringAsync(cart);

            if (!isRecurring && await _productService.GetProductByIdAsync(productId ?? 0) is Product product)
                isRecurring = product.IsRecurring;

            //recurring payments not yet supported
            if (isRecurring)
                return (bool?)null;

            return isRecurring;
        }, false);
    }

    #endregion

    #region Orders

    /// <summary>
    /// Get the order by id
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="orderId">Order id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order; error message if exists
    /// </returns>
    public async Task<(Order Order, string Error)> GetOrderAsync(PayPalCommerceSettings settings, string orderId)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var paymentRequest = await _actionContextAccessor.ActionContext.HttpContext.Session
                .GetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey)
                ?? throw new NopException("Order payment info not found");

            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (!paymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderIdValue) ||
                !string.Equals(orderIdValue.ToString(), orderId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var placementKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement");
            if (!paymentRequest.CustomValues.TryGetValue(placementKey, out var placementValue) ||
                !Enum.TryParse<ButtonPlacement>(placementValue.ToString(), out var placement))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var order = await _httpClient.RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderId }, settings);

            return order;
        });
    }

    /// <summary>
    /// Get a previously created order if exists
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="paymentRequest">Payment request</param>
    /// <param name="placement">Button placement</param>
    /// <param name="shippingIsRequired">Whether the shipping is required (used for validation)</param>
    /// <param name="paymentSource">Payment source</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the created order; error message if exists
    /// </returns>
    public async Task<(Order Order, string Error)> GetCreatedOrderAsync(PayPalCommerceSettings settings,
        ProcessPaymentRequest paymentRequest, ButtonPlacement placement, bool shippingIsRequired, string paymentSource)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (paymentRequest is null)
                return null;

            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (!paymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderIdValue) || string.IsNullOrEmpty(orderIdValue.ToString()))
                return null;

            var placementKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement");
            if (!paymentRequest.CustomValues.TryGetValue(placementKey, out var placementValue) ||
                !Enum.TryParse<ButtonPlacement>(placementValue.ToString(), out var previousPlacement) ||
                previousPlacement != placement)
            {
                return null;
            }

            var order = await _httpClient
                .RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderIdValue.ToString() }, settings);

            //we cannot use completed order
            if (order.Status?.ToUpper() != OrderStatusType.CREATED.ToString() &&
                order.Status?.ToUpper() != OrderStatusType.PAYER_ACTION_REQUIRED.ToString() &&
                order.Status?.ToUpper() != OrderStatusType.APPROVED.ToString())
            {
                return null;
            }

            //check validity interval
            if (!DateTime.TryParse(order.CreateTime, out var createTime) ||
                (DateTime.UtcNow - createTime.ToUniversalTime()).TotalSeconds > settings.OrderValidityInterval)
            {
                return null;
            }

            if (order.PurchaseUnits.FirstOrDefault() is not PurchaseUnit unit || (unit.Shipping is null != !shippingIsRequired))
                return null;

            //payment sources must match
            if (string.Equals(paymentSource, nameof(PaymentSource.PayPal), StringComparison.InvariantCultureIgnoreCase) &&
                order.PaymentSource?.PayPal is null)
            {
                return null;
            }

            if (string.Equals(paymentSource, nameof(PaymentSource.Card), StringComparison.InvariantCultureIgnoreCase) &&
                order.PaymentSource?.Card is null)
            {
                return null;
            }

            return order;
        }, false);
    }

    /// <summary>
    /// Create an order
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="placement">Button placement</param>
    /// <param name="paymentSource">Payment source</param>
    /// <param name="cardId">Saved card id</param>
    /// <param name="saveCard">Whether to save card payment token</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the created order; error message if exists
    /// </returns>
    public async Task<(Order Order, string Error)> CreateOrderAsync(PayPalCommerceSettings settings,
        ButtonPlacement placement, string paymentSource, int? cardId, bool saveCard)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            if (string.IsNullOrEmpty(settings.MerchantId))
                throw new NopException("Merchant PayPal ID not set");

            //get the primary store currency
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var billingAddress = await _addressService.GetAddressByIdAsync(customer.BillingAddressId ?? 0);
            if (placement == ButtonPlacement.PaymentMethod && billingAddress is null)
                throw new NopException("Customer billing address not set");

            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var shippingOption = await _genericAttributeService
                .GetAttributeAsync<NopShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
            var pickupPoint = await _genericAttributeService
                .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            var pickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint is not null;
            var shippingAddress = pickupInStore ? new NopAddress
            {
                Address1 = pickupPoint.Address,
                City = pickupPoint.City,
                County = pickupPoint.County,
                CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode))?.Id,
                StateProvinceId = (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation,
                    (await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode))?.Id))?.Id,
                ZipPostalCode = pickupPoint.ZipPostalCode,
                CreatedOnUtc = DateTime.UtcNow
            } : await _addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);
            if (placement == ButtonPlacement.PaymentMethod && shippingIsRequired && shippingAddress is null)
                throw new NopException("Customer shipping address not set");

            var savedPaymentToken = await _tokenService.GetByIdAsync(cardId ?? 0);
            if (savedPaymentToken is not null && savedPaymentToken.CustomerId != customer.Id)
                throw new NopException("Card details not found");

            if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
                throw new NopException("Recurring payment not supported");

            var paymentRequest = await _actionContextAccessor.ActionContext.HttpContext.Session
                .GetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey);
            var (order, _) = await GetCreatedOrderAsync(settings, paymentRequest, placement, shippingIsRequired, paymentSource);
            if (paymentRequest is null || order is null)
            {
                paymentRequest = new();
                await _paymentService.GenerateOrderGuidAsync(paymentRequest);
            }
            var orderGuid = paymentRequest.OrderGuid.ToString();

            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode,
                BillingAddress = billingAddress,
                ShippingAddress = shippingAddress,
                ShippingIsRequired = shippingIsRequired,
                IsPickup = pickupInStore,
                ShippingOption = shippingOption,
                PickupPoint = pickupPoint
            };

            //prepare purchase unit
            var shipping = await PrepareShippingDetailsAsync(details, shippingOption?.Name);
            var items = await PrepareOrderItemsAsync(details);
            var orderAmount = await PrepareOrderMoneyAsync(details, items);
            var cardData = new CardData
            {
                Level2 = new() { InvoiceId = CommonHelper.EnsureMaximumLength(orderGuid, 127) },
                Level3 = new()
                {
                    LineItems = items,
                    ShippingAmount = orderAmount.Breakdown.Shipping,
                    ShippingAddress = shipping?.Address,
                    ShipsFromPostalCode = CommonHelper.EnsureMaximumLength((await _addressService
                        .GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId))?.ZipPostalCode, 60)
                },
            };
            var purchaseUnit = new PurchaseUnit
            {
                CustomId = CommonHelper.EnsureMaximumLength(orderGuid, 127),
                InvoiceId = CommonHelper.EnsureMaximumLength(orderGuid, 127),
                Description = CommonHelper.EnsureMaximumLength($"Purchase at '{store.Name}'", 127),
                SoftDescriptor = CommonHelper.EnsureMaximumLength(store.Name, 22),
                Payee = new() { MerchantId = settings.MerchantId },
                Items = items,
                Amount = orderAmount,
                Shipping = shipping,
                SupplementaryData = new() { Card = cardData }
            };

            //whether we should create a new order
            if (order is null)
            {
                var isCard = string.Equals(paymentSource, nameof(PaymentSource.Card), StringComparison.InvariantCultureIgnoreCase);
                var isVenmo = string.Equals(paymentSource, nameof(PaymentSource.Venmo), StringComparison.InvariantCultureIgnoreCase);
                var isApplepay = string.Equals(paymentSource, nameof(PaymentSource.ApplePay), StringComparison.InvariantCultureIgnoreCase);

                var context = PrepareOrderContext(settings, details, orderGuid, isApplepay);
                var payer = await PrepareBillingDetailsAsync(details);

                //only registered customers can save payment tokens
                var isGuest = await _customerService.IsGuestAsync(customer);
                var vault = !settings.UseVault || isGuest ? null : new VaultInstruction
                {
                    UsageType = VaultUsageType.MERCHANT.ToString().ToUpper(),
                    CustomerType = VaultUsageType.CONSUMER.ToString().ToUpper(),
                    StoreInVault = VaultInstructionType.ON_SUCCESS.ToString().ToUpper(),
                    PermitMultiplePaymentTokens = false
                };
                if (vault is not null)
                {
                    payer.Id = (await _tokenService.GetAllTokensAsync(settings.ClientId, customer.Id))
                        .OrderBy(token => token.IsPrimaryMethod ? 0 : 1)
                        .ThenBy(token => token.Type == nameof(PaymentSource.Card) ? 0 : 1)
                        .ThenBy(token => token.Id)
                        .FirstOrDefault()
                        ?.VaultCustomerId;
                }

                //set payment source
                var paymentSourceDetails = new PaymentSource();
                if (isCard)
                {
                    paymentSourceDetails.Card = new()
                    {
                        ExperienceContext = context,
                        BillingAddress = !string.IsNullOrEmpty(savedPaymentToken?.VaultId) ? null : payer.Address,
                        VaultId = savedPaymentToken?.VaultId,
                        Attributes = vault is null || !saveCard || !string.IsNullOrEmpty(savedPaymentToken?.VaultId) ? null : new()
                        {
                            Vault = vault,
                            Customer = payer
                        }
                    };

                    if (vault is not null && (saveCard || !string.IsNullOrEmpty(savedPaymentToken?.VaultId)))
                    {
                        paymentSourceDetails.Card.StoredCredential = new()
                        {
                            PaymentInitiator = PaymentInitiatorType.CUSTOMER.ToString().ToUpper(),
                            PaymentType = Api.Models.Enums.PaymentType.ONE_TIME.ToString().ToUpper(),
                            Usage = saveCard
                                ? StoredPaymentUsageType.FIRST.ToString().ToUpper()
                                : StoredPaymentUsageType.SUBSEQUENT.ToString().ToUpper()
                        };
                    }

                    if (placement == ButtonPlacement.PaymentMethod && settings.UseCardFields)
                    {
                        paymentSourceDetails.Card.Attributes = new()
                        {
                            Vault = vault is not null && saveCard && string.IsNullOrEmpty(savedPaymentToken?.VaultId) ? vault : null,
                            Customer = vault is not null && saveCard && string.IsNullOrEmpty(savedPaymentToken?.VaultId) ? payer : null,
                            Verification = new()
                            {
                                Method = settings.CustomerAuthenticationRequired
                                    ? VerificationInstructionMethodType.SCA_ALWAYS.ToString().ToUpper()
                                    : VerificationInstructionMethodType.SCA_WHEN_REQUIRED.ToString().ToUpper()
                            }
                        };
                    }
                }
                else if (isVenmo)
                {
                    paymentSourceDetails.Venmo = new()
                    {
                        ExperienceContext = context,
                        EmailAddress = payer.EmailAddress,
                        Attributes = vault is not null ? new() { Vault = vault, Customer = payer } : null
                    };
                }
                else
                {
                    paymentSourceDetails.PayPal = new()
                    {
                        ExperienceContext = context,
                        EmailAddress = payer.EmailAddress,
                        Name = payer.Name,
                        BirthDate = payer.BirthDate,
                        Address = payer.Address,
                        Attributes = vault is not null ? new() { Vault = vault, Customer = payer } : null
                    };
                }

                order = await _httpClient.RequestAsync<CreateOrderRequest, CreateOrderResponse>(new CreateOrderRequest
                {
                    Intent = settings.PaymentType.ToString().ToUpper(),
                    PaymentSource = paymentSourceDetails,
                    PurchaseUnits = [purchaseUnit]
                }, settings);
            }
            else
            {
                //order exists, so just update some details
                var patches = PreparePatches(purchaseUnit);
                patches.Add(new()
                {
                    Op = PatchOpType.REPLACE.ToString().ToLower(),
                    Path = "/intent",
                    Value = settings.PaymentType.ToString().ToUpper()
                });
                var updateRequest = new UpdateOrderRequest<object>(patches) { OrderId = order.Id };
                await _httpClient.RequestAsync<UpdateOrderRequest<object>, EmptyResponse>(updateRequest, settings);
            }

            //save order details for future using as the payment request
            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            paymentRequest.CustomValues[orderIdKey] = order.Id;
            var placementKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement");
            paymentRequest.CustomValues[placementKey] = placement.ToString();
            await _actionContextAccessor.ActionContext.HttpContext.Session
                .SetAsync(PayPalCommerceDefaults.PaymentRequestSessionKey, paymentRequest);

            return order;
        });
    }

    /// <summary>
    /// Update order shipping details
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="orderId">Order id</param>
    /// <param name="selectedAddress">Selected shipping address</param>
    /// <param name="selectedOption">Selected shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result of update; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> UpdateOrderShippingAsync(PayPalCommerceSettings settings, string orderId,
        (string City, string State, string Country, string PostalCode) selectedAddress,
        (string Id, string Type) selectedOption)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var paymentRequest = await _actionContextAccessor.ActionContext.HttpContext.Session
                .GetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey)
                ?? throw new NopException("Order payment info not found");

            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (!paymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderIdValue) ||
                (!string.IsNullOrEmpty(orderId) && !string.Equals(orderIdValue.ToString(), orderId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var placementKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement");
            if (!paymentRequest.CustomValues.TryGetValue(placementKey, out var placementValue) ||
                !Enum.TryParse<ButtonPlacement>(placementValue.ToString(), out var placement))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            //changing shipping address or option on the payment method page is not available
            if (placement == ButtonPlacement.PaymentMethod)
                return false;

            //check the order status
            var order = await _httpClient
                .RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderIdValue.ToString() }, settings);
            if (order.Status?.ToUpper() != OrderStatusType.CREATED.ToString() &&
                order.Status?.ToUpper() != OrderStatusType.PAYER_ACTION_REQUIRED.ToString() &&
                order.Status?.ToUpper() != OrderStatusType.APPROVED.ToString())
            {
                throw new NopException($"Order is in '{order.Status}' status");
            }

            if (order.PurchaseUnits.FirstOrDefault() is not PurchaseUnit unit ||
                !string.Equals(unit.CustomId, paymentRequest.OrderGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            if (!shippingIsRequired)
                return false;

            //update shipping details
            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode,
                ShippingIsRequired = shippingIsRequired
            };
            var shipping = await PrepareUpdatedShippingAsync(details, order.Payer?.EmailAddress, selectedAddress, selectedOption);
            if (shipping is null)
                return false;

            //recalculate the total and update the items, since the shipping price may have changed
            var items = await PrepareOrderItemsAsync(details);
            var orderAmount = await PrepareOrderMoneyAsync(details, items);
            var cardData = new CardData
            {
                Level2 = new() { InvoiceId = CommonHelper.EnsureMaximumLength(paymentRequest.OrderGuid.ToString(), 127) },
                Level3 = new()
                {
                    LineItems = items,
                    ShippingAmount = orderAmount.Breakdown.Shipping,
                    ShippingAddress = shipping?.Address,
                    ShipsFromPostalCode = CommonHelper.EnsureMaximumLength((await _addressService
                        .GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId))?.ZipPostalCode, 60)
                },
            };

            var patches = PreparePatches(new()
            {
                Shipping = shipping,
                Items = items,
                Amount = orderAmount,
                SupplementaryData = new() { Card = cardData }
            });
            var updateRequest = new UpdateOrderRequest<object>(patches) { OrderId = order.Id };
            await _httpClient.RequestAsync<UpdateOrderRequest<object>, EmptyResponse>(updateRequest, settings);

            return true;
        });
    }

    /// <summary>
    /// The method is called after the customer approves the transaction
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="orderId">Order id</param>
    /// <param name="orderGuid">Internal order id</param>
    /// <param name="liabilityShift">Liability shift</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order; whether to process the payment immediately; error message if exists
    /// </returns>
    public async Task<((Order Order, bool PayNow), string Error)>
        OrderIsApprovedAsync(PayPalCommerceSettings settings, string orderId, string orderGuid, string liabilityShift)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var paymentRequest = await _actionContextAccessor.ActionContext.HttpContext.Session
                .GetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey)
                ?? throw new NopException("Order payment info not found");

            if (!string.IsNullOrEmpty(orderGuid) &&
                !string.Equals(orderGuid, paymentRequest.OrderGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (!paymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderIdValue) ||
                (!string.IsNullOrEmpty(orderId) && !string.Equals(orderIdValue.ToString(), orderId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            var placementKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement");
            if (!paymentRequest.CustomValues.TryGetValue(placementKey, out var placementValue) ||
                !Enum.TryParse<ButtonPlacement>(placementValue.ToString(), out var placement))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            //check the order status
            var order = await _httpClient
                .RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderIdValue.ToString() }, settings);
            if (order.Status?.ToUpper() != OrderStatusType.APPROVED.ToString() && order.Status?.ToUpper() != OrderStatusType.COMPLETED.ToString())
            {
                if (order.Status?.ToUpper() == OrderStatusType.CREATED.ToString())
                {
                    if (liabilityShift?.ToUpper() == LiabilityShiftType.NO.ToString())
                        throw new NopException($"3D Secure contingency is not resolved");

                    if (liabilityShift?.ToUpper() == LiabilityShiftType.UNKNOWN.ToString())
                        throw new NopException($"The authentication system isn't available, please retry later");
                }
                else
                    throw new NopException($"Order is in '{order.Status}' status");
            }

            if (order.PurchaseUnits.FirstOrDefault() is not PurchaseUnit unit ||
                !string.Equals(unit.CustomId, paymentRequest.OrderGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            await _genericAttributeService
                .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, PayPalCommerceDefaults.SystemName, store.Id);

            //place order immediately, once order is completed
            if (order.Status.ToUpper() == OrderStatusType.COMPLETED.ToString())
                return (order, true);

            var pickupPoint = await _genericAttributeService
                .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            var pickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint is not null;
            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode,
                IsPickup = pickupInStore,
                PickupPoint = pickupPoint
            };

            //recalculate the total and update the items, since the amounts may have changed
            var items = await PrepareOrderItemsAsync(details);
            var orderAmount = await PrepareOrderMoneyAsync(details, items);
            var cardData = new CardData
            {
                Level2 = new() { InvoiceId = CommonHelper.EnsureMaximumLength(paymentRequest.OrderGuid.ToString(), 127) },
                Level3 = new()
                {
                    LineItems = items,
                    ShippingAmount = orderAmount.Breakdown.Shipping,
                    ShippingAddress = unit.Shipping?.Address,
                    ShipsFromPostalCode = CommonHelper.EnsureMaximumLength((await _addressService
                        .GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId))?.ZipPostalCode, 60)
                },
            };

            var patches = PreparePatches(new()
            {
                //if the shipping option type is set to PICKUP, then the full name should start with S2S meaning ship to store (for example, S2S My Store)
                Shipping = details.IsPickup && details.PickupPoint is not null
                    ? new Shipping { Name = new() { FullName = $"S2S {details.PickupPoint.Name}" } }
                    : null,
                Items = items,
                Amount = orderAmount,
                SupplementaryData = new() { Card = cardData }
            });
            var updateRequest = new UpdateOrderRequest<object>(patches) { OrderId = order.Id };
            await _httpClient.RequestAsync<UpdateOrderRequest<object>, EmptyResponse>(updateRequest, settings);

            //place order immediately, if the appropriate setting is enabled
            if (placement == ButtonPlacement.PaymentMethod)
                return (order, settings.SkipOrderConfirmPage);

            //or update billing details and redirect customer to the confirmation page
            if (order.Payer is not null)
            {
                var billingCountry = await _countryService.GetCountryByTwoLetterIsoCodeAsync(order.Payer.Address?.CountryCode);
                var billingState = await _stateProvinceService
                    .GetStateProvinceByAbbreviationAsync(order.Payer.Address?.AdminArea1, billingCountry?.Id);
                var billingAddress = await PrepareCustomerAddressAsync(customer, new()
                {
                    Email = order.Payer.EmailAddress ?? customer.Email,
                    FirstName = order.Payer.Name?.GivenName ?? customer.FirstName,
                    LastName = order.Payer.Name?.Surname ?? customer.LastName,
                    Address1 = order.Payer.Address?.AddressLine1,
                    Address2 = order.Payer.Address?.AddressLine2,
                    City = order.Payer.Address?.AdminArea2,
                    ZipPostalCode = order.Payer.Address?.PostalCode,
                    StateProvinceId = billingState?.Id,
                    CountryId = billingCountry?.Id
                });
                if (billingAddress.Id != customer.BillingAddressId)
                    customer.BillingAddressId = billingAddress.Id;

                if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart) &&
                    await _genericAttributeService.GetAttributeAsync<NopShippingOption>(customer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id) is NopShippingOption shippingOption &&
                    !shippingOption.IsPickupInStore &&
                    order.PurchaseUnits.FirstOrDefault()?.Shipping is Shipping shipping &&
                    shipping.Address is Address shippingAddress)
                {
                    var shippingCountry = await _countryService.GetCountryByTwoLetterIsoCodeAsync(shippingAddress.CountryCode);
                    var shippingState = await _stateProvinceService
                        .GetStateProvinceByAbbreviationAsync(shippingAddress.AdminArea1, shippingCountry?.Id);
                    var newShippingAddress = await PrepareCustomerAddressAsync(customer, new()
                    {
                        Email = order.Payer.EmailAddress ?? customer.Email,
                        Address1 = shippingAddress.AddressLine1,
                        Address2 = shippingAddress.AddressLine2,
                        City = shippingAddress.AdminArea2,
                        ZipPostalCode = shippingAddress.PostalCode,
                        StateProvinceId = shippingState?.Id,
                        CountryId = shippingCountry?.Id
                    });
                    if (newShippingAddress.Id != customer.ShippingAddressId)
                        customer.ShippingAddressId = newShippingAddress.Id;
                }

                await _customerService.UpdateCustomerAsync(customer);
            }

            return (order, false);
        });
    }

    /// <summary>
    /// Place an order
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="orderId">Order id</param>
    /// <param name="liabilityShift">Liability shift</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the placed order; created order; error message if exists
    /// </returns>
    public async Task<((NopOrder NopOrder, Order Order), string Error)>
        PlaceOrderAsync(PayPalCommerceSettings settings, string orderId, string liabilityShift)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var paymentRequest = await _actionContextAccessor.ActionContext.HttpContext.Session
                .GetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey);
            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (paymentRequest is null ||
                !paymentRequest.CustomValues.TryGetValue(orderIdKey, out var orderIdValue) ||
                !string.Equals(orderIdValue.ToString(), orderId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get the PayPal order ID");
            }

            //check the order status
            var order = await _httpClient
                .RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderId }, settings) as Order;
            if (order.Status?.ToUpper() != OrderStatusType.APPROVED.ToString() && order.Status?.ToUpper() != OrderStatusType.COMPLETED.ToString())
            {
                if (order.Status?.ToUpper() == OrderStatusType.CREATED.ToString())
                {
                    if (liabilityShift?.ToUpper() == LiabilityShiftType.NO.ToString())
                        throw new NopException($"3D Secure contingency is not resolved");

                    if (liabilityShift?.ToUpper() == LiabilityShiftType.UNKNOWN.ToString())
                        throw new NopException($"The authentication system isn't available, please retry later");
                }
                else
                    throw new NopException($"Order is in '{order.Status}' status");
            }

            if (order.PurchaseUnits.FirstOrDefault() is not PurchaseUnit unit ||
                !string.Equals(unit.CustomId, paymentRequest.OrderGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NopException("Failed to get PayPal order info");
            }

            //totals must match
            var (cartTotal, _, _, _, _, _) = await _orderTotalCalculationService
                .GetShoppingCartTotalAsync(cart, usePaymentMethodAdditionalFee: false);
            var difference = Math.Abs(ConvertMoney(unit.Amount) - Math.Round(cartTotal ?? decimal.Zero, 2));
            if (difference > decimal.Zero)
                throw new NopException($"Shopping cart total and approved order amount differ by {difference}");

            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval > 0)
            {
                var lastOrder = (await _orderService.SearchOrdersAsync(storeId: store.Id, customerId: customer.Id, pageSize: 1)).FirstOrDefault();
                if (lastOrder is not null && (DateTime.UtcNow - lastOrder.CreatedOnUtc).TotalMinutes < _orderSettings.MinimumOrderPlacementInterval)
                    throw new NopException(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));
            }

            paymentRequest.StoreId = store.Id;
            paymentRequest.CustomerId = customer.Id;
            paymentRequest.PaymentMethodSystemName = PayPalCommerceDefaults.SystemName;
            paymentRequest.CustomValues.Remove(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Placement"));

            //try to place an order
            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(paymentRequest);
            if (placeOrderResult?.Success != true || placeOrderResult.PlacedOrder is null)
                throw new NopException(string.Join(',', placeOrderResult?.Errors ?? new List<string>()));

            //clear payment request
            await _actionContextAccessor.ActionContext.HttpContext.Session
                .SetAsync<ProcessPaymentRequest>(PayPalCommerceDefaults.PaymentRequestSessionKey, null);

            return (placeOrderResult.PlacedOrder, order);
        });
    }

    /// <summary>
    /// Confirm the placed order
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="nopOrder">Placed order</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the confirmed order; error message if exists
    /// </returns>
    public async Task<(Order Order, string Error)> ConfirmOrderAsync(PayPalCommerceSettings settings, NopOrder nopOrder, Order order)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            //authorize or capture previously created order if not yet completed
            if (order.Status?.ToUpper() != OrderStatusType.COMPLETED.ToString())
            {
                //update invoice id
                var patch = new Patch<object>
                {
                    Op = PatchOpType.REPLACE.ToString().ToLower(),
                    Path = "/purchase_units/@reference_id=='default'/invoice_id",
                    Value = nopOrder.CustomOrderNumber
                };
                var updateRequest = new UpdateOrderRequest<object>([patch]) { OrderId = order.Id };
                await _httpClient.RequestAsync<UpdateOrderRequest<object>, EmptyResponse>(updateRequest, settings);

                order = settings.PaymentType switch
                {
                    Domain.PaymentType.Authorize => await _httpClient.RequestAsync<CreateAuthorizationRequest, CreateAuthorizationResponse>
                        (new CreateAuthorizationRequest { OrderId = order.Id }, settings),
                    Domain.PaymentType.Capture => await _httpClient.RequestAsync<Api.Orders.CreateCaptureRequest, Api.Orders.CreateCaptureResponse>
                        (new Api.Orders.CreateCaptureRequest { OrderId = order.Id }, settings),
                    _ => null
                };
            }

            //check the authorization object or the capture object
            var purchaseUnit = order.PurchaseUnits.FirstOrDefault();
            var authorization = purchaseUnit.Payments?.Authorizations?.FirstOrDefault();
            if (authorization is not null)
            {
                if (authorization.Status?.ToUpper() == AuthorizationStatusType.DENIED.ToString())
                    throw new NopException("Cannot authorize funds for this authorized payment");

                if (authorization.Status?.ToUpper() == AuthorizationStatusType.PENDING.ToString())
                {
                    await _orderService.InsertOrderNoteAsync(new()
                    {
                        OrderId = nopOrder.Id,
                        Note = $"Authorization is in {authorization.Status} status due to {authorization.StatusDetails?.Reason}",
                        DisplayToCustomer = true,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    if (settings.PaymentType == Domain.PaymentType.Authorize && settings.ImmediatePaymentRequired)
                        throw new NopException($"Immediate payment required but authorization is {authorization.Status}");
                }

                if (authorization.Status?.ToUpper() == AuthorizationStatusType.CREATED.ToString())
                {
                    if (_orderProcessingService.CanMarkOrderAsAuthorized(nopOrder))
                    {
                        nopOrder.AuthorizationTransactionId = authorization.Id;
                        nopOrder.AuthorizationTransactionResult = authorization.Status;
                        nopOrder.AuthorizationTransactionCode = authorization.ProcessorResponse?.ResponseCode;
                        await _orderProcessingService.MarkAsAuthorizedAsync(nopOrder);
                    }
                }
            }

            var capture = purchaseUnit.Payments?.Captures?.FirstOrDefault();
            if (capture is not null)
            {
                if (capture.Status?.ToUpper() == CaptureStatusType.DECLINED.ToString())
                    throw new NopException("The funds could not be captured");

                if (capture.Status?.ToUpper() == CaptureStatusType.FAILED.ToString())
                    throw new NopException("There was an error while capturing payment");

                if (capture.Status?.ToUpper() == CaptureStatusType.PENDING.ToString())
                {
                    await _orderService.InsertOrderNoteAsync(new()
                    {
                        OrderId = nopOrder.Id,
                        Note = $"Capture is in {capture.Status} status due to {capture.StatusDetails?.Reason}",
                        DisplayToCustomer = true,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    if (settings.ImmediatePaymentRequired)
                        throw new NopException($"Immediate payment required but capture is {capture.Status}");
                }

                if (capture.Status?.ToUpper() == CaptureStatusType.COMPLETED.ToString())
                {
                    if (_orderProcessingService.CanMarkOrderAsPaid(nopOrder))
                    {
                        nopOrder.CaptureTransactionId = capture.Id;
                        nopOrder.CaptureTransactionResult = capture.Status;
                        await _orderProcessingService.MarkOrderAsPaidAsync(nopOrder);
                    }
                }
            }

            //try to get saved in vault payment token
            var vaultedPaymentMethod = order.PaymentSource?.Vault;
            if (vaultedPaymentMethod?.Status?.ToUpper() == VaultStatusType.VAULTED.ToString())
            {
                await _tokenService.InsertAsync(new()
                {
                    ClientId = settings.ClientId,
                    CustomerId = nopOrder.CustomerId,
                    IsPrimaryMethod = false,
                    VaultId = vaultedPaymentMethod.Id,
                    VaultCustomerId = vaultedPaymentMethod.Customer?.Id,
                    TransactionId = order.Id,
                    Type = order.PaymentSource?.Card is not null
                        ? nameof(order.PaymentSource.Card)
                        : (order.PaymentSource?.Venmo is not null
                        ? nameof(order.PaymentSource.Venmo)
                        : (order.PaymentSource?.PayPal is not null
                        ? nameof(order.PaymentSource.PayPal)
                        : null))
                });
            }

            return order;
        });
    }

    #region Alternative payment methods

    /// <summary>
    /// Get Apple Pay transaction info
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Apple Pay transaction info; error message if exists
    /// </returns>
    public async Task<((OrderMoney Amount, Contact BillingAddress, Contact ShippingAddress, Shipping Shipping, string StoreName), string Error)>
        GetAppleTransactionInfoAsync(ButtonPlacement placement)
    {
        return await HandleFunctionAsync(async () =>
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
            if (placement == ButtonPlacement.PaymentMethod && billingAddress is null)
                throw new NopException("Customer billing address not set");

            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var shippingOption = await _genericAttributeService
                .GetAttributeAsync<NopShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
            var pickupPoint = await _genericAttributeService
                .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            var pickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint is not null;
            var shippingAddress = pickupInStore ? new NopAddress
            {
                Address1 = pickupPoint.Address,
                City = pickupPoint.City,
                County = pickupPoint.County,
                CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode))?.Id,
                StateProvinceId = (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation,
                    (await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode))?.Id))?.Id,
                ZipPostalCode = pickupPoint.ZipPostalCode,
                CreatedOnUtc = DateTime.UtcNow
            } : await _addressService.GetAddressByIdAsync(customer.ShippingAddressId ?? 0);
            if (placement == ButtonPlacement.PaymentMethod && shippingIsRequired && shippingAddress is null)
                throw new NopException("Customer shipping address not set");

            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                CurrencyCode = currencyCode,
                BillingAddress = billingAddress,
                Cart = cart.ToList(),
                ShippingAddress = shippingAddress,
                ShippingIsRequired = shippingIsRequired,
                IsPickup = pickupInStore,
                PickupPoint = pickupPoint,
                ShippingOption = shippingOption
            };
            var payer = await PrepareBillingDetailsAsync(details);
            var billingContact = new Contact
            {
                Email = payer.EmailAddress,
                FirstName = payer.Name.GivenName,
                LastName = payer.Name.Surname,
                AddressLines = [payer.Address?.AddressLine1, payer.Address?.AddressLine2],
                City = payer.Address?.AdminArea2,
                State = payer.Address?.AdminArea1,
                Country = payer.Address?.CountryCode,
                PostalCode = payer.Address?.PostalCode
            };

            var items = await PrepareOrderItemsAsync(details);
            var orderAmount = await PrepareOrderMoneyAsync(details, items);

            var shipping = await PrepareShippingDetailsAsync(details, shippingOption?.Name, true);
            var shippingContact = new Contact
            {
                FirstName = shippingAddress is not null && !pickupInStore ? shippingAddress.FirstName : customer.FirstName,
                LastName = shippingAddress is not null && !pickupInStore ? shippingAddress.LastName : customer.LastName,
                AddressLines = [shipping.Address?.AddressLine1, shipping.Address?.AddressLine2],
                City = shipping.Address?.AdminArea2,
                State = shipping.Address?.AdminArea1,
                Country = shipping.Address?.CountryCode,
                PostalCode = shipping.Address?.PostalCode,
                PickupInStore = pickupInStore
            };

            return (orderAmount, billingContact, shippingContact, shipping, store.Name);
        });
    }

    /// <summary>
    /// Update Apple Pay shipping details
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="selectedAddress">Selected shipping address</param>
    /// <param name="selectedOption">Selected shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the updated shipping details; error message if exists
    /// </returns>
    public async Task<(Shipping Shipping, string Error)> UpdateAppleShippingAsync(ButtonPlacement placement,
        (string City, string State, string Country, string PostalCode) selectedAddress, string selectedOption)
    {
        return await HandleFunctionAsync(async () =>
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            //changing shipping address or option on the payment method page is not available
            if (placement == ButtonPlacement.PaymentMethod)
                return null;

            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            if (!shippingIsRequired)
                return null;

            //get option id
            var optionValues = selectedOption?.Split('|', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new();
            var option = (optionValues.FirstOrDefault(), optionValues.LastOrDefault());

            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode,
                ShippingIsRequired = shippingIsRequired
            };
            var shipping = await PrepareUpdatedShippingAsync(details, customer.Email, selectedAddress, option);

            return shipping;
        });
    }

    /// <summary>
    /// Get Google Pay transaction info
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Google Pay transaction info; error message if exists
    /// </returns>
    public async Task<((OrderMoney Amount, string Country, bool ShippingIsRequired), string Error)>
        GetGoogleTransactionInfoAsync(ButtonPlacement placement)
    {
        return await HandleFunctionAsync(async () =>
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            var billingAddress = await _customerService.GetCustomerBillingAddressAsync(customer);
            if (placement == ButtonPlacement.PaymentMethod && billingAddress is null)
                throw new NopException("Customer billing address not set");

            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode
            };
            var items = await PrepareOrderItemsAsync(details);
            var orderAmount = await PrepareOrderMoneyAsync(details, items);

            var country = await _countryService.GetCountryByIdAsync(billingAddress?.CountryId ?? customer.CountryId);
            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);

            return (orderAmount, country?.TwoLetterIsoCode ?? "US", shippingIsRequired);
        });
    }

    /// <summary>
    /// Update Google Pay shipping details
    /// </summary>
    /// <param name="placement">Button placement</param>
    /// <param name="selectedAddress">Selected shipping address</param>
    /// <param name="selectedOption">Selected shipping option</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the updated shipping details; error message if exists
    /// </returns>
    public async Task<(Shipping Shipping, string Error)> UpdateGoogleShippingAsync(ButtonPlacement placement,
        (string City, string State, string Country, string PostalCode) selectedAddress, string selectedOption)
    {
        return await HandleFunctionAsync(async () =>
        {
            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                throw new NopException("Shopping cart is empty");

            //changing shipping address or option on the payment method page is not available
            if (placement == ButtonPlacement.PaymentMethod)
                return null;

            var shippingIsRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            if (!shippingIsRequired)
                return null;

            //get option id
            var optionValues = selectedOption?.Split('|', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new();
            var option = (optionValues.FirstOrDefault(), optionValues.LastOrDefault());

            var details = new CartDetails
            {
                Placement = placement,
                Customer = customer,
                Store = store,
                Cart = cart.ToList(),
                CurrencyCode = currencyCode,
                ShippingIsRequired = shippingIsRequired
            };
            var shipping = await PrepareUpdatedShippingAsync(details, customer.Email, selectedAddress, option);

            return shipping;
        });
    }

    #endregion

    #endregion

    #region Payments

    /// <summary>
    /// Capture an authorization
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="authorizationId">Authorization id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the capture details; error message if exists
    /// </returns>
    public async Task<(Capture Capture, string Error)> CaptureAuthorizationAsync(PayPalCommerceSettings settings, string authorizationId)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            if (string.IsNullOrEmpty(authorizationId))
                throw new NopException("Authorization ID not set");

            var request = new Api.Payments.CreateCaptureRequest { AuthorizationId = authorizationId };
            var capture = await _httpClient.RequestAsync<Api.Payments.CreateCaptureRequest, Api.Payments.CreateCaptureResponse>(request, settings);

            if (capture.Status?.ToUpper() == CaptureStatusType.DECLINED.ToString())
                throw new NopException("The funds could not be captured");

            if (capture.Status?.ToUpper() == CaptureStatusType.FAILED.ToString())
                throw new NopException("There was an error while capturing payment");

            if (capture.Status?.ToUpper() == CaptureStatusType.PENDING.ToString())
                throw new NopException($"Capture is in {capture.Status} status due to {capture.StatusDetails?.Reason}");

            return capture;
        });
    }

    /// <summary>
    /// Void an authorization
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="authorizationId">Authorization id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the void result; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> VoidAsync(PayPalCommerceSettings settings, string authorizationId)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            if (string.IsNullOrEmpty(authorizationId))
                throw new NopException("Authorization ID not set");

            var request = new CreateVoidRequest { AuthorizationId = authorizationId };
            await _httpClient.RequestAsync<CreateVoidRequest, EmptyResponse>(request, settings);

            return true;
        });
    }

    /// <summary>
    /// Refund a captured payment
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="nopOrder">Order</param>
    /// <param name="amount">Amount to refund; pass null to refund the full captured amount</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the refund details; error message if exists
    /// </returns>
    public async Task<(Refund Refund, string Error)> RefundAsync(PayPalCommerceSettings settings, NopOrder nopOrder, decimal? amount = null)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
                throw new NopException("Primary store currency not set");

            if (string.IsNullOrEmpty(nopOrder.CaptureTransactionId))
                throw new NopException("Capture ID not set");

            var request = new CreateRefundRequest
            {
                CaptureId = nopOrder.CaptureTransactionId,
                Amount = amount.HasValue ? PrepareMoney(amount.Value, currencyCode) : null
            };
            var refund = await _httpClient.RequestAsync<CreateRefundRequest, CreateRefundResponse>(request, settings);

            if (refund.Status?.ToUpper() == RefundStatusType.CANCELLED.ToString())
                throw new NopException("The refund was cancelled");

            if (refund.Status?.ToUpper() == RefundStatusType.FAILED.ToString())
                throw new NopException("The refund could not be processed");

            if (refund.Status?.ToUpper() == RefundStatusType.PENDING.ToString())
                throw new NopException($"Capture is in {refund.Status} status due to {refund.StatusDetails?.Reason}");

            //save id to avoid double refund
            var refundIds = await _genericAttributeService
                .GetAttributeAsync<List<string>>(nopOrder, PayPalCommerceDefaults.RefundIdAttributeName)
                ?? new();
            if (!refundIds.Contains(refund.Id))
                refundIds.Add(refund.Id);
            await _genericAttributeService.SaveAttributeAsync(nopOrder, PayPalCommerceDefaults.RefundIdAttributeName, refundIds);

            return refund;
        });
    }

    #endregion

    #region Shipping

    /// <summary>
    /// Add package tracking number to an order
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="shipment">Shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the operation result; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> SetTrackingAsync(PayPalCommerceSettings settings, Shipment shipment)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var carrier = await _genericAttributeService.GetAttributeAsync<string>(shipment, PayPalCommerceDefaults.ShipmentCarrierAttribute);
            if (string.IsNullOrEmpty(carrier))
                return false;

            var nopOrder = await _orderService.GetOrderByIdAsync(shipment?.OrderId ?? 0)
                ?? throw new NopException("Order cannot be loaded");

            if (!string.Equals(nopOrder.PaymentMethodSystemName, PayPalCommerceDefaults.SystemName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            var customValues = _paymentService.DeserializeCustomValues(nopOrder);
            var orderIdKey = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerce.Order.Id");
            if (!customValues.TryGetValue(orderIdKey, out var orderIdValue))
                throw new NopException("Failed to get PayPal order info");

            var order = await _httpClient
                .RequestAsync<GetOrderRequest, GetOrderResponse>(new GetOrderRequest { OrderId = orderIdValue.ToString() }, settings) as Order;
            if (order.Status?.ToUpper() != OrderStatusType.COMPLETED.ToString())
                throw new NopException($"Unable to assign tracking information to orders in {order.Status} status");

            if (order.PurchaseUnits?.FirstOrDefault() is not PurchaseUnit unit || unit.Shipping is null)
                throw new NopException("No shipping info found for PayPal order");

            if (unit.Payments?.Captures?.FirstOrDefault() is not Capture capture ||
                (capture.Status?.ToUpper() != CaptureStatusType.COMPLETED.ToString() &&
                capture.Status?.ToUpper() != CaptureStatusType.PARTIALLY_REFUNDED.ToString() &&
                capture.Status?.ToUpper() != CaptureStatusType.PENDING.ToString()))
            {
                throw new NopException($"Unable to assign tracking information to orders with the payment in {order.Status} status");
            }

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
            var items = await shipmentItems.SelectAwait(async shipmentItem =>
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(shipmentItem.OrderItemId);
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                var sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                var seName = await _urlRecordService.GetSeNameAsync(product);
                var url = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());
                var picture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
                var (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture);

                return new Item
                {
                    Name = CommonHelper.EnsureMaximumLength(product.Name, 127),
                    Description = CommonHelper.EnsureMaximumLength(product.ShortDescription, 127),
                    Sku = CommonHelper.EnsureMaximumLength(sku, 127),
                    Quantity = shipmentItem.Quantity.ToString(),
                    Url = url,
                    ImageUrl = imageUrl
                };
            }).ToListAsync();

            var request = new CreateTrackingRequest
            {
                OrderId = order.Id,
                CaptureId = capture.Id,
                TrackingNumber = shipment.TrackingNumber,
                NotifyPayer = true,
                Carrier = carrier,
                Items = items
            };
            order = await _httpClient.RequestAsync<CreateTrackingRequest, CreateTrackingResponse>(request, settings);

            return true;
        });
    }

    #endregion

    #region Webhooks

    /// <summary>
    /// Get webhook by the URL
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="webhookUrl">Webhook URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook; error message if exists
    /// </returns>
    public async Task<(Webhook Webhook, string Error)> GetWebhookAsync(PayPalCommerceSettings settings, string webhookUrl)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            var webhookList = await _httpClient.RequestAsync<GetWebhooksRequest, GetWebhooksResponse>(new(), settings);
            var webhookByUrl = webhookList?.Webhooks
                ?.FirstOrDefault(webhook => webhook.Url?.Equals(webhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);

            return webhookByUrl;
        });
    }

    /// <summary>
    /// Create webhook that receive events for the subscribed event types
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="storeId">Store id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook; error message if exists
    /// </returns>
    public async Task<(Webhook Webhook, string Error)> CreateWebhookAsync(PayPalCommerceSettings settings, int storeId)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (!IsConfigured(settings))
                throw new NopException("Plugin not configured");

            //prepare webhook URL
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var store = storeId > 0
                ? await _storeService.GetStoreByIdAsync(storeId)
                : await _storeContext.GetCurrentStoreAsync();
            var webhookUrl = $"{store.Url.TrimEnd('/')}{urlHelper.RouteUrl(PayPalCommerceDefaults.Route.Webhook)}".ToLowerInvariant();

            //check whether the webhook already exists
            var (webhook, _) = await GetWebhookAsync(settings, webhookUrl);
            if (webhook is not null)
                return webhook;

            //or try to create a new one
            var request = new CreateWebhookRequest
            {
                EventTypes = PayPalCommerceDefaults.WebhookEventNames.Select(name => new EventType { Name = name }).ToList(),
                Url = webhookUrl
            };
            var result = await _httpClient.RequestAsync<CreateWebhookRequest, CreateWebhookResponse>(request, settings);

            return result;
        });
    }

    /// <summary>
    /// Delete webhook
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteWebhookAsync(PayPalCommerceSettings settings)
    {
        await HandleFunctionAsync(async () =>
        {
            if (!IsConnected(settings))
                throw new NopException("Plugin not connected");

            var (webhook, _) = await GetWebhookAsync(settings, settings.WebhookUrl);
            if (webhook is not null)
                await _httpClient.RequestAsync<DeleteWebhookRequest, EmptyResponse>(new() { WebhookId = webhook.Id }, settings);

            return true;
        });
    }

    /// <summary>
    /// Handle webhook request
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="request">HTTP request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleWebhookAsync(PayPalCommerceSettings settings, Microsoft.AspNetCore.Http.HttpRequest request)
    {
        await HandleFunctionAsync(async () =>
        {
            //ensure that plugin is configured and connected
            if (!IsConnected(settings))
                throw new NopException("Webhook error", new NopException("Plugin not connected"));

            //get request content
            var webhookEvent = string.Empty;
            using (var streamReader = new StreamReader(request.Body))
                webhookEvent = await streamReader.ReadToEndAsync();

            var (webhook, _) = await GetWebhookAsync(settings, settings.WebhookUrl);
            if (webhook is null)
                throw new NopException("Webhook error", new NopException($"No webhook configured for URL '{settings.WebhookUrl}'"));

            //define a local function to validate the webhook event and get its resource
            async Task<IWebhookResource> getWebhookResource<TResource>() where TResource : class, IWebhookResource
            {
                //verify webhook event data
                var verifyRequest = new CreateWebhookSignatureRequest
                {
                    AuthAlgo = request.Headers["PAYPAL-AUTH-ALGO"],
                    CertUrl = request.Headers["PAYPAL-CERT-URL"],
                    TransmissionId = request.Headers["PAYPAL-TRANSMISSION-ID"],
                    TransmissionSig = request.Headers["PAYPAL-TRANSMISSION-SIG"],
                    TransmissionTime = request.Headers["PAYPAL-TRANSMISSION-TIME"],
                    WebhookId = webhook.Id,
                    WebhookEvent = new(webhookEvent)
                };
                var result = await _httpClient.RequestAsync<CreateWebhookSignatureRequest, CreateWebhookSignatureResponse>(verifyRequest, settings);

                if (result?.VerificationStatus?.ToUpper() != WebhookSignatureVerificationStatusType.SUCCESS.ToString())
                    throw new NopException("Webhook error", new NopException($"Webhook signature verification {result?.VerificationStatus}"));

                return JsonConvert.DeserializeObject<Event<TResource>>(webhookEvent)?.Resource;
            }

            //try to get webhook resource
            var webhookResourceType = JsonConvert.DeserializeObject<Event<WebhookResource>>(webhookEvent)?.ResourceType;
            var webhookResource = webhookResourceType switch
            {
                var type when string.Equals(type, nameof(Authorization), StringComparison.InvariantCultureIgnoreCase)
                    => await getWebhookResource<Authorization>(),
                var type when string.Equals(type, nameof(Capture), StringComparison.InvariantCultureIgnoreCase)
                    => await getWebhookResource<Capture>(),
                var type when string.Equals(type, nameof(Refund), StringComparison.InvariantCultureIgnoreCase)
                    => await getWebhookResource<Refund>(),
                var type when string.Equals(type?.Replace("checkout-", string.Empty), nameof(Order), StringComparison.InvariantCultureIgnoreCase)
                    => await getWebhookResource<Order>(),
                var type when string.Equals(type?.Replace("_", string.Empty), nameof(PaymentToken), StringComparison.InvariantCultureIgnoreCase)
                    => await getWebhookResource<PaymentToken>(),
                _ => null
            } ?? throw new NopException("Webhook error", new NopException($"Unknown webhook resource type '{webhookResourceType}'"));

            var paymentToken = webhookResource as PaymentToken;
            if (paymentToken is not null)
            {
                //payment token actions
                var eventType = JsonConvert.DeserializeObject<Event<WebhookResource>>(webhookEvent)?.EventType;
                var paymentTokenCreated = string.Equals(eventType, "VAULT.PAYMENT-TOKEN.CREATED", StringComparison.InvariantCultureIgnoreCase);
                var paymentTokenDeleted =
                    string.Equals(eventType, "VAULT.PAYMENT-TOKEN.DELETION-INITIATED", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(eventType, "VAULT.PAYMENT-TOKEN.DELETED", StringComparison.InvariantCultureIgnoreCase);

                if (paymentTokenCreated)
                {
                    var customerId = int.TryParse(paymentToken.Customer?.MerchantCustomerId, out var id) ? id : (int?)null;

                    //try to get associated transaction
                    if (!string.IsNullOrEmpty(paymentToken.Metadata?.OrderId))
                    {
                        try
                        {
                            var orderRequest = new GetOrderRequest { OrderId = paymentToken.Metadata.OrderId };
                            var paymentTokenOrder = await _httpClient.RequestAsync<GetOrderRequest, GetOrderResponse>(orderRequest, settings);
                            if (Guid.TryParse(paymentTokenOrder.CustomId, out var guid))
                                customerId = (await _orderService.GetOrderByGuidAsync(guid))?.CustomerId;
                        }
                        catch { }
                    }

                    await _tokenService.InsertAsync(new()
                    {
                        ClientId = settings.ClientId,
                        CustomerId = customerId ?? 0,
                        IsPrimaryMethod = false,
                        VaultId = paymentToken.Id,
                        VaultCustomerId = paymentToken.Customer?.Id,
                        TransactionId = paymentToken.Metadata?.OrderId,
                        Type = paymentToken.PaymentSource?.Card is not null
                            ? nameof(paymentToken.PaymentSource.Card)
                            : (paymentToken.PaymentSource?.Venmo is not null
                            ? nameof(paymentToken.PaymentSource.Venmo)
                            : (paymentToken.PaymentSource?.PayPal is not null
                            ? nameof(paymentToken.PaymentSource.PayPal)
                            : null))
                    });

                    return true;
                }

                if (paymentTokenDeleted)
                {
                    var tokens = await _tokenService.GetAllTokensAsync(settings.ClientId, vaultId: paymentToken.Id);
                    if (tokens.Any())
                        await _tokenService.DeleteAsync(tokens);

                    return true;
                }
            }

            if (!Guid.TryParse(webhookResource.CustomId, out var orderGuid) ||
                await _orderService.GetOrderByGuidAsync(orderGuid) is not NopOrder nopOrder)
            {
                throw new NopException("Webhook error", new NopException($"Could not find an order '{orderGuid}'"));
            }

            await _orderService.InsertOrderNoteAsync(new()
            {
                OrderId = nopOrder.Id,
                Note = $"Webhook details: {Environment.NewLine}{Newtonsoft.Json.Linq.JToken.Parse(webhookEvent).ToString(Formatting.Indented)}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            //authorization actions
            var authorization = webhookResource as Authorization;
            if (authorization is not null && Enum.TryParse<AuthorizationStatusType>(authorization.Status, true, out var authorizationStatus))
            {
                nopOrder.AuthorizationTransactionId = authorization.Id;
                nopOrder.AuthorizationTransactionResult = authorization.Status;

                switch (authorizationStatus)
                {
                    case AuthorizationStatusType.PENDING:
                        nopOrder.PaymentStatus = PaymentStatus.Pending;
                        await _orderProcessingService.CheckOrderStatusAsync(nopOrder);

                        break;

                    case AuthorizationStatusType.VOIDED:
                        if (_orderProcessingService.CanVoidOffline(nopOrder))
                            await _orderProcessingService.VoidOfflineAsync(nopOrder);

                        break;

                    case AuthorizationStatusType.CREATED:
                        if (!_orderProcessingService.CanMarkOrderAsAuthorized(nopOrder))
                            break;

                        if (ConvertMoney(authorization.Amount) >= Math.Round(nopOrder.OrderTotal, 2))
                            await _orderProcessingService.MarkAsAuthorizedAsync(nopOrder);

                        break;

                    case AuthorizationStatusType.DENIED:
                        await _orderService.InsertOrderNoteAsync(new()
                        {
                            OrderId = nopOrder.Id,
                            Note = "Cannot authorize funds for this authorized payment",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });

                        break;

                    case AuthorizationStatusType.CAPTURED:
                    case AuthorizationStatusType.PARTIALLY_CAPTURED:

                        //processed by the capture object

                        break;
                }
            }

            //capture actions
            var capture = webhookResource as Capture;
            if (capture is not null && Enum.TryParse<CaptureStatusType>(capture.Status, true, out var captureStatus))
            {
                nopOrder.CaptureTransactionId = capture.Id;
                nopOrder.CaptureTransactionResult = capture.Status;

                switch (captureStatus)
                {
                    case CaptureStatusType.PENDING:
                        nopOrder.PaymentStatus = PaymentStatus.Pending;
                        await _orderProcessingService.CheckOrderStatusAsync(nopOrder);

                        break;

                    case CaptureStatusType.COMPLETED:
                        if (!_orderProcessingService.CanMarkOrderAsPaid(nopOrder))
                            break;

                        if (ConvertMoney(capture.Amount) >= Math.Round(nopOrder.OrderTotal, 2))
                            await _orderProcessingService.MarkOrderAsPaidAsync(nopOrder);

                        break;

                    case CaptureStatusType.DECLINED:
                    case CaptureStatusType.FAILED:
                        await _orderService.InsertOrderNoteAsync(new()
                        {
                            OrderId = nopOrder.Id,
                            Note = "The funds could not be captured",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });

                        break;

                    case CaptureStatusType.PARTIALLY_REFUNDED:
                    case CaptureStatusType.REFUNDED:

                        //processed by the refund object

                        break;
                }
            }

            //refund actions
            var refund = webhookResource as Refund;
            if (refund is not null && Enum.TryParse<RefundStatusType>(refund.Status, true, out var refundStatus))
            {
                switch (refundStatus)
                {
                    case RefundStatusType.CANCELLED:
                    case RefundStatusType.FAILED:
                        await _orderService.InsertOrderNoteAsync(new()
                        {
                            OrderId = nopOrder.Id,
                            Note = "The refund could not be processed or was cancelled",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow
                        });

                        break;

                    case RefundStatusType.COMPLETED:
                        var refundIds = await _genericAttributeService
                            .GetAttributeAsync<List<string>>(nopOrder, PayPalCommerceDefaults.RefundIdAttributeName)
                            ?? new();
                        if (refundIds.Contains(refund.Id))
                            break;

                        if (!decimal.TryParse(refund.Amount?.Value, out var refundedAmount))
                            break;

                        if (!_orderProcessingService.CanPartiallyRefundOffline(nopOrder, refundedAmount))
                            break;

                        await _orderProcessingService.PartiallyRefundOfflineAsync(nopOrder, refundedAmount);

                        refundIds.Add(refund.Id);
                        await _genericAttributeService.SaveAttributeAsync(nopOrder, PayPalCommerceDefaults.RefundIdAttributeName, refundIds);

                        break;

                    case RefundStatusType.PENDING:

                        //waiting the subsequent notification

                        break;
                }
            }

            //order actions
            var order = webhookResource as Order;
            if (order is not null && Enum.TryParse<OrderStatusType>(order.Status, true, out var orderStatus))
            {
                switch (orderStatus)
                {
                    case OrderStatusType.COMPLETED:
                        if (order.PurchaseUnits.FirstOrDefault().Payments?.Captures?.FirstOrDefault() is not Capture orderCapture)
                            break;

                        if (orderCapture.Status?.ToUpper() != CaptureStatusType.COMPLETED.ToString())
                            break;

                        if (!_orderProcessingService.CanMarkOrderAsPaid(nopOrder))
                            break;

                        nopOrder.CaptureTransactionId = orderCapture.Id;
                        nopOrder.CaptureTransactionResult = orderCapture.Status;

                        if (ConvertMoney(orderCapture.Amount) >= Math.Round(nopOrder.OrderTotal, 2))
                            await _orderProcessingService.MarkOrderAsPaidAsync(nopOrder);

                        break;
                }
            }

            await _orderService.UpdateOrderAsync(nopOrder);

            return true;
        });
    }

    #endregion

    #region Onboarding

    /// <summary>
    /// Prepare URL to sign up a merchant
    /// </summary>
    /// <param name="merchantGuid">Merchant internal id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the URL to sign up; error message if exists
    /// </returns>
    public async Task<((string SandboxUrl, string LiveUrl), string Error)> PrepareSignUpUrlAsync(string merchantGuid)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (string.IsNullOrEmpty(merchantGuid))
                throw new NopException("Merchant internal id is not set");

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var store = storeId > 0
                ? await _storeService.GetStoreByIdAsync(storeId)
                : await _storeContext.GetCurrentStoreAsync();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var returnUrl = $"{store.Url.TrimEnd('/')}" +
                $"{urlHelper.RouteUrl(PayPalCommerceDefaults.Route.OnboardingCallback, new { storeId = storeId })}";

            //sandbox URL
            var onboarding = new Onboarding
            {
                Id = PayPalCommerceDefaults.Onboarding.Id.Sandbox,
                Product = PayPalProductType.PPCP.ToString().ToLower(),
                SecondaryProducts = string.Join(',',
                    PayPalProductType.PAYMENT_METHODS.ToString().ToLower(),
                    PayPalProductType.ADVANCED_VAULTING.ToString().ToLower()),
                Capabilities = string.Join(',',
                    ProductCapabilityType.APPLE_PAY.ToString().ToLower(),
                    ProductCapabilityType.GOOGLE_PAY.ToString().ToLower(),
                    ProductCapabilityType.PAYPAL_WALLET_VAULTING_ADVANCED.ToString().ToLower()),
                IntegrationType = IntegrationType.FO.ToString().ToUpper(),
                Features = string.Join(',',
                    FeatureType.PAYMENT.ToString().ToLower(),
                    FeatureType.REFUND.ToString().ToLower(),
                    FeatureType.ACCESS_MERCHANT_INFORMATION.ToString().ToLower(),
                    FeatureType.BILLING_AGREEMENT.ToString().ToLower(),
                    FeatureType.VAULT.ToString().ToLower()),
                ClientId = PayPalCommerceDefaults.Onboarding.ClientId.Sandbox,
                ReturnToUrl = returnUrl.ToLowerInvariant(),
                LogoUrl = PayPalCommerceDefaults.Onboarding.LogoUrl,
                SellerNonce = GetSha256Hash(merchantGuid),
                DisplayMode = "minibrowser"
            };
            var sandboxUrl = QueryHelpers.AddQueryString(PayPalCommerceDefaults.Onboarding.Url.Sandbox, ObjectToDictionary(onboarding));

            //live URL
            onboarding.Id = PayPalCommerceDefaults.Onboarding.Id.Live;
            onboarding.ClientId = PayPalCommerceDefaults.Onboarding.ClientId.Live;
            var liveUrl = QueryHelpers.AddQueryString(PayPalCommerceDefaults.Onboarding.Url.Live, ObjectToDictionary(onboarding));

            return (sandboxUrl, liveUrl);
        });
    }

    /// <summary>
    /// Sign up a merchant with the passed authentication parameters
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="authCode">Authentication parameters</param>
    /// <param name="sharedId">Authentication parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the REST API application credentials; error message if exists
    /// </returns>
    public async Task<(Credentials Credentials, string Error)> SignUpAsync(PayPalCommerceSettings settings, string authCode, string sharedId)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (string.IsNullOrEmpty(settings.MerchantGuid))
                throw new NopException("Merchant internal id is not set");

            if (string.IsNullOrEmpty(sharedId) || string.IsNullOrEmpty(authCode))
                throw new NopException("Authentication parameters are empty");

            //try to get an access token
            var accessTokenRequest = new GetAccessTokenRequest
            {
                GrantType = "authorization_code",
                CodeVerifier = GetSha256Hash(settings.MerchantGuid.ToString()),
                Code = authCode,
                ClientId = sharedId,
                Secret = string.Empty
            };
            var accessToken = await _httpClient.RequestAsync<GetAccessTokenRequest, GetAccessTokenResponse>(accessTokenRequest, settings);

            //and change it to the credentials
            var credentialsRequest = new GetCredentialsRequest
            {
                Id = settings.UseSandbox ? PayPalCommerceDefaults.Onboarding.Id.Sandbox : PayPalCommerceDefaults.Onboarding.Id.Live,
                AccessToken = accessToken?.Token
            };
            var credentials = await _httpClient.RequestAsync<GetCredentialsRequest, GetCredentialsResponse>(credentialsRequest, settings);

            return credentials;
        });
    }

    /// <summary>
    /// Get the merchant details
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the merchant details; error message if exists
    /// </returns>
    public async Task<(Merchant Merchant, string Error)> GetMerchantAsync(PayPalCommerceSettings settings)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (string.IsNullOrEmpty(settings.MerchantGuid))
                throw new NopException("Merchant internal id is not set");

            //ensure that merchant id exists
            if (string.IsNullOrEmpty(settings.MerchantId))
                throw new NopException("Onboarding process failed, please try again");

            var request = new GetMerchantRequest
            {
                Id = settings.UseSandbox ? PayPalCommerceDefaults.Onboarding.Id.Sandbox : PayPalCommerceDefaults.Onboarding.Id.Live,
                MerchantId = settings.MerchantId
            };

            var merchant = await _httpClient.RequestAsync<GetMerchantRequest, GetMerchantResponse>(request, settings);

            //check capabilities statuses
            var ppcpStatus = merchant.Products
                ?.FirstOrDefault(product => product.Name?.ToUpper() == PayPalProductType.PPCP_CUSTOM.ToString())
                ?.VettingStatus?.ToUpper()
                ?? ProductStatusType.PENDING.ToString();
            var advancedProcessingEnabled = ppcpStatus == ProductStatusType.SUBSCRIBED.ToString();
            (bool Active, string Status) getCapability(ProductCapabilityType type)
            {
                var capabilityStatus = merchant.Capabilities
                    ?.FirstOrDefault(capability => capability.Name?.ToUpper() == type.ToString())
                    ?.Status?.ToUpper();
                var active = capabilityStatus == ProductCapabilityStatusType.ACTIVE.ToString();
                return (active && advancedProcessingEnabled, capabilityStatus ?? ProductCapabilityStatusType.PENDING.ToString());
            }

            merchant.AdvancedCards = getCapability(ProductCapabilityType.CUSTOM_CARD_PROCESSING);
            merchant.ApplePay = getCapability(ProductCapabilityType.APPLE_PAY);
            merchant.GooglePay = getCapability(ProductCapabilityType.GOOGLE_PAY);
            merchant.Vaulting = getCapability(ProductCapabilityType.PAYPAL_WALLET_VAULTING_ADVANCED);

            var review = ppcpStatus == ProductStatusType.IN_REVIEW.ToString();
            var needMoreData = ppcpStatus == ProductStatusType.NEED_MORE_DATA.ToString();
            var denied = ppcpStatus == ProductStatusType.DENIED.ToString();

            var cardsCapability = merchant.Capabilities
                ?.FirstOrDefault(capability => capability.Name?.ToUpper() == ProductCapabilityType.CUSTOM_CARD_PROCESSING.ToString());
            var withdrawCapability = merchant.Capabilities
                ?.FirstOrDefault(capability => capability.Name?.ToUpper() == ProductCapabilityType.WITHDRAW_MONEY.ToString());
            var sendMoneyCapability = merchant.Capabilities
                ?.FirstOrDefault(capability => capability.Name?.ToUpper() == ProductCapabilityType.SEND_MONEY.ToString());
            var inLimit = advancedProcessingEnabled &&
                merchant.AdvancedCards.Active &&
                cardsCapability?.Limits?.FirstOrDefault()?.Type?.ToUpper() == "GENERAL";
            var belowLimit = inLimit && withdrawCapability?.Limits is null && sendMoneyCapability?.Limits is null;
            var overLimit = inLimit && withdrawCapability?.Limits is not null && sendMoneyCapability?.Limits is not null;

            merchant.AdvancedCardsDetails = (review, needMoreData, belowLimit, overLimit, denied);

            merchant.ConfiguratorSupported = PayPalCommerceDefaults.PayLaterSupportedCountries.Contains(merchant.Country);

            return merchant;
        }, false);
    }

    #endregion

    #region Payment tokens

    /// <summary>
    /// Get customer's payment tokens
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="withDetails">Whether to load additional details of payment tokens</param>
    /// <param name="deleteTokenId">Identifier of the token to delete</param>
    /// <param name="defaultTokenId">Identifier of the token to mark as default</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of payment tokens; error message if exists
    /// </returns>
    public async Task<(List<PayPalToken> PaymentTokens, string Error)>
        GetPaymentTokensAsync(PayPalCommerceSettings settings, bool withDetails = false, int? deleteTokenId = null, int? defaultTokenId = null)
    {
        return await HandleFunctionAsync(async () =>
        {
            //only registered customers can save payment tokens
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
                return new List<PayPalToken>();

            //try to delete token
            if (deleteTokenId is not null)
            {
                var deleteToken = await _tokenService.GetByIdAsync(deleteTokenId.Value)
                    ?? throw new NopException("No payment token found with the specified id");

                if (deleteToken.CustomerId != customer.Id)
                    throw new NopException("You cannot delete this token");

                await _tokenService.DeleteAsync(deleteToken);
                await _httpClient.RequestAsync<DeletePaymentTokenRequest, EmptyResponse>(new() { Id = deleteToken.VaultId }, settings);
            }

            //try to mark token as default
            if (defaultTokenId is not null)
            {
                var defaultToken = await _tokenService.GetByIdAsync(defaultTokenId.Value)
                    ?? throw new NopException("No payment token found with the specified id");

                if (defaultToken.CustomerId != customer.Id)
                    throw new NopException("You cannot edit this token");

                defaultToken.IsPrimaryMethod = true;
                await _tokenService.UpdateAsync(defaultToken);

                var tokensToUpdate = (await _tokenService.GetAllTokensAsync(settings.ClientId, customer.Id))
                    .Where(token => token.Id != defaultToken.Id && token.IsPrimaryMethod);
                foreach (var token in tokensToUpdate)
                {
                    token.IsPrimaryMethod = false;
                    await _tokenService.UpdateAsync(token);
                }
            }

            var tokens = await _tokenService.GetAllTokensAsync(settings.ClientId, customer.Id);
            if (!tokens.Any())
                return new List<PayPalToken>();

            if (!withDetails)
                return tokens.ToList();

            //load additional details
            return await PreparePaymentTokensAsync(settings, tokens);
        });
    }

    /// <summary>
    /// Delete all customer's payment tokens
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="customerId">Customer id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the delete result; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> DeletePaymentTokensAsync(PayPalCommerceSettings settings, int customerId)
    {
        return await HandleFunctionAsync(async () =>
        {
            var tokens = await _tokenService.GetAllTokensAsync(settings.ClientId, customerId);
            await _tokenService.DeleteAsync(tokens);
            foreach (var token in tokens)
            {
                try
                {
                    await _httpClient.RequestAsync<DeletePaymentTokenRequest, EmptyResponse>(new() { Id = token.VaultId }, settings);
                }
                catch { }
            }

            return true;
        });
    }

    /// <summary>
    /// Get previously saved cards (payment tokens)
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <param name="placement">Button placement</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of payment tokens; error message if exists
    /// </returns>
    public async Task<(List<PayPalToken> PaymentTokens, string Error)> GetSavedCardsAsync(PayPalCommerceSettings settings, ButtonPlacement placement)
    {
        return await HandleFunctionAsync(async () =>
        {
            if (placement != ButtonPlacement.PaymentMethod || !settings.UseCardFields || !settings.UseVault)
                return null;

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
                return null;

            //get cards only
            var tokens = await _tokenService.GetAllTokensAsync(settings.ClientId, customer.Id, type: nameof(PaymentSource.Card));

            return await PreparePaymentTokensAsync(settings, tokens);
        });
    }

    #endregion

    #endregion

    #region Nested classes

    /// <summary>
    /// Represents the shopping cart details
    /// </summary>
    private class CartDetails
    {
        #region Properties

        /// <summary>
        /// Gets or sets the button placement
        /// </summary>
        public ButtonPlacement Placement { get; set; }

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        public Store Store { get; set; }

        /// <summary>
        /// Gets or sets the customer's shopping cart
        /// </summary>
        public List<ShoppingCartItem> Cart { get; set; } = new();

        /// <summary>
        /// Gets or sets the primary store currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the customer's billing address
        /// </summary>
        public NopAddress BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer's shipping address
        /// </summary>
        public NopAddress ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the shipping is required for this cart
        /// </summary>
        public bool ShippingIsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pick up in store option is selected
        /// </summary>
        public bool IsPickup { get; set; }

        /// <summary>
        /// Gets or sets the selected shipping option
        /// </summary>
        public NopShippingOption ShippingOption { get; set; }

        /// <summary>
        /// Gets or sets the selected pickup point
        /// </summary>
        public PickupPoint PickupPoint { get; set; }

        #endregion
    }

    #endregion
}