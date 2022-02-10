using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyPost;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Shipping.EasyPost.Domain;
using Nop.Plugin.Shipping.EasyPost.Domain.Batch;
using Nop.Plugin.Shipping.EasyPost.Domain.Shipment;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Services.Stores;

namespace Nop.Plugin.Shipping.EasyPost.Services
{
    /// <summary>
    /// Represents plugin service
    /// </summary>
    public class EasyPostService
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly EasyPostSettings _easyPostSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IRepository<EasyPostBatch> _batchRepository;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly Nop.Core.Domain.Shipping.ShippingSettings _shippingSettings;

        private static bool? _isConfigured;

        #endregion

        #region Ctor

        public EasyPostService(CurrencySettings currencySettings,
            EasyPostSettings easyPostSettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IMeasureService measureService,
            INotificationService notificationService,
            IOrderService orderService,
            IProductService productService,
            IRepository<EasyPostBatch> batchRepository,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            Nop.Core.Domain.Shipping.ShippingSettings shippingSettings)
        {
            _currencySettings = currencySettings;
            _easyPostSettings = easyPostSettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _measureService = measureService;
            _notificationService = notificationService;
            _orderService = orderService;
            _productService = productService;
            _batchRepository = batchRepository;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _shippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the client is configured
        /// </summary>
        /// <returns>Result</returns>
        private static bool IsConfigured()
        {
            if (!_isConfigured.HasValue)
            {
                var settings = EngineContext.Current.Resolve<EasyPostSettings>();
                var key = settings.UseSandbox ? settings.TestApiKey : settings.ApiKey;
                ClientManager.SetCurrent(key);
                _isConfigured = !string.IsNullOrEmpty(key);
            }

            return _isConfigured.Value;
        }

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function to invoke</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error if exists
        /// </returns>
        private async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                return (await function(), default);
            }
            catch (Exception exception)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var errorMessage = exception.Message;

                //log error details
                if (exception is HttpException httpException)
                {
                    try
                    {
                        //for some reason, sometimes we need to manually get exception details from its message
                        var tmpException = JsonConvert.DeserializeObject<EasyPostException>(exception.Message)?.Exception;
                        if (tmpException is not null)
                        {
                            httpException = new HttpException(httpException.StatusCode, tmpException.Code, tmpException.Message,
                                tmpException.Errors?.Select(error => new Error { message = string.Join("; ", error.Message) }).ToList());
                        }
                    }
                    catch { }

                    errorMessage = httpException.Message;
                    var fullMessage = $"Message: {httpException.Message}";
                    if (!string.IsNullOrEmpty(httpException.Code))
                        fullMessage = $"{fullMessage}{Environment.NewLine}Code: {httpException.Code}";

                    if (httpException.Errors?.Any() ?? false)
                    {
                        var details = httpException.Errors
                            .Aggregate(string.Empty, (text, error) => $"{text}{error.message} ({error.field}); ");
                        fullMessage = $"{fullMessage}{Environment.NewLine}Details: {details.TrimEnd(' ')}";
                    }

                    fullMessage = $"{fullMessage}{Environment.NewLine}{Environment.NewLine}{exception}";
                    await _logger.InsertLogAsync(LogLevel.Error, $"{EasyPostDefaults.SystemName} error. {errorMessage}", fullMessage, customer);
                }
                else
                    await _logger.ErrorAsync($"{EasyPostDefaults.SystemName} error. {errorMessage}", exception, customer);

                return (default, errorMessage);
            }
        }

        /// <summary>
        /// Prepare parcel details by the request
        /// </summary>
        /// <param name="request">A request for getting shipping options</param>
        /// <param name="adminRequest">Whether this is a request from the admin area</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the parcel
        /// </returns>
        private async Task<Parcel> PrepareParcelAsync(GetShippingOptionRequest request, bool adminRequest)
        {
            var measureWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync(EasyPostDefaults.MeasureWeightSystemName)
                ?? throw new NopException($"'{EasyPostDefaults.MeasureWeightSystemName}' measure weight is not found");

            var measureDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(EasyPostDefaults.MeasureDimensionSystemName)
                ?? throw new NopException($"'{EasyPostDefaults.MeasureDimensionSystemName}' measure dimension is not found");

            var parcel = new Parcel();

            //get total weight
            var weight = await _shippingService.GetTotalWeightAsync(request, !adminRequest, !adminRequest);
            if (weight == decimal.Zero && adminRequest)
                throw new NopException("Parcel weight cannot be zero");

            weight = await _measureService.ConvertFromPrimaryMeasureWeightAsync(weight, measureWeight);
            parcel.weight = Convert.ToDouble(Math.Max(Math.Round(weight, 1, MidpointRounding.ToPositiveInfinity), 0.1M));

            //if there is a single item, try to get a predefined package for this
            if (request.Items.Count == 1 && request.Items.FirstOrDefault().GetQuantity() == 1)
            {
                var product = request.Items.FirstOrDefault().Product;
                var predefinedPackageValue = await _genericAttributeService
                    .GetAttributeAsync<string>(product, EasyPostDefaults.ProductPredefinedPackageAttribute) ?? string.Empty;
                var predefinedPackage = predefinedPackageValue.Split('.').LastOrDefault();
                if (!string.IsNullOrEmpty(predefinedPackage))
                    parcel.predefined_package = predefinedPackage;
            }

            if (string.IsNullOrEmpty(parcel.predefined_package))
            {
                //get dimensions
                var (width, length, height) = await _shippingService.GetDimensionsAsync(request.Items, !adminRequest);

                width = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(width, measureDimension);
                parcel.width = Convert.ToDouble(Math.Max(Math.Round(width, 1, MidpointRounding.ToPositiveInfinity), 0.1M));

                length = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(length, measureDimension);
                parcel.length = Convert.ToDouble(Math.Max(Math.Round(length, 1, MidpointRounding.ToPositiveInfinity), 0.1M));

                height = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(height, measureDimension);
                parcel.height = Convert.ToDouble(Math.Max(Math.Round(height, 1, MidpointRounding.ToPositiveInfinity), 0.1M));
            }

            return parcel;
        }

        /// <summary>
        /// Prepare dummy shipping options request
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the request
        /// </returns>
        private async Task<GetShippingOptionRequest> PrepareShippingOptionRequestAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry,
            Nop.Core.Domain.Orders.Order order)
        {
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId)
                ?? throw new NopException($"Customer is not set");

            var address = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0)
                ?? throw new NopException($"Shipping address is not set");

            var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipmentEntry.Id);
            var warehouses = (await items
                .SelectAwait(async item => await _shippingService.GetWarehouseByIdAsync(item.WarehouseId))
                .ToListAsync())
                .Where(warehouse => warehouse is not null)
                .Distinct()
                .ToList();
            if (warehouses.Count > 1)
                throw new NopException($"Cannot ship items from different warehouses in the same shipment");

            var warehouse = warehouses.FirstOrDefault();
            var originAddress = warehouse != null
                ? await _addressService.GetAddressByIdAsync(warehouse.AddressId)
                : await _addressService.GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId);

            var request = new GetShippingOptionRequest
            {
                Customer = customer,
                StoreId = order.StoreId,
                ShippingAddress = address,
                WarehouseFrom = warehouse,
                CountryFrom = await _countryService.GetCountryByAddressAsync(originAddress),
                StateProvinceFrom = await _stateProvinceService.GetStateProvinceByAddressAsync(originAddress),
                ZipPostalCodeFrom = originAddress?.ZipPostalCode,
                CountyFrom = originAddress?.County,
                CityFrom = originAddress?.City,
                AddressFrom = originAddress?.Address1,
            };

            request.Items = await items.SelectAwait(async item =>
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId)
                    ?? throw new NopException($"Order item '#{item.OrderItemId}'is not found");

                var product = await _productService.GetProductByIdAsync(orderItem.ProductId)
                    ?? throw new NopException($"Product '#{orderItem.ProductId}'is not found");

                var shoppingCartItem = new Nop.Core.Domain.Orders.ShoppingCartItem
                {
                    StoreId = order.StoreId,
                    CustomerId = customer.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    ShoppingCartType = Nop.Core.Domain.Orders.ShoppingCartType.ShoppingCart,
                    AttributesXml = orderItem.AttributesXml
                };

                return new GetShippingOptionRequest.PackageItem(shoppingCartItem, product, item.Quantity);
            }).ToListAsync();

            return request;
        }

        /// <summary>
        /// Prepare the destination address
        /// </summary>
        /// <param name="request">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        private async Task<Address> PrepareAddressToAsync(GetShippingOptionRequest request)
        {
            return new Address
            {
                name = !string.IsNullOrEmpty(request.ShippingAddress.FirstName)
                    ? $"{request.ShippingAddress.FirstName} {request.ShippingAddress.LastName}"
                    : null,
                email = request.ShippingAddress.Email,
                phone = request.ShippingAddress.PhoneNumber,
                company = request.ShippingAddress.Company,
                street1 = request.ShippingAddress.Address1,
                street2 = request.ShippingAddress.Address2,
                city = request.ShippingAddress.City,
                state = (await _stateProvinceService.GetStateProvinceByAddressAsync(request.ShippingAddress))?.Abbreviation,
                country = (await _countryService.GetCountryByAddressAsync(request.ShippingAddress))?.TwoLetterIsoCode,
                zip = request.ShippingAddress.ZipPostalCode
            };
        }

        /// <summary>
        /// Prepare the origin address
        /// </summary>
        /// <param name="request">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        private async Task<Address> PrepareAddressFromAsync(GetShippingOptionRequest request)
        {
            var store = await _storeService.GetStoreByIdAsync(request.StoreId);
            return new Address
            {
                company = store.CompanyName,
                phone = store.CompanyPhoneNumber,
                street1 = request.AddressFrom,
                city = request.CityFrom,
                state = request.StateProvinceFrom?.Abbreviation,
                country = request.CountryFrom?.TwoLetterIsoCode,
                zip = request.ZipPostalCodeFrom
            };
        }

        /// <summary>
        /// Create a shipment
        /// </summary>
        /// <param name="addressTo">Destination address</param>
        /// <param name="addressFrom">Origin address</param>
        /// <param name="parcel">Parcel</param>
        /// <param name="adminRequest">Whether this is a request from the admin area</param>
        /// <param name="customsInfo">Customs info</param>
        /// <param name="options">Options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the created shipment
        /// </returns>
        private async Task<Shipment> CreateShipmentAsync(Address addressTo, Address addressFrom, Parcel parcel, bool? adminRequest,
            CustomsInfo customsInfo = null, Options options = null)
        {
            //set address verification (ignore estimate requests)
            if (adminRequest.HasValue &&
                _easyPostSettings.AddressVerification &&
                !_actionContextAccessor.ActionContext.HttpContext.Request.Path.Value.Contains("estimate"))
            {
                var verificationParameters = new List<string>() { "delivery" };

                //always non-strictly check origin address
                addressFrom.verify = verificationParameters;

                //set strict or regular check of destination address in public store
                if (adminRequest == false)
                {
                    if (_easyPostSettings.StrictAddressVerification)
                        addressTo.verify_strict = verificationParameters;
                    else
                        addressTo.verify = verificationParameters;
                }
            }

            //set currency
            options ??= new();
            if (string.IsNullOrEmpty(options.currency))
                options.currency = EasyPostDefaults.CurrencyCode;

            //create shipment
            var shipment = new Shipment
            {
                to_address = addressTo,
                from_address = addressFrom,
                parcel = parcel,
                customs_info = customsInfo,
                options = options
            };
            if (!_easyPostSettings.UseSandbox && !_easyPostSettings.UseAllAvailableCarriers)
                shipment.carrier_accounts = _easyPostSettings.CarrierAccounts?.Select(value => new CarrierAccount { id = value }).ToList();
            shipment.Create();

            //log warning messages if any
            if (_easyPostSettings.LogShipmentMessages && shipment.messages?.Any() == true)
            {
                var warning = shipment.messages
                    .Aggregate(string.Empty, (text, message) => $"{text}{message.carrier}: {message.message};{Environment.NewLine}");
                await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. {warning}");
            }

            //check address verification results
            if (adminRequest.HasValue && _easyPostSettings.AddressVerification)
            {
                async Task addressWarning(Address address, bool log)
                {
                    if (address.verifications?.delivery?.success != false)
                        return;

                    var errors = address.verifications.delivery.errors;
                    if (log)
                        throw new HttpException(422, "ADDRESS.VERIFY.FAILURE", "Unable to verify the origin address", errors);

                    var details = errors
                        .Aggregate(string.Empty, (text, error) => $"{text}{error.message} ({error.field}); ")
                        .TrimEnd(' ').TrimEnd(';');
                    var warning = await _localizationService.GetResourceAsync("Plugins.Shipping.EasyPost.Checkout.AddressVerification.Warning");
                    _notificationService.WarningNotification(string.Format(warning, details));
                }

                if (adminRequest == true)
                    await addressWarning(shipment.from_address, false);
                else
                {
                    await addressWarning(shipment.from_address, true);
                    await addressWarning(shipment.to_address, false);
                }
            }

            return shipment;
        }

        /// <summary>
        /// Get batch status
        /// </summary>
        /// <param name="batch">Batch</param>
        /// <returns>Batch status</returns>
        private BatchStatus GetBatchStatus(Batch batch)
        {
            return batch.state?.ToLower() switch
            {
                "creating" => BatchStatus.Creating,
                "creation_failed" => BatchStatus.CreationFailed,
                "created" => BatchStatus.Created,
                "purchasing" => BatchStatus.Purchasing,
                "purchase_failed" => BatchStatus.PurchaseFailed,
                "purchased" => BatchStatus.Purchased,
                "label_generating" => BatchStatus.LabelGenerating,
                "label_generated" => BatchStatus.LabelGenerated,
                _ => BatchStatus.Unknown
            };
        }

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Reset client configuration 
        /// </summary>
        public static void ResetClientConfiguration()
        {
            _isConfigured = null;
            ClientManager.Unconfigure();
        }

        /// <summary>
        /// Get carrier accounts available to the account
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of carrier accounts; error if exists
        /// </returns>
        public async Task<(List<CarrierAccount> Accounts, string error)> GetCarrierAccountsAsync()
        {
            return await HandleFunctionAsync(() =>
            {
                //no need to log configuration errors here
                return Task.FromResult(IsConfigured() ? CarrierAccount.List() : new List<CarrierAccount>());
            });
        }

        #endregion

        #region Common

        /// <summary>
        /// Convert rate value
        /// </summary>
        /// <param name="rateValue">String rate value</param>
        /// <param name="currencyCode">Rate currency</param>
        /// <param name="storeCurrency">Primary store currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the decimal rate value
        /// </returns>
        public async Task<decimal> ConvertRateAsync(string rateValue, string currencyCode, Currency storeCurrency = null)
        {
            storeCurrency ??= await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                ?? throw new NopException("Primary store currency is not set");

            var rate = decimal.TryParse(rateValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                ? value
                : decimal.Zero;

            //convert currency if necessary
            if (string.IsNullOrEmpty(currencyCode))
                currencyCode = EasyPostDefaults.CurrencyCode;
            if (rate > decimal.Zero && !currencyCode.Equals(storeCurrency.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
            {
                var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                    ?? throw new NopException($"'{currencyCode}' currency is not found");

                rate = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(rate, currency);
            }

            return Math.Max(rate, decimal.Zero);
        }

        #endregion

        #region Webhooks

        /// <summary>
        /// Create the webhook that receives third-party requests
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the webhook; error message if exists
        /// </returns>
        public async Task<(Webhook Webhook, string Error)> CreateWebhookAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var store = await _storeContext.GetCurrentStoreAsync();
                if (string.IsNullOrEmpty(store?.Url))
                    throw new NopException("Store URL is not set");

                //prepare webhook URL
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var url = $"{store.Url.TrimEnd('/')}{urlHelper.RouteUrl(EasyPostDefaults.WebhookRouteName)}".ToLowerInvariant();

                //check whether the webhook already exists
                var webhook = Webhook.List()
                    ?.FirstOrDefault(webhook => webhook.url?.Equals(url, StringComparison.InvariantCultureIgnoreCase) ?? false);
                if (webhook is not null)
                    return webhook;

                //try to create new one if doesn't exist
                return Webhook.Create(new() { [nameof(url)] = url })
                    ?? throw new NopException("No response from the service");
            });
        }

        /// <summary>
        /// Delete the webhook
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteWebhookAsync()
        {
            await HandleFunctionAsync(() =>
            {
                //no need to log configuration errors here
                if (!IsConfigured())
                    return Task.FromResult(false);

                var url = _easyPostSettings.WebhookUrl;
                if (string.IsNullOrEmpty(url))
                    return Task.FromResult(false);

                Webhook.List()
                    ?.FirstOrDefault(webhook => webhook.url?.Equals(url, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    ?.Destroy();

                return Task.FromResult(true);
            });
        }

        /// <summary>
        /// Handle webhook request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                //get request details
                var rawRequestString = string.Empty;
                using var streamReader = new StreamReader(request.Body, System.Text.Encoding.UTF8, true);
                rawRequestString = await streamReader.ReadToEndAsync();

                if (string.IsNullOrEmpty(_easyPostSettings.WebhookUrl))
                    throw new NopException("Webhook is not set");

                var webhook = Webhook.List()
                    ?.FirstOrDefault(webhook => webhook.url?.Equals(_easyPostSettings.WebhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    ?? throw new NopException($"No webhook configured for URL '{_easyPostSettings.WebhookUrl}'");

                try
                {
                    var eventEntry = JsonConvert.DeserializeObject<Event>(rawRequestString);
                    var eventResult = JsonConvert.SerializeObject(eventEntry.result, Formatting.Indented);
                    switch (eventEntry.description?.ToLower())
                    {
                        case "tracker.created":
                        case "tracker.updated":
                            {
                                var tracker = JsonConvert.DeserializeObject<Tracker>(eventResult);
                                if (string.IsNullOrEmpty(tracker?.tracking_code))
                                    break;

                                //try to get a shipment by the tracking number
                                var shipments = await _shipmentService.GetAllShipmentsAsync(trackingNumber: tracker.tracking_code);
                                if (shipments.Count != 1)
                                    break;

                                var shipmentEntry = shipments.FirstOrDefault();
                                var order = await _orderService.GetOrderByIdAsync(shipmentEntry.OrderId);
                                if (order is null || order.Deleted || order.OrderStatus == Nop.Core.Domain.Orders.OrderStatus.Cancelled)
                                    break;

                                //add order note just for information, anyway we request tracking details on the shipment page
                                await _orderService.InsertOrderNoteAsync(new()
                                {
                                    OrderId = order.Id,
                                    DisplayToCustomer = false,
                                    Note = $"Tracking information updated for the shipment '#{shipmentEntry.Id}'",
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                break;
                            }

                        case "batch.created":
                        case "batch.updated":
                            {
                                var batch = JsonConvert.DeserializeObject<Batch>(eventResult);
                                if (string.IsNullOrEmpty(batch?.id) || string.IsNullOrEmpty(batch?.reference))
                                    break;

                                var batchEntry = (await GetAllBatchesAsync(batchId: batch.id)).FirstOrDefault();
                                if (batchEntry is null)
                                    break;

                                //match reference to prevent sending fraudulent data
                                if (!string.Equals(batch.reference, batchEntry.BatchGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                    break;

                                //update the batch status
                                batchEntry.StatusId = (int)GetBatchStatus(batch);
                                batchEntry.UpdatedOnUtc = DateTime.UtcNow;
                                await UpdateBatchAsync(batchEntry);

                                //log errors if any
                                if (batchEntry.StatusId == (int)BatchStatus.CreationFailed ||
                                    batchEntry.StatusId == (int)BatchStatus.PurchaseFailed ||
                                    !string.IsNullOrEmpty(batch.error))
                                {
                                    var warning = !string.IsNullOrEmpty(batch.error) ? batch.error : string.Empty;
                                    var failedShipments = batch.shipments.Where(shipment => !string.IsNullOrEmpty(shipment.batch_message));
                                    if (failedShipments.Any())
                                    {
                                        warning += failedShipments
                                            .Aggregate(string.Empty, (text, shipment) => $"{text}{shipment.id}: {shipment.batch_message};{Environment.NewLine}");
                                    }
                                    await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. Batch '#{batchEntry.Id}': {warning}");
                                }

                                break;
                            }

                        case "scan_form.created":
                        case "scan_form.updated":
                            {
                                var scanForm = JsonConvert.DeserializeObject<ScanForm>(eventResult);
                                if (string.IsNullOrEmpty(scanForm?.batch_id))
                                    break;

                                var batchEntry = (await GetAllBatchesAsync(batchId: scanForm.batch_id)).FirstOrDefault();
                                if (batchEntry is null)
                                    break;

                                //set manifest URL
                                if (!string.IsNullOrEmpty(scanForm.form_url))
                                {
                                    batchEntry.ManifestUrl = scanForm.form_url;
                                    await UpdateBatchAsync(batchEntry);
                                }

                                //log errors if any
                                if (!string.IsNullOrEmpty(scanForm.message))
                                    await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. Batch '#{batchEntry.Id}': {scanForm.message}");

                                break;
                            }
                    }
                }
                catch (Exception exception)
                {
                    throw new NopException("Failed to deserialize data from the webhook request", exception);
                }

                return true;
            });
        }

        #endregion

        #region Shipment

        /// <summary>
        /// Save shipment details of the last order placed
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task SaveShipmentAsync(Nop.Core.Domain.Orders.Order order)
        {
            await HandleFunctionAsync(async () =>
            {
                if (order is null)
                    throw new ArgumentNullException(nameof(order));

                if (order.ShippingRateComputationMethodSystemName != EasyPostDefaults.SystemName)
                    return false;

                //move the shipment from the customer to the order entry
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var shipmentId = await _genericAttributeService
                    .GetAttributeAsync<string>(customer, EasyPostDefaults.ShipmentIdAttribute, order.StoreId);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException($"Shipment for the order '{order.CustomOrderNumber}' is not found");

                await _genericAttributeService.SaveAttributeAsync(order, EasyPostDefaults.ShipmentIdAttribute, shipmentId);
                await _genericAttributeService.SaveAttributeAsync<string>(customer, EasyPostDefaults.ShipmentIdAttribute, null, order.StoreId);

                return true;
            });
        }

        /// <summary>
        /// Save shipment details of the order to the specific shipment entry
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task SaveShipmentAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry)
        {
            await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                if (!IsConfigured())
                    throw new NopException($"Plugin is not configured");

                var order = await _orderService.GetOrderByIdAsync(shipmentEntry.OrderId)
                    ?? throw new NopException($"Order '#{shipmentEntry.OrderId}' is not found");

                var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(order, EasyPostDefaults.ShipmentIdAttribute);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException($"Shipment for the order '{order.CustomOrderNumber}' is not found");

                var orderShipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                if (orderShipment.to_address is null || orderShipment.from_address is null || orderShipment.parcel is null)
                    throw new NopException("Failed to get the shipment details");

                //whether the origin address and parcel details are matched
                var request = await PrepareShippingOptionRequestAsync(shipmentEntry, order);
                var addressFrom = await PrepareAddressFromAsync(request);
                var parcel = await PrepareParcelAsync(request, true);
                if (!parcel.Matches(orderShipment.parcel) || !addressFrom.Matches(orderShipment.from_address))
                {
                    //if details are not matched, create a specific shipment from the common order shipment
                    var shipment = await CreateShipmentAsync(orderShipment.to_address, addressFrom, parcel, true);
                    shipmentId = shipment.id;
                }

                //save the shipment id
                await _genericAttributeService.SaveAttributeAsync(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute, shipmentId);

                return true;
            });
        }

        /// <summary>
        /// Get shipment details
        /// </summary>
        /// <param name="shipmentId">Shipment id</param>
        /// <returns></returns>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment; error message if exists
        /// </returns>
        public async Task<(Shipment Shipment, string Error)> GetShipmentAsync(string shipmentId)
        {
            return await HandleFunctionAsync(() =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException("Shipment id is not set");

                var shipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(shipment);
            });
        }

        /// <summary>
        /// Gets shipping rates for the passed shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="useSmartRates">Whether to use SmartRate feature to get an expected time in transit for the rates</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping rates; error message if exists
        /// </returns>
        public async Task<(List<ShippingRate> Rates, string Error)> GetShippingRatesAsync(Shipment shipment, bool useSmartRates = false)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (shipment is null)
                    throw new ArgumentNullException(nameof(shipment));

                if (!IsConfigured())
                    throw new NopException($"Plugin is not configured");

                var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                    ?? throw new NopException("Primary store currency is not set");

                //log warning messages if any
                if (_easyPostSettings.LogShipmentMessages && shipment.messages?.Any() == true)
                {
                    var warning = shipment.messages
                        .Aggregate(string.Empty, (text, message) => $"{text}{message.carrier}: {message.message};{Environment.NewLine}");
                    await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. {warning}");
                }

                if (!shipment?.rates?.Any() ?? true)
                    throw new NopException("Failed to get rates");

                //get rates from the created shipment
                var rates = await shipment.rates.SelectAwait(async rate => new ShippingRate
                {
                    Id = rate.id,
                    Carrier = rate.carrier,
                    Service = rate.service,
                    Rate = await ConvertRateAsync(rate.rate, rate.currency, storeCurrency),
                    //Rate = await ConvertRateAsync(rate.rate, rate.currency, storeCurrency) + _easyPostSettings.AdditionalHandlingCharge,
                    DeliveryDays = rate.delivery_days,
                    Currency = rate.currency
                }).ToListAsync();

                if (!useSmartRates || !_easyPostSettings.UseSmartRates)
                    return rates;

                await HandleFunctionAsync(() =>
                {
                    var smartRates = shipment.GetSmartrates();
                    if (!smartRates?.Any() ?? true)
                        throw new NopException("Failed to get smart rates");

                    foreach (var rate in rates)
                    {
                        var expectedDays = smartRates
                            .FirstOrDefault(smartRate => smartRate.carrier == rate.Carrier && smartRate.service == rate.Service)
                            ?.time_in_transit;
                        if (expectedDays is not null)
                        {
                            rate.TimeInTransit.Add((50, expectedDays.percentile_50));
                            rate.TimeInTransit.Add((75, expectedDays.percentile_75));
                            rate.TimeInTransit.Add((85, expectedDays.percentile_85));
                            rate.TimeInTransit.Add((90, expectedDays.percentile_90));
                            rate.TimeInTransit.Add((95, expectedDays.percentile_95));
                            rate.TimeInTransit.Add((97, expectedDays.percentile_97));
                            rate.TimeInTransit.Add((99, expectedDays.percentile_99));
                        }
                    }

                    return Task.FromResult(rates);
                });

                return rates;
            });
        }

        /// <summary>
        /// Update shipment with the passed details
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="request">Details to update the shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> UpdateShipmentAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry,
            UpdateShipmentRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException($"Shipment '#{shipmentEntry.Id}'is not found");

                var shipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                if (shipment.to_address is null || shipment.from_address is null || shipment.parcel is null)
                    throw new NopException("Failed to get shipment details");

                var options = new Options
                {
                    additional_handling = request.OptionsDetails.AdditionalHandling,
                    alcohol = request.OptionsDetails.Alcohol,
                    by_drone = request.OptionsDetails.ByDrone,
                    carbon_neutral = request.OptionsDetails.CarbonNeutral,
                    delivery_confirmation = request.OptionsDetails.DeliveryConfirmation,
                    endorsement = request.OptionsDetails.Endorsement,
                    handling_instructions = request.OptionsDetails.HandlingInstructions,
                    hazmat = request.OptionsDetails.Hazmat,
                    invoice_number = request.OptionsDetails.InvoiceNumber,
                    machinable = request.OptionsDetails.Machinable.ToString().ToLower(),
                    print_custom_1 = request.OptionsDetails.PrintCustom1,
                    print_custom_1_code = request.OptionsDetails.PrintCustomCode1,
                    print_custom_2 = request.OptionsDetails.PrintCustom2,
                    print_custom_2_code = request.OptionsDetails.PrintCustomCode2,
                    print_custom_3 = request.OptionsDetails.PrintCustom3,
                    print_custom_3_code = request.OptionsDetails.PrintCustomCode3,
                    special_rates_eligibility = request.OptionsDetails.SpecialRatesEligibility,
                    certified_mail = request.OptionsDetails.CertifiedMail,
                    registered_mail = request.OptionsDetails.RegisteredMail,
                    registered_mail_amount = request.OptionsDetails.RegisteredMail
                        ? Convert.ToDouble(request.OptionsDetails.RegisteredMailAmount)
                        : null,
                    return_receipt = request.OptionsDetails.ReturnReceipt
                };

                CustomsInfo customsInfo = null;
                if (request.CustomsInfoDetails is not null)
                {
                    var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                        ?? throw new NopException("Primary store currency is not set");

                    var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipmentEntry.Id);
                    var customsItems = await items.SelectAwait(async item =>
                    {
                        var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId)
                            ?? throw new NopException($"Order item '#{item.OrderItemId}'is not found");

                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId)
                            ?? throw new NopException($"Product '#{orderItem.ProductId}'is not found");

                        //prepare customs item
                        var code = await _productService.FormatSkuAsync(product, orderItem.AttributesXml);
                        var description = product.Name;
                        var hsNumber = await _genericAttributeService
                            .GetAttributeAsync<string>(product, EasyPostDefaults.ProductHtsNumberAttribute);
                        var originCountry = await _genericAttributeService
                            .GetAttributeAsync<string>(product, EasyPostDefaults.ProductOriginCountryAttribute);
                        var itemValue = orderItem.PriceExclTax;
                        if (!storeCurrency.CurrencyCode.Equals(EasyPostDefaults.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //total value in US dollars 
                            var usdCurrency = await _currencyService.GetCurrencyByCodeAsync(EasyPostDefaults.CurrencyCode)
                                ?? throw new NopException($"{EasyPostDefaults.CurrencyCode} currency is not found");

                            itemValue = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(itemValue, usdCurrency);
                        }
                        var itemWeight = (orderItem.ItemWeight ?? decimal.Zero) * item.Quantity;
                        return new CustomsItem
                        {
                            code = code,
                            description = description,
                            hs_tariff_number = hsNumber,
                            origin_country = originCountry,
                            quantity = item.Quantity,
                            value = Convert.ToDouble(itemValue),
                            weight = Convert.ToDouble(itemWeight),
                            currency = EasyPostDefaults.CurrencyCode
                        };
                    }).ToListAsync();

                    customsInfo = new CustomsInfo
                    {
                        customs_items = customsItems,
                        contents_type = request.CustomsInfoDetails.ContentsType,
                        restriction_type = request.CustomsInfoDetails.RestrictionType,
                        non_delivery_option = request.CustomsInfoDetails.NonDeliveryOption,
                        contents_explanation = request.CustomsInfoDetails.ContentsExplanation,
                        restriction_comments = request.CustomsInfoDetails.RestrictionComments,
                        customs_certify = request.CustomsInfoDetails.CustomsCertify.ToString().ToLower(),
                        customs_signer = request.CustomsInfoDetails.CustomsSigner,
                        eel_pfc = request.CustomsInfoDetails.EelPfc
                    };
                }

                //whether the shipment details are matched
                if (!options.Matches(shipment.options) || (customsInfo is not null && !customsInfo.Matches(shipment.customs_info)))
                {
                    //if details are not matched, create new shipment, since there is no update method
                    var newShipment = await CreateShipmentAsync(shipment.to_address,
                        shipment.from_address, shipment.parcel, null, customsInfo, options);
                    await _genericAttributeService.SaveAttributeAsync(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute, newShipment.id);
                }

                return true;
            });
        }

        /// <summary>
        /// Buy label for the passed shipment
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="rateId">Selected rate id</param>
        /// <param name="insurance">Amount to insure the shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> BuyLabelAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry,
            string rateId, decimal insurance)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException($"Shipment '#{shipmentEntry.Id}' is not found");

                if (string.IsNullOrEmpty(rateId))
                    throw new NopException($"Rate is not selected");

                var shipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                //whether the shipment has already been created and purchased
                if (shipment.selected_rate is not null)
                    return false;

                string insuranceValue = null;
                if (insurance > decimal.Zero)
                {
                    var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                        ?? throw new NopException("Primary store currency is not set");

                    //the currency of all insurance is USD
                    if (!storeCurrency.CurrencyCode.Equals(EasyPostDefaults.CurrencyCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var usdCurrency = await _currencyService.GetCurrencyByCodeAsync(EasyPostDefaults.CurrencyCode)
                            ?? throw new NopException($"{EasyPostDefaults.CurrencyCode} currency is not found");

                        insurance = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(insurance, usdCurrency);
                    }

                    insuranceValue = insurance.ToString("0.00", CultureInfo.InvariantCulture);
                }

                shipment.Buy(rateId, insuranceValue);

                //set tracking number
                if (!string.IsNullOrEmpty(shipment.tracker?.tracking_code))
                {
                    shipmentEntry.TrackingNumber = shipment.tracker.tracking_code;
                    await _shipmentService.UpdateShipmentAsync(shipmentEntry);
                }

                return true;
            });
        }

        /// <summary>
        /// Download a label for the shipment
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="labelFormat">Label format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file details; error message if exists
        /// </returns>
        public async Task<((string Url, string ContentType), string Error)> DownloadLabelAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry,
            string labelFormat)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException("Shipment is not yet purchased");

                var format = labelFormat ?? "png";

                var shipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                (string DownloadUrl, string ContentType) getLabelDetails() =>
                    format.ToLowerInvariant() switch
                    {
                        "pdf" => !string.IsNullOrEmpty(shipment.postage_label.label_pdf_url)
                            ? (shipment.postage_label.label_pdf_url, MimeTypes.ApplicationPdf)
                            : (default, default),
                        "zpl" => !string.IsNullOrEmpty(shipment.postage_label.label_zpl_url)
                            ? (shipment.postage_label.label_zpl_url, "application/zpl")
                            : (default, default),
                        _ => !string.IsNullOrEmpty(shipment.postage_label.label_url)
                            ? (shipment.postage_label.label_url, shipment.postage_label.label_file_type)
                            : (default, default),
                    };

                //try to get the generated label
                var (url, type) = getLabelDetails();

                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(type))
                {
                    //or generate new one with the specified format
                    shipment.GenerateLabel(format.ToUpper());
                    (url, type) = getLabelDetails();
                }

                return (url, type);
            });
        }

        /// <summary>
        /// Download a commercial invoice for the shipment
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file details; error message if exists
        /// </returns>
        public async Task<((string Url, string ContentType), string Error)> DownloadInvoiceAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                if (string.IsNullOrEmpty(shipmentId))
                    throw new NopException("Shipment is not yet purchased");

                var shipment = Shipment.Retrieve(shipmentId)
                    ?? throw new NopException("No response from the service");

                //try to get a commercial invoice from the shipment forms
                if (shipment.forms
                    ?.FirstOrDefault(form => string.Equals(form.form_type, "commercial_invoice", StringComparison.InvariantCultureIgnoreCase)) is not Form form ||
                    string.IsNullOrEmpty(form.form_url))
                {
                    throw new NopException("Commercial invoice is not found");
                }

                return (form.form_url, MimeTypes.ApplicationPdf);
            });
        }

        /// <summary>
        /// Delete shipment details
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteShipmentAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry)
        {
            await HandleFunctionAsync(async () =>
            {
                if (shipmentEntry is null)
                    throw new ArgumentNullException(nameof(shipmentEntry));

                //clear attributes of the deleted entry
                await _genericAttributeService.SaveAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<string>(shipmentEntry, EasyPostDefaults.PickupIdAttribute, null);

                return true;
            });
        }

        #endregion

        #region Tracking

        /// <summary>
        /// Get URL for a page to show the tracking info
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the URL; error message if exists
        /// </returns>
        public async Task<(string Url, string Error)> GetTrackingUrlAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry, string trackingNumber)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                //try to get a specific tracker
                if (shipmentEntry is not null)
                {
                    var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                    if (!string.IsNullOrEmpty(shipmentId))
                    {
                        //no need to log errors here
                        try
                        {
                            var shipment = Shipment.Retrieve(shipmentId);
                            if (!string.IsNullOrEmpty(shipment?.tracker?.id))
                            {
                                var shipmentTracker = Tracker.Retrieve(shipment.tracker.id);
                                if (!string.IsNullOrEmpty(shipmentTracker?.public_url))
                                    return shipmentTracker.public_url;
                            }
                        }
                        catch { }
                    }
                }

                //or find one by the number from the common list
                var trackerList = Tracker.List(new() { ["tracking_code"] = trackingNumber })
                    ?? throw new NopException("No response from the service");

                return trackerList.trackers?.FirstOrDefault()?.public_url
                    ?? throw new NopException($"No tracker found with number '{trackingNumber}'");
            });
        }

        /// <summary>
        /// Get shipment events by the tracking number
        /// </summary>
        /// <param name="shipmentEntry">Shipment entry</param>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the events; error message if exists
        /// </returns>
        public async Task<(List<ShipmentStatusEvent> Events, string Error)> GetTrackingEventsAsync(
            Nop.Core.Domain.Shipping.Shipment shipmentEntry, string trackingNumber)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                List<ShipmentStatusEvent> getEvents(Tracker tracker)
                {
                    return tracker?.tracking_details?.Select(details => new ShipmentStatusEvent
                    {
                        Date = details.datetime,
                        EventName = details.message,
                        CountryCode = details.tracking_location?.country,
                        Location = $"{details.tracking_location?.city} {details.tracking_location?.state} " +
                            $"{details.tracking_location?.country} {details.tracking_location?.zip}".TrimEnd(' ')
                    }).ToList();
                }

                //try to get a specific tracker
                if (shipmentEntry is not null)
                {
                    var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                    if (!string.IsNullOrEmpty(shipmentId))
                    {
                        //no need to log errors here
                        try
                        {
                            var shipment = Shipment.Retrieve(shipmentId);
                            if (!string.IsNullOrEmpty(shipment?.tracker?.id))
                            {
                                var shipmentTracker = Tracker.Retrieve(shipment.tracker.id);
                                if (shipmentTracker?.tracking_details?.Any() ?? false)
                                    return getEvents(shipmentTracker);
                            }
                        }
                        catch { }
                    }
                }

                //or find one by the number from the common list
                var trackerList = Tracker.List(new() { ["tracking_code"] = trackingNumber })
                    ?? throw new NopException("No response from the service");

                return getEvents(trackerList.trackers?.FirstOrDefault())
                    ?? throw new NopException($"No tracker found with number '{trackingNumber}'");
            });
        }

        #endregion

        #region Pickup

        /// <summary>
        /// Create new pickup for the shipment
        /// </summary>
        /// <param name="shipmentEntry">Shipment; pass null if it's a pickup for the batch</param>
        /// <param name="batchEntry">Batch; pass null if it's a pickup for the shipment</param>
        /// <param name="request">Details to create a pickup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> CreatePickupAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry, EasyPostBatch batchEntry,
            CreatePickupRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                Shipment shipment = null;
                if (shipmentEntry is not null)
                {
                    var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                    if (string.IsNullOrEmpty(shipmentId))
                        throw new NopException("Shipment is not yet purchased");

                    shipment = Shipment.Retrieve(shipmentId)
                        ?? throw new NopException("No response from the service");
                }

                Batch batch = null;
                if (batchEntry is not null)
                {
                    if (string.IsNullOrEmpty(batchEntry.BatchId))
                        throw new NopException("Batch is not yet purchased");

                    batch = Batch.Retrieve(batchEntry.BatchId)
                        ?? throw new NopException("No response from the service");
                }

                //create new pickup with the passed details
                var parameters = new Pickup
                {
                    address = new Address
                    {
                        company = request.Address.Company,
                        email = request.Address.Email,
                        phone = request.Address.PhoneNumber,
                        street1 = request.Address.Address1,
                        street2 = request.Address.Address2,
                        city = request.Address.City,
                        state = (await _stateProvinceService.GetStateProvinceByIdAsync(request.Address.StateProvinceId ?? 0))?.Abbreviation,
                        country = (await _countryService.GetCountryByIdAsync(request.Address.CountryId ?? 0))?.TwoLetterIsoCode,
                        zip = request.Address.ZipPostalCode
                    },
                    instructions = request.Instructions,
                    max_datetime = request.MaxDate,
                    min_datetime = request.MinDate,
                }.AsDictionary();
                if (shipment is not null)
                    parameters.Add(nameof(shipment), shipment);
                if (batch is not null)
                    parameters.Add(nameof(batch), batch);

                var pickup = Pickup.Create(parameters);

                //log warning messages if any
                if (_easyPostSettings.LogShipmentMessages && pickup.messages?.Any() == true)
                {
                    var warning = pickup.messages.Aggregate(string.Empty, (text, messageText) =>
                    {
                        try
                        {
                            //for some reason, sometimes we need to manually get message details
                            var message = JsonConvert.DeserializeObject<Message>(messageText);
                            if (message is not null)
                                return $"{text}{message.carrier}: {message.message};{Environment.NewLine}";
                        }
                        catch { }

                        return $"{text}{messageText};{Environment.NewLine}";
                    });
                    await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. {warning}");
                }

                //save the pickup id
                if (shipmentEntry is not null)
                    await _genericAttributeService.SaveAttributeAsync(shipmentEntry, EasyPostDefaults.PickupIdAttribute, pickup.id);
                if (batchEntry is not null)
                {
                    batchEntry.PickupId = pickup.id;
                    await UpdateBatchAsync(batchEntry);
                }

                return true;
            });
        }

        /// <summary>
        /// Get pickup details
        /// </summary>
        /// <param name="pickupId">Pickup id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the pickup; error message if exists
        /// </returns>
        public async Task<(Pickup Pickup, string Error)> GetPickupAsync(string pickupId)
        {
            return await HandleFunctionAsync(() =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if (string.IsNullOrEmpty(pickupId))
                    throw new NopException("Pickup id is not set");

                var pickup = Pickup.Retrieve(pickupId)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(pickup);
            });
        }

        /// <summary>
        /// Gets shipping rates for the passed pickup
        /// </summary>
        /// <param name="pickup">Pickup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping rates; error message if exists
        /// </returns>
        public async Task<(List<ShippingRate> Rates, string Error)> GetPickupRatesAsync(Pickup pickup)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (pickup is null)
                    throw new ArgumentNullException(nameof(pickup));

                if (!IsConfigured())
                    throw new NopException($"Plugin is not configured");

                var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)
                    ?? throw new NopException("Primary store currency is not set");

                //log warning messages if any
                if (_easyPostSettings.LogShipmentMessages && pickup.messages?.Any() == true)
                {
                    var warning = pickup.messages.Aggregate(string.Empty, (text, messageText) =>
                    {
                        try
                        {
                            //for some reason, sometimes we need to manually get message details
                            var message = JsonConvert.DeserializeObject<Message>(messageText);
                            if (message is not null)
                                return $"{text}{message.carrier}: {message.message};{Environment.NewLine}";
                        }
                        catch { }

                        return $"{text}{messageText};{Environment.NewLine}";
                    });
                    await _logger.WarningAsync($"{EasyPostDefaults.SystemName} warning. {warning}");
                }

                if (!pickup?.pickup_rates?.Any() ?? true)
                    throw new NopException("Failed to get rates");

                //get rates from the pickup
                return await pickup.pickup_rates.SelectAwait(async rate => new ShippingRate
                {
                    Id = rate.id,
                    Carrier = rate.carrier,
                    Service = rate.service,
                    Rate = await ConvertRateAsync(rate.rate, rate.currency, storeCurrency),
                    DeliveryDays = rate.delivery_days,
                    Currency = rate.currency
                }).ToListAsync();
            });
        }

        /// <summary>
        /// Buy the pickup
        /// </summary>
        /// <param name="shipmentEntry">Shipment; pass null if it's a pickup for the batch</param>
        /// <param name="batchEntry">Batch; pass null if it's a pickup for the shipment</param>
        /// <param name="rateId">Selected pickup rate id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> BuyPickupAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry, EasyPostBatch batchEntry,
            string rateId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if (string.IsNullOrEmpty(rateId))
                    throw new NopException($"Rate is not selected");

                var pickupId = shipmentEntry is not null
                    ? await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.PickupIdAttribute)
                    : batchEntry?.PickupId;
                if (string.IsNullOrEmpty(pickupId))
                    throw new NopException("Pickup id is not set");

                var pickup = Pickup.Retrieve(pickupId)
                    ?? throw new NopException("No response from the service");

                //whether the pickup has already been purchased and scheduled
                if (string.Equals(pickup.status, "scheduled", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                //purchase the pickup with the selected rate
                var selectedRate = pickup.pickup_rates?.FirstOrDefault(rate => rate.id == rateId)
                    ?? throw new NopException($"Selected rate is not available");

                pickup.Buy(selectedRate.carrier, selectedRate.service);

                return true;
            });
        }

        /// <summary>
        /// Cancel the pickup
        /// </summary>
        /// <param name="shipmentEntry">Shipment; pass null if it's a pickup for the batch</param>
        /// <param name="batchEntry">Batch; pass null if it's a pickup for the shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> CancelPickupAsync(Nop.Core.Domain.Shipping.Shipment shipmentEntry, EasyPostBatch batchEntry)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var pickupId = shipmentEntry is not null
                    ? await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.PickupIdAttribute)
                    : batchEntry?.PickupId;
                if (string.IsNullOrEmpty(pickupId))
                    throw new NopException("Pickup id is not set");

                var pickup = Pickup.Retrieve(pickupId)
                    ?? throw new NopException("No response from the service");

                //cancel the pickup and clear the pickup id
                if (shipmentEntry is not null)
                    await _genericAttributeService.SaveAttributeAsync<string>(shipmentEntry, EasyPostDefaults.PickupIdAttribute, null);
                if (batchEntry is not null)
                {
                    batchEntry.PickupId = string.Empty;
                    await UpdateBatchAsync(batchEntry);
                }
                pickup.Cancel();

                return true;
            });
        }

        #endregion

        #region Shipping options

        /// <summary>
        /// Gets shipping options by the request
        /// </summary>
        /// <param name="request">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping options; error message if exists
        /// </returns>
        public async Task<(List<Nop.Core.Domain.Shipping.ShippingOption> Options, string Error)> GetShippingOptionsAsync(
            GetShippingOptionRequest request)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var addressTo = await PrepareAddressToAsync(request);
                var addressFrom = await PrepareAddressFromAsync(request);
                var parcel = await PrepareParcelAsync(request, false);
                var shipment = await CreateShipmentAsync(addressTo, addressFrom, parcel, false);
                var (rates, ratesError) = await GetShippingRatesAsync(shipment);
                if (!string.IsNullOrEmpty(ratesError))
                    throw new NopException(ratesError);

                //save shipment as a customer attribute during the checkout, move it to an order entry after placing
                if (!string.IsNullOrEmpty(shipment?.id))
                {
                    await _genericAttributeService
                        .SaveAttributeAsync(request.Customer, EasyPostDefaults.ShipmentIdAttribute, shipment.id, request.StoreId);
                }

                //whether this is a free shipping
                var freeShipping = request.Items.All(item => item.Product.IsFreeShipping);

                var options = rates?.Select(rate => new Nop.Core.Domain.Shipping.ShippingOption
                {
                    Name = $"{rate.Carrier} {rate.Service}".TrimEnd(' '),
                    Rate = freeShipping ? decimal.Zero : rate.Rate,
                    TransitDays = rate.DeliveryDays,
                    ShippingRateComputationMethodSystemName = EasyPostDefaults.SystemName
                }).ToList() ?? new();

                return options;
            });
        }

        #endregion

        #region Batch

        /// <summary>
        /// Insert a batch
        /// </summary>
        /// <param name="batch">Batch</param>        
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InsertBatchAsync(EasyPostBatch batch)
        {
            await _batchRepository.InsertAsync(batch);
        }

        /// <summary>
        /// Update the batch
        /// </summary>
        /// <param name="batch">Batch</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateBatchAsync(EasyPostBatch batch)
        {
            await _batchRepository.UpdateAsync(batch);
        }

        /// <summary>
        /// Delete the batch
        /// </summary>
        /// <param name="batch">Batch</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteBatchAsync(EasyPostBatch batch)
        {
            await _batchRepository.DeleteAsync(batch);
        }

        /// <summary>
        /// Get a batch the by identifier
        /// </summary>
        /// <param name="batchId">Batch identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the batch
        /// </returns>
        public async Task<EasyPostBatch> GetBatchByIdAsync(int batchId)
        {
            return await _batchRepository.GetByIdAsync(batchId, null);
        }

        /// <summary>
        /// Get all batches
        /// </summary>
        /// <param name="status">Batch status; pass null to load all records</param>
        /// <param name="batchId">Batch id; pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the batches
        /// </returns>
        public async Task<IList<EasyPostBatch>> GetAllBatchesAsync(BatchStatus? status = null, string batchId = null)
        {
            return await _batchRepository.GetAllAsync(query =>
            {
                if (status.HasValue)
                    query = query.Where(entry => entry.StatusId == (int)status.Value);

                if (!string.IsNullOrEmpty(batchId))
                    query = query.Where(entry => entry.BatchId == batchId);

                query = query.OrderByDescending(entry => entry.CreatedOnUtc);

                return query;
            });
        }

        /// <summary>
        /// Create new batch or update the existing one
        /// </summary>
        /// <param name="batchEntry">Batch entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> CreateOrUpdateBatchAsync(EasyPostBatch batchEntry)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (batchEntry is null)
                    throw new ArgumentNullException(nameof(batchEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                var ids = batchEntry.ShipmentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(idValue => int.TryParse(idValue, out var id) ? id : 0)
                    .Distinct()
                    .ToArray();
                var currentShipmentIds = await (await _shipmentService.GetShipmentsByIdsAsync(ids)).SelectAwait(async shipmentEntry =>
                {
                    var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                    return new { Id = shipmentEntry.Id, ShipmentId = shipmentId };
                }).ToListAsync();
                var notCreatedShipments = currentShipmentIds.Where(pair => string.IsNullOrEmpty(pair.ShipmentId)).ToList();
                if (notCreatedShipments.Any())
                {
                    var idsMessage = notCreatedShipments.Aggregate(string.Empty, (text, id) => $"{text}'#{id.Id}', ").TrimEnd(' ').TrimEnd(',');
                    throw new NopException($"Shipments {idsMessage} are not yet purchased");
                }

                Batch batch;
                if (string.IsNullOrEmpty(batchEntry.BatchId))
                {
                    //create new batch
                    var reference = Guid.NewGuid();
                    batch = Batch.Create(new() { [nameof(reference)] = reference.ToString().ToLower() });

                    batchEntry.BatchGuid = reference;
                    batchEntry.BatchId = batch.id;
                    await UpdateBatchAsync(batchEntry);
                }
                else
                {
                    //or get the existing one
                    batch = Batch.Retrieve(batchEntry.BatchId)
                        ?? throw new NopException("No response from the service");
                }

                //update associated shipments
                var batchShipmentIds = batch.shipments.Select(shipment => shipment.id).ToList();
                var idsToAdd = currentShipmentIds.Where(pair => !batchShipmentIds.Contains(pair.ShipmentId)).ToList();
                var idsToRemove = batchShipmentIds.Where(id => !currentShipmentIds.Any(pair => id == pair.ShipmentId)).ToList();
                if (idsToAdd.Any())
                    batch.AddShipments(idsToAdd.Select(pair => pair.ShipmentId));
                if (idsToRemove.Any())
                    batch.RemoveShipments(idsToRemove);

                batchEntry.StatusId = (int)GetBatchStatus(batch);
                batchEntry.UpdatedOnUtc = DateTime.UtcNow;
                await UpdateBatchAsync(batchEntry);

                return true;
            });
        }

        /// <summary>
        /// Gets batch shipments
        /// </summary>
        /// <param name="batchEntry">Batch entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the batch shipments; error message if exists
        /// </returns>
        public async Task<(List<Domain.Batch.BatchShipment> Shipments, string Error)> GetBatchShipmentsAsync(EasyPostBatch batchEntry)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (batchEntry is null)
                    throw new ArgumentNullException(nameof(batchEntry));

                var measureWeight = await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)
                    ?? throw new NopException("Base measure weight is not found");

                var ids = batchEntry.ShipmentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(idValue => int.TryParse(idValue, out var id) ? id : 0)
                    .Distinct()
                    .ToArray();
                var batchShipments = await (await _shipmentService.GetShipmentsByIdsAsync(ids)).SelectAwait(async shipmentEntry =>
                {
                    var order = await _orderService.GetOrderByIdAsync(shipmentEntry.OrderId);
                    var shipmentId = await _genericAttributeService.GetAttributeAsync<string>(shipmentEntry, EasyPostDefaults.ShipmentIdAttribute);
                    return new Domain.Batch.BatchShipment
                    {
                        Id = shipmentEntry.Id,
                        BatchId = batchEntry.Id,
                        ShipmentId = shipmentId,
                        CustomOrderNumber = order?.CustomOrderNumber,
                        TotalWeight = shipmentEntry.TotalWeight.HasValue ? $"{shipmentEntry.TotalWeight:F2} [{measureWeight.Name}]" : null,
                        Status = BatchShipmentStatus.Unknown
                    };
                }).ToListAsync();

                return batchShipments;
            });
        }

        /// <summary>
        /// Generate a label for the batch
        /// </summary>
        /// <param name="batchEntry">Batch entry</param>
        /// <param name="labelFormat">Label format</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> GenerateBatchLabelAsync(EasyPostBatch batchEntry, string labelFormat)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (batchEntry is null)
                    throw new ArgumentNullException(nameof(batchEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if ((BatchStatus)batchEntry.StatusId != BatchStatus.Purchased)
                    throw new NopException("Batch is not yet purchased");

                var batch = Batch.Retrieve(batchEntry.BatchId)
                    ?? throw new NopException("No response from the service");

                batch.GenerateLabel((labelFormat ?? "pdf").ToUpper());
                batchEntry.LabelFormat = labelFormat;
                await UpdateBatchAsync(batchEntry);

                return true;
            });
        }

        /// <summary>
        /// Download a label for the batch
        /// </summary>
        /// <param name="batchEntry">Batch entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file details; error message if exists
        /// </returns>
        public async Task<((string Url, string ContentType), string Error)> DownloadBatchLabelAsync(EasyPostBatch batchEntry)
        {
            return await HandleFunctionAsync(() =>
            {
                if (batchEntry is null)
                    throw new ArgumentNullException(nameof(batchEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if ((BatchStatus)batchEntry.StatusId != BatchStatus.LabelGenerated)
                    throw new NopException("Batch label is not yet generated");

                var batch = Batch.Retrieve(batchEntry.BatchId)
                    ?? throw new NopException("No response from the service");

                if (string.IsNullOrEmpty(batch.label_url))
                    throw new NopException("Batch label is not found");

                var fileType = batchEntry.LabelFormat?.ToLower() switch
                {
                    "zpl" => "application/zpl",
                    _ => MimeTypes.ApplicationPdf
                };

                return Task.FromResult((batch.label_url, fileType));
            });
        }

        /// <summary>
        /// Generate a manifest file for the batch
        /// </summary>
        /// <param name="batchEntry">Batch entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the function result; error message if exists
        /// </returns>
        public async Task<(bool Result, string Error)> GenerateBatchManifestAsync(EasyPostBatch batchEntry)
        {
            return await HandleFunctionAsync(() =>
            {
                if (batchEntry is null)
                    throw new ArgumentNullException(nameof(batchEntry));

                if (!IsConfigured())
                    throw new NopException("Plugin not configured");

                if ((BatchStatus)batchEntry.StatusId == BatchStatus.Unknown ||
                    (BatchStatus)batchEntry.StatusId == BatchStatus.Creating ||
                    (BatchStatus)batchEntry.StatusId == BatchStatus.CreationFailed)
                {
                    throw new NopException("Batch is not yet created");
                }

                var batch = Batch.Retrieve(batchEntry.BatchId)
                    ?? throw new NopException("No response from the service");

                batch.GenerateScanForm();

                return Task.FromResult(true);
            });
        }

        #endregion

        #endregion
    }
}