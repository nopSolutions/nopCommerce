using System.Globalization;
using System.Text;
using Avalara.AvaTax.RestClient;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents the manager that operates with requests to the Avalara services
    /// </summary>
    public class AvalaraTaxManager : IDisposable
    {
        #region Fields

        protected readonly AvalaraTaxSettings _avalaraTaxSettings;
        protected readonly IAddressService _addressService;
        protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
        protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
        protected readonly ICountryService _countryService;
        protected readonly ICustomerService _customerService;
        protected readonly IGenericAttributeService _genericAttributeService;
        protected readonly IGeoLookupService _geoLookupService;
        protected readonly ILogger _logger;
        protected readonly INopFileProvider _fileProvider;
        protected readonly IOrderService _orderService;
        protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
        protected readonly IPaymentService _paymentService;
        protected readonly IProductAttributeService _productAttributeService;
        protected readonly IProductService _productService;
        protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
        protected readonly IRepository<TaxCategory> _taxCategoryRepository;
        protected readonly IShoppingCartService _shoppingCartService;
        protected readonly IStateProvinceService _stateProvinceService;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly ITaxCategoryService _taxCategoryService;
        protected readonly IWorkContext _workContext;
        protected readonly ShippingSettings _shippingSettings;
        protected readonly TaxSettings _taxSettings;
        protected readonly TaxTransactionLogService _taxTransactionLogService;

        protected AvaTaxClient _serviceClient;
        protected bool _disposed;

        #endregion

        #region Ctor

        public AvalaraTaxManager(AvalaraTaxSettings avalaraTaxSettings,
            IAddressService addressService,
            IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
            IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILogger logger,
            INopFileProvider fileProvider,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            ITaxCategoryService taxCategoryService,
            IWorkContext workContext,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            TaxTransactionLogService taxTransactionLogService)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _addressService = addressService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _logger = logger;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _genericAttributeRepository = genericAttributeRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _taxCategoryService = taxCategoryService;
            _workContext = workContext;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _taxTransactionLogService = taxTransactionLogService;
        }

        #endregion

        #region Utilities

        #region Common

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event args</param>
        protected async void OnCallCompleted(object sender, EventArgs args)
        {
            if (args is not AvaTaxCallEventArgs avaTaxCallEventArgs)
                return;

            var customer = await _workContext.GetCurrentCustomerAsync();

            //log request results
            await _taxTransactionLogService.InsertTaxTransactionLogAsync(new TaxTransactionLog
            {
                StatusCode = (int)avaTaxCallEventArgs.Code,
                Url = avaTaxCallEventArgs.RequestUri.ToString(),
                RequestMessage = avaTaxCallEventArgs.RequestBody,
                ResponseMessage = avaTaxCallEventArgs.ResponseString,
                CustomerId = customer.Id,
                CreatedDateUtc = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Check that tax provider is configured
        /// </summary>
        /// <returns>True if it's configured; otherwise false</returns>
        protected bool IsConfigured()
        {
            return !string.IsNullOrEmpty(_avalaraTaxSettings.AccountId)
                && !string.IsNullOrEmpty(_avalaraTaxSettings.LicenseKey);
        }

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected async Task<TResult> HandleFunctionAsync<TResult>(Func<Task<TResult>> function)
        {
            try
            {
                //ensure that Avalara tax provider is configured
                if (!IsConfigured())
                    throw new NopException("Tax provider is not configured");

                return await function();
            }
            catch (Exception exception)
            {
                //compose an error message
                var errorMessage = exception.Message;
                if (exception is AvaTaxError avaTaxError && avaTaxError.error != null)
                {
                    var errorInfo = avaTaxError.error.error;
                    if (errorInfo != null)
                    {
                        errorMessage = $"{errorInfo.code} - {errorInfo.message}{Environment.NewLine}";
                        if (errorInfo.details?.Any() ?? false)
                        {
                            var errorDetails = errorInfo.details.Aggregate(string.Empty, (error, detail) => $"{error}{detail.description}{Environment.NewLine}");
                            errorMessage = $"{errorMessage} Details: {errorDetails}";
                        }
                    }
                }

                //log errors
                await _logger.ErrorAsync($"{AvalaraTaxDefaults.SystemName} error. {errorMessage}", exception, await _workContext.GetCurrentCustomerAsync());

                return default;
            }
        }

        #endregion

        #region Tax calculation

        /// <summary>
        /// Create tax transaction
        /// </summary>
        /// <param name="model">Transaction details</param>
        /// <returns>Created transaction</returns>
        protected TransactionModel CreateTransaction(CreateTransactionModel model)
        {
            var transaction = ServiceClient.CreateTransaction(null, model)
                ?? throw new NopException("No response from the service");

            //whether there are any errors
            if (transaction.messages?.Any() ?? false)
            {
                var message = transaction.messages.Aggregate(string.Empty, (error, message) => $"{error}{message.summary}{Environment.NewLine}");
                throw new NopException(message);
            }

            return transaction;
        }

        /// <summary>
        /// Prepare model to create a tax transaction
        /// </summary>
        /// <param name="address">Tax address</param>
        /// <param name="customerCode">Customer code</param>
        /// <param name="documentType">Transaction document type</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the model
        /// </returns>
        protected async Task<CreateTransactionModel> PrepareTransactionModelAsync(Address address, string customerCode, DocumentType documentType)
        {
            var model = new CreateTransactionModel
            {
                customerCode = CommonHelper.EnsureMaximumLength(customerCode, 50),
                date = DateTime.UtcNow,
                type = documentType
            };

            //set company code
            var companyCode = !string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode)
                && !_avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString())
                ? _avalaraTaxSettings.CompanyCode
                : null;
            model.companyCode = CommonHelper.EnsureMaximumLength(companyCode, 25);

            //set tax addresses
            model.addresses = new AddressesModel();
            var originAddress = _avalaraTaxSettings.TaxOriginAddressType switch
            {
                TaxOriginAddressType.ShippingOrigin => await _addressService.GetAddressByIdAsync(_shippingSettings.ShippingOriginAddressId),
                TaxOriginAddressType.DefaultTaxAddress => await _addressService.GetAddressByIdAsync(_taxSettings.DefaultTaxAddressId),
                _ => null
            };
            var shipFromAddress = await MapAddressAsync(originAddress);
            var shipToAddress = await MapAddressAsync(address);
            if (shipFromAddress != null && shipToAddress != null)
            {
                model.addresses.shipFrom = shipFromAddress;
                model.addresses.shipTo = shipToAddress;
            }
            else
                model.addresses.singleLocation = shipToAddress ?? shipFromAddress;

            return model;
        }

        /// <summary>
        /// Prepare order addresses
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="order">Order</param>
        /// <param name="storeId">Store id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task PrepareOrderAddressesAsync(Customer customer, Order order, int storeId)
        {
            order.BillingAddressId = customer.BillingAddressId ?? 0;
            order.ShippingAddressId = customer.ShippingAddressId;
            if (_shippingSettings.AllowPickupInStore)
            {
                var pickupPoint = await _genericAttributeService
                    .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, storeId);
                if (pickupPoint != null)
                {
                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
                    var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);
                    var pickupAddress = new Address
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        CountryId = country?.Id,
                        StateProvinceId = state?.Id,
                        ZipPostalCode = pickupPoint.ZipPostalCode,
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                    await _addressService.InsertAddressAsync(pickupAddress);
                    order.PickupAddressId = pickupAddress.Id;
                }
            }
        }

        /// <summary>
        /// Get a tax address of the passed order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        protected async Task<Address> GetTaxAddressAsync(Order order)
        {
            Address address = null;

            //tax is based on billing address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress &&
                await _addressService.GetAddressByIdAsync(order.BillingAddressId) is Address billingAddress)
            {
                address = billingAddress;
            }

            //tax is based on shipping address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress &&
                order.ShippingAddressId.HasValue &&
                await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value) is Address shippingAddress)
            {
                address = shippingAddress;
            }

            //tax is based on pickup point address
            if (_taxSettings.TaxBasedOnPickupPointAddress &&
                order.PickupAddressId.HasValue &&
                await _addressService.GetAddressByIdAsync(order.PickupAddressId.Value) is Address pickupAddress)
            {
                address = pickupAddress;
            }

            //or use default address for tax calculation
            if (address == null)
                address = await _addressService.GetAddressByIdAsync(_taxSettings.DefaultTaxAddressId);

            return address;
        }

        /// <summary>
        /// Map address model
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address model
        /// </returns>
        protected async Task<AddressLocationInfo> MapAddressAsync(Address address)
        {
            return address == null ? null : new AddressLocationInfo
            {
                city = CommonHelper.EnsureMaximumLength(address.City, 50),
                country = CommonHelper.EnsureMaximumLength((await _countryService.GetCountryByAddressAsync(address))?.TwoLetterIsoCode, 2),
                line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                region = CommonHelper.EnsureMaximumLength((await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Abbreviation, 3)
            };
        }

        /// <summary>
        /// Get item lines to create tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Order items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of item lines
        /// </returns>
        protected async Task<List<LineItemModel>> GetItemLinesAsync(Order order, IList<OrderItem> orderItems)
        {
            //get purchased products details
            var items = await CreateLinesForOrderItemsAsync(order, orderItems);

            //set payment method additional fee as the separate item line
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                items.Add(await CreateLineForPaymentMethodAsync(order));

            //set shipping rate as the separate item line
            if (order.OrderShippingExclTax > decimal.Zero)
                items.Add(await CreateLineForShippingAsync(order));

            //set checkout attributes as the separate item lines
            if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
                items.AddRange(await CreateLinesForCheckoutAttributesAsync(order));

            return items;
        }

        /// <summary>
        /// Create item lines for purchased order items
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Order items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of item lines
        /// </returns>
        protected async Task<List<LineItemModel>> CreateLinesForOrderItemsAsync(Order order, IList<OrderItem> orderItems)
        {
            return await orderItems.SelectAwait(async orderItem =>
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                var item = new LineItemModel
                {
                    amount = orderItem.PriceExclTax,

                    //set name as item description to avoid long values
                    description = CommonHelper.EnsureMaximumLength(product?.Name, 2096),

                    //whether the discount to the item was applied
                    discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                    //product exemption
                    exemptionCode = product?.IsTaxExempt ?? false
                        ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{product.Id}", 25)
                        : string.Empty,

                    //set SKU as item code
                    itemCode = product != null
                        ? CommonHelper.EnsureMaximumLength(await _productService.FormatSkuAsync(product, orderItem.AttributesXml), 50)
                        : string.Empty,

                    quantity = orderItem.Quantity
                };

                //force to use billing address as the tax address one in the accordance with EU VAT rules (if enabled)
                if (_taxSettings.EuVatEnabled)
                {
                    var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                    var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                    var useEuVatRules = (product?.IsTelecommunicationsOrBroadcastingOrElectronicServices ?? false)
                        && ((await _countryService.GetCountryByAddressAsync(billingAddress)
                            ?? await _countryService.GetCountryByIdAsync(customer.CountryId)
                            ?? await _countryService.GetCountryByTwoLetterIsoCodeAsync(_geoLookupService.LookupCountryIsoCode(customer.LastIpAddress)))
                            ?.SubjectToVat ?? false)
                        && customer.VatNumberStatusId != (int)VatNumberStatus.Valid;

                    if (useEuVatRules)
                    {
                        var address = await MapAddressAsync(billingAddress);
                        if (address != null)
                            item.addresses = new AddressesModel { singleLocation = address };
                    }
                }

                //set tax code
                var productTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(product?.TaxCategoryId ?? 0);
                item.taxCode = CommonHelper.EnsureMaximumLength(productTaxCategory?.Name, 25);

                //whether entity use code is set
                var entityUseCode = product != null
                    ? await _genericAttributeService.GetAttributeAsync<string>(product, AvalaraTaxDefaults.EntityUseCodeAttribute)
                    : string.Empty;
                item.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                return item;
            }).ToListAsync();
        }

        /// <summary>
        /// Create a separate item line for the order payment method additional fee
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the item line
        /// </returns>
        protected async Task<LineItemModel> CreateLineForPaymentMethodAsync(Order order)
        {
            var paymentItem = new LineItemModel
            {
                amount = order.PaymentMethodAdditionalFeeExclTax,

                //item description
                description = "Payment method additional fee",

                //set payment method system name as item code
                itemCode = CommonHelper.EnsureMaximumLength(order.PaymentMethodSystemName, 50),

                quantity = 1
            };

            //whether payment is taxable
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                //try to get tax code
                var paymentTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(_taxSettings.PaymentMethodAdditionalFeeTaxClassId);
                paymentItem.taxCode = CommonHelper.EnsureMaximumLength(paymentTaxCategory?.Name, 25);
            }
            else
            {
                //if payment is non-taxable, set it as exempt
                paymentItem.exemptionCode = "Payment-fee-non-taxable";
            }

            return paymentItem;
        }

        /// <summary>
        /// Create a separate item line for the order shipping charge
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the item line
        /// </returns>
        protected async Task<LineItemModel> CreateLineForShippingAsync(Order order)
        {
            var shippingItem = new LineItemModel
            {
                amount = order.OrderShippingExclTax,

                //item description
                description = "Shipping rate",

                //set shipping method name as item code
                itemCode = CommonHelper.EnsureMaximumLength(order.ShippingMethod, 50),

                quantity = 1
            };

            //whether shipping is taxable
            if (_taxSettings.ShippingIsTaxable)
            {
                //try to get tax code
                var shippingTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(_taxSettings.ShippingTaxClassId);
                shippingItem.taxCode = CommonHelper.EnsureMaximumLength(shippingTaxCategory?.Name, 25);
            }
            else
            {
                //if shipping is non-taxable, set it as exempt
                shippingItem.exemptionCode = "Shipping-rate-non-taxable";
            }

            return shippingItem;
        }

        /// <summary>
        /// Create item lines for order checkout attributes
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of item lines
        /// </returns>
        protected async Task<IEnumerable<LineItemModel>> CreateLinesForCheckoutAttributesAsync(Order order)
        {
            //get checkout attributes values
            var attributeValues = _checkoutAttributeParser.ParseAttributeValues(order.CheckoutAttributesXml);
            return await attributeValues.SelectManyAwait(async attributeWithValues =>
            {
                var attribute = attributeWithValues.attribute;
                return (await attributeWithValues.values.SelectAwait(async value =>
                {
                    //create line
                    var checkoutAttributeItem = new LineItemModel
                    {
                        amount = value.PriceAdjustment,

                        //item description
                        description = CommonHelper.EnsureMaximumLength($"{attribute.Name} ({value.Name})", 2096),

                        //whether the discount to the item was applied
                        discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                        //set checkout attribute name and value as item code
                        itemCode = CommonHelper.EnsureMaximumLength($"{attribute.Name}-{value.Name}", 50),

                        quantity = 1
                    };

                    //whether checkout attribute is tax exempt
                    if (attribute.IsTaxExempt)
                        checkoutAttributeItem.exemptionCode = "Attribute-non-taxable";
                    else
                    {
                        //or try to get tax code
                        var attributeTaxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(attribute.TaxCategoryId);
                        checkoutAttributeItem.taxCode = CommonHelper.EnsureMaximumLength(attributeTaxCategory?.Name, 25);
                    }

                    //whether entity use code is set
                    var entityUseCode = await _genericAttributeService.GetAttributeAsync<string>(attribute, AvalaraTaxDefaults.EntityUseCodeAttribute);
                    checkoutAttributeItem.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                    return checkoutAttributeItem;
                }).ToListAsync()).ToAsyncEnumerable();
            }).ToListAsync();
        }

        /// <summary>
        /// Prepare model tax exemption details
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the model
        /// </returns>
        protected async Task<CreateTransactionModel> PrepareModelTaxExemptionAsync(CreateTransactionModel model, Customer customer)
        {
            if (customer.IsTaxExempt)
                model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-customer-#{customer.Id}", 25);
            else
            {
                var customerRole = (await _customerService.GetCustomerRolesAsync(customer)).FirstOrDefault(role => role.TaxExempt);
                if (customerRole != null)
                    model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-{customerRole.Name}", 25);
            }

            var entityUseCode = await _genericAttributeService.GetAttributeAsync<string>(customer, AvalaraTaxDefaults.EntityUseCodeAttribute);
            if (!string.IsNullOrEmpty(entityUseCode))
                model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
            else
            {
                entityUseCode = await (await _customerService.GetCustomerRolesAsync(customer))
                    .SelectAwait(async customerRole => await _genericAttributeService.GetAttributeAsync<string>(customerRole, AvalaraTaxDefaults.EntityUseCodeAttribute))
                    .FirstOrDefaultAsync(code => !string.IsNullOrEmpty(code));
                model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
            }

            return model;
        }

        /// <summary>
        /// Get tax rates from the file
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tax rates list
        /// </returns>
        protected async Task<List<TaxRate>> GetTaxRatesFromFileAsync()
        {
            //try to create file if doesn't exist
            var filePath = _fileProvider.MapPath(AvalaraTaxDefaults.TaxRatesFilePath);
            if (!_fileProvider.FileExists(filePath))
                await DownloadTaxRatesAsync();

            if (!_fileProvider.FileExists(filePath))
                throw new NopException($"File {AvalaraTaxDefaults.TaxRatesFilePath} not found");

            //get file lines
            var text = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                throw new NopException($"File {AvalaraTaxDefaults.TaxRatesFilePath} is empty");

            var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            if (!lines.Any() || lines[0].Split(',').Length < 14)
                throw new NopException($"Unsupported file {AvalaraTaxDefaults.TaxRatesFilePath} structure");

            //prepare tax rates
            var taxRates = lines.Skip(1).Select(line =>
            {
                try
                {
                    var values = line.Split(',', StringSplitOptions.TrimEntries);
                    return new TaxRate
                    {
                        Zip = values[0], //ZIP_CODE
                        State = values[1], //STATE_ABBREV
                        County = values[2], //COUNTY_NAME
                        City = values[3], //CITY_NAME
                        StateTax = decimal.Parse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture), //STATE_SALES_TAX
                        CountyTax = decimal.Parse(values[6], NumberStyles.Any, CultureInfo.InvariantCulture), //COUNTY_SALES_TAX
                        CityTax = decimal.Parse(values[8], NumberStyles.Any, CultureInfo.InvariantCulture), //CITY_SALES_TAX
                        TotalTax = decimal.Parse(values[10], NumberStyles.Any, CultureInfo.InvariantCulture), //TOTAL_SALES_TAX
                        ShippingTaxable = string.Equals(values[11], "y", StringComparison.InvariantCultureIgnoreCase), //TAX_SHIPPING_ALONE
                        ShippingAndHadlingTaxable = string.Equals(values[12], "y", StringComparison.InvariantCultureIgnoreCase) //TAX_SHIPPING_AND_HANDLING_TOGETHER
                    };
                }
                catch
                {
                    return null;
                }
            }).Where(taxRate => taxRate is not null).ToList();

            return taxRates;
        }

        #endregion

        #region Certificates

        /// <summary>
        /// Create or update the passed customer for the company
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="companyId">Selected company id</param>
        /// <param name="customerExists">Whether the customer is already created</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer details
        /// </returns>
        protected async Task<CustomerModel> CreateOrUpdateCustomerAsync(Customer customer, int companyId, bool customerExists)
        {
            var defaultAddress = new Address
            {
                Address1 = customer.StreetAddress,
                Address2 = customer.StreetAddress2,
                City = customer.City,
                ZipPostalCode = customer.ZipPostalCode,
                StateProvinceId = customer.StateProvinceId,
                CountryId = customer.CountryId
            };
            var address = await MapAddressAsync(defaultAddress);
            var model = new CustomerModel
            {
                companyId = companyId,
                customerCode = customer.Id.ToString(),
                alternateId = customer.CustomerGuid.ToString().ToLowerInvariant(),
                name = await _customerService.GetCustomerFullNameAsync(customer),
                emailAddress = customer.Email,
                line1 = address.line1,
                line2 = address.line2,
                city = address.city,
                postalCode = address.postalCode,
                country = address.country,
                region = address.region
            };

            var customerDetails = customerExists
                ? await ServiceClient.UpdateCustomerAsync(companyId, customer.Id.ToString(), model)
                : (await ServiceClient.CreateCustomersAsync(companyId, new List<CustomerModel> { model }))?.FirstOrDefault();

            return customerDetails;
        }

        #endregion

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Ping service (test conection)
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ping result
        /// </returns>
        public async Task<PingResultModel> PingAsync()
        {
            return await HandleFunctionAsync(() => Task.FromResult(ServiceClient.Ping() ?? throw new NopException("No response from the service")));
        }

        /// <summary>
        /// Get account companies
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of companies
        /// </returns>
        public async Task<List<CompanyModel>> GetAccountCompaniesAsync()
        {
            return await HandleFunctionAsync(() =>
            {
                var result = ServiceClient.QueryCompanies(null, null, null, null, null)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(result.value);
            });
        }

        /// <summary>
        /// Get pre-defined entity use codes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of entity use codes
        /// </returns>
        public async Task<List<EntityUseCodeModel>> GetEntityUseCodesAsync()
        {
            return await HandleFunctionAsync(() =>
            {
                var result = ServiceClient.ListEntityUseCodes(null, null, null, null)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(result.value);
            });
        }

        /// <summary>
        /// Get pre-defined tax code types
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the key-value pairs of tax code types
        /// </returns>
        public async Task<Dictionary<string, string>> GetTaxCodeTypesAsync()
        {
            return await HandleFunctionAsync(() =>
            {
                var result = ServiceClient.ListTaxCodeTypes(null, null)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(result.types);
            });
        }

        /// <summary>
        /// Import tax codes from Avalara services
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of imported tax codes; null in case of error
        /// </returns>
        public async Task<int?> ImportTaxCodesAsync()
        {
            return await HandleFunctionAsync<int?>(async () =>
            {
                //get Avalara pre-defined system tax codes (only active)
                var systemTaxCodes = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                if (!systemTaxCodes.value?.Any() ?? true)
                    return null;

                //get existing tax categories
                var existingTaxCategories = (await _taxCategoryService.GetAllTaxCategoriesAsync())
                    .Select(taxCategory => taxCategory.Name)
                    .ToList();

                //remove duplicates
                var taxCodesToImport = systemTaxCodes.value
                    .Where(taxCode => !string.IsNullOrEmpty(taxCode?.taxCode) && !existingTaxCategories.Contains(taxCode.taxCode))
                    .ToList();

                var importedTaxCodesNumber = 0;
                foreach (var taxCode in taxCodesToImport)
                {
                    //create new tax category
                    var taxCategory = new TaxCategory { Name = taxCode.taxCode };
                    await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

                    //save description and type
                    if (!string.IsNullOrEmpty(taxCode.description))
                        await _genericAttributeService.SaveAttributeAsync(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, taxCode.description);
                    if (!string.IsNullOrEmpty(taxCode.taxCodeTypeId))
                        await _genericAttributeService.SaveAttributeAsync(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, taxCode.taxCodeTypeId);

                    importedTaxCodesNumber++;
                }

                return importedTaxCodesNumber;
            });
        }

        /// <summary>
        /// Export current tax codes to Avalara services
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of exported tax codes; null in case of error
        /// </returns>
        public async Task<int?> ExportTaxCodesAsync()
        {
            return await HandleFunctionAsync<int?>(async () =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                //get selected company
                var selectedCompany = (await GetAccountCompaniesAsync())
                    ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                    ?? throw new NopException("Failed to retrieve company");

                //get existing tax codes (only active)
                var taxCodes = await ServiceClient.ListTaxCodesByCompanyAsync(selectedCompany.id, "isActive eq true", null, null, null, null)
                    ?? throw new NopException("No response from the service");

                var existingTaxCodes = taxCodes.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();

                //prepare tax codes to export
                var taxCodesToExport = await (await _taxCategoryService.GetAllTaxCategoriesAsync()).SelectAwait(async taxCategory => new TaxCodeModel
                {
                    createdDate = DateTime.UtcNow,
                    description = CommonHelper.EnsureMaximumLength(taxCategory.Name, 255),
                    isActive = true,
                    taxCode = CommonHelper.EnsureMaximumLength(taxCategory.Name, 25),
                    taxCodeTypeId = CommonHelper.EnsureMaximumLength(await _genericAttributeService
                        .GetAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute) ?? "P", 2)
                }).Where(taxCode => !string.IsNullOrEmpty(taxCode.taxCode)).ToListAsync();

                //add Avalara pre-defined system tax codes
                var systemTaxCodesResult = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();
                existingTaxCodes.AddRange(systemTaxCodes);

                //remove duplicates
                taxCodesToExport = taxCodesToExport.Where(taxCode => !existingTaxCodes.Contains(taxCode.taxCode)).Distinct().ToList();

                //export tax codes
                if (!taxCodesToExport.Any())
                    return 0;

                //create items and get the result
                var createdTaxCodes = await ServiceClient.CreateTaxCodesAsync(selectedCompany.id, taxCodesToExport)
                    ?? throw new NopException("No response from the service");

                //display results
                var result = createdTaxCodes?.Count;
                if (result.HasValue && result > 0)
                    return result.Value;

                return null;
            });
        }

        /// <summary>
        /// Delete pre-defined system tax codes
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<bool> DeleteSystemTaxCodesAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                //get Avalara pre-defined system tax codes (only active)
                var systemTaxCodesResult = await ServiceClient.ListTaxCodesAsync("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList();
                if (!systemTaxCodes?.Any() ?? true)
                    return false;

                //prepare tax categories to delete
                var categoriesIds = await _taxCategoryRepository.Table
                    .Where(taxCategory => systemTaxCodes.Contains(taxCategory.Name))
                    .Select(taxCategory => taxCategory.Id)
                    .ToListAsync();

                //delete tax categories
                await _taxCategoryRepository.DeleteAsync(taxCategory => categoriesIds.Contains(taxCategory.Id));
                await _staticCacheManager.RemoveByPrefixAsync(NopEntityCacheDefaults<TaxCategory>.Prefix);

                //delete generic attributes
                await _genericAttributeRepository
                    .DeleteAsync(attribute => attribute.KeyGroup == nameof(TaxCategory) && categoriesIds.Contains(attribute.EntityId));

                return true;
            });
        }

        /// <summary>
        /// Delete generic attributes used in the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteAttributesAsync()
        {
            await DeleteSystemTaxCodesAsync();

            await _genericAttributeRepository.DeleteAsync(attribute => attribute.Key == AvalaraTaxDefaults.EntityUseCodeAttribute ||
                attribute.Key == AvalaraTaxDefaults.TaxCodeTypeAttribute || attribute.Key == AvalaraTaxDefaults.TaxCodeDescriptionAttribute);
        }

        /// <summary>
        /// Export items (products) with the passed ids to Avalara services
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of exported items; null in case of error
        /// </returns>
        public async Task<int?> ExportProductsAsync(string selectedIds)
        {
            return await HandleFunctionAsync<int?>(async () =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                //get selected company
                var selectedCompany = (await GetAccountCompaniesAsync())
                    ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                    ?? throw new NopException("Failed to retrieve company");

                //get existing items
                var items = await ServiceClient.ListItemsByCompanyAsync(selectedCompany.id, null, null, null, null, null, null)
                    ?? throw new NopException("No response from the service");

                //return the paginated and filtered list
                var existingItemCodes = items.value?.Select(item => item.itemCode).ToList() ?? new List<string>();

                //prepare exported items
                var productIds = selectedIds?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToArray();
                var exportedItems = new List<ItemModel>();
                foreach (var product in await _productService.GetProductsByIdsAsync(productIds))
                {
                    //find product combinations
                    var combinations = (await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
                        .Where(combination => !string.IsNullOrEmpty(combination.Sku));

                    //export items with specified SKU only
                    if (string.IsNullOrEmpty(product.Sku) && !combinations.Any())
                        continue;

                    //prepare common properties
                    var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(product.TaxCategoryId);
                    var taxCode = CommonHelper.EnsureMaximumLength(taxCategory?.Name, 25);
                    var description = CommonHelper.EnsureMaximumLength(product.Name, 255);

                    //add the product as exported item
                    if (!string.IsNullOrEmpty(product.Sku))
                    {
                        exportedItems.Add(new ItemModel
                        {
                            createdDate = DateTime.UtcNow,
                            description = description,
                            itemCode = CommonHelper.EnsureMaximumLength(product.Sku, 50),
                            taxCode = taxCode
                        });
                    }

                    //add product combinations
                    exportedItems.AddRange(combinations.Select(combination => new ItemModel
                    {
                        createdDate = DateTime.UtcNow,
                        description = description,
                        itemCode = CommonHelper.EnsureMaximumLength(combination.Sku, 50),
                        taxCode = taxCode
                    }));
                }

                //remove duplicates
                exportedItems = exportedItems.Where(item => !existingItemCodes.Contains(item.itemCode)).Distinct().ToList();

                //export items
                if (!exportedItems.Any())
                    return 0;

                //create items and get the result
                var createdItems = await ServiceClient.CreateItemsAsync(selectedCompany.id, exportedItems)
                    ?? throw new NopException("No response from the service");

                //display results
                var result = createdItems?.Count;
                if (result.HasValue && result > 0)
                    return result.Value;

                return null;
            });
        }

        #endregion

        #region Validation

        /// <summary>
        /// Resolve the passed address against Avalara's address-validation system
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the validated address
        /// </returns>
        public async Task<AddressResolutionModel> ValidateAddressAsync(Address address)
        {
            return (await HandleFunctionAsync(async () => await ServiceClient.ResolveAddressPostAsync(new AddressValidationInfo
            {
                city = CommonHelper.EnsureMaximumLength(address.City, 50),
                country = CommonHelper.EnsureMaximumLength((await _countryService.GetCountryByAddressAsync(address))?.TwoLetterIsoCode, 2),
                line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                region = CommonHelper.EnsureMaximumLength((await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Abbreviation, 3),
                textCase = TextCase.Mixed
            }) ?? throw new NopException("No response from the service")));
        }

        #endregion

        #region Tax calculation

        /// <summary>
        /// Create test tax transaction
        /// </summary>
        /// <param name="address">Tax address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ransaction
        /// </returns>
        public async Task<TransactionModel> CreateTestTaxTransactionAsync(Address address)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.UseTaxRateTables)
                {
                    var taxRates = await GetTaxRatesFromFileAsync();
                    var taxRate = taxRates.FirstOrDefault(record => address.ZipPostalCode?.StartsWith(record.Zip) ?? false);
                    if (taxRate?.TotalTax is null)
                        throw new NopException($"No rate found for zip code {address.ZipPostalCode}");

                    var summary = new List<TransactionSummary>();
                    if (!string.IsNullOrEmpty(taxRate.State))
                        summary.Add(new() { jurisName = taxRate.State, rate = taxRate.StateTax });
                    if (!string.IsNullOrEmpty(taxRate.County))
                        summary.Add(new() { jurisName = taxRate.County, rate = taxRate.CountyTax });
                    if (!string.IsNullOrEmpty(taxRate.City))
                        summary.Add(new() { jurisName = taxRate.City, rate = taxRate.CityTax });
                    return new TransactionModel { totalTax = taxRate.TotalTax * 100, summary = summary };
                }

                var customer = await _workContext.GetCurrentCustomerAsync();
                //create tax transaction for a simplified item and without saving 
                var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);
                model.lines = new List<LineItemModel> { new LineItemModel { amount = 100, quantity = 1 } };
                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Create transaction to get tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ransaction
        /// </returns>
        public async Task<decimal?> GetTaxRateAsync(TaxRateRequest taxRateRequest)
        {
            if (_avalaraTaxSettings.UseTaxRateTables)
            {
                var key = _staticCacheManager
                    .PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxRateByZipCacheKey, taxRateRequest.Address.ZipPostalCode);
                return await _staticCacheManager.GetAsync(key, async () => await HandleFunctionAsync(async () =>
                {
                    var taxRates = await GetTaxRatesFromFileAsync();
                    var taxRate = taxRates.FirstOrDefault(record => taxRateRequest.Address.ZipPostalCode?.StartsWith(record.Zip) ?? false);
                    return taxRate?.TotalTax * 100;
                }));
            }

            //prepare cache key
            var address = await _addressService.GetAddressByIdAsync(taxRateRequest.Address.Id);
            var customer = taxRateRequest.Customer ?? await _workContext.GetCurrentCustomerAsync();
            var taxCategoryId = taxRateRequest.TaxCategoryId > 0
                ? taxRateRequest.TaxCategoryId
                : taxRateRequest.Product?.TaxCategoryId ?? 0;
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxRateCacheKey,
                _avalaraTaxSettings.GetTaxRateByAddressOnly ? null : customer,
                _avalaraTaxSettings.GetTaxRateByAddressOnly ? 0 : taxCategoryId,
                taxRateRequest.Address.Address1,
                taxRateRequest.Address.City,
                taxRateRequest.Address.StateProvinceId ?? 0,
                taxRateRequest.Address.CountryId ?? 0,
                taxRateRequest.Address.ZipPostalCode);
            if (_avalaraTaxSettings.GetTaxRateByAddressOnly && _avalaraTaxSettings.TaxRateByAddressCacheTime > 0)
                cacheKey.CacheTime = _avalaraTaxSettings.TaxRateByAddressCacheTime;

            //get tax rate
            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                return await HandleFunctionAsync(async () =>
                {
                    //create tax transaction for a single item and without saving
                    var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);
                    var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(taxCategoryId);
                    model.lines = new List<LineItemModel>
                    {
                        new LineItemModel
                        {
                            amount = 100,
                            quantity = 1,
                            itemCode = CommonHelper.EnsureMaximumLength(taxRateRequest.Product?.Sku, 50),
                            taxCode = CommonHelper.EnsureMaximumLength(taxCategory?.Name, 25),
                            exemptionCode = !_avalaraTaxSettings.GetTaxRateByAddressOnly && (taxRateRequest.Product?.IsTaxExempt ?? false)
                                ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{taxRateRequest.Product.Id}", 25)
                                : string.Empty,
                        }
                    };

                    //prepare tax exemption 
                    if (!_avalaraTaxSettings.GetTaxRateByAddressOnly)
                        await PrepareModelTaxExemptionAsync(model, customer);

                    var transaction = CreateTransaction(model);

                    //we return the tax total, since we used the amount of 100 when requesting, so the total is the same as the rate
                    return transaction?.totalTax;
                });
            });
        }

        /// <summary>
        /// Create transaction to get tax total for the passed request
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ransaction
        /// </returns>
        public async Task<TransactionModel> CreateTaxTotalTransactionAsync(TaxTotalRequest taxTotalRequest)
        {
            return await HandleFunctionAsync(async () =>
            {
                //create dummy order to create tax transaction
                var customer = taxTotalRequest.Customer;
                var order = new Order { CustomerId = customer.Id };

                //addresses
                await PrepareOrderAddressesAsync(customer, order, taxTotalRequest.StoreId);

                //checkout attributes
                order.CheckoutAttributesXml = await _genericAttributeService
                    .GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, taxTotalRequest.StoreId);

                //shipping method
                order.ShippingMethod = (await _genericAttributeService
                    .GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, taxTotalRequest.StoreId))?.Name;
                order.OrderShippingExclTax = (await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, false)).shippingTotal ?? 0;

                //payment method
                if (taxTotalRequest.UsePaymentMethodAdditionalFee)
                {
                    order.PaymentMethodSystemName = await _genericAttributeService
                        .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId);
                    if (!string.IsNullOrEmpty(order.PaymentMethodSystemName))
                        order.PaymentMethodAdditionalFeeExclTax = await _paymentService.GetAdditionalHandlingFeeAsync(taxTotalRequest.ShoppingCart, order.PaymentMethodSystemName);
                }

                //discount amount
                var (orderSubTotalDiscountExclTax, _, _, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(taxTotalRequest.ShoppingCart, false);
                order.OrderSubTotalDiscountExclTax = orderSubTotalDiscountExclTax;

                //create dummy order items
                var orderItems = await taxTotalRequest.ShoppingCart.SelectAwait(async cartItem => new OrderItem
                {
                    AttributesXml = cartItem.AttributesXml,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceExclTax = (await _shoppingCartService.GetSubTotalAsync(cartItem, true)).subTotal
                }).ToListAsync();

                //prepare transaction model
                var address = await GetTaxAddressAsync(order);
                var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesOrder);
                model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
                model.discount = order.OrderSubTotalDiscountExclTax;

                //set purchased item lines
                model.lines = await GetItemLinesAsync(order, orderItems);

                //set whole request tax exemption
                await PrepareModelTaxExemptionAsync(model, customer);

                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Create tax transaction for the placed order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ransaction
        /// </returns>
        public async Task<TransactionModel> CreateOrderTaxTransactionAsync(Order order)
        {
            return await HandleFunctionAsync(async () =>
            {
                //prepare transaction model
                var address = await GetTaxAddressAsync(order);
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var model = await PrepareTransactionModelAsync(address, customer.Id.ToString(), DocumentType.SalesInvoice);
                model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
                model.code = CommonHelper.EnsureMaximumLength(order.CustomOrderNumber, 50);
                model.commit = _avalaraTaxSettings.CommitTransactions;
                model.discount = order.OrderSubTotalDiscountExclTax;

                //set purchased item lines
                var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
                model.lines = await GetItemLinesAsync(order, orderItems);

                //set whole request tax exemption
                await PrepareModelTaxExemptionAsync(model, customer);

                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Void tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task VoidTaxTransactionAsync(Order order)
        {
            await HandleFunctionAsync(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                var model = new VoidTransactionModel { code = VoidReasonCode.DocVoided };
                var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(transaction);
            });
        }

        /// <summary>
        /// Delete tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DeleteTaxTransactionAsync(Order order)
        {
            await HandleFunctionAsync(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                var model = new VoidTransactionModel { code = VoidReasonCode.DocDeleted };
                var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(transaction);
            });
        }

        /// <summary>
        /// Refund tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task RefundTaxTransactionAsync(Order order, decimal amountToRefund)
        {
            await HandleFunctionAsync(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                //first try to get saved tax transaction
                var transaction = ServiceClient.GetTransactionByCodeAndType(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, DocumentType.SalesInvoice, null)
                    ?? throw new NopException("No response from the service");

                //create refund transaction model
                var model = new RefundTransactionModel
                {
                    referenceCode = CommonHelper.EnsureMaximumLength(transaction.code, 50),
                    refundDate = transaction.date ?? DateTime.UtcNow,
                    refundType = RefundType.Full
                };

                //whether it's a partial refund
                var isPartialRefund = amountToRefund < order.OrderTotal;
                if (isPartialRefund)
                {
                    model.refundType = RefundType.Percentage;
                    model.refundPercentage = amountToRefund / (order.OrderTotal - order.OrderTax) * 100;
                }

                transaction = ServiceClient.RefundTransaction(_avalaraTaxSettings.CompanyCode, transaction.code, null, null, null, model)
                    ?? throw new NopException("No response from the service");

                return Task.FromResult(transaction);
            });
        }

        /// <summary>
        /// Download a file listing tax rates by postal code
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task DownloadTaxRatesAsync()
        {
            await HandleFunctionAsync(async () =>
            {
                var file = ServiceClient.DownloadTaxRatesByZipCode(DateTime.UtcNow, null)
                    ?? throw new NopException("No response from the service");

                var filePath = _fileProvider.MapPath(AvalaraTaxDefaults.TaxRatesFilePath);
                await _fileProvider.WriteAllBytesAsync(filePath, file.Data);

                return true;
            });
        }

        #endregion

        #region Certificates

        /// <summary>
        /// Checks whether the company is configured to use exemption certificates
        /// </summary>
        /// <param name="request">Whether to request the certificate setup</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result of check
        /// </returns>
        public async Task<bool?> GetCertificateSetupStatusAsync(bool request = false)
        {
            return await HandleFunctionAsync<bool?>(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                var provisionStatus = request
                    ? await ServiceClient.RequestCertificateSetupAsync(_avalaraTaxSettings.CompanyId.Value)
                    : await ServiceClient.GetCertificateSetupAsync(_avalaraTaxSettings.CompanyId.Value)
                    ?? throw new NopException("Failed to get certificate setup status");

                if (provisionStatus.status == CertCaptureProvisionStatus.NotProvisioned)
                    return default;

                return provisionStatus.status == CertCaptureProvisionStatus.Provisioned;
            });
        }

        /// <summary>
        /// Create a new authorization token to launch the certificates services
        /// </summary>
        /// <param name="customer">Customer for which token is creating</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the generated token
        /// </returns>
        public async Task<string> CreateTokenAsync(Customer customer)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                //no need to log the error if the customer is not created yet
                var customerExists = false;
                try
                {
                    customerExists = await ServiceClient
                        .GetCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null) is not null;
                }
                catch { }
                if (!customerExists)
                    await CreateOrUpdateCustomerAsync(customer, _avalaraTaxSettings.CompanyId.Value, customerExists);

                var model = new CreateECommerceTokenInputModel { customerNumber = customer.Id.ToString() };
                return (await ServiceClient.CreateECommerceTokenAsync(_avalaraTaxSettings.CompanyId.Value, model))?.token
                    ?? throw new NopException("Failed to get token");
            });
        }

        /// <summary>
        /// Get the certificate exposure zones defined by the company
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of exposure zones
        /// </returns>
        public async Task<List<ExposureZoneModel>> GetExposureZonesAsync()
        {
            return await HandleFunctionAsync(async () =>
            {
                var result = await ServiceClient.ListCertificateExposureZonesAsync(null, null, null, null)
                    ?? throw new NopException("Failed to get exposure zones");

                return result.value;
            });
        }

        /// <summary>
        /// Create or update the passed customer for the company
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer details
        /// </returns>
        public async Task<CustomerModel> CreateOrUpdateCustomerAsync(Customer customer)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                //no need to log the error if the customer is not created yet
                var customerExists = false;
                try
                {
                    customerExists = await ServiceClient
                        .GetCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null) is not null;
                }
                catch { }

                return await CreateOrUpdateCustomerAsync(customer, _avalaraTaxSettings.CompanyId.Value, customerExists)
                    ?? throw new NopException("Failed to update customer details");
            });
        }

        /// <summary>
        /// Delete a customer with the passed identifier
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer details
        /// </returns>
        public async Task<CustomerModel> DeleteCustomerAsync(int customerId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                return await ServiceClient.DeleteCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customerId.ToString())
                    ?? throw new NopException("Failed to delete customer");
            });
        }

        /// <summary>
        /// Get valid certificates linked to a customer in a particular country and region
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Current store id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of certificates
        /// </returns>
        public async Task<CertificateModel> GetValidCertificatesAsync(Customer customer, int storeId)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                //create dummy order to get selected address
                var order = new Order { CustomerId = customer.Id };
                await PrepareOrderAddressesAsync(customer, order, storeId);
                var address = await GetTaxAddressAsync(order);
                var shipTo = await MapAddressAsync(address);

                //check exemption status
                var exemptionStatus = await ServiceClient
                    .ListValidCertificatesForCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), shipTo.country, shipTo.region)
                    ?? throw new NopException("Failed to get customer's certificates");

                var exempt = string.Equals(exemptionStatus.status, "Exempt", StringComparison.InvariantCultureIgnoreCase);
                return exempt ? exemptionStatus.certificate : null;
            });
        }

        /// <summary>
        /// Get all certificates linked to a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of certificates
        /// </returns>
        public async Task<List<CertificateModel>> GetCustomerCertificatesAsync(Customer customer)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                var certificates = await ServiceClient
                    .ListCertificatesForCustomerAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), null, null, null, null, null)
                    ?? throw new NopException("Failed to get customer's certificates");

                return certificates.value;
            });
        }

        /// <summary>
        /// Download a PDF file for the certificate
        /// </summary>
        /// <param name="certificateId">The unique ID number of the certificate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file details
        /// </returns>
        public async Task<FileResult> DownloadCertificateAsync(int certificateId)
        {
            return await HandleFunctionAsync(() =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                var file = ServiceClient
                    .DownloadCertificateImage(_avalaraTaxSettings.CompanyId.Value, certificateId, null, CertificatePreviewType.Pdf)
                    ?? throw new NopException("Failed to download certificate");

                return Task.FromResult(file);
            });
        }

        /// <summary>
        /// Get an existing invitation to upload certificates on the external website
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the URL to redirect customer
        /// </returns>
        public async Task<string> GetInvitationAsync(Customer customer)
        {
            return await HandleFunctionAsync(async () =>
            {
                if (_avalaraTaxSettings.CompanyId is null)
                    throw new NopException("Company not selected");

                //create invitation for customer
                var invitationModel = new List<CreateCertExpressInvitationModel>
                {
                    new CreateCertExpressInvitationModel  { deliveryMethod = CertificateRequestDeliveryMethod.Download }
                };
                var invitation = (await ServiceClient
                    .CreateCertExpressInvitationAsync(_avalaraTaxSettings.CompanyId.Value, customer.Id.ToString(), invitationModel))
                    ?.FirstOrDefault()?.invitation
                    ?? throw new NopException("Failed to get invitation");

                return invitation.requestLink;
            });
        }

        #endregion

        /// <summary>
        /// Dispose object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_serviceClient != null)
                    _serviceClient.CallCompleted -= OnCallCompleted;
            }

            _disposed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets client that connects to Avalara services
        /// </summary>
        protected AvaTaxClient ServiceClient
        {
            get
            {
                if (_serviceClient == null)
                {
                    //create a client with credentials
                    _serviceClient = new AvaTaxClient(AvalaraTaxDefaults.ApplicationName,
                            AvalaraTaxDefaults.ApplicationVersion, Environment.MachineName,
                            _avalaraTaxSettings.UseSandbox ? AvaTaxEnvironment.Sandbox : AvaTaxEnvironment.Production)
                        .WithSecurity(_avalaraTaxSettings.AccountId, _avalaraTaxSettings.LicenseKey);

                    //invoke method after each request to services completed
                    if (_avalaraTaxSettings.EnableLogging)
                        _serviceClient.CallCompleted += OnCallCompleted;
                }

                return _serviceClient;
            }
        }

        #endregion
    }
}
