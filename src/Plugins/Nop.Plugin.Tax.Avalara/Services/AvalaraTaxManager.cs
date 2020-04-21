using System;
using System.Collections.Generic;
using System.Linq;
using Avalara.AvaTax.RestClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
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

        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addressService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly TaxTransactionLogService _taxTransactionLogService;
        private readonly WidgetSettings _widgetSettings;

        private AvaTaxClient _serviceClient;
        private bool _disposed;

        #endregion

        #region Ctor

        public AvalaraTaxManager(AvalaraTaxSettings avalaraTaxSettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            ICacheKeyService cacheKeyService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            ISettingService settingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager cacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            TaxTransactionLogService taxTransactionLogService,
            WidgetSettings widgetSettings)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _cacheKeyService = cacheKeyService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _localizationService = localizationService;
            _logger = logger;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _genericAttributeRepository = genericAttributeRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _settingService = settingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _cacheManager = cacheManager;
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
            _workContext = workContext;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _taxTransactionLogService = taxTransactionLogService;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets client that connects to Avalara services
        /// </summary>
        private AvaTaxClient ServiceClient
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

        #region Utilities

        #region Common

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event args</param>
        private void OnCallCompleted(object sender, EventArgs args)
        {
            if (!(args is AvaTaxCallEventArgs avaTaxCallEventArgs))
                return;

            //log request results
            _taxTransactionLogService.InsertTaxTransactionLog(new TaxTransactionLog
            {
                StatusCode = (int)avaTaxCallEventArgs.Code,
                Url = avaTaxCallEventArgs.RequestUri.ToString(),
                RequestMessage = avaTaxCallEventArgs.RequestBody,
                ResponseMessage = avaTaxCallEventArgs.ResponseString,
                CustomerId = _workContext.CurrentCustomer.Id,
                CreatedDateUtc = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Check that tax provider is configured
        /// </summary>
        /// <returns>True if it's configured; otherwise false</returns>
        private bool IsConfigured()
        {
            return !string.IsNullOrEmpty(_avalaraTaxSettings.AccountId)
                && !string.IsNullOrEmpty(_avalaraTaxSettings.LicenseKey);
        }

        /// <summary>
        /// Handle function and get result
        /// </summary>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="function">Function</param>
        /// <returns>Result</returns>
        private TResult HandleFunction<TResult>(Func<TResult> function)
        {
            try
            {
                //ensure that Avalara tax provider is configured
                if (!IsConfigured())
                    throw new NopException("Tax provider is not configured");

                return function();
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
                _logger.Error($"{AvalaraTaxDefaults.SystemName} error. {errorMessage}", exception, _workContext.CurrentCustomer);

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
        private TransactionModel CreateTransaction(CreateTransactionModel model)
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
        /// <returns>Model</returns>
        private CreateTransactionModel PrepareTransactionModel(Address address, string customerCode, DocumentType documentType)
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
                TaxOriginAddressType.ShippingOrigin => _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId),
                TaxOriginAddressType.DefaultTaxAddress => _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId),
                _ => null
            };
            var shipFromAddress = MapAddress(originAddress);
            var shipToAddress = MapAddress(address);
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
        /// Get a tax address of the passed order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Address</returns>
        private Address GetTaxAddress(Order order)
        {
            Address address = null;

            //tax is based on billing address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress &&
                _addressService.GetAddressById(order.BillingAddressId) is Address billingAddress)
            {
                address = billingAddress;
            }

            //tax is based on shipping address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress &&
                order.ShippingAddressId.HasValue &&
                _addressService.GetAddressById(order.ShippingAddressId.Value) is Address shippingAddress)
            {
                address = shippingAddress;
            }

            //tax is based on pickup point address
            if (_taxSettings.TaxBasedOnPickupPointAddress &&
                order.PickupAddressId.HasValue &&
                _addressService.GetAddressById(order.PickupAddressId.Value) is Address pickupAddress)
            {
                address = pickupAddress;
            }

            //or use default address for tax calculation
            if (address == null)
                address = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);

            return address;
        }

        /// <summary>
        /// Map address model
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Address model</returns>
        private AddressLocationInfo MapAddress(Address address)
        {
            return address == null ? null : new AddressLocationInfo
            {
                city = CommonHelper.EnsureMaximumLength(address.City, 50),
                country = CommonHelper.EnsureMaximumLength(_countryService.GetCountryByAddress(address)?.TwoLetterIsoCode, 2),
                line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                region = CommonHelper.EnsureMaximumLength(_stateProvinceService.GetStateProvinceByAddress(address)?.Abbreviation, 3)
            };
        }

        /// <summary>
        /// Get item lines to create tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Order items</param>
        /// <returns>List of item lines</returns>
        private List<LineItemModel> GetItemLines(Order order, IList<OrderItem> orderItems)
        {
            //get purchased products details
            var items = CreateLinesForOrderItems(order, orderItems).ToList();

            //set payment method additional fee as the separate item line
            if (order.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                items.Add(CreateLineForPaymentMethod(order));

            //set shipping rate as the separate item line
            if (order.OrderShippingExclTax > decimal.Zero)
                items.Add(CreateLineForShipping(order));

            //set checkout attributes as the separate item lines
            if (!string.IsNullOrEmpty(order.CheckoutAttributesXml))
                items.AddRange(CreateLinesForCheckoutAttributes(order));

            return items;
        }

        /// <summary>
        /// Create item lines for purchased order items
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="orderItems">Order items</param>
        /// <returns>Collection of item lines</returns>
        private IEnumerable<LineItemModel> CreateLinesForOrderItems(Order order, IList<OrderItem> orderItems)
        {
            return orderItems.Select(orderItem =>
            {
                var product = _productService.GetProductById(orderItem.ProductId);

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
                        ? CommonHelper.EnsureMaximumLength(_productService.FormatSku(product, orderItem.AttributesXml), 50)
                        : string.Empty,

                    quantity = orderItem.Quantity
                };

                //force to use billing address as the tax address one in the accordance with EU VAT rules (if enabled)
                if (_taxSettings.EuVatEnabled)
                {
                    var customer = _customerService.GetCustomerById(order.CustomerId);
                    var billingAddress = _addressService.GetAddressById(order.BillingAddressId);
                    var useEuVatRules = (product?.IsTelecommunicationsOrBroadcastingOrElectronicServices ?? false)
                        && ((_countryService.GetCountryByAddress(billingAddress)
                            ?? _countryService.GetCountryById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute))
                            ?? _countryService.GetCountryByTwoLetterIsoCode(_geoLookupService.LookupCountryIsoCode(customer.LastIpAddress)))
                            ?.SubjectToVat ?? false)
                        && _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute) != (int)VatNumberStatus.Valid;

                    if (useEuVatRules)
                    {
                        var address = MapAddress(billingAddress);
                        if (address != null)
                            item.addresses = new AddressesModel { singleLocation = address };
                    }
                }

                //set tax code
                var productTaxCategory = _taxCategoryService.GetTaxCategoryById(product?.TaxCategoryId ?? 0);
                item.taxCode = CommonHelper.EnsureMaximumLength(productTaxCategory?.Name, 25);

                //whether entity use code is set
                var entityUseCode = product != null
                    ? _genericAttributeService.GetAttribute<string>(product, AvalaraTaxDefaults.EntityUseCodeAttribute)
                    : string.Empty;
                item.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                return item;
            });
        }

        /// <summary>
        /// Create a separate item line for the order payment method additional fee
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Item line</returns>
        private LineItemModel CreateLineForPaymentMethod(Order order)
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
                var paymentTaxCategory = _taxCategoryService.GetTaxCategoryById(_taxSettings.PaymentMethodAdditionalFeeTaxClassId);
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
        /// <returns>Item line</returns>
        private LineItemModel CreateLineForShipping(Order order)
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
                var shippingTaxCategory = _taxCategoryService.GetTaxCategoryById(_taxSettings.ShippingTaxClassId);
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
        /// <returns>Collection of item lines</returns>
        private IEnumerable<LineItemModel> CreateLinesForCheckoutAttributes(Order order)
        {
            //get checkout attributes values
            var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(order.CheckoutAttributesXml);
            return attributeValues.SelectMany(attributeWithValues =>
            {
                var attribute = attributeWithValues.attribute;
                return attributeWithValues.values.Select(value =>
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
                        var attributeTaxCategory = _taxCategoryService.GetTaxCategoryById(attribute.TaxCategoryId);
                        checkoutAttributeItem.taxCode = CommonHelper.EnsureMaximumLength(attributeTaxCategory?.Name, 25);
                    }

                    //whether entity use code is set
                    var entityUseCode = _genericAttributeService.GetAttribute<string>(attribute, AvalaraTaxDefaults.EntityUseCodeAttribute);
                    checkoutAttributeItem.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                    return checkoutAttributeItem;
                });
            });
        }

        /// <summary>
        /// Prepare model tax exemption details
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Model</returns>
        private CreateTransactionModel PrepareModelTaxExemption(CreateTransactionModel model, Customer customer)
        {
            if (customer.IsTaxExempt)
                model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-customer-#{customer.Id}", 25);
            else
            {
                var customerRole = _customerService.GetCustomerRoles(customer).FirstOrDefault(role => role.TaxExempt);
                if (customerRole != null)
                    model.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-{customerRole.Name}", 25);
            }

            var entityUseCode = _genericAttributeService.GetAttribute<string>(customer, AvalaraTaxDefaults.EntityUseCodeAttribute);
            if (!string.IsNullOrEmpty(entityUseCode))
                model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
            else
            {
                entityUseCode = _customerService.GetCustomerRoles(customer)
                    .Select(customerRole => _genericAttributeService.GetAttribute<string>(customerRole, AvalaraTaxDefaults.EntityUseCodeAttribute))
                    .FirstOrDefault(code => !string.IsNullOrEmpty(code));
                model.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);
            }

            return model;
        }

        #endregion

        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// Ping service (test conection)
        /// </summary>
        /// <returns>Ping result</returns>
        public PingResultModel Ping()
        {
            return HandleFunction(() => ServiceClient.Ping() ?? throw new NopException("No response from the service"));
        }

        /// <summary>
        /// Get account companies
        /// </summary>
        /// <returns>List of companies</returns>
        public List<CompanyModel> GetAccountCompanies()
        {
            return HandleFunction(() =>
            {
                var result = ServiceClient.QueryCompanies(null, null, null, null, null)
                    ?? throw new NopException("No response from the service");

                return result.value;
            });
        }

        /// <summary>
        /// Get pre-defined entity use codes
        /// </summary>
        /// <returns>List of entity use codes</returns>
        public List<EntityUseCodeModel> GetEntityUseCodes()
        {
            return HandleFunction(() =>
            {
                var result = ServiceClient.ListEntityUseCodes(null, null, null, null)
                    ?? throw new NopException("No response from the service");

                return result.value;
            });
        }

        /// <summary>
        /// Get pre-defined tax code types
        /// </summary>
        /// <returns>Key-value pairs of tax code types</returns>
        public Dictionary<string, string> GetTaxCodeTypes()
        {
            return HandleFunction(() =>
            {
                var result = ServiceClient.ListTaxCodeTypes(null, null)
                    ?? throw new NopException("No response from the service");

                return result.types;
            });
        }

        /// <summary>
        /// Import tax codes from Avalara services
        /// </summary>
        /// <returns>Number of imported tax codes; null in case of error</returns>
        public int? ImportTaxCodes()
        {
            return HandleFunction<int?>(() =>
            {
                //get Avalara pre-defined system tax codes (only active)
                var systemTaxCodes = ServiceClient.ListTaxCodes("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                if (!systemTaxCodes.value?.Any() ?? true)
                    return null;

                //get existing tax categories
                var existingTaxCategories = _taxCategoryService.GetAllTaxCategories()
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
                    _taxCategoryService.InsertTaxCategory(taxCategory);

                    //save description and type
                    if (!string.IsNullOrEmpty(taxCode.description))
                        _genericAttributeService.SaveAttribute(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, taxCode.description);
                    if (!string.IsNullOrEmpty(taxCode.taxCodeTypeId))
                        _genericAttributeService.SaveAttribute(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, taxCode.taxCodeTypeId);

                    importedTaxCodesNumber++;
                }

                return importedTaxCodesNumber;
            });
        }

        /// <summary>
        /// Export current tax codes to Avalara services
        /// </summary>
        /// <returns>Number of exported tax codes; null in case of error</returns>
        public int? ExportTaxCodes()
        {
            return HandleFunction<int?>(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                //get selected company
                var selectedCompany = GetAccountCompanies()
                    ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                    ?? throw new NopException("Failed to retrieve company");

                //get existing tax codes (only active)
                var taxCodes = ServiceClient.ListTaxCodesByCompany(selectedCompany.id, "isActive eq true", null, null, null, null)
                    ?? throw new NopException("No response from the service");

                var existingTaxCodes = taxCodes.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();

                //prepare tax codes to export
                var taxCodesToExport = _taxCategoryService.GetAllTaxCategories().Select(taxCategory => new TaxCodeModel
                {
                    createdDate = DateTime.UtcNow,
                    description = CommonHelper.EnsureMaximumLength(taxCategory.Name, 255),
                    isActive = true,
                    taxCode = CommonHelper.EnsureMaximumLength(taxCategory.Name, 25),
                    taxCodeTypeId = CommonHelper.EnsureMaximumLength(_genericAttributeService
                        .GetAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute) ?? "P", 2)
                }).Where(taxCode => !string.IsNullOrEmpty(taxCode.taxCode)).ToList();

                //add Avalara pre-defined system tax codes
                var systemTaxCodesResult = ServiceClient.ListTaxCodes("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();
                existingTaxCodes.AddRange(systemTaxCodes);

                //remove duplicates
                taxCodesToExport = taxCodesToExport.Where(taxCode => !existingTaxCodes.Contains(taxCode.taxCode)).Distinct().ToList();

                //export tax codes
                if (!taxCodesToExport.Any())
                    return 0;

                //create items and get the result
                var createdTaxCodes = ServiceClient.CreateTaxCodes(selectedCompany.id, taxCodesToExport)
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
        /// <returns>Result</returns>
        public bool DeleteSystemTaxCodes()
        {
            return HandleFunction(() =>
            {
                //get Avalara pre-defined system tax codes (only active)
                var systemTaxCodesResult = ServiceClient.ListTaxCodes("isActive eq true", null, null, null)
                    ?? throw new NopException("No response from the service");

                var systemTaxCodes = systemTaxCodesResult.value?.Select(taxCode => taxCode.taxCode).ToList();
                if (!systemTaxCodes?.Any() ?? true)
                    return false;

                //prepare tax categories to delete
                var categoriesIds = _taxCategoryRepository.Table
                    .Where(taxCategory => systemTaxCodes.Contains(taxCategory.Name))
                    .Select(taxCategory => taxCategory.Id)
                    .ToList();

                //delete tax categories
                _taxCategoryRepository.Delete(taxCategory => categoriesIds.Contains(taxCategory.Id));
                _cacheManager.Remove(NopTaxDefaults.TaxCategoriesAllCacheKey);

                //delete generic attributes
                _genericAttributeRepository
                    .Delete(attribute => attribute.KeyGroup == nameof(TaxCategory) && categoriesIds.Contains(attribute.EntityId));

                return true;
            });
        }

        /// <summary>
        /// Delete generic attributes used in the plugin
        /// </summary>
        public void DeleteAttributes()
        {
            DeleteSystemTaxCodes();
            _genericAttributeRepository.Delete(attribute => attribute.Key == AvalaraTaxDefaults.EntityUseCodeAttribute ||
                attribute.Key == AvalaraTaxDefaults.TaxCodeTypeAttribute || attribute.Key == AvalaraTaxDefaults.TaxCodeDescriptionAttribute);
        }

        /// <summary>
        /// Export items (products) with the passed ids to Avalara services
        /// </summary>
        /// <returns>Number of exported items; null in case of error</returns>
        public int? ExportProducts(string selectedIds)
        {
            return HandleFunction<int?>(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                //get selected company
                var selectedCompany = GetAccountCompanies()
                    ?.FirstOrDefault(company => _avalaraTaxSettings.CompanyCode.Equals(company?.companyCode))
                    ?? throw new NopException("Failed to retrieve company");

                //get existing items
                var items = ServiceClient.ListItemsByCompany(selectedCompany.id, null, null, null, null, null)
                    ?? throw new NopException("No response from the service");

                //return the paginated and filtered list
                var existingItemCodes = items.value?.Select(item => item.itemCode).ToList() ?? new List<string>();

                //prepare exported items
                var productIds = selectedIds?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToArray();
                var exportedItems = new List<ItemModel>();
                foreach (var product in _productService.GetProductsByIds(productIds))
                {
                    //find product combinations
                    var combinations = _productAttributeService.GetAllProductAttributeCombinations(product.Id)
                        .Where(combination => !string.IsNullOrEmpty(combination.Sku));

                    //export items with specified SKU only
                    if (string.IsNullOrEmpty(product.Sku) && !combinations.Any())
                        continue;

                    //prepare common properties
                    var taxCategory = _taxCategoryService.GetTaxCategoryById(product.TaxCategoryId);
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
                var createdItems = ServiceClient.CreateItems(selectedCompany.id, exportedItems)
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
        /// <returns>Validated address</returns>
        public AddressResolutionModel ValidateAddress(Address address)
        {
            return HandleFunction(() =>
            {
                //return result
                return ServiceClient.ResolveAddressPost(new AddressValidationInfo
                {
                    city = CommonHelper.EnsureMaximumLength(address.City, 50),
                    country = CommonHelper.EnsureMaximumLength(_countryService.GetCountryByAddress(address)?.TwoLetterIsoCode, 2),
                    line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                    line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                    postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                    region = CommonHelper.EnsureMaximumLength(_stateProvinceService.GetStateProvinceByAddress(address)?.Abbreviation, 3),
                    textCase = TextCase.Mixed
                }) ?? throw new NopException("No response from the service");
            });
        }

        #endregion

        #region Tax calculation

        /// <summary>
        /// Create test tax transaction
        /// </summary>
        /// <param name="address">Tax address</param>
        /// <returns>Transaction</returns>
        public TransactionModel CreateTestTaxTransaction(Address address)
        {
            return HandleFunction(() =>
            {
                //create tax transaction for a simplified item and without saving 
                var model = PrepareTransactionModel(address, _workContext.CurrentCustomer.Id.ToString(), DocumentType.SalesOrder);
                model.lines = new List<LineItemModel> { new LineItemModel { amount = 100, quantity = 1 } };
                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Create transaction to get tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>Transaction</returns>
        public decimal? GetTaxRate(TaxRateRequest taxRateRequest)
        {
            //prepare cache key
            var address = _addressService.GetAddressById(taxRateRequest.Address.Id);
            var customer = taxRateRequest.Customer ?? _workContext.CurrentCustomer;
            var taxCode = _taxCategoryService.GetTaxCategoryById(taxRateRequest.TaxCategoryId > 0
                ? taxRateRequest.TaxCategoryId
                : taxRateRequest.Product?.TaxCategoryId
                ?? 0)?.Name;
            var itemCode = taxRateRequest.Product?.Sku;
            var cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(AvalaraTaxDefaults.TaxRateCacheKey,
                customer,
                taxCode,
                itemCode,
                taxRateRequest.Address.Address1,
                taxRateRequest.Address.City,
                taxRateRequest.Address.StateProvinceId ?? 0,
                taxRateRequest.Address.CountryId ?? 0,
                taxRateRequest.Address.ZipPostalCode);

            //get tax rate
            return _cacheManager.Get(cacheKey, () =>
            {
                return HandleFunction(() =>
                {
                    //create tax transaction for a single item and without saving
                    var model = PrepareTransactionModel(address, customer.Id.ToString(), DocumentType.SalesOrder);
                    model.lines = new List<LineItemModel>
                    {
                        new LineItemModel
                        {
                            amount = 100,
                            quantity = 1,
                            itemCode = CommonHelper.EnsureMaximumLength(itemCode, 50),
                            taxCode = CommonHelper.EnsureMaximumLength(taxCode, 25),
                            exemptionCode = (taxRateRequest.Product?.IsTaxExempt ?? false)
                                ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{taxRateRequest.Product.Id}", 25)
                                : string.Empty,
                        }
                    };
                    PrepareModelTaxExemption(model, customer);
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
        /// <returns>Transaction</returns>
        public TransactionModel CreateTaxTotalTransaction(TaxTotalRequest taxTotalRequest)
        {
            return HandleFunction(() =>
            {
                //create dummy order to create tax transaction
                var customer = taxTotalRequest.Customer;
                var order = new Order { CustomerId = customer.Id };

                //addresses
                order.BillingAddressId = customer.BillingAddressId ?? 0;
                order.ShippingAddressId = customer.ShippingAddressId;
                if (_shippingSettings.AllowPickupInStore)
                {
                    var pickupPoint = _genericAttributeService
                        .GetAttribute<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, taxTotalRequest.StoreId);
                    if (pickupPoint != null)
                    {
                        var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                        var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);
                        var pickupAddress = new Address
                        {
                            Address1 = pickupPoint.Address,
                            City = pickupPoint.City,
                            CountryId = country?.Id,
                            StateProvinceId = state?.Id,
                            ZipPostalCode = pickupPoint.ZipPostalCode,
                            CreatedOnUtc = DateTime.UtcNow,
                        };
                        _addressService.InsertAddress(pickupAddress);
                        order.PickupAddressId = pickupAddress.Id;
                    }
                }

                //checkout attributes
                order.CheckoutAttributesXml = _genericAttributeService
                    .GetAttribute<string>(customer, NopCustomerDefaults.CheckoutAttributes, taxTotalRequest.StoreId);

                //shipping method
                order.ShippingMethod = _genericAttributeService
                    .GetAttribute<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, taxTotalRequest.StoreId)?.Name;
                order.OrderShippingExclTax = _orderTotalCalculationService.GetShoppingCartShippingTotal(taxTotalRequest.ShoppingCart, false) ?? 0;

                //payment method
                if (taxTotalRequest.UsePaymentMethodAdditionalFee)
                {
                    order.PaymentMethodSystemName = _genericAttributeService
                        .GetAttribute<string>(customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId);
                    if (!string.IsNullOrEmpty(order.PaymentMethodSystemName))
                        order.PaymentMethodAdditionalFeeExclTax = _paymentService.GetAdditionalHandlingFee(taxTotalRequest.ShoppingCart, order.PaymentMethodSystemName);
                }

                //discount amount
                _orderTotalCalculationService
                    .GetShoppingCartSubTotal(taxTotalRequest.ShoppingCart, false, out var orderSubTotalDiscountExclTax, out _, out _, out _);
                order.OrderSubTotalDiscountExclTax = orderSubTotalDiscountExclTax;

                //create dummy order items
                var orderItems = taxTotalRequest.ShoppingCart.Select(cartItem => new OrderItem
                {
                    AttributesXml = cartItem.AttributesXml,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceExclTax = _shoppingCartService.GetSubTotal(cartItem)
                }).ToList();

                //prepare transaction model
                var address = GetTaxAddress(order);
                var model = PrepareTransactionModel(address, customer.Id.ToString(), DocumentType.SalesOrder);
                model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
                model.discount = order.OrderSubTotalDiscountExclTax;

                //set purchased item lines
                model.lines = GetItemLines(order, orderItems);

                //set whole request tax exemption
                PrepareModelTaxExemption(model, customer);

                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Create tax transaction for the placed order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Transaction</returns>
        public TransactionModel CreateOrderTaxTransaction(Order order)
        {
            return HandleFunction(() =>
            {
                //prepare transaction model
                var address = GetTaxAddress(order);
                var customer = _customerService.GetCustomerById(order.CustomerId);
                var model = PrepareTransactionModel(address, customer.Id.ToString(), DocumentType.SalesInvoice);
                model.email = CommonHelper.EnsureMaximumLength(customer.Email, 50);
                model.code = CommonHelper.EnsureMaximumLength(order.CustomOrderNumber, 50);
                model.commit = _avalaraTaxSettings.CommitTransactions;
                model.discount = order.OrderSubTotalDiscountExclTax;

                //set purchased item lines
                var orderItems = _orderService.GetOrderItems(order.Id);
                model.lines = GetItemLines(order, orderItems);

                //set whole request tax exemption
                PrepareModelTaxExemption(model, customer);

                return CreateTransaction(model);
            });
        }

        /// <summary>
        /// Void tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        public void VoidTaxTransaction(Order order)
        {
            HandleFunction(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                var model = new VoidTransactionModel { code = VoidReasonCode.DocVoided };
                var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                    ?? throw new NopException("No response from the service");

                return transaction;
            });
        }

        /// <summary>
        /// Delete tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        public void DeleteTaxTransaction(Order order)
        {
            HandleFunction(() =>
            {
                if (string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode) || _avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString()))
                    throw new NopException("Company not selected");

                var model = new VoidTransactionModel { code = VoidReasonCode.DocDeleted };
                var transaction = ServiceClient.VoidTransaction(_avalaraTaxSettings.CompanyCode, order.CustomOrderNumber, null, null, model)
                    ?? throw new NopException("No response from the service");

                return transaction;
            });
        }

        /// <summary>
        /// Refund tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public void RefundTaxTransaction(Order order, decimal amountToRefund)
        {
            HandleFunction(() =>
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

                return transaction;
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
        protected virtual void Dispose(bool disposing)
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
    }
}