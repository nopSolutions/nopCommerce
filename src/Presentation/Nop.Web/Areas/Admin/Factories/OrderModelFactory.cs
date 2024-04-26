using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the order model factory implementation
/// </summary>
public partial class OrderModelFactory : IOrderModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAffiliateService _affiliateService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountService _discountService;
    protected readonly IDownloadService _downloadService;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureService _measureService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderReportService _orderReportService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceCalculationService _priceCalculationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly ISettingService _settingService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShippingService _shippingService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreService _storeService;
    protected readonly ITaxService _taxService;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly NopHttpClient _nopHttpClient;
    protected readonly OrderSettings _orderSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly TaxSettings _taxSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public OrderModelFactory(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IActionContextAccessor actionContextAccessor,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAffiliateService affiliateService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IDownloadService downloadService,
        IEncryptionService encryptionService,
        IGiftCardService giftCardService,
        ILocalizationService localizationService,
        IMeasureService measureService,
        IOrderProcessingService orderProcessingService,
        IOrderReportService orderReportService,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPictureService pictureService,
        IPriceCalculationService priceCalculationService,
        IPriceFormatter priceFormatter,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IRewardPointService rewardPointService,
        ISettingService settingService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IStateProvinceService stateProvinceService,
        IStoreService storeService,
        ITaxService taxService,
        IUrlHelperFactory urlHelperFactory,
        IVendorService vendorService,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        NopHttpClient nopHttpClient,
        OrderSettings orderSettings,
        ShippingSettings shippingSettings,
        IUrlRecordService urlRecordService,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _actionContextAccessor = actionContextAccessor;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _affiliateService = affiliateService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _discountService = discountService;
        _downloadService = downloadService;
        _encryptionService = encryptionService;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _measureService = measureService;
        _orderProcessingService = orderProcessingService;
        _orderReportService = orderReportService;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pictureService = pictureService;
        _priceCalculationService = priceCalculationService;
        _priceFormatter = priceFormatter;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _rewardPointService = rewardPointService;
        _settingService = settingService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _taxService = taxService;
        _urlHelperFactory = urlHelperFactory;
        _vendorService = vendorService;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _nopHttpClient = nopHttpClient;
        _orderSettings = orderSettings;
        _shippingSettings = shippingSettings;
        _urlRecordService = urlRecordService;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepares the shipment model
    /// </summary>
    /// <param name="shipment">Shipment</param>
    /// <param name="model">Predefined shipment model if any</param>
    /// <returns>The <see cref="Task"/> containing the <see cref="ShipmentModel"/></returns>
    protected virtual async Task<ShipmentModel> PrepareShipmentModelAsync(Shipment shipment, ShipmentModel model = null)
    {
        //fill in model values from the entity
        var shipmentModel = model ?? shipment.ToModel<ShipmentModel>();

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

        shipmentModel.PickupInStore = order.PickupInStore;
        shipmentModel.CustomOrderNumber = order.CustomOrderNumber;

        //convert dates to the user time
        if (order.PickupInStore)
        {
            shipmentModel.ShippedDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
            shipmentModel.ReadyForPickupDate = shipment.ReadyForPickupDateUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ReadyForPickupDateUtc.Value, DateTimeKind.Utc)).ToString()
                : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.ReadyForPickupDate.NotYet");
        }
        else
        {
            shipmentModel.ReadyForPickupDate = await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DateNotAvailable");
            shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue
                ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString()
                : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.ShippedDate.NotYet");
        }

        shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue
            ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString()
            : await _localizationService.GetResourceAsync("Admin.Orders.Shipments.DeliveryDate.NotYet");

        //fill in additional values (not existing in the entity)
        shipmentModel.CanShip = !order.PickupInStore && !shipment.ShippedDateUtc.HasValue;
        shipmentModel.CanMarkAsReadyForPickup = order.PickupInStore && !shipment.ReadyForPickupDateUtc.HasValue;
        shipmentModel.CanDeliver = (shipment.ShippedDateUtc.HasValue || shipment.ReadyForPickupDateUtc.HasValue) && !shipment.DeliveryDateUtc.HasValue;

        if (shipment.TotalWeight.HasValue)
            shipmentModel.TotalWeight = $"{shipment.TotalWeight:F2} [{(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name}]";

        return shipmentModel;
    }

    /// <summary>
    /// Set some address fields as required
    /// </summary>
    /// <param name="model">Address model</param>
    protected virtual void SetAddressFieldsAsRequired(AddressModel model)
    {
        model.FirstNameRequired = true;
        model.LastNameRequired = true;
        model.EmailRequired = true;
        model.CompanyRequired = _addressSettings.CompanyRequired;
        model.CountyRequired = _addressSettings.CountyRequired;
        model.CityRequired = _addressSettings.CityRequired;
        model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
        model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
        model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
        model.PhoneRequired = _addressSettings.PhoneRequired;
        model.FaxRequired = _addressSettings.FaxRequired;
    }

    /// <summary>
    /// Prepare order item models
    /// </summary>
    /// <param name="models">List of order item models</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderItemModelsAsync(IList<OrderItemModel> models, Order order)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(order);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

        //get order items
        var vendor = await _workContext.GetCurrentVendorAsync();
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

        foreach (var orderItem in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            //fill in model values from the entity
            var orderItemModel = new OrderItemModel
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                ProductName = product.Name,
                Quantity = orderItem.Quantity,
                IsDownload = product.IsDownload,
                DownloadCount = orderItem.DownloadCount,
                DownloadActivationType = product.DownloadActivationType,
                IsDownloadActivated = orderItem.IsDownloadActivated,
                UnitPriceInclTaxValue = orderItem.UnitPriceInclTax,
                UnitPriceExclTaxValue = orderItem.UnitPriceExclTax,
                DiscountInclTaxValue = orderItem.DiscountAmountInclTax,
                DiscountExclTaxValue = orderItem.DiscountAmountExclTax,
                SubTotalInclTaxValue = orderItem.PriceInclTax,
                SubTotalExclTaxValue = orderItem.PriceExclTax,
                AttributeInfo = orderItem.AttributeDescription
            };

            //fill in additional values (not existing in the entity)
            orderItemModel.Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
            orderItemModel.VendorName = (await _vendorService.GetVendorByIdAsync(product.VendorId))?.Name;

            //picture
            var orderItemPicture = await _pictureService.GetProductPictureAsync(product, orderItem.AttributesXml);
            (orderItemModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(orderItemPicture, 75);

            //license file
            if (orderItem.LicenseDownloadId.HasValue)
            {
                orderItemModel.LicenseDownloadGuid = (await _downloadService
                    .GetDownloadByIdAsync(orderItem.LicenseDownloadId.Value))?.DownloadGuid ?? Guid.Empty;
            }

            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

            //unit price
            orderItemModel.UnitPriceInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.UnitPriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
            orderItemModel.UnitPriceExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.UnitPriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

            //discounts
            orderItemModel.DiscountInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.DiscountAmountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
            orderItemModel.DiscountExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.DiscountAmountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

            //subtotal
            orderItemModel.SubTotalInclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.PriceInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true, true);
            orderItemModel.SubTotalExclTax = await _priceFormatter
                .FormatOrderPriceAsync(orderItem.PriceExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false, true);

            //recurring info
            if (product.IsRecurring)
            {
                orderItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Products.RecurringPeriod"),
                    product.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(product.RecurringCyclePeriod));
            }

            //rental info
            if (product.IsRental)
            {
                var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                    rentalStartDate, rentalEndDate);
            }

            //prepare return request models
            await PrepareReturnRequestBriefModelsAsync(orderItemModel.ReturnRequests, orderItem);

            //gift card identifiers
            orderItemModel.PurchasedGiftCardIds = (await _giftCardService
                .GetGiftCardsByPurchasedWithOrderItemIdAsync(orderItem.Id)).Select(card => card.Id).ToList();

            models.Add(orderItemModel);
        }
    }

    /// <summary>
    /// Prepare return request brief models
    /// </summary>
    /// <param name="models">List of return request brief models</param>
    /// <param name="orderItem">Order item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareReturnRequestBriefModelsAsync(IList<OrderItemModel.ReturnRequestBriefModel> models, OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(orderItem);

        var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(orderItemId: orderItem.Id);
        foreach (var returnRequest in returnRequests)
        {
            models.Add(new OrderItemModel.ReturnRequestBriefModel
            {
                CustomNumber = returnRequest.CustomNumber,
                Id = returnRequest.Id
            });
        }
    }

    /// <summary>
    /// Prepare order model totals
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelTotalsAsync(OrderModel model, Order order)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

        //subtotal
        model.OrderSubtotalInclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubtotalInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
        model.OrderSubtotalExclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubtotalExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
        model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
        model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;

        //discount (applied to order subtotal)
        var orderSubtotalDiscountInclTaxStr = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubTotalDiscountInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true);
        var orderSubtotalDiscountExclTaxStr = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderSubTotalDiscountExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false);
        if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
            model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
        if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
            model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
        model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
        model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

        //shipping
        model.OrderShippingInclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderShippingInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
        model.OrderShippingExclTax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderShippingExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix);
        model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
        model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

        //payment method additional fee
        if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
        {
            model.PaymentMethodAdditionalFeeInclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, true,
                    _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
            model.PaymentMethodAdditionalFeeExclTax = await _priceFormatter
                .FormatOrderPriceAsync(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, false,
                    _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix);
        }

        model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
        model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;

        //tax
        model.Tax = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderTax, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
        var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
        var displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
        var displayTax = !displayTaxRates;
        foreach (var tr in taxRates)
        {
            model.TaxRates.Add(new OrderModel.TaxRate
            {
                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                Value = await _priceFormatter
                    .FormatOrderPriceAsync(tr.Value, order.CurrencyRate, order.CustomerCurrencyCode,
                        _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false)
            });
        }

        model.DisplayTaxRates = displayTaxRates;
        model.DisplayTax = displayTax;
        model.TaxValue = order.OrderTax;
        model.TaxRatesValue = order.TaxRates;

        //discount
        if (order.OrderDiscount > 0)
        {
            model.OrderTotalDiscount = await _priceFormatter
                .FormatOrderPriceAsync(-order.OrderDiscount, order.CurrencyRate, order.CustomerCurrencyCode,
                    _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
        }
        model.OrderTotalDiscountValue = order.OrderDiscount;

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            model.GiftCards.Add(new OrderModel.GiftCard
            {
                CouponCode = (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId)).GiftCardCouponCode,
                Amount = await _priceFormatter.FormatPriceAsync(-gcuh.UsedValue, true, false)
            });
        }

        //reward points
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
            model.RedeemedRewardPointsAmount =
                await _priceFormatter.FormatPriceAsync(-redeemedRewardPointsEntry.UsedAmount, true, false);
        }

        //total
        model.OrderTotal = await _priceFormatter
            .FormatOrderPriceAsync(order.OrderTotal, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
        model.OrderTotalValue = order.OrderTotal;

        //refunded amount
        if (order.RefundedAmount > decimal.Zero)
            model.RefundedAmount = await _priceFormatter.FormatPriceAsync(order.RefundedAmount, true, false);

        //used discounts
        var duh = await _discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);
        foreach (var d in duh)
        {
            var discount = await _discountService.GetDiscountByIdAsync(d.DiscountId);

            model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
            {
                DiscountId = d.DiscountId,
                DiscountName = discount.Name
            });
        }

        //profit (hide for vendors)
        if (await _workContext.GetCurrentVendorAsync() != null)
            return;

        var profit = await _orderReportService.ProfitReportAsync(orderId: order.Id);
        model.Profit = await _priceFormatter
            .FormatOrderPriceAsync(profit, order.CurrencyRate, order.CustomerCurrencyCode,
                _orderSettings.DisplayCustomerCurrencyOnOrders, primaryStoreCurrency, languageId, null, false);
    }

    /// <summary>
    /// Prepare order model payment info
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelPaymentInfoAsync(OrderModel model, Order order)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        //prepare billing address
        model.BillingAddress = billingAddress.ToModel(model.BillingAddress);

        model.BillingAddress.CountryName = (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name;
        model.BillingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress))?.Name;

        await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress, billingAddress);
        SetAddressFieldsAsRequired(model.BillingAddress);

        if (order.AllowStoringCreditCardNumber)
        {
            //card type
            model.CardType = _encryptionService.DecryptText(order.CardType);
            //cardholder name
            model.CardName = _encryptionService.DecryptText(order.CardName);
            //card number
            model.CardNumber = _encryptionService.DecryptText(order.CardNumber);
            //cvv
            model.CardCvv2 = _encryptionService.DecryptText(order.CardCvv2);
            //expiry date
            var cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
            if (!string.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                model.CardExpirationMonth = cardExpirationMonthDecrypted;
            var cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
            if (!string.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                model.CardExpirationYear = cardExpirationYearDecrypted;

            model.AllowStoringCreditCardNumber = true;
        }
        else
        {
            var maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(order.MaskedCreditCardNumber);
            if (!string.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                model.CardNumber = maskedCreditCardNumberDecrypted;
        }

        //payment transaction info
        model.AuthorizationTransactionId = order.AuthorizationTransactionId;
        model.CaptureTransactionId = order.CaptureTransactionId;
        model.SubscriptionTransactionId = order.SubscriptionTransactionId;

        //payment method info
        var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
        model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
        model.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);

        //payment method buttons
        model.CanCancelOrder = _orderProcessingService.CanCancelOrder(order);
        model.CanCapture = await _orderProcessingService.CanCaptureAsync(order);
        model.CanMarkOrderAsPaid = _orderProcessingService.CanMarkOrderAsPaid(order);
        model.CanRefund = await _orderProcessingService.CanRefundAsync(order);
        model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);
        model.CanPartiallyRefund = await _orderProcessingService.CanPartiallyRefundAsync(order, decimal.Zero);
        model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);
        model.CanVoid = await _orderProcessingService.CanVoidAsync(order);
        model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
        model.MaxAmountToRefund = order.OrderTotal - order.RefundedAmount;

        //recurring payment record
        model.RecurringPaymentId = (await _orderService.SearchRecurringPaymentsAsync(initialOrderId: order.Id, showHidden: true)).FirstOrDefault()?.Id ?? 0;
    }

    /// <summary>
    /// Prepare order model shipping info
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareOrderModelShippingInfoAsync(OrderModel model, Order order)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        model.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
        if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
            return;

        model.IsShippable = true;
        model.ShippingMethod = order.ShippingMethod;
        model.CanAddNewShipments = await _orderService.HasItemsToAddToShipmentAsync(order);
        model.PickupInStore = order.PickupInStore;
        if (!order.PickupInStore)
        {
            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);
            var shippingCountry = await _countryService.GetCountryByAddressAsync(shippingAddress);

            model.ShippingAddress = shippingAddress.ToModel(model.ShippingAddress);
            model.ShippingAddress.CountryName = shippingCountry?.Name;
            model.ShippingAddress.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress))?.Name;
            await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress, shippingAddress);
            SetAddressFieldsAsRequired(model.ShippingAddress);
            model.ShippingAddressGoogleMapsUrl = "https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                                                 $"{WebUtility.UrlEncode(shippingAddress.Address1 + " " + shippingAddress.ZipPostalCode + " " + shippingAddress.City + " " + (shippingCountry?.Name ?? string.Empty))}";
        }
        else
        {
            if (order.PickupAddressId is null)
                return;

            var pickupAddress = await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value);

            var pickupCountry = await _countryService.GetCountryByAddressAsync(pickupAddress);

            model.PickupAddress = pickupAddress.ToModel(model.PickupAddress);
            model.PickupAddressGoogleMapsUrl = $"https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q=" +
                                               $"{WebUtility.UrlEncode($"{pickupAddress.Address1} {pickupAddress.ZipPostalCode} {pickupAddress.City} {(pickupCountry?.Name ?? string.Empty)}")}";
        }
    }

    /// <summary>
    /// Prepare product attribute models
    /// </summary>
    /// <param name="models">List of product attribute models</param>
    /// <param name="order">Order</param>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProductAttributeModelsAsync(IList<AddProductToOrderModel.ProductAttributeModel> models, Order order, Product product)
    {
        ArgumentNullException.ThrowIfNull(models);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(product);

        var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
        foreach (var attribute in attributes)
        {
            var attributeModel = new AddProductToOrderModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
                TextPrompt = attribute.TextPrompt,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
                HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
            };
            if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
            {
                attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                    .Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            if (attribute.ShouldHaveValues())
            {
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var store = await _storeService.GetStoreByIdAsync(order.StoreId);

                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    //price adjustment
                    var (priceAdjustment, _) = await _taxService.GetProductPriceAsync(product,
                        await _priceCalculationService.GetProductAttributeValuePriceAdjustmentAsync(product, attributeValue, customer, store));

                    var priceAdjustmentStr = string.Empty;
                    if (priceAdjustment != 0)
                    {
                        if (attributeValue.PriceAdjustmentUsePercentage)
                        {
                            priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                            priceAdjustmentStr = priceAdjustment > 0 ? $"+{priceAdjustmentStr}%" : $"{priceAdjustmentStr}%";
                        }
                        else
                        {
                            priceAdjustmentStr = priceAdjustment > 0 ? $"+{await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false)}" : $"-{await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false)}";
                        }
                    }

                    attributeModel.Values.Add(new AddProductToOrderModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected,
                        CustomerEntersQty = attributeValue.CustomerEntersQty,
                        Quantity = attributeValue.Quantity,
                        PriceAdjustment = priceAdjustmentStr,
                        PriceAdjustmentValue = priceAdjustment
                    });
                }
            }

            models.Add(attributeModel);
        }
    }

    /// <summary>
    /// Prepare shipment item model
    /// </summary>
    /// <param name="model">Shipment item model</param>
    /// <param name="orderItem">Order item</param>
    /// <param name="product">Product item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareShipmentItemModelAsync(ShipmentItemModel model, OrderItem orderItem, Product product)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(product);

        if (orderItem.ProductId != product.Id)
            throw new ArgumentException($"{nameof(orderItem.ProductId)} != {nameof(product.Id)}");

        //fill in additional values (not existing in the entity)
        model.OrderItemId = orderItem.Id;
        model.ProductId = orderItem.ProductId;
        model.ProductName = product.Name;
        model.Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
        model.AttributeInfo = orderItem.AttributeDescription;
        model.ShipSeparately = product.ShipSeparately;
        model.QuantityOrdered = orderItem.Quantity;
        model.QuantityInAllShipments = await _orderService.GetTotalNumberOfItemsInAllShipmentsAsync(orderItem);
        model.QuantityToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);

        var baseWeight = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name;
        var baseDimension = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId))?.Name;
        if (orderItem.ItemWeight.HasValue)
            model.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";
        model.ItemDimensions =
            $"{product.Length:F2} x {product.Width:F2} x {product.Height:F2} [{baseDimension}]";

        if (!product.IsRental)
            return;

        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
            ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
            ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
        model.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"), rentalStartDate, rentalEndDate);
    }

    /// <summary>
    /// Prepare shipment status event models
    /// </summary>
    /// <param name="models">List of shipment status event models</param>
    /// <param name="shipment">Shipment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareShipmentStatusEventModelsAsync(IList<ShipmentStatusEventModel> models, Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(models);

        var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
        var shipmentEvents = await shipmentTracker?.GetShipmentEventsAsync(shipment.TrackingNumber, shipment);
        if (shipmentEvents == null)
            return;

        foreach (var shipmentEvent in shipmentEvents)
        {
            var shipmentStatusEventModel = new ShipmentStatusEventModel
            {
                Status = shipmentEvent.Status,
                Date = shipmentEvent.Date,
                EventName = shipmentEvent.EventName,
                Location = shipmentEvent.Location
            };
            var shipmentEventCountry = await _countryService.GetCountryByTwoLetterIsoCodeAsync(shipmentEvent.CountryCode);
            shipmentStatusEventModel.Country = shipmentEventCountry != null
                ? await _localizationService.GetLocalizedAsync(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
            models.Add(shipmentStatusEventModel);
        }
    }

    /// <summary>
    /// Prepare order shipment search model
    /// </summary>
    /// <param name="searchModel">Order shipment search model</param>
    /// <param name="order">Order</param>
    /// <returns>Order shipment search model</returns>
    protected virtual OrderShipmentSearchModel PrepareOrderShipmentSearchModel(OrderShipmentSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;

        //prepare nested search model
        PrepareShipmentItemSearchModel(searchModel.ShipmentItemSearchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare shipment item search model
    /// </summary>
    /// <param name="searchModel">Shipment item search model</param>
    /// <returns>Shipment item search model</returns>
    protected virtual ShipmentItemSearchModel PrepareShipmentItemSearchModel(ShipmentItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare order note search model
    /// </summary>
    /// <param name="searchModel">Order note search model</param>
    /// <param name="order">Order</param>
    /// <returns>Order note search model</returns>
    protected virtual OrderNoteSearchModel PrepareOrderNoteSearchModel(OrderNoteSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare order search model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order search model
    /// </returns>
    public virtual async Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;

        var licenseCheckModel = new LicenseCheckModel();
        try
        {
            var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
            if (!string.IsNullOrEmpty(result))
            {
                licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                    await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
            }
        }
        catch { }
        searchModel.LicenseCheckModel = licenseCheckModel;

        //prepare available order, payment and shipping statuses
        await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
        if (searchModel.AvailableOrderStatuses.Any())
        {
            if (searchModel.OrderStatusIds?.Any() ?? false)
            {
                var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
        }

        await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
        if (searchModel.AvailablePaymentStatuses.Any())
        {
            if (searchModel.PaymentStatusIds?.Any() ?? false)
            {
                var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
        }

        await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);
        if (searchModel.AvailableShippingStatuses.Any())
        {
            if (searchModel.ShippingStatusIds?.Any() ?? false)
            {
                var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                var statusItems = searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                foreach (var statusItem in statusItems)
                {
                    statusItem.Selected = true;
                }
            }
            else
                searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
        }

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        //prepare available payment methods
        searchModel.AvailablePaymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).Select(method =>
            new SelectListItem { Text = method.PluginDescriptor.FriendlyName, Value = method.PluginDescriptor.SystemName }).ToList();
        searchModel.AvailablePaymentMethods.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = string.Empty });

        //prepare available billing countries
        searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
            .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
        searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

        //prepare grid
        searchModel.SetGridPageSize();

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged order list model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order list model
    /// </returns>
    public virtual async Task<OrderListModel> PrepareOrderListModelAsync(OrderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter orders
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
        var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
        var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
            ? searchModel.ProductId : 0;

        //get orders
        var orders = await _orderService.SearchOrdersAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            createdFromUtc: startDateValue,
            createdToUtc: endDateValue,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new OrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            //fill in model values from the entity
            return orders.SelectAwait(async order =>
            {
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                //fill in model values from the entity
                var orderModel = new OrderModel
                {
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                    PaymentStatusId = order.PaymentStatusId,
                    ShippingStatusId = order.ShippingStatusId,
                    CustomerEmail = billingAddress.Email,
                    CustomerFullName = $"{billingAddress.FirstName} {billingAddress.LastName}",
                    CustomerId = order.CustomerId,
                    CustomOrderNumber = order.CustomOrderNumber
                };

                //convert dates to the user time
                orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                return orderModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare order aggregator model
    /// </summary>
    /// <param name="searchModel">Order search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order aggregator model
    /// </returns>
    public virtual async Task<OrderAggreratorModel> PrepareOrderAggregatorModelAsync(OrderSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        if (!_orderSettings.DisplayOrderSummary)
            return null;

        //get parameters to filter orders
        var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
        var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
        var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.VendorId = currentVendor.Id;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
        var filterByProductId = product != null && (currentVendor == null || product.VendorId == currentVendor.Id)
            ? searchModel.ProductId : 0;

        //prepare additional model data
        var reportSummary = await _orderReportService.GetOrderAverageReportLineAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes);

        var profit = await _orderReportService.ProfitReportAsync(storeId: searchModel.StoreId,
            vendorId: searchModel.VendorId,
            productId: filterByProductId,
            warehouseId: searchModel.WarehouseId,
            paymentMethodSystemName: searchModel.PaymentMethodSystemName,
            osIds: orderStatusIds,
            psIds: paymentStatusIds,
            ssIds: shippingStatusIds,
            startTimeUtc: startDateValue,
            endTimeUtc: endDateValue,
            billingPhone: searchModel.BillingPhone,
            billingEmail: searchModel.BillingEmail,
            billingLastName: searchModel.BillingLastName,
            billingCountryId: searchModel.BillingCountryId,
            orderNotes: searchModel.OrderNotes);

        var primaryStoreCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var shippingSum = await _priceFormatter
            .FormatShippingPriceAsync(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, false);
        var taxSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumTax, true, false);
        var totalSum = await _priceFormatter.FormatPriceAsync(reportSummary.SumOrders, true, false);
        var profitSum = await _priceFormatter.FormatPriceAsync(profit, true, false);

        var model = new OrderAggreratorModel
        {
            AggregatorProfit = profitSum,
            AggregatorShipping = shippingSum,
            AggregatorTax = taxSum,
            AggregatorTotal = totalSum
        };

        return model;
    }

    /// <summary>
    /// Prepare order model
    /// </summary>
    /// <param name="model">Order model</param>
    /// <param name="order">Order</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order model
    /// </returns>
    public virtual async Task<OrderModel> PrepareOrderModelAsync(OrderModel model, Order order, bool excludeProperties = false)
    {
        if (order != null)
        {
            //fill in model values from the entity
            model ??= new OrderModel
            {
                Id = order.Id,
                OrderStatusId = order.OrderStatusId,
                VatNumber = order.VatNumber,
                CheckoutAttributeInfo = order.CheckoutAttributeDescription
            };

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            model.OrderGuid = order.OrderGuid;
            model.CustomOrderNumber = order.CustomOrderNumber;
            model.CustomerIp = order.CustomerIp;
            model.CustomerId = customer.Id;
            model.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
            model.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
            model.CustomerInfo = await _customerService.IsRegisteredAsync(customer) ? customer.Email : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
            model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            var affiliate = await _affiliateService.GetAffiliateByIdAsync(order.AffiliateId);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
                model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
            }

            //prepare order totals
            await PrepareOrderModelTotalsAsync(model, order);

            //prepare order items
            await PrepareOrderItemModelsAsync(model.Items, order);
            model.HasDownloadableProducts = model.Items.Any(item => item.IsDownload);

            //prepare payment info
            await PrepareOrderModelPaymentInfoAsync(model, order);

            //prepare shipping info
            await PrepareOrderModelShippingInfoAsync(model, order);

            //prepare nested search model
            PrepareOrderShipmentSearchModel(model.OrderShipmentSearchModel, order);
            PrepareOrderNoteSearchModel(model.OrderNoteSearchModel, order);
        }

        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
        model.TaxDisplayType = _taxSettings.TaxDisplayType;

        return model;
    }

    /// <summary>
    /// Prepare upload license model
    /// </summary>
    /// <param name="model">Upload license model</param>
    /// <param name="order">Order</param>
    /// <param name="orderItem">Order item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the upload license model
    /// </returns>
    public virtual Task<UploadLicenseModel> PrepareUploadLicenseModelAsync(UploadLicenseModel model, Order order, OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(orderItem);

        model.LicenseDownloadId = orderItem.LicenseDownloadId ?? 0;
        model.OrderId = order.Id;
        model.OrderItemId = orderItem.Id;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare product search model to add to the order
    /// </summary>
    /// <param name="searchModel">Product search model to add to the order</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderSearchModel> PrepareAddProductToOrderSearchModelAsync(AddProductToOrderSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        searchModel.OrderId = order.Id;

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model to add to the order
    /// </summary>
    /// <param name="searchModel">Product search model to add to the order</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderListModel> PrepareAddProductToOrderListModelAsync(AddProductToOrderSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddProductToOrderListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product model to add to the order
    /// </summary>
    /// <param name="model">Product model to add to the order</param>
    /// <param name="order">Order</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model to add to the order
    /// </returns>
    public virtual async Task<AddProductToOrderModel> PrepareAddProductToOrderModelAsync(AddProductToOrderModel model, Order order, Product product)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(product);

        var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
        var store = await _storeService.GetStoreByIdAsync(order.StoreId);

        model.ProductId = product.Id;
        model.OrderId = order.Id;
        model.Name = product.Name;
        model.IsRental = product.IsRental;
        model.ProductType = product.ProductType;
        model.AutoUpdateOrderTotals = _orderSettings.AutoUpdateOrderTotalsOnEditingOrder;

        var presetQty = 1;
        var (_, presetPrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(product, customer, store, decimal.Zero, true, presetQty);

        var (presetPriceInclTax, _) = await _taxService.GetProductPriceAsync(product, presetPrice, true, customer);
        var (presetPriceExclTax, _) = await _taxService.GetProductPriceAsync(product, presetPrice, false, customer);
        model.UnitPriceExclTax = presetPriceExclTax;
        model.UnitPriceInclTax = presetPriceInclTax;
        model.Quantity = presetQty;
        model.SubTotalExclTax = presetPriceExclTax;
        model.SubTotalInclTax = presetPriceInclTax;

        //attributes
        await PrepareProductAttributeModelsAsync(model.ProductAttributes, order, product);
        model.HasCondition = model.ProductAttributes.Any(attribute => attribute.HasCondition);

        //gift card
        model.GiftCard.IsGiftCard = product.IsGiftCard;
        if (model.GiftCard.IsGiftCard)
            model.GiftCard.GiftCardType = product.GiftCardType;

        return model;
    }

    /// <summary>
    /// Prepare order address model
    /// </summary>
    /// <param name="model">Order address model</param>
    /// <param name="order">Order</param>
    /// <param name="address">Address</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order address model
    /// </returns>
    public virtual async Task<OrderAddressModel> PrepareOrderAddressModelAsync(OrderAddressModel model, Order order, Address address)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(order);

        ArgumentNullException.ThrowIfNull(address);

        model.OrderId = order.Id;

        //prepare address model
        model.Address = address.ToModel(model.Address);
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        SetAddressFieldsAsRequired(model.Address);

        return model;
    }

    /// <summary>
    /// Prepare shipment search model
    /// </summary>
    /// <param name="searchModel">Shipment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment search model
    /// </returns>
    public virtual async Task<ShipmentSearchModel> PrepareShipmentSearchModelAsync(ShipmentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available countries
        await _baseAdminModelFactory.PrepareCountriesAsync(searchModel.AvailableCountries);

        //prepare available states and provinces
        await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(searchModel.AvailableStates, searchModel.CountryId);

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        //prepare nested search model
        PrepareShipmentItemSearchModel(searchModel.ShipmentItemSearchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged shipment list model
    /// </summary>
    /// <param name="searchModel">Shipment search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment list model
    /// </returns>
    public virtual async Task<ShipmentListModel> PrepareShipmentListModelAsync(ShipmentSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter shipments
        var vendor = await _workContext.GetCurrentVendorAsync();
        var vendorId = vendor?.Id ?? 0;
        var startDateValue = !searchModel.StartDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = !searchModel.EndDate.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get shipments
        var shipments = await _shipmentService.GetAllShipmentsAsync(vendorId,
            searchModel.WarehouseId,
            searchModel.CountryId,
            searchModel.StateProvinceId,
            searchModel.County,
            searchModel.City,
            searchModel.TrackingNumber,
            searchModel.LoadNotShipped,
            searchModel.LoadNotReadyForPickup,
            searchModel.LoadNotDelivered,
            0,
            startDateValue,
            endDateValue,
            searchModel.Page - 1,
            searchModel.PageSize);

        //prepare list model
        var model = await new ShipmentListModel().PrepareToGridAsync(searchModel, shipments, () =>
        {
            //fill in model values from the entity
            return shipments.SelectAwait(async shipment => await PrepareShipmentModelAsync(shipment));
        });

        return model;
    }

    /// <summary>
    /// Prepare shipment model
    /// </summary>
    /// <param name="model">Shipment model</param>
    /// <param name="shipment">Shipment</param>
    /// <param name="order">Order</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment model
    /// </returns>
    public virtual async Task<ShipmentModel> PrepareShipmentModelAsync(ShipmentModel model, Shipment shipment, Order order,
        bool excludeProperties = false)
    {
        if (shipment != null)
        {
            //fill in model values from the entity
            model = await PrepareShipmentModelAsync(shipment, model);

            //prepare shipment items
            foreach (var item in await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id))
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //fill in model values from the entity
                var shipmentItemModel = new ShipmentItemModel
                {
                    Id = item.Id,
                    QuantityInThisShipment = item.Quantity,
                    ShippedFromWarehouse = (await _shippingService.GetWarehouseByIdAsync(item.WarehouseId))?.Name
                };

                await PrepareShipmentItemModelAsync(shipmentItemModel, orderItem, product);

                model.Items.Add(shipmentItemModel);
            }

            //prepare shipment events
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                var shipmentTracker = await _shipmentService.GetShipmentTrackerAsync(shipment);
                if (shipmentTracker != null)
                {
                    model.TrackingNumberUrl = await shipmentTracker.GetUrlAsync(shipment.TrackingNumber, shipment);
                    if (_shippingSettings.DisplayShipmentEventsToStoreOwner)
                        await PrepareShipmentStatusEventModelsAsync(model.ShipmentStatusEvents, shipment);
                }
            }
        }

        if (shipment != null)
            return model;

        model.OrderId = order.Id;
        model.PickupInStore = order.PickupInStore;
        model.CustomOrderNumber = order.CustomOrderNumber;

        var vendor = await _workContext.GetCurrentVendorAsync();
        var orderItems = (await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true, vendorId: vendor?.Id ?? 0)).ToList();

        foreach (var orderItem in orderItems)
        {
            var shipmentItemModel = new ShipmentItemModel();

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            await PrepareShipmentItemModelAsync(shipmentItemModel, orderItem, product);

            //ensure that this product can be added to a shipment
            if (shipmentItemModel.QuantityToAdd <= 0)
                continue;

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.UseMultipleWarehouses)
            {
                //multiple warehouses supported
                shipmentItemModel.AllowToChooseWarehouse = true;
                foreach (var pwi in (await _productService.GetAllProductWarehouseInventoryRecordsAsync(orderItem.ProductId)).OrderBy(w => w.WarehouseId).ToList())
                {
                    if (await _shippingService.GetWarehouseByIdAsync(pwi.WarehouseId) is Warehouse warehouse)
                    {
                        shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                        {
                            WarehouseId = warehouse.Id,
                            WarehouseName = warehouse.Name,
                            StockQuantity = pwi.StockQuantity,
                            ReservedQuantity = pwi.ReservedQuantity,
                            PlannedQuantity =
                                await _shipmentService.GetQuantityInShipmentsAsync(product, warehouse.Id, true, true)
                        });
                    }
                }
            }
            else
            {
                //multiple warehouses are not supported
                var warehouse = await _shippingService.GetWarehouseByIdAsync(product.WarehouseId);
                if (warehouse != null)
                {
                    shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                    {
                        WarehouseId = warehouse.Id,
                        WarehouseName = warehouse.Name,
                        StockQuantity = product.StockQuantity
                    });
                }
            }

            model.Items.Add(shipmentItemModel);
        }

        return model;
    }

    /// <summary>
    /// Prepare paged order shipment list model
    /// </summary>
    /// <param name="searchModel">Order shipment search model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order shipment list model
    /// </returns>
    public virtual async Task<OrderShipmentListModel> PrepareOrderShipmentListModelAsync(OrderShipmentSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        var vendor = await _workContext.GetCurrentVendorAsync();
        //get shipments
        var shipments = (await _shipmentService.GetAllShipmentsAsync(
                orderId: order.Id,
                //a vendor should have access only to his products
                vendorId: vendor?.Id ?? 0))
            .OrderBy(shipment => shipment.CreatedOnUtc)
            .ToList();

        var pagedShipments = shipments.ToPagedList(searchModel);

        //prepare list model
        var model = await new OrderShipmentListModel().PrepareToGridAsync(searchModel, pagedShipments, () =>
        {
            //fill in model values from the entity
            return pagedShipments.SelectAwait(async shipment => await PrepareShipmentModelAsync(shipment));
        });

        return model;
    }

    /// <summary>
    /// Prepare paged shipment item list model
    /// </summary>
    /// <param name="searchModel">Shipment item search model</param>
    /// <param name="shipment">Shipment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment item list model
    /// </returns>
    public virtual async Task<ShipmentItemListModel> PrepareShipmentItemListModelAsync(ShipmentItemSearchModel searchModel, Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(shipment);

        //get shipments
        var shipmentItems = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = await new ShipmentItemListModel().PrepareToGridAsync(searchModel, shipmentItems, () =>
        {
            //fill in model values from the entity
            return shipmentItems.SelectAwait(async item =>
            {
                //fill in model values from the entity
                var shipmentItemModel = new ShipmentItemModel
                {
                    Id = item.Id,
                    QuantityInThisShipment = item.Quantity
                };

                //fill in additional values (not existing in the entity)
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId);
                if (orderItem == null)
                    return shipmentItemModel;

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                shipmentItemModel.OrderItemId = orderItem.Id;
                shipmentItemModel.ProductId = orderItem.ProductId;
                shipmentItemModel.ProductName = product.Name;

                shipmentItemModel.ShippedFromWarehouse = (await _shippingService.GetWarehouseByIdAsync(item.WarehouseId))?.Name;

                var baseWeight = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId))?.Name;
                var baseDimension = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId))?.Name;

                if (orderItem.ItemWeight.HasValue)
                    shipmentItemModel.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";

                shipmentItemModel.ItemDimensions =
                    $"{product.Length:F2} x {product.Width:F2} x {product.Height:F2} [{baseDimension}]";

                return shipmentItemModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged order note list model
    /// </summary>
    /// <param name="searchModel">Order note search model</param>
    /// <param name="order">Order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order note list model
    /// </returns>
    public virtual async Task<OrderNoteListModel> PrepareOrderNoteListModelAsync(OrderNoteSearchModel searchModel, Order order)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(order);

        //get notes
        var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id)).OrderByDescending(on => on.CreatedOnUtc).ToList().ToPagedList(searchModel);

        //prepare list model
        var model = await new OrderNoteListModel().PrepareToGridAsync(searchModel, orderNotes, () =>
        {
            //fill in model values from the entity
            return orderNotes.SelectAwait(async orderNote =>
            {
                //fill in model values from the entity
                var orderNoteModel = orderNote.ToModel<OrderNoteModel>();

                //convert dates to the user time
                orderNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderNoteModel.Note = _orderService.FormatOrderNoteText(orderNote);

                orderNoteModel.DownloadGuid = (await _downloadService.GetDownloadByIdAsync(orderNote.DownloadId))?.DownloadGuid ?? Guid.Empty;

                return orderNoteModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare bestseller brief search model
    /// </summary>
    /// <param name="searchModel">Bestseller brief search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller brief search model
    /// </returns>
    public virtual Task<BestsellerBriefSearchModel> PrepareBestsellerBriefSearchModelAsync(BestsellerBriefSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize(5);

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged bestseller brief list model
    /// </summary>
    /// <param name="searchModel">Bestseller brief search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the bestseller brief list model
    /// </returns>
    public virtual async Task<BestsellerBriefListModel> PrepareBestsellerBriefListModelAsync(BestsellerBriefSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        var vendor = await _workContext.GetCurrentVendorAsync();
        //get bestsellers
        var bestsellers = await _orderReportService.BestSellersReportAsync(showHidden: true,
            vendorId: vendor?.Id ?? 0,
            orderBy: searchModel.OrderBy,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new BestsellerBriefListModel().PrepareToGridAsync(searchModel, bestsellers, () =>
        {
            //fill in model values from the entity
            return bestsellers.SelectAwait(async bestseller =>
            {
                //fill in model values from the entity
                var bestsellerModel = new BestsellerModel
                {
                    ProductId = bestseller.ProductId,
                    TotalQuantity = bestseller.TotalQuantity,
                    ProductName = bestseller.ProductName
                };

                //fill in additional values (not existing in the entity)
                bestsellerModel.TotalAmount = await _priceFormatter.FormatPriceAsync(bestseller.TotalAmount, true, false);

                return bestsellerModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare order average line summary report list model
    /// </summary>
    /// <param name="searchModel">Order average line summary report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the order average line summary report list model
    /// </returns>
    public virtual async Task<OrderAverageReportListModel> PrepareOrderAverageReportListModelAsync(OrderAverageReportSearchModel searchModel)
    {
        //get report
        var report = new List<OrderAverageReportLineSummary>
        {
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Pending),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Processing),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Complete),
            await _orderReportService.OrderAverageReportAsync(0, OrderStatus.Cancelled)
        };

        var pagedList = new PagedList<OrderAverageReportLineSummary>(report, 0, int.MaxValue);

        //prepare list model
        var model = await new OrderAverageReportListModel().PrepareToGridAsync(searchModel, pagedList, () =>
        {
            //fill in model values from the entity
            return pagedList.SelectAwait(async reportItem => new OrderAverageReportModel
            {
                OrderStatus = await _localizationService.GetLocalizedEnumAsync(reportItem.OrderStatus),
                SumTodayOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumTodayOrders, true, false),
                SumThisWeekOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisWeekOrders, true, false),
                SumThisMonthOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisMonthOrders, true, false),
                SumThisYearOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumThisYearOrders, true, false),
                SumAllTimeOrders = await _priceFormatter.FormatPriceAsync(reportItem.SumAllTimeOrders, true, false)
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare incomplete order report list model
    /// </summary>
    /// <param name="searchModel">Incomplete order report search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the incomplete order report list model
    /// </returns>
    public virtual async Task<OrderIncompleteReportListModel> PrepareOrderIncompleteReportListModelAsync(OrderIncompleteReportSearchModel searchModel)
    {
        var orderIncompleteReportModels = new List<OrderIncompleteReportModel>();

        //get URL helper
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        //not paid
        var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().Where(os => os != (int)OrderStatus.Cancelled).ToList();
        var paymentStatuses = new List<int> { (int)PaymentStatus.Pending };
        var psPending = await _orderReportService.GetOrderAverageReportLineAsync(psIds: paymentStatuses, osIds: orderStatuses);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
            Count = psPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(psPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new
            {
                orderStatuses = string.Join(",", orderStatuses),
                paymentStatuses = string.Join(",", paymentStatuses)
            })
        });

        //not shipped
        var shippingStatuses = new List<int> { (int)ShippingStatus.NotYetShipped };
        var ssPending = await _orderReportService.GetOrderAverageReportLineAsync(osIds: orderStatuses, ssIds: shippingStatuses);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
            Count = ssPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(ssPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new
            {
                orderStatuses = string.Join(",", orderStatuses),
                shippingStatuses = string.Join(",", shippingStatuses)
            })
        });

        //pending
        orderStatuses = [(int)OrderStatus.Pending];
        var osPending = await _orderReportService.GetOrderAverageReportLineAsync(osIds: orderStatuses);
        orderIncompleteReportModels.Add(new OrderIncompleteReportModel
        {
            Item = await _localizationService.GetResourceAsync("Admin.SalesReport.Incomplete.TotalIncompleteOrders"),
            Count = osPending.CountOrders,
            Total = await _priceFormatter.FormatPriceAsync(osPending.SumOrders, true, false),
            ViewLink = urlHelper.Action("List", "Order", new { orderStatuses = string.Join(",", orderStatuses) })
        });

        var pagedList = new PagedList<OrderIncompleteReportModel>(orderIncompleteReportModels, 0, int.MaxValue);

        //prepare list model
        var model = new OrderIncompleteReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);
        return model;
    }

    #endregion
}