using System.IO.Compression;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common.Pdf;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Nop.Services.Common;

/// <summary>
/// PDF service
/// </summary>
public partial class PdfService : IPdfService
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressService _addressService;
    protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IGiftCardService _giftCardService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureService _measureService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPictureService _pictureService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IProductService _productService;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly ISettingService _settingService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IVendorService _vendorService;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public PdfService(AddressSettings addressSettings,
        CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IAddressService addressService,
        IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        ICountryService countryService,
        ICurrencyService currencyService,
        IDateTimeHelper dateTimeHelper,
        IGiftCardService giftCardService,
        IHtmlFormatter htmlFormatter,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IMeasureService measureService,
        INopFileProvider fileProvider,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IPictureService pictureService,
        IPriceFormatter priceFormatter,
        IProductService productService,
        IRewardPointService rewardPointService,
        ISettingService settingService,
        IShipmentService shipmentService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreService storeService,
        IVendorService vendorService,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _addressSettings = addressSettings;
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _addressService = addressService;
        _addressAttributeFormatter = addressAttributeFormatter;
        _countryService = countryService;
        _currencyService = currencyService;
        _dateTimeHelper = dateTimeHelper;
        _giftCardService = giftCardService;
        _htmlFormatter = htmlFormatter;
        _languageService = languageService;
        _localizationService = localizationService;
        _measureService = measureService;
        _fileProvider = fileProvider;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pictureService = pictureService;
        _priceFormatter = priceFormatter;
        _productService = productService;
        _rewardPointService = rewardPointService;
        _settingService = settingService;
        _shipmentService = shipmentService;
        _storeContext = storeContext;
        _stateProvinceService = stateProvinceService;
        _storeService = storeService;
        _vendorService = vendorService;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get billing address
    /// </summary>
    /// <param name="vendor">Vendor</param>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains address item</returns>
    protected virtual async Task<AddressItem> GetBillingAddressAsync(Vendor vendor, Language lang, Order order)
    {
        var addressResult = new AddressItem();

        var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

        if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(billingAddress.Company))
            addressResult.Company = billingAddress.Company;

        addressResult.Name = $"{billingAddress.FirstName} {billingAddress.LastName}";

        if (_addressSettings.PhoneEnabled)
            addressResult.Phone = billingAddress.PhoneNumber;

        if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(billingAddress.FaxNumber))
            addressResult.Fax = billingAddress.FaxNumber;

        if (_addressSettings.StreetAddressEnabled)
            addressResult.Address = billingAddress.Address1;

        if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(billingAddress.Address2))
            addressResult.Address2 = billingAddress.Address2;

        if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(billingAddress.City))
            addressResult.City = billingAddress.City;

        if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(billingAddress.County))
            addressResult.County = billingAddress.County;

        if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(billingAddress.ZipPostalCode))
            addressResult.ZipPostalCode = billingAddress.ZipPostalCode;

        var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(billingAddress);
        addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

        if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(billingAddress) is Country country)
            addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);

        var (addressLine, _) = await _addressService.FormatAddressAsync(billingAddress, lang.Id);
        addressResult.AddressLine = addressLine;

        //VAT number
        if (!string.IsNullOrEmpty(order.VatNumber))
            addressResult.VATNumber = order.VatNumber;

        //custom attributes
        var customBillingAddressAttributes = await _addressAttributeFormatter
            .FormatAttributesAsync(billingAddress.CustomAttributes, "<br />");

        if (!string.IsNullOrEmpty(customBillingAddressAttributes))
        {
            var text = _htmlFormatter.ConvertHtmlToPlainText(customBillingAddressAttributes, true, true);
            addressResult.AddressAttributes = text.Split('\n').ToList();
        }

        //vendors payment details
        if (vendor is null)
        {
            //payment method
            var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
            var paymentMethodStr = paymentMethod != null
                ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, lang.Id)
                : order.PaymentMethodSystemName;
            if (!string.IsNullOrEmpty(paymentMethodStr))
            {
                addressResult.PaymentMethod = paymentMethodStr;
            }

            //custom values
            var customValues = _paymentService.DeserializeCustomValues(order);
            if (customValues != null)
                addressResult.CustomValues = customValues;
        }

        return addressResult;
    }

    /// <summary>
    /// Get shipping address
    /// </summary>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains address item</returns>
    protected virtual async Task<AddressItem> GetShippingAddressAsync(Language lang, Order order)
    {
        var addressResult = new AddressItem();

        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            if (!order.PickupInStore)
            {
                if (order.ShippingAddressId == null || await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is not Address shippingAddress)
                    throw new NopException($"Shipping is required, but address is not available. Order ID = {order.Id}");

                if (!string.IsNullOrEmpty(shippingAddress.Company))
                    addressResult.Company = shippingAddress.Company;

                addressResult.Name = $"{shippingAddress.FirstName} {shippingAddress.LastName}";

                if (_addressSettings.PhoneEnabled)
                    addressResult.Phone = shippingAddress.PhoneNumber;

                if (_addressSettings.FaxEnabled && !string.IsNullOrEmpty(shippingAddress.FaxNumber))
                    addressResult.Fax = shippingAddress.FaxNumber;

                if (_addressSettings.StreetAddressEnabled)
                    addressResult.Address = shippingAddress.Address1;

                if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAddress.Address2))
                    addressResult.Address2 = shippingAddress.Address2;

                if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(shippingAddress.City))
                    addressResult.City = shippingAddress.City;

                if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(shippingAddress.County))
                    addressResult.County = shippingAddress.County;

                if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(shippingAddress.ZipPostalCode))
                    addressResult.ZipPostalCode = shippingAddress.ZipPostalCode;

                var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(shippingAddress);
                addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

                if (_addressSettings.CountryEnabled && await _countryService.GetCountryByAddressAsync(shippingAddress) is Country country)
                {
                    addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
                }

                var (addressLine, _) = await _addressService.FormatAddressAsync(shippingAddress, lang.Id);
                addressResult.AddressLine = addressLine;

                //custom attributes
                var customShippingAddressAttributes = await _addressAttributeFormatter
                    .FormatAttributesAsync(shippingAddress.CustomAttributes, "<br />");
                if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                {
                    var text = _htmlFormatter.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true);
                    addressResult.AddressAttributes = text.Split('\n').ToList();
                }
            }
            else if (order.PickupAddressId.HasValue && await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
            {
                if (!string.IsNullOrEmpty(pickupAddress.Address1))
                    addressResult.Address = pickupAddress.Address1;

                if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(pickupAddress.Address2))
                    addressResult.Address2 = pickupAddress.Address2;

                if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(pickupAddress.City))
                    addressResult.City = pickupAddress.City;

                if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(pickupAddress.County))
                    addressResult.County = pickupAddress.County;

                if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(pickupAddress.ZipPostalCode))
                    addressResult.ZipPostalCode = pickupAddress.ZipPostalCode;

                var (addressLine, _) = await _addressService.FormatAddressAsync(pickupAddress, lang.Id);
                addressResult.AddressLine = addressLine;

                var stateProvince = await _stateProvinceService.GetStateProvinceByAddressAsync(pickupAddress);
                addressResult.StateProvinceName = stateProvince != null ? await _localizationService.GetLocalizedAsync(stateProvince, x => x.Name, lang.Id) : string.Empty;

                if (await _countryService.GetCountryByAddressAsync(pickupAddress) is Country country)
                    addressResult.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name, lang.Id);
            }

            addressResult.ShippingMethod = order.ShippingMethod;
        }

        return addressResult;
    }

    /// <summary>
    /// Get order notes
    /// </summary>
    /// <param name="pdfSettingsByStore">PDF settings</param>
    /// <param name="order">Order</param>
    /// <param name="lang">Language</param>
    /// <returns>A task that contains collection of date/note pairs</returns>
    protected virtual async Task<List<(string, string)>> GetOrderNotesAsync(PdfSettings pdfSettingsByStore, Order order, Language lang)
    {
        var notesResult = new List<(string, string)>();

        if (!pdfSettingsByStore.RenderOrderNotes)
            return notesResult;

        var orderNotes = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id, true))
            .OrderByDescending(on => on.CreatedOnUtc)
            .ToList();

        if (!orderNotes.Any())
            return notesResult;

        foreach (var orderNote in orderNotes)
        {
            var createdOn = (await _dateTimeHelper.ConvertToUserTimeAsync(orderNote.CreatedOnUtc, DateTimeKind.Utc)).ToString();
            var note = _htmlFormatter.ConvertHtmlToPlainText(_orderService.FormatOrderNoteText(orderNote), true, true);

            notesResult.Add((createdOn, note));

            //should we display a link to downloadable files here?
            //I think, no. Anyway, PDFs are printable documents and links (files) are useful here
        }

        return notesResult;
    }

    /// <summary>
    /// Get product entries for document data source
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="orderItems">Collection of order items</param>
    /// <param name="language">Language</param>
    /// <param name="shipmentItems">Collection of shipment items; when using to prepare shipment items</param>
    /// <returns>A task that contains collection of product entries</returns>
    protected virtual async Task<List<ProductItem>> GetOrderProductItemsAsync(Order order, IList<OrderItem> orderItems, Language language, IList<ShipmentItem> shipmentItems = null)
    {
        var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? await _vendorService.GetVendorsByProductIdsAsync(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

        var result = new List<ProductItem>();

        foreach (var oi in orderItems)
        {
            var productItem = new ProductItem();
            var product = await _productService.GetProductByIdAsync(oi.ProductId);

            //product name
            productItem.Name = await _localizationService.GetLocalizedAsync(product, x => x.Name, language.Id);

            //attributes
            if (!string.IsNullOrEmpty(oi.AttributeDescription))
            {
                var attributes = _htmlFormatter.ConvertHtmlToPlainText(oi.AttributeDescription, true, true);
                productItem.ProductAttributes = attributes.Split('\n').ToList();
            }

            //SKU
            if (_catalogSettings.ShowSkuOnProductDetailsPage)
                productItem.Sku = await _productService.FormatSkuAsync(product, oi.AttributesXml);

            //Vendor name
            if (_vendorSettings.ShowVendorOnOrderDetailsPage)
                productItem.VendorName = vendors.FirstOrDefault(v => v.Id == product.VendorId)?.Name ?? string.Empty;

            //price
            string unitPrice;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.UnitPriceInclTax, order.CurrencyRate);
                unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, language.Id, true);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.UnitPriceExclTax, order.CurrencyRate);
                unitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true,
                    order.CustomerCurrencyCode, language.Id, false);
            }

            productItem.Price = unitPrice;

            //qty
            productItem.Quantity = shipmentItems is null ?
                oi.Quantity.ToString() :
                shipmentItems.FirstOrDefault(x => x.OrderItemId == oi.Id).Quantity.ToString();

            //total
            string subTotal;
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var priceInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.PriceInclTax, order.CurrencyRate);
                subTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    language.Id, true);
            }
            else
            {
                //excluding tax
                var priceExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(oi.PriceExclTax, order.CurrencyRate);
                subTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    language.Id, false);
            }

            productItem.Total = subTotal;

            result.Add(productItem);
        }

        return result;
    }

    /// <summary>
    /// Get invoice totals
    /// </summary>
    /// <param name="lang">Language</param>
    /// <param name="order">Order</param>
    /// <returns>A task that contains invoice totals</returns>
    protected virtual async Task<InvoiceTotals> GetTotalsAsync(Language lang, Order order)
    {
        var result = new InvoiceTotals();
        var languageId = lang.Id;

        //order subtotal
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
            !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
        {
            //including tax
            var orderSubtotalInclTaxInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
            result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true,
                order.CustomerCurrencyCode, languageId, true);
        }
        else
        {
            //excluding tax
            var orderSubtotalExclTaxInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
            result.SubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true,
                order.CustomerCurrencyCode, languageId, false);
        }

        //discount (applied to order subtotal)
        if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
        {
            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax &&
                !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax
                var orderSubTotalDiscountInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(
                    -orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderSubTotalDiscountExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                result.Discount = await _priceFormatter.FormatPriceAsync(
                    -orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //shipping
        if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
        {
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var orderShippingInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                    orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var orderShippingExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                result.Shipping = await _priceFormatter.FormatShippingPriceAsync(
                    orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //payment fee
        if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
        {
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                    paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency =
                    _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                result.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(
                    paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }
        }

        //tax
        var taxStr = string.Empty;
        var taxRates = new SortedDictionary<decimal, decimal>();
        bool displayTax;
        var displayTaxRates = true;
        if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            displayTax = false;
        }
        else
        {
            if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                taxRates = _orderService.ParseTaxRates(order, order.TaxRates);

                displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                displayTax = !displayTaxRates;

                var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    false, languageId);
            }
        }

        if (displayTax)
        {
            result.Tax = taxStr;
        }

        if (displayTaxRates)
        {
            foreach (var item in taxRates)
            {
                var taxRate = string.Format(await _localizationService.GetResourceAsync("Pdf.TaxRate", languageId),
                    _priceFormatter.FormatTaxRate(item.Key));
                var taxValue = await _priceFormatter.FormatPriceAsync(
                    _currencyService.ConvertCurrency(item.Value, order.CurrencyRate), true, order.CustomerCurrencyCode,
                    false, languageId);

                result.TaxRates.Add($"{taxRate} {taxValue}");
            }
        }

        //discount (applied to order total)
        if (order.OrderDiscount > decimal.Zero)
        {
            var orderDiscountInCustomerCurrency =
                _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            result.Discount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency,
                true, order.CustomerCurrencyCode, false, languageId);
        }

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            var gcTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.GiftCardInfo", languageId),
                (await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode);
            var gcAmountStr = await _priceFormatter.FormatPriceAsync(
                -_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true,
                order.CustomerCurrencyCode, false, languageId);

            result.GiftCards.Add($"{gcTitle} {gcAmountStr}");
        }

        //reward points
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            var rpTitle = string.Format(await _localizationService.GetResourceAsync("Pdf.RewardPoints", languageId),
                -redeemedRewardPointsEntry.Points);
            var rpAmount = await _priceFormatter.FormatPriceAsync(
                -_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate),
                true, order.CustomerCurrencyCode, false, languageId);

            result.RewardPoints = $"{rpTitle} {rpAmount}";
        }

        //order total
        var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
        var orderTotalStr = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
        result.OrderTotal = $"{await _localizationService.GetResourceAsync("Pdf.OrderTotal", languageId)} {orderTotalStr}";

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Write PDF invoice to the specified stream
    /// </summary>
    /// <param name="stream">Stream to save PDF</param>
    /// <param name="order">Order</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <param name="store">Store</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null, Vendor vendor = null)
    {
        ArgumentNullException.ThrowIfNull(order);

        //store info
        store ??= await _storeContext.GetCurrentStoreAsync();

        var orderStore = order.StoreId == 0 || order.StoreId == store?.Id ?
            store : await _storeService.GetStoreByIdAsync(order.StoreId);

        //language info
        language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

        if (language?.Published != true)
            language = await _workContext.GetWorkingLanguageAsync();

        //by default _pdfSettings contains settings for the current active store
        //and we need PdfSettings for the store which was used to place an order
        //so let's load it based on a store of the current order
        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(orderStore.Id);

        byte[] logo = null;
        var logoPicture = await _pictureService.GetPictureByIdAsync(pdfSettingsByStore.LogoPictureId);
        if (logoPicture != null)
        {
            logo = await _pictureService.LoadPictureBinaryAsync(logoPicture);

            if (logoPicture.MimeType == MimeTypes.ImageSvg)
            {
                await using var logoStream = new MemoryStream(logo);
                logo = await _pictureService.ConvertSvgToPngAsync(logoStream);
            }
        }

        var date = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

        //a vendor should have access only to products
        var orderItems = await _orderService.GetOrderItemsAsync(order.Id, vendorId: vendor?.Id ?? 0);

        var column1Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn1) ?
            new List<string>()
            : pdfSettingsByStore.InvoiceFooterTextColumn1
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var column2Lines = string.IsNullOrEmpty(pdfSettingsByStore.InvoiceFooterTextColumn2) ?
            new List<string>()
            : pdfSettingsByStore.InvoiceFooterTextColumn2
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        var source = new InvoiceSource()
        {
            StoreUrl = orderStore.Url?.Trim('/'),
            Language = language,
            FontFamily = pdfSettingsByStore.FontFamily,
            OrderDateUser = date,
            LogoData = logo,
            OrderNumberText = order.CustomOrderNumber,
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
            BillingAddress = await GetBillingAddressAsync(vendor, language, order),
            ShippingAddress = await GetShippingAddressAsync(language, order),
            Products = await GetOrderProductItemsAsync(order, orderItems, language),
            ShowSkuInProductList = _catalogSettings.ShowSkuOnProductDetailsPage,
            ShowVendorInProductList = _vendorSettings.ShowVendorOnOrderDetailsPage,
            CheckoutAttributes = vendor is null ? _htmlFormatter.ConvertHtmlToPlainText(order.CheckoutAttributeDescription, true, true) : string.Empty, //vendors cannot see checkout attributes
            Totals = vendor is null ? await GetTotalsAsync(language, order) : new(), //vendors cannot see totals
            OrderNotes = await GetOrderNotesAsync(pdfSettingsByStore, order, language),
            FooterTextColumn1 = column1Lines,
            FooterTextColumn2 = column2Lines
        };

        await using var pdfStream = new MemoryStream();
        new InvoiceDocument(source, _localizationService)
            .GeneratePdf(pdfStream);

        pdfStream.Position = 0;
        await pdfStream.CopyToAsync(stream);
    }

    /// <summary>
    /// Write ZIP archive with invoices to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="orders">Orders</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null, Vendor vendor = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(orders);

        var currentStore = await _storeContext.GetCurrentStoreAsync();

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

        foreach (var order in orders)
        {
            var entryName = string.Format("{0} {1}", await _localizationService.GetResourceAsync("Pdf.Order"), order.CustomOrderNumber);

            await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
            await using var pdfStream = new MemoryStream();
            await PrintOrderToPdfAsync(pdfStream, order, language, currentStore, vendor);
            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(fileStreamInZip);
        }
    }

    /// <summary>
    /// Write ZIP archive with packaging slips to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="shipments">Shipments</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintPackagingSlipsToPdfAsync(Stream stream, IList<Shipment> shipments, Language language = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(shipments);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);

        foreach (var shipment in shipments)
        {
            var entryName = $"{await _localizationService.GetResourceAsync("Pdf.Shipment")}{shipment.Id}";

            await using var fileStreamInZip = archive.CreateEntry($"{entryName}.pdf").Open();
            await using var pdfStream = new MemoryStream();
            await PrintPackagingSlipToPdfAsync(pdfStream, shipment, language);

            pdfStream.Position = 0;
            await pdfStream.CopyToAsync(fileStreamInZip);
        }
    }

    /// <summary>
    /// Write packaging slip to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="shipment">Shipment</param>
    /// <param name="language">Language; null to use a language used when placing an order</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(shipment);

        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(order.StoreId);

        //language info
        language ??= await _languageService.GetLanguageByIdAsync(order.CustomerLanguageId);

        if (language?.Published != true)
            language = await _workContext.GetWorkingLanguageAsync();

        var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

        if (shipmentItems?.Any() != true)
            return;

        var orderItems = await shipmentItems
            .SelectAwait(async si => await _orderService.GetOrderItemByIdAsync(si.OrderItemId))
            .Where(pi => pi != null)
            .ToListAsync();

        if (orderItems?.Any() != true)
            return;

        var source = new ShipmentSource
        {
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
            Language = language,
            FontFamily = pdfSettingsByStore.FontFamily,
            ShipmentNumberText = shipment.Id.ToString(),
            OrderNumberText = order.CustomOrderNumber,
            Address = await GetShippingAddressAsync(language, order),
            Products = await GetOrderProductItemsAsync(order, orderItems, language, shipmentItems)
        };

        await using var pdfStream = new MemoryStream();

        new ShipmentDocument(source, _localizationService)
            .GeneratePdf(pdfStream);

        pdfStream.Position = 0;
        await pdfStream.CopyToAsync(stream);
    }

    /// <summary>
    /// Write PDF catalog to the specified stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="products">Products</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrintProductsToPdfAsync(Stream stream, IList<Product> products)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ArgumentNullException.ThrowIfNull(products);

        var currentStore = await _storeContext.GetCurrentStoreAsync();
        var pdfSettingsByStore = await _settingService.LoadSettingAsync<PdfSettings>(currentStore.Id);
        var lang = await _workContext.GetWorkingLanguageAsync();

        var productItems = new List<CatalogItem>();

        foreach (var product in products)
        {
            var priceStr = $"{product.Price:0.00} {(await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode}";
            if (product.IsRental)
                priceStr = await _priceFormatter.FormatRentalProductPeriodAsync(product, priceStr);

            var rawDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, lang.Id);

            var productNumber = products.IndexOf(product) + 1;
            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, lang.Id);

            var item = new CatalogItem()
            {
                Name = $"{productNumber}. {productName}",
                Description = _htmlFormatter.StripTags(_htmlFormatter.ConvertHtmlToPlainText(rawDescription, decode: true)),
                Price = priceStr,
                Sku = product.Sku,
                Weight = product.IsShipEnabled && product.Weight > decimal.Zero ?
                    $"{product.Weight:0.00} {(await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name}" :
                    string.Empty,
                Stock = product.ManageInventoryMethod == ManageInventoryMethod.ManageStock ?
                    $"{await _productService.GetTotalStockQuantityAsync(product)}" :
                    string.Empty
            };

            var pictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);

            if (pictures.Any())
            {
                var picturePaths = new HashSet<string>();

                foreach (var pic in pictures)
                {
                    var picPath = await _pictureService.GetThumbLocalPathAsync(pic, 200, false);
                    if (!string.IsNullOrEmpty(picPath))
                    {
                        picturePaths.Add(picPath);
                    }
                }

                item.PicturePaths = picturePaths;
            }

            productItems.Add(item);
        }

        var source = new CatalogSource
        {
            Language = lang,
            PageSize = pdfSettingsByStore.LetterPageSizeEnabled ? PageSizes.Letter : PageSizes.A4,
            FontFamily = pdfSettingsByStore.FontFamily,
            Products = productItems
        };

        await using var pdfStream = new MemoryStream();

        new CatalogDocument(source, _localizationService)
            .GeneratePdf(pdfStream);

        pdfStream.Position = 0;
        await pdfStream.CopyToAsync(stream);
    }

    /// <summary>
    /// Export an order to PDF and save to disk
    /// </summary>
    /// <param name="order">Order</param>
    /// <param name="language">Language identifier; null to use a language used when placing an order</param>
    /// <param name="vendor">Vendor to limit products; null to print all products. If specified, then totals won't be printed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a path of generated file
    /// </returns>
    public virtual async Task<string> SaveOrderPdfToDiskAsync(Order order, Language language = null, Vendor vendor = null)
    {
        var fileName = $"order_{order.OrderGuid}_{CommonHelper.GenerateRandomDigitCode(4)}.pdf";
        var filePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot/files/exportimport"), fileName);
        await using var fileStream = new FileStream(filePath, FileMode.Create);

        await PrintOrderToPdfAsync(fileStream, order, language, store: null, vendor: vendor);

        return filePath;
    }

    #endregion
}