using System;
using System.Collections.Generic;
using System.Linq;
using Avalara.AvaTax.RestClient;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Data;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Tax.Avalara
{
    /// <summary>
    /// Represents Avalara tax provider
    /// </summary>
    public class AvalaraTaxProvider : BasePlugin, ITaxProvider, IWidgetPlugin
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IAddressService _addressService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWebHelper _webHelper;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly TaxTransactionLogObjectContext _objectContext;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public AvalaraTaxProvider(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IAddressService addressService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            IProductService productService,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager,
            IWebHelper webHelper,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            TaxTransactionLogObjectContext objectContext,
            WidgetSettings widgetSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _addressService = addressService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _localizationService = localizationService;
            _productService = productService;
            _settingService = settingService;
            _cacheManager = cacheManager;
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
            _webHelper = webHelper;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _objectContext = objectContext;
            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare request parameters to get an estimated (not finally calculated) tax 
        /// </summary>
        /// <param name="destinationAddress">Destination tax address</param>
        /// <param name="customerCode">Customer code</param>
        /// <returns>Request parameters to create tax transaction</returns>
        private CreateTransactionModel PrepareEstimatedTaxModel(Address destinationAddress, string customerCode)
        {
            //prepare common parameters
            var transactionModel = PrepareTaxModel(destinationAddress, customerCode, false);

            //create a simplified item line
            transactionModel.lines.Add(new LineItemModel
            {
                amount = 100,
                quantity = 1
            });

            return transactionModel;
        }

        /// <summary>
        /// Prepare request parameters to get a tax for the order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="save">Whether to save tax transaction</param>
        /// <returns>Request parameters to create tax transaction</returns>
        private CreateTransactionModel PrepareOrderTaxModel(Order order, bool save)
        {
            //prepare common parameters
            var transactionModel = PrepareTaxModel(GetOrderDestinationAddress(order), order.Customer?.Id.ToString(), save);

            //prepare specific order parameters
            transactionModel.code = CommonHelper.EnsureMaximumLength(save ? order.CustomOrderNumber : Guid.NewGuid().ToString(), 50);
            transactionModel.commit = save && _avalaraTaxSettings.CommitTransactions;
            transactionModel.discount = order.OrderSubTotalDiscountExclTax;
            transactionModel.email = CommonHelper.EnsureMaximumLength(order.BillingAddress?.Email, 50);

            //set purchased item lines
            transactionModel.lines = GetItemLines(order);

            //set whole request tax exemption
            var exemptedCustomerRole = order.Customer?.CustomerRoles.FirstOrDefault(role => role.Active && role.TaxExempt);
            if (order.Customer?.IsTaxExempt ?? false)
                transactionModel.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-customer-#{order.Customer?.Id}", 25);
            else if (!string.IsNullOrEmpty(exemptedCustomerRole?.Name))
                transactionModel.exemptionNo = CommonHelper.EnsureMaximumLength($"Exempt-{exemptedCustomerRole.Name}", 25);

            //whether entity use code is set
            var entityUseCode = order.Customer != null
                ? _genericAttributeService.GetAttribute<string>(order.Customer, AvalaraTaxDefaults.EntityUseCodeAttribute)
                : exemptedCustomerRole != null
                ? _genericAttributeService.GetAttribute<string>(exemptedCustomerRole, AvalaraTaxDefaults.EntityUseCodeAttribute)
                : null;
            if (!string.IsNullOrEmpty(entityUseCode))
                transactionModel.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

            return transactionModel;
        }

        /// <summary>
        /// Prepare common request parameters to get a tax
        /// </summary>
        /// <param name="destinationAddress">Destination tax address</param>
        /// <param name="customerCode">Customer code</param>
        /// <param name="save">Whether to save tax transaction</param>
        /// <returns>Request parameters to create tax transaction</returns>
        private CreateTransactionModel PrepareTaxModel(Address destinationAddress, string customerCode, bool save)
        {
            //prepare request parameters
            var transactionModel = new CreateTransactionModel
            {
                addresses = new AddressesModel(),
                customerCode = CommonHelper.EnsureMaximumLength(customerCode, 50),
                date = DateTime.UtcNow,
                lines = new List<LineItemModel>(),
                type = save ? DocumentType.SalesInvoice : DocumentType.SalesOrder
            };

            //set company code
            var companyCode = !string.IsNullOrEmpty(_avalaraTaxSettings.CompanyCode)
                && !_avalaraTaxSettings.CompanyCode.Equals(Guid.Empty.ToString())
                ? _avalaraTaxSettings.CompanyCode : null;
            transactionModel.companyCode = CommonHelper.EnsureMaximumLength(companyCode, 25);

            //set destination and origin addresses
            transactionModel.addresses = GetModelAddresses(destinationAddress);

            return transactionModel;
        }

        /// <summary>
        /// Get tax origin and tax destination addresses
        /// </summary>
        /// <param name="destinationAddress">Destination address</param>
        /// <returns>Addresses</returns>
        private AddressesModel GetModelAddresses(Address destinationAddress)
        {
            var addresses = new AddressesModel();

            //get tax origin address
            var originAddress = _avalaraTaxSettings.TaxOriginAddressType == TaxOriginAddressType.ShippingOrigin
                ? _addressService.GetAddressById(_shippingSettings.ShippingOriginAddressId)
                : _avalaraTaxSettings.TaxOriginAddressType == TaxOriginAddressType.DefaultTaxAddress
                ? _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId)
                : null;

            //set destination and origin addresses
            var shipFromAddress = MapAddress(originAddress);
            var shipToAddress = MapAddress(destinationAddress);
            if (shipFromAddress != null && shipToAddress != null)
            {
                addresses.shipFrom = shipFromAddress;
                addresses.shipTo = shipToAddress;
            }
            else
                addresses.singleLocation = shipToAddress ?? shipFromAddress;

            return addresses;
        }

        /// <summary>
        /// Map nopCommerce address to Avalara model address info
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Address info</returns>
        private AddressLocationInfo MapAddress(Address address)
        {
            return address == null ? null : new AddressLocationInfo
            {
                city = CommonHelper.EnsureMaximumLength(address.City, 50),
                country = CommonHelper.EnsureMaximumLength(address.Country?.TwoLetterIsoCode, 2),
                line1 = CommonHelper.EnsureMaximumLength(address.Address1, 50),
                line2 = CommonHelper.EnsureMaximumLength(address.Address2, 100),
                postalCode = CommonHelper.EnsureMaximumLength(address.ZipPostalCode, 11),
                region = CommonHelper.EnsureMaximumLength(address.StateProvince?.Abbreviation, 3)
            };
        }

        /// <summary>
        /// Get a tax destination address of the passed order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Address</returns>
        private Address GetOrderDestinationAddress(Order order)
        {
            Address destinationAddress = null;

            //tax is based on billing address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.BillingAddress)
                destinationAddress = order.BillingAddress;

            //tax is based on shipping address
            if (_taxSettings.TaxBasedOn == TaxBasedOn.ShippingAddress)
                destinationAddress = order.ShippingAddress;

            //tax is based on pickup point address
            if (_taxSettings.TaxBasedOnPickupPointAddress && order.PickupAddress != null)
                destinationAddress = order.PickupAddress;

            //or use default address for tax calculation
            if (destinationAddress == null)
                destinationAddress = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);

            return destinationAddress;
        }

        /// <summary>
        /// Get item lines to create tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>List of item lines</returns>
        private List<LineItemModel> GetItemLines(Order order)
        {
            //get purchased products details
            var items = CreateLinesForOrderItems(order).ToList();

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
        /// <returns>Collection of item lines</returns>
        private IEnumerable<LineItemModel> CreateLinesForOrderItems(Order order)
        {
            return order.OrderItems.Select(orderItem =>
            {
                var item = new LineItemModel
                {
                    amount = orderItem.PriceExclTax,

                    //item description
                    description = CommonHelper.EnsureMaximumLength(orderItem.Product?.ShortDescription ?? orderItem.Product?.Name, 2096),

                    //whether the discount to the item was applied
                    discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                    //product exemption
                    exemptionCode = orderItem.Product?.IsTaxExempt ?? false
                        ? CommonHelper.EnsureMaximumLength($"Exempt-product-#{orderItem.Product.Id}", 25) : null,

                    //set SKU as item code
                    itemCode = CommonHelper.EnsureMaximumLength(orderItem.Product != null ?
                        _productService.FormatSku(orderItem.Product, orderItem.AttributesXml) : string.Empty, 50),

                    quantity = orderItem.Quantity
                };

                //force to use billing address as the destination one in the accordance with EU VAT rules (if enabled)
                var useEuVatRules = _taxSettings.EuVatEnabled
                    && (orderItem.Product?.IsTelecommunicationsOrBroadcastingOrElectronicServices ?? false)
                    && ((order.BillingAddress.Country
                        ?? _countryService.GetCountryById(_genericAttributeService.GetAttribute<int>(order.Customer, NopCustomerDefaults.CountryIdAttribute))
                        ?? _countryService.GetCountryByTwoLetterIsoCode(_geoLookupService.LookupCountryIsoCode(order.Customer.LastIpAddress)))
                        ?.SubjectToVat ?? false)
                    && _genericAttributeService.GetAttribute<int>(order.Customer, NopCustomerDefaults.VatNumberStatusIdAttribute) != (int)VatNumberStatus.Valid;
                if (useEuVatRules)
                {
                    var destinationAddress = MapAddress(order.BillingAddress);
                    if (destinationAddress != null)
                        item.addresses = new AddressesModel { shipTo = destinationAddress };
                }

                //set tax code
                var productTaxCategory = _taxCategoryService.GetTaxCategoryById(orderItem.Product?.TaxCategoryId ?? 0);
                item.taxCode = CommonHelper.EnsureMaximumLength(productTaxCategory?.Name, 25);

                //whether entity use code is set
                var entityUseCode = orderItem.Product != null
                    ? _genericAttributeService.GetAttribute<string>(orderItem.Product, AvalaraTaxDefaults.EntityUseCodeAttribute) : null;
                if (!string.IsNullOrEmpty(entityUseCode))
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

            return attributeValues.Where(attributeValue => attributeValue.CheckoutAttribute != null).Select(attributeValue =>
            {
                //create line
                var checkoutAttributeItem = new LineItemModel
                {
                    amount = attributeValue.PriceAdjustment,

                    //item description
                    description = CommonHelper.EnsureMaximumLength($"{attributeValue.CheckoutAttribute.Name} ({attributeValue.Name})", 2096),

                    //whether the discount to the item was applied
                    discounted = order.OrderSubTotalDiscountExclTax > decimal.Zero,

                    //set checkout attribute name and value as item code
                    itemCode = CommonHelper.EnsureMaximumLength($"{attributeValue.CheckoutAttribute.Name}-{attributeValue.Name}", 50),

                    quantity = 1
                };

                //whether checkout attribute is tax exempt
                if (attributeValue.CheckoutAttribute.IsTaxExempt)
                    checkoutAttributeItem.exemptionCode = "Attribute-non-taxable";
                else
                {
                    //or try to get tax code
                    var attributeTaxCategory = _taxCategoryService.GetTaxCategoryById(attributeValue.CheckoutAttribute.TaxCategoryId);
                    checkoutAttributeItem.taxCode = CommonHelper.EnsureMaximumLength(attributeTaxCategory?.Name, 25);
                }

                //whether entity use code is set
                var entityUseCode = _genericAttributeService
                    .GetAttribute<string>(attributeValue.CheckoutAttribute, AvalaraTaxDefaults.EntityUseCodeAttribute);
                if (!string.IsNullOrEmpty(entityUseCode))
                    checkoutAttributeItem.customerUsageType = CommonHelper.EnsureMaximumLength(entityUseCode, 25);

                return checkoutAttributeItem;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            if (calculateTaxRequest.Address == null)
                return new CalculateTaxResult { Errors = new[] { "Address is not set" } };

            //construct a cache key
            var cacheKey = string.Format(AvalaraTaxDefaults.TaxRateCacheKey,
                calculateTaxRequest.Address.Address1,
                calculateTaxRequest.Address.City,
                calculateTaxRequest.Address.StateProvinceId ?? 0,
                calculateTaxRequest.Address.CountryId ?? 0,
                calculateTaxRequest.Address.ZipPostalCode);

            //we don't use standard way _cacheManager.Get() due the need write errors to CalculateTaxResult
            if (_cacheManager.IsSet(cacheKey))
                return new CalculateTaxResult { TaxRate = _cacheManager.Get<decimal>(cacheKey, () => default(decimal)) };

            //get estimated tax
            var address = _addressService.GetAddressById(calculateTaxRequest.Address.AddressId);
            var totalTax = CreateEstimatedTaxTransaction(address, calculateTaxRequest.Customer?.Id.ToString())?.totalTax;
            if (!totalTax.HasValue)
                return new CalculateTaxResult { Errors = new[] { "No response from the service" }.ToList() };

            //tax rate successfully received, so cache it
            _cacheManager.Set(cacheKey, totalTax.Value, 60);

            return new CalculateTaxResult { TaxRate = totalTax.Value };
        }

        /// <summary>
        /// Create tax transaction to get estimated (not finally calculated) tax
        /// </summary>
        /// <param name="destinationAddress">Destination tax address</param>
        /// <param name="customerCode">Customer code</param>
        /// <returns>Transaction</returns>
        public TransactionModel CreateEstimatedTaxTransaction(Address destinationAddress, string customerCode)
        {
            var transactionModel = PrepareEstimatedTaxModel(destinationAddress, customerCode);
            return _avalaraTaxManager.CreateTaxTransaction(transactionModel, false);
        }

        /// <summary>
        /// Create tax transaction to get tax for the order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="save">Whether to save tax transaction</param>
        /// <returns>Transaction</returns>
        public TransactionModel CreateOrderTaxTransaction(Order order, bool save)
        {
            var transactionModel = PrepareOrderTaxModel(order, save);
            return _avalaraTaxManager.CreateTaxTransaction(transactionModel, save);
        }

        /// <summary>
        /// Void tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        public void VoidTaxTransaction(Order order)
        {
            _avalaraTaxManager.VoidTax(new VoidTransactionModel
            {
                code = VoidReasonCode.DocVoided
            }, order.CustomOrderNumber);
        }

        /// <summary>
        /// Delete tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        public void DeleteTaxTransaction(Order order)
        {
            _avalaraTaxManager.VoidTax(new VoidTransactionModel
            {
                code = VoidReasonCode.DocDeleted
            }, order.CustomOrderNumber);
        }

        /// <summary>
        /// Refund tax transaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="amountToRefund">Amount to refund</param>
        public void RefundTaxTransaction(Order order, decimal amountToRefund)
        {
            //first try to get saved tax transaction
            var transaction = _avalaraTaxManager.GetTransaction(order.CustomOrderNumber);

            //create request parameters to refund transaction
            var refundTransaction = new RefundTransactionModel
            {
                referenceCode = CommonHelper.EnsureMaximumLength(transaction.code, 50),
                refundDate = transaction.date ?? DateTime.UtcNow,
                refundType = RefundType.Full
            };

            //whether refund is partial
            var isPartialRefund = amountToRefund < order.OrderTotal;
            if (isPartialRefund)
            {
                refundTransaction.refundType = RefundType.Percentage;
                refundTransaction.refundPercentage = amountToRefund / (order.OrderTotal - order.OrderTax) * 100;
            }

            _avalaraTaxManager.RefundTax(refundTransaction, transaction.code);
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AvalaraTax/Configure";
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                AdminWidgetZones.CustomerDetailsBlock,
                AdminWidgetZones.CustomerRoleDetailsTop,
                AdminWidgetZones.TaxSettingsDetailsBlock,
                AdminWidgetZones.ProductListButtons,
                AdminWidgetZones.TaxCategoryListButtons,
                PublicWidgetZones.CheckoutConfirmTop,
                PublicWidgetZones.OpCheckoutConfirmTop
            };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone.Equals(AdminWidgetZones.CustomerDetailsBlock) ||
                widgetZone.Equals(AdminWidgetZones.CustomerRoleDetailsTop))
            {
                return AvalaraTaxDefaults.ENTITY_USE_CODE_VIEW_COMPONENT_NAME;
            }

            if (widgetZone.Equals(AdminWidgetZones.TaxSettingsDetailsBlock))
                return AvalaraTaxDefaults.TAX_ORIGIN_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(AdminWidgetZones.ProductListButtons))
                return AvalaraTaxDefaults.EXPORT_ITEMS_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(AdminWidgetZones.TaxCategoryListButtons))
                return AvalaraTaxDefaults.TAX_CODES_VIEW_COMPONENT_NAME;

            if (widgetZone.Equals(PublicWidgetZones.CheckoutConfirmTop) ||
                widgetZone.Equals(PublicWidgetZones.OpCheckoutConfirmTop))
            {
                return AvalaraTaxDefaults.ADDRESS_VALIDATION_VIEW_COMPONENT_NAME;
            }

            return null;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //settings
            _settingService.SaveSetting(new AvalaraTaxSettings
            {
                CompanyCode = Guid.Empty.ToString(),
                UseSandbox = true,
                CommitTransactions = true,
                TaxOriginAddressType = TaxOriginAddressType.ShippingOrigin
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AvalaraTaxDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(AvalaraTaxDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Create", "Create request");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.CreateResponse", "Create response");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Error", "Error");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Refund", "Refund request");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.RefundResponse", "Refund response");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Void", "Void request");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.VoidResponse", "Void response");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.DefaultTaxAddress", "Default tax address");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.ShippingOrigin", "Shipping origin address");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.AddressValidation.Confirm", "For the correct tax calculation we need the most accurate address, so we clarified the address you entered ({0}) through the validation system. Do you confirm the use of _localizationService updated address ({1})?");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.AddressValidation.Error", "For the correct tax calculation we need the most accurate address. There are some errors from the validation system: {0}");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Configuration", "Configuration");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.AccountId", "Account ID");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.AccountId.Hint", "Specify Avalara account ID.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.CommitTransactions", "Commit transactions");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.CommitTransactions.Hint", "Determine whether to commit tax transactions right after they are saved.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company", "Company");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.Currency.Warning", "The default currency used by _localizationService company does not match the primary store currency");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.Hint", "Choose a company that was previously added to the Avalara account.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.NotExist", "There are no active companies");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode", "Entity use code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode.Hint", "Choose a code that can be used to designate the reason for a particular sale being exempt. Each entity use code stands for a different exemption reason, the logic of which can be found in Avalara exemption reason documentation.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode.None", "None");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.LicenseKey", "License key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.LicenseKey.Hint", "Specify Avalara account license key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeDescription", "Description");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeType", "Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeType.Hint", "Choose a tax code type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxOriginAddressType", "Tax origin address");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.Hint", "Choose which address will be used as the origin for tax requests to Avalara services.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.UseSandbox", "Use sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.UseSandbox.Hint", "Determine whether to use sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.ValidateAddress", "Validate address");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Fields.ValidateAddress.Hint", "Determine whether to validate entered by customer addresses before the tax calculation.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Items.Export", "Export to Avalara (selected)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.AlreadyExported", "Selected products have already been exported");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.Error", "An error has occurred on export products");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.Success", "Successfully exported {0} products");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log", "Log");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.BackToList", "back to log");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.ClearLog", "Clear log");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.CreatedDate", "Created on");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.CreatedDate.Hint", "Date and time the log entry was created.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Customer", "Customer");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Customer.Hint", "Name of the customer.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Deleted", "The log entry has been deleted successfully.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Hint", "View log entry details");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.RequestMessage", "Request message");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.RequestMessage.Hint", "The details of the request.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.ResponseMessage", "Response message");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.ResponseMessage.Hint", "The details of the response.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.StatusCode", "Status code");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.StatusCode.Hint", "The status code of the response.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Url", "Url");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Url.Hint", "The requested URL.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedFrom", "Created from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedFrom.Hint", "The creation from date for the search.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedTo", "Created to");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedTo.Hint", "The creation to date for the search.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes", "Avalara tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete", "Delete Avalara system tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete.Error", "An error has occurred on delete tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete.Success", "System tax codes successfully deleted");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export", "Export tax codes to Avalara");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.AlreadyExported", "All tax codes have already been exported");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.Error", "An error has occurred on export tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.Success", "Successfully exported {0} tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import", "Import Avalara system tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import.Error", "An error has occurred on import tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import.Success", "Successfully imported {0} tax codes");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TestTax", "Test tax calculation");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TestTax.Error", "An error has occurred on tax request");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.TestTax.Success", "The tax was successfully received");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials", "Test connection");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials.Declined", "Credentials declined");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials.Verified", "Credentials verified");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //generic attributes
            foreach (var taxCategory in _taxCategoryService.GetAllTaxCategories())
            {
                _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, null);
                _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, null);
            }
            foreach (var customer in _customerService.GetAllCustomers())
            {
                _genericAttributeService.SaveAttribute<string>(customer, AvalaraTaxDefaults.EntityUseCodeAttribute, null);
            }
            foreach (var customerRole in _customerService.GetAllCustomerRoles(true))
            {
                _genericAttributeService.SaveAttribute<string>(customerRole, AvalaraTaxDefaults.EntityUseCodeAttribute, null);
            }
            foreach (var product in _productService.SearchProducts(showHidden: true))
            {
                _genericAttributeService.SaveAttribute<string>(product, AvalaraTaxDefaults.EntityUseCodeAttribute, null);
            }
            foreach (var attribute in _checkoutAttributeService.GetAllCheckoutAttributes())
            {
                _genericAttributeService.SaveAttribute<string>(attribute, AvalaraTaxDefaults.EntityUseCodeAttribute, null);
            }

            //settings            
            _taxSettings.ActiveTaxProviderSystemName = _taxPluginManager.LoadAllPlugins()
                .FirstOrDefault(taxProvider => !taxProvider.PluginDescriptor.SystemName.Equals(AvalaraTaxDefaults.SystemName))
                ?.PluginDescriptor.SystemName;
            _settingService.SaveSetting(_taxSettings);
            _widgetSettings.ActiveWidgetSystemNames.Remove(AvalaraTaxDefaults.SystemName);
            _settingService.SaveSetting(_widgetSettings);
            _settingService.DeleteSetting<AvalaraTaxSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Create");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.CreateResponse");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Error");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Refund");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.RefundResponse");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.Void");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.LogType.VoidResponse");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.DefaultTaxAddress");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Tax.Avalara.Domain.TaxOriginAddressType.ShippingOrigin");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.AddressValidation.Confirm");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.AddressValidation.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Configuration");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.AccountId");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.AccountId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.CommitTransactions");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.CommitTransactions.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.Currency.Warning");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.Company.NotExist");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.EntityUseCode.None");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.LicenseKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.LicenseKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeDescription");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeType");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxCodeType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxOriginAddressType");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.TaxOriginAddressType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.ValidateAddress");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Fields.ValidateAddress.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Items.Export");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.AlreadyExported");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Items.Export.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.BackToList");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.ClearLog");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.CreatedDate");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.CreatedDate.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Customer");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Customer.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Deleted");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Message");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Message.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.RequestMessage");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.RequestMessage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.ResponseMessage");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.ResponseMessage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.StatusCode");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.StatusCode.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Url");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Url.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedFrom");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedFrom.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedTo");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.Log.Search.CreatedTo.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Delete.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.AlreadyExported");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Export.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TaxCodes.Import.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TestTax");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TestTax.Error");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.TestTax.Success");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials.Declined");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.Avalara.VerifyCredentials.Verified");

            base.Uninstall();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;
    }
}