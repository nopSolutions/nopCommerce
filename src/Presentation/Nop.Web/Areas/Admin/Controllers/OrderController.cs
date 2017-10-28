using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class OrderController : BaseAdminController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IReturnRequestService _returnRequestService;
	    private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IEncryptionService _encryptionService;
        private readonly IPaymentService _paymentService;
        private readonly IMeasureService _measureService;
        private readonly IPdfService _pdfService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IProductService _productService;
        private readonly IExportManager _exportManager;
        private readonly IPermissionService _permissionService;
	    private readonly IWorkflowMessageService _workflowMessageService;
	    private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
	    private readonly IProductAttributeService _productAttributeService;
	    private readonly IProductAttributeParser _productAttributeParser;
	    private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGiftCardService _giftCardService;
        private readonly IDownloadService _downloadService;
        private readonly IShipmentService _shipmentService;
	    private readonly IShippingService _shippingService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressAttributeService _addressAttributeService;
	    private readonly IAddressAttributeFormatter _addressAttributeFormatter;
	    private readonly IAffiliateService _affiliateService;
	    private readonly IPictureService _pictureService;
        private readonly ICustomerActivityService _customerActivityService;
	    private readonly IStaticCacheManager _cacheManager;

        private readonly OrderSettings _orderSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly AddressSettings _addressSettings;
	    private readonly ShippingSettings _shippingSettings;
        
        #endregion
        
        #region Ctor

        public OrderController(IOrderService orderService, 
            IOrderReportService orderReportService, 
            IOrderProcessingService orderProcessingService,
            IReturnRequestService returnRequestService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IEncryptionService encryptionService,
            IPaymentService paymentService,
            IMeasureService measureService,
            IPdfService pdfService,
            IAddressService addressService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IProductService productService,
            IExportManager exportManager,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            IProductAttributeService productAttributeService, 
            IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter, 
            IShoppingCartService shoppingCartService,
            IGiftCardService giftCardService, 
            IDownloadService downloadService,
            IShipmentService shipmentService, 
            IShippingService shippingService,
            IStoreService storeService,
            IVendorService vendorService,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAffiliateService affiliateService,
            IPictureService pictureService,
            ICustomerActivityService customerActivityService,
            IStaticCacheManager cacheManager,
            OrderSettings orderSettings,
            CurrencySettings currencySettings, 
            TaxSettings taxSettings,
            MeasureSettings measureSettings,
            AddressSettings addressSettings,
            ShippingSettings shippingSettings)
		{
            this._orderService = orderService;
            this._orderReportService = orderReportService;
            this._orderProcessingService = orderProcessingService;
            this._returnRequestService = returnRequestService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._encryptionService = encryptionService;
            this._paymentService = paymentService;
            this._measureService = measureService;
            this._pdfService = pdfService;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._productService = productService;
            this._exportManager = exportManager;
            this._permissionService = permissionService;
            this._workflowMessageService = workflowMessageService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeFormatter = productAttributeFormatter;
            this._shoppingCartService = shoppingCartService;
            this._giftCardService = giftCardService;
            this._downloadService = downloadService;
            this._shipmentService = shipmentService;
            this._shippingService = shippingService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._addressAttributeParser = addressAttributeParser;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._affiliateService = affiliateService;
            this._pictureService = pictureService;
            this._customerActivityService = customerActivityService;
            this._cacheManager = cacheManager;
            this._orderSettings = orderSettings;
            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._addressSettings = addressSettings;
            this._shippingSettings = shippingSettings;
		}
        
        #endregion
        
        #region Utilities
        
        protected virtual bool HasAccessToOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = order.OrderItems.Any(orderItem => orderItem.Product.VendorId == vendorId);
            return hasVendorProducts;
        }
        
        protected virtual bool HasAccessToOrderItem(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return orderItem.Product.VendorId == vendorId;
        }
        
        protected virtual bool HasAccessToProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return product.VendorId == vendorId;
        }
        
        protected virtual bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var hasVendorProducts = false;
            var vendorId = _workContext.CurrentVendor.Id;
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem != null)
                {
                    if (orderItem.Product.VendorId == vendorId)
                    {
                        hasVendorProducts = true;
                        break;
                    }
                }
            }
            return hasVendorProducts;
        }

	    /// <summary>
	    /// Parse product attributes on the add product to order details page
	    /// </summary>
	    /// <param name="product">Product</param>
	    /// <param name="form">Form</param>
        /// <param name="errors">Errors</param>
        /// <returns>Parsed attributes</returns>
        protected virtual string ParseProductAttributes(Product product, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"product_attribute_{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) && (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            return attributesXml;
        }

        /// <summary>
        /// Parse rental dates on the add product to order details page
        /// </summary>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        protected virtual void ParseRentalDates(IFormCollection form, out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var ctrlStartDate = form["rental_start_date"];
            var ctrlEndDate = form["rental_end_date"];
            try
            {
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        protected virtual void PrepareOrderDetailsModel(OrderModel model, Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = order.Id;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.OrderStatusId = order.OrderStatusId;
            model.OrderGuid = order.OrderGuid;
            model.CustomOrderNumber = order.CustomOrderNumber;
            var store = _storeService.GetStoreById(order.StoreId);
            model.StoreName = store != null ? store.Name : "Unknown";
            model.CustomerId = order.CustomerId;
            var customer = order.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CustomerIp = order.CustomerIp;
            model.VatNumber = order.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;

            var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
                model.AffiliateName = affiliate.GetFullName();
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            //custom values
            model.CustomValues = order.DeserializeCustomValues();

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            //order totals
            PrepareOrderTotals(model, order, primaryStoreCurrency);

            //payment info
            PreparePaymentInfo(model, order);
            
            //billing info
            PrepareBillingInfo(model, order);

            //shipping info
            PrepareShippingInfo(model, order);
            
            //products
            PrepareProducts(model, order, primaryStoreCurrency);
        }

        protected virtual void PrepareProducts(OrderModel model, Order order, Currency primaryStoreCurrency)
        {
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            var hasDownloadableItems = false;
            var products = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products
                    .Where(orderItem => orderItem.Product.VendorId == _workContext.CurrentVendor.Id)
                    .ToList();
            }

            foreach (var orderItem in products)
            {
                if (orderItem.Product.IsDownload)
                    hasDownloadableItems = true;

                var orderItemModel = new OrderModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                    Quantity = orderItem.Quantity,
                    IsDownload = orderItem.Product.IsDownload,
                    DownloadCount = orderItem.DownloadCount,
                    DownloadActivationType = orderItem.Product.DownloadActivationType,
                    IsDownloadActivated = orderItem.IsDownloadActivated
                };
                //picture
                var orderItemPicture =
                    orderItem.Product.GetProductPicture(orderItem.AttributesXml, _pictureService, _productAttributeParser);
                orderItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 75, true);

                //license file
                if (orderItem.LicenseDownloadId.HasValue)
                {
                    var licenseDownload = _downloadService.GetDownloadById(orderItem.LicenseDownloadId.Value);
                    if (licenseDownload != null)
                    {
                        orderItemModel.LicenseDownloadGuid = licenseDownload.DownloadGuid;
                    }
                }

                //vendor
                var vendor = _vendorService.GetVendorById(orderItem.Product.VendorId);
                orderItemModel.VendorName = vendor != null ? vendor.Name : "";

                //unit price
                orderItemModel.UnitPriceInclTaxValue = orderItem.UnitPriceInclTax;
                orderItemModel.UnitPriceExclTaxValue = orderItem.UnitPriceExclTax;
                orderItemModel.UnitPriceInclTax = _priceFormatter.FormatPrice(orderItem.UnitPriceInclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.UnitPriceExclTax = _priceFormatter.FormatPrice(orderItem.UnitPriceExclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //discounts
                orderItemModel.DiscountInclTaxValue = orderItem.DiscountAmountInclTax;
                orderItemModel.DiscountExclTaxValue = orderItem.DiscountAmountExclTax;
                orderItemModel.DiscountInclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountInclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.DiscountExclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountExclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //subtotal
                orderItemModel.SubTotalInclTaxValue = orderItem.PriceInclTax;
                orderItemModel.SubTotalExclTaxValue = orderItem.PriceExclTax;
                orderItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(orderItem.PriceInclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, true, true);
                orderItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(orderItem.PriceExclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, false, true);

                orderItemModel.AttributeInfo = orderItem.AttributeDescription;
                if (orderItem.Product.IsRecurring)
                    orderItemModel.RecurringInfo = string.Format(
                        _localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"),
                        orderItem.Product.RecurringCycleLength,
                        orderItem.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value)
                        : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value)
                        : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //return requests
                orderItemModel.ReturnRequests = _returnRequestService
                    .SearchReturnRequests(orderItemId: orderItem.Id)
                    .Select(item => new OrderModel.OrderItemModel.ReturnRequestBriefModel
                    {
                        CustomNumber = item.CustomNumber,
                        Id = item.Id
                    }).ToList();

                //gift cards
                orderItemModel.PurchasedGiftCardIds = _giftCardService.GetGiftCardsByPurchasedWithOrderItemId(orderItem.Id)
                    .Select(gc => gc.Id).ToList();

                model.Items.Add(orderItemModel);
            }
            model.HasDownloadableProducts = hasDownloadableItems;
        }

        protected virtual void PrepareShippingInfo(OrderModel model, Order order)
        {
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);

            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
                return;

            model.IsShippable = true;
            model.PickUpInStore = order.PickUpInStore;

            if (!order.PickUpInStore)
            {
                model.ShippingAddress = order.ShippingAddress.ToModel();
                model.ShippingAddress.FormattedCustomAddressAttributes =
                    _addressAttributeFormatter.FormatAttributes(order.ShippingAddress.CustomAttributes);
                model.ShippingAddress.FirstNameEnabled = true;
                model.ShippingAddress.FirstNameRequired = true;
                model.ShippingAddress.LastNameEnabled = true;
                model.ShippingAddress.LastNameRequired = true;
                model.ShippingAddress.EmailEnabled = true;
                model.ShippingAddress.EmailRequired = true;
                model.ShippingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
                model.ShippingAddress.CompanyRequired = _addressSettings.CompanyRequired;
                model.ShippingAddress.CountryEnabled = _addressSettings.CountryEnabled;
                model.ShippingAddress.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
                model.ShippingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
                model.ShippingAddress.CityEnabled = _addressSettings.CityEnabled;
                model.ShippingAddress.CityRequired = _addressSettings.CityRequired;
                model.ShippingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
                model.ShippingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
                model.ShippingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
                model.ShippingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
                model.ShippingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
                model.ShippingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
                model.ShippingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
                model.ShippingAddress.PhoneRequired = _addressSettings.PhoneRequired;
                model.ShippingAddress.FaxEnabled = _addressSettings.FaxEnabled;
                model.ShippingAddress.FaxRequired = _addressSettings.FaxRequired;

                model.ShippingAddressGoogleMapsUrl = $"http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={WebUtility.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : ""))}";
            }
            else
            {
                if (order.PickupAddress != null)
                {
                    model.PickupAddress = order.PickupAddress.ToModel();
                    model.PickupAddressGoogleMapsUrl = $"http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={WebUtility.UrlEncode($"{order.PickupAddress.Address1} {order.PickupAddress.ZipPostalCode} {order.PickupAddress.City} {(order.PickupAddress.Country != null ? order.PickupAddress.Country.Name : string.Empty)}")}";
                }
            }
            model.ShippingMethod = order.ShippingMethod;

            model.CanAddNewShipments = order.HasItemsToAddToShipment();
        }

        protected virtual void PrepareBillingInfo(OrderModel model, Order order)
        {
            model.BillingAddress = order.BillingAddress.ToModel();
            model.BillingAddress.FormattedCustomAddressAttributes =
                _addressAttributeFormatter.FormatAttributes(order.BillingAddress.CustomAttributes);
            model.BillingAddress.FirstNameEnabled = true;
            model.BillingAddress.FirstNameRequired = true;
            model.BillingAddress.LastNameEnabled = true;
            model.BillingAddress.LastNameRequired = true;
            model.BillingAddress.EmailEnabled = true;
            model.BillingAddress.EmailRequired = true;
            model.BillingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.BillingAddress.CompanyRequired = _addressSettings.CompanyRequired;
            model.BillingAddress.CountryEnabled = _addressSettings.CountryEnabled;
            model.BillingAddress.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.BillingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.BillingAddress.CityEnabled = _addressSettings.CityEnabled;
            model.BillingAddress.CityRequired = _addressSettings.CityRequired;
            model.BillingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.BillingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.BillingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.BillingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.BillingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.BillingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.BillingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.BillingAddress.PhoneRequired = _addressSettings.PhoneRequired;
            model.BillingAddress.FaxEnabled = _addressSettings.FaxEnabled;
            model.BillingAddress.FaxRequired = _addressSettings.FaxRequired;
        }

        protected virtual void PreparePaymentInfo(OrderModel model, Order order)
        {
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
            var pm = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
            model.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);

            //payment method buttons
            model.CanCancelOrder = _orderProcessingService.CanCancelOrder(order);
            model.CanCapture = _orderProcessingService.CanCapture(order);
            model.CanMarkOrderAsPaid = _orderProcessingService.CanMarkOrderAsPaid(order);
            model.CanRefund = _orderProcessingService.CanRefund(order);
            model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);
            model.CanPartiallyRefund = _orderProcessingService.CanPartiallyRefund(order, decimal.Zero);
            model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);
            model.CanVoid = _orderProcessingService.CanVoid(order);
            model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.MaxAmountToRefund = order.OrderTotal - order.RefundedAmount;

            //recurring payment record
            var recurringPayment = _orderService.SearchRecurringPayments(initialOrderId: order.Id, showHidden: true).FirstOrDefault();

            if (recurringPayment != null)
            {
                model.RecurringPaymentId = recurringPayment.Id;
            }
        }

        protected virtual void PrepareOrderTotals(OrderModel model, Order order, Currency primaryStoreCurrency)
        {
            //subtotal
            model.OrderSubtotalInclTax = _priceFormatter.FormatPrice(order.OrderSubtotalInclTax, true, primaryStoreCurrency,
                _workContext.WorkingLanguage, true);
            model.OrderSubtotalExclTax = _priceFormatter.FormatPrice(order.OrderSubtotalExclTax, true, primaryStoreCurrency,
                _workContext.WorkingLanguage, false);
            model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
            model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;
            //discount (applied to order subtotal)
            var orderSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountInclTax, true,
                primaryStoreCurrency, _workContext.WorkingLanguage, true);
            var orderSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountExclTax, true,
                primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
            model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
            model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

            //shipping
            model.OrderShippingInclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingInclTax, true,
                primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderShippingExclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingExclTax, true,
                primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
            model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(
                    order.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(
                    order.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }
            model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;

            //tax
            model.Tax = _priceFormatter.FormatPrice(order.OrderTax, true, false);
            var taxRates = order.TaxRatesDictionary;
            var displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
            var displayTax = !displayTaxRates;
            foreach (var tr in order.TaxRatesDictionary)
            {
                model.TaxRates.Add(new OrderModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = _priceFormatter.FormatPrice(tr.Value, true, false),
                });
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.TaxValue = order.OrderTax;
            model.TaxRatesValue = order.TaxRates;

            //discount
            if (order.OrderDiscount > 0)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-order.OrderDiscount, true, false);
            model.OrderTotalDiscountValue = order.OrderDiscount;

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-gcuh.UsedValue, true, false),
                });
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount =
                    _priceFormatter.FormatPrice(-order.RedeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
            model.OrderTotalValue = order.OrderTotal;

            //refunded amount
            if (order.RefundedAmount > decimal.Zero)
                model.RefundedAmount = _priceFormatter.FormatPrice(order.RefundedAmount, true, false);

            //used discounts
            var duh = _discountService.GetAllDiscountUsageHistory(orderId: order.Id);
            foreach (var d in duh)
            {
                model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.Discount.Name
                });
            }

            //profit (hide for vendors)
            if (_workContext.CurrentVendor == null)
            {
                var profit = _orderReportService.ProfitReport(orderId: order.Id);
                model.Profit = _priceFormatter.FormatPrice(profit, true, false);
            }
        }

        protected virtual OrderModel.AddOrderProductModel.ProductDetailsModel PrepareAddProductToOrderModel(int orderId, int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            var presetQty = 1;
            var presetPrice = _priceCalculationService.GetFinalPrice(product, order.Customer, decimal.Zero, true, presetQty);
            var presetPriceInclTax = _taxService.GetProductPrice(product, presetPrice, true, order.Customer, out decimal taxRate);
            var presetPriceExclTax = _taxService.GetProductPrice(product, presetPrice, false, order.Customer, out taxRate);

            var model = new OrderModel.AddOrderProductModel.ProductDetailsModel
            {
                ProductId = productId,
                OrderId = orderId,
                Name = product.Name,
                ProductType = product.ProductType,
                UnitPriceExclTax = presetPriceExclTax,
                UnitPriceInclTax = presetPriceInclTax,
                Quantity = presetQty,
                SubTotalExclTax = presetPriceExclTax,
                SubTotalInclTax = presetPriceInclTax,
                AutoUpdateOrderTotals = _orderSettings.AutoUpdateOrderTotalsOnEditingOrder
            };

            //attributes
            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in attributes)
            {
                var attributeModel = new OrderModel.AddOrderProductModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        //price adjustment
                        var priceAdjustment = _taxService.GetProductPrice(product,
                            _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue), out taxRate);

                        attributeModel.Values.Add(new OrderModel.AddOrderProductModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity,
                            PriceAdjustment = priceAdjustment == decimal.Zero ? string.Empty : priceAdjustment > decimal.Zero
                                ? string.Concat("+", _priceFormatter.FormatPrice(priceAdjustment, false, false))
                                : string.Concat("-", _priceFormatter.FormatPrice(-priceAdjustment, false, false)),
                            PriceAdjustmentValue = priceAdjustment
                        });
                    }
                }

                model.ProductAttributes.Add(attributeModel);
            }
            model.HasCondition = model.ProductAttributes.Any(a => a.HasCondition);

            //gift card
            model.GiftCard.IsGiftCard = product.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = product.GiftCardType;
            }

            //rental
            model.IsRental = product.IsRental;
            return model;
        }
        
        protected virtual ShipmentModel PrepareShipmentModel(Shipment shipment, bool prepareProducts, bool prepareShipmentEvent = false)
        {
            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var model = new ShipmentModel
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                TotalWeight = shipment.TotalWeight.HasValue ? $"{shipment.TotalWeight:F2} [{baseWeightIn}]" : "",
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet"),
                ShippedDateUtc = shipment.ShippedDateUtc,
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet"),
                DeliveryDateUtc = shipment.DeliveryDateUtc,
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                AdminComment = shipment.AdminComment,
                CustomOrderNumber = shipment.Order.CustomOrderNumber
            };

            if (prepareProducts)
            {
                foreach (var shipmentItem in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    //quantities
                    var qtyInThisShipment = shipmentItem.Quantity;
                    var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = orderItem.Quantity;
                    var qtyInAllShipments = orderItem.GetTotalNumberOfItemsInAllShipment();

                    var warehouse = _shippingService.GetWarehouseById(shipmentItem.WarehouseId);
                    var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                    {
                        Id = shipmentItem.Id,
                        OrderItemId = orderItem.Id,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.Product.Name,
                        Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                        AttributeInfo = orderItem.AttributeDescription,
                        ShippedFromWarehouse = warehouse != null ? warehouse.Name : null,
                        ShipSeparately = orderItem.Product.ShipSeparately,
                        ItemWeight = orderItem.ItemWeight.HasValue ? $"{orderItem.ItemWeight:F2} [{baseWeightIn}]" : "",
                        ItemDimensions = $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimensionIn}]",
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                    };
                    //rental info
                    if (orderItem.Product.IsRental)
                    {
                        var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                        var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                        shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                            rentalStartDate, rentalEndDate);
                    }

                    model.Items.Add(shipmentItemModel);
                }
            }

            if (!prepareShipmentEvent || string.IsNullOrEmpty(shipment.TrackingNumber))
                return model;

            var shipmentTracker = shipment.GetShipmentTracker(_shippingService, _shippingSettings);
            if (shipmentTracker == null)
                return model;

            model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
            if (!_shippingSettings.DisplayShipmentEventsToStoreOwner)
                return model;

            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
            if (shipmentEvents == null)
                return model;

            foreach (var shipmentEvent in shipmentEvents)
            {
                var shipmentStatusEventModel = new ShipmentModel.ShipmentStatusEventModel();
                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                shipmentStatusEventModel.Country = shipmentEventCountry != null
                    ? shipmentEventCountry.GetLocalized(x => x.Name)
                    : shipmentEvent.CountryCode;
                shipmentStatusEventModel.Date = shipmentEvent.Date;
                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                shipmentStatusEventModel.Location = shipmentEvent.Location;
                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
            }

            return model;
        }

        protected virtual void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder", _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
        }

        protected virtual DataSourceResult GetBestsellersBriefReportModel(int pageIndex, int pageSize, int orderBy)
        {
            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            var items = _orderReportService.BestSellersReport(
                vendorId: vendorId,
                orderBy: orderBy,
                pageIndex: pageIndex,
                pageSize: pageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        ProductId = x.ProductId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var product = _productService.GetProductById(x.ProductId);
                    if (product != null)
                        m.ProductName = product.Name;
                    return m;
                }),
                Total = items.TotalCount
            };
            return gridModel;
        }

        #endregion

        #region Order list

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List(List<string> orderStatusIds = null, List<string> paymentStatusIds = null, List<string> shippingStatusIds = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //order statuses
            var model = new OrderListModel
            {
                AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList()
            };
            model.AvailableOrderStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (orderStatusIds != null && orderStatusIds.Any())
            {
                foreach (var item in model.AvailableOrderStatuses.Where(os => orderStatusIds.Contains(os.Value)))
                    item.Selected = true;
                model.AvailableOrderStatuses.First().Selected = false;
            }
            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (paymentStatusIds != null && paymentStatusIds.Any())
            {
                foreach (var item in model.AvailablePaymentStatuses.Where(ps => paymentStatusIds.Contains(ps.Value)))
                    item.Selected = true;
                model.AvailablePaymentStatuses.First().Selected = false;
            }

            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (shippingStatusIds != null && shippingStatusIds.Any())
            {
                foreach (var item in model.AvailableShippingStatuses.Where(ss => shippingStatusIds.Contains(ss.Value)))
                    item.Selected = true;
                model.AvailableShippingStatuses.First().Selected = false;
            }

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            //payment methods
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var pm in _paymentService.LoadAllPaymentMethods())
                model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
            {
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //a vendor should have access only to orders with his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            
            return View(model);
        }

        [HttpPost]
		public virtual IActionResult OrderList(DataSourceRequest command, OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
		    var product = _productService.GetProductById(model.ProductId);
		    if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue, 
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes,
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new OrderModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        OrderStatusId = x.OrderStatusId,
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatusId = x.PaymentStatusId,
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatusId = x.ShippingStatusId,
                        CustomerEmail = x.BillingAddress.Email,
                        CustomerFullName = $"{x.BillingAddress.FirstName} {x.BillingAddress.LastName}",
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomOrderNumber = x.CustomOrderNumber
                    };
                }),
                Total = orders.TotalCount
            };

            //summary report
            //currently we do not support productId and warehouseId parameters for this report
            var reportSummary = _orderReportService.GetOrderAverageReportLine(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                orderId: 0,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            var profit = _orderReportService.ProfitReport(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue, 
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            gridModel.ExtraData = new OrderAggreratorModel
            {
                aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                aggregatorshipping = _priceFormatter.FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false),
                aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
                aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumOrders, true, false)
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public virtual IActionResult GoToOrderId(OrderListModel model)
        {
            var order = _orderService.GetOrderByCustomOrderNumber(model.GoDirectlyToCustomOrderNumber);

            if (order == null)
                return List();

            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        public virtual IActionResult ProductSearchAutoComplete(string term)
        {
            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content("");

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            //products
            const int productNumber = 15;
            var products = _productService.SearchProducts(
                vendorId: vendorId,
                keywords: term,
                pageSize: productNumber,
                showHidden: true);

            var result = (from p in products
                select new
                {
                    label = p.Name,
                    productid = p.Id
                }).ToList();
            return Json(result);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            try
            {
                var xml = _exportManager.ExportOrdersToXml(orders);

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "orders.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            
            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids).Where(HasAccessToOrder));
            }

            var xml = _exportManager.ExportOrdersToXml(orders);

            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "orders.xml");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            
            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            try
            {
                var bytes = _exportManager.ExportOrdersToXlsx(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids).Where(HasAccessToOrder));
            }

            try
            {
                var bytes = _exportManager.ExportOrdersToXlsx(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }
        
        #endregion

        #region Order details

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public virtual IActionResult CancelOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.CancelOrder(order, true);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public virtual IActionResult CaptureOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Capture(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }

        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public virtual IActionResult MarkOrderAsPaid(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.MarkOrderAsPaid(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public virtual IActionResult RefundOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Refund(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public virtual IActionResult RefundOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.RefundOffline(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public virtual IActionResult VoidOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Void(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public virtual IActionResult VoidOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.VoidOffline(order);
                LogEditOrder(order.Id);
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }
        
        public virtual IActionResult PartiallyRefundOrderPopup(int id, bool online)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public virtual IActionResult PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new NopException("Enter amount to refund");

                var maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = _orderProcessingService.PartiallyRefund(order, amountToRefund).ToList();
                else
                    _orderProcessingService.PartiallyRefundOffline(order, amountToRefund);

                LogEditOrder(order.Id);

                if (!errors.Any())
                {
                    //success
                    ViewBag.RefreshPage = true;

                    PrepareOrderDetailsModel(model, order);
                    return View(model);
                }
                
                //error
                PrepareOrderDetailsModel(model, order);
                foreach (var error in errors)
                    ErrorNotification(error, false);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderStatus")]
        public virtual IActionResult ChangeOrderStatus(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                order.OrderStatusId = model.OrderStatusId;
                _orderService.UpdateOrder(order);

                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = $"Order status has been edited. New status: {order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);

                model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            _orderProcessingService.DeleteOrder(order);

            //activity log
            _customerActivityService.InsertActivity("DeleteOrder", _localizationService.GetResource("ActivityLog.DeleteOrder"), order.Id);

            return RedirectToAction("List");
        }

        public virtual IActionResult PdfInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var order = _orderService.GetOrderById(orderId);
            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, vendorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("pdf-invoice-all")]
        public virtual IActionResult PdfInvoiceAll(OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, model.VendorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
        }

        [HttpPost]
        public virtual IActionResult PdfInvoiceSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids));
            }

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                orders = orders.Where(HasAccessToOrder).ToList();
                vendorId = _workContext.CurrentVendor.Id;
            }
            
            //ensure that we at least one order selected
            if (!orders.Any())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.PdfInvoice.NoOrders"));
                return RedirectToAction("List");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, vendorId);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
        }

        //currently we use this method on the add product to order details pages
        [HttpPost]
        public virtual IActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
	    {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = ParseProductAttributes(product, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            return Json(new
            {
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public virtual IActionResult EditCreditCardInfo(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            if (order.AllowStoringCreditCardNumber)
            {
                var cardType = model.CardType;
                var cardName = model.CardName;
                var cardNumber = model.CardNumber;
                var cardCvv2 = model.CardCvv2;
                var cardExpirationMonth = model.CardExpirationMonth;
                var cardExpirationYear = model.CardExpirationYear;

                order.CardType = _encryptionService.EncryptText(cardType);
                order.CardName = _encryptionService.EncryptText(cardName);
                order.CardNumber = _encryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                _orderService.UpdateOrder(order);
            }

            //add a note
            order.OrderNotes.Add(new OrderNote
            {
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public virtual IActionResult EditOrderTotals(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
            order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
            order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
            order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
            order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
            order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
            order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
            order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
            order.TaxRates = model.TaxRatesValue;
            order.OrderTax = model.TaxValue;
            order.OrderDiscount = model.OrderTotalDiscountValue;
            order.OrderTotal = model.OrderTotalValue;
            _orderService.UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote
            {
                Note = "Order totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public virtual IActionResult EditShippingMethod(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            order.ShippingMethod = model.ShippingMethod;
            _orderService.UpdateOrder(order);

            //add a note
            order.OrderNotes.Add(new OrderNote
            {
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            PrepareOrderDetailsModel(model, order);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
        public virtual IActionResult EditOrderItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnSaveOrderItem".Length));

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            if (!decimal.TryParse(form["pvUnitPriceInclTax" + orderItemId], out decimal unitPriceInclTax))
                unitPriceInclTax = orderItem.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + orderItemId], out decimal unitPriceExclTax))
                unitPriceExclTax = orderItem.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + orderItemId], out int quantity))
                quantity = orderItem.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + orderItemId], out decimal discountInclTax))
                discountInclTax = orderItem.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + orderItemId], out decimal discountExclTax))
                discountExclTax = orderItem.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + orderItemId], out decimal priceInclTax))
                priceInclTax = orderItem.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + orderItemId], out decimal priceExclTax))
                priceExclTax = orderItem.PriceExclTax;

            if (quantity > 0)
            {
                var qtyDifference = orderItem.Quantity - quantity;

                if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                {
                    orderItem.UnitPriceInclTax = unitPriceInclTax;
                    orderItem.UnitPriceExclTax = unitPriceExclTax;
                    orderItem.Quantity = quantity;
                    orderItem.DiscountAmountInclTax = discountInclTax;
                    orderItem.DiscountAmountExclTax = discountExclTax;
                    orderItem.PriceInclTax = priceInclTax;
                    orderItem.PriceExclTax = priceExclTax;
                    _orderService.UpdateOrder(order);
                }

                //adjust inventory
                _productService.AdjustInventory(orderItem.Product, qtyDifference, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

            }
            else
            {
                //adjust inventory
                _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                _orderService.DeleteOrderItem(orderItem);
            }

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters
            {
                UpdatedOrder = order,
                UpdatedOrderItem = orderItem,
                PriceInclTax = unitPriceInclTax,
                PriceExclTax = unitPriceExclTax,
                DiscountAmountInclTax = discountInclTax,
                DiscountAmountExclTax = discountExclTax,
                SubTotalInclTax = priceInclTax,
                SubTotalExclTax = priceExclTax,
                Quantity = quantity
            };
            _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

            //add a note
            order.OrderNotes.Add(new OrderNote
            {
                Note = "Order item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            foreach (var warning in updateOrderParameters.Warnings)
                WarningNotification(warning, false);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
        public virtual IActionResult DeleteOrderItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnDeleteOrderItem".Length));

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            if (_giftCardService.GetGiftCardsByPurchasedWithOrderItemId(orderItem.Id).Any())
            {
                //we cannot delete an order item with associated gift cards
                //a store owner should delete them first

                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);

                ErrorNotification(_localizationService.GetResource("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"), false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);

            }
            else
            {
                //adjust inventory
                _productService.AdjustInventory(orderItem.Product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                _orderService.DeleteOrderItem(orderItem);

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters
                {
                    UpdatedOrder = order,
                    UpdatedOrderItem = orderItem
                };
                _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = "Order item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);

                var model = new OrderModel();
                PrepareOrderDetailsModel(model, order);
                foreach (var warning in updateOrderParameters.Warnings)
                    WarningNotification(warning, false);

                //selected tab
                SaveSelectedTabName(persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        public virtual IActionResult ResetDownloadCount(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderItem(orderItem))
                return RedirectToAction("List");

            orderItem.DownloadCount = 0;
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        public virtual IActionResult ActivateDownloadItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderItem(orderItem))
                return RedirectToAction("List");

            orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            //selected tab
            SaveSelectedTabName(persistForTheNextRequest: false);

            return View(model);
        }

        public virtual IActionResult UploadLicenseFilePopup(int id, int orderItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            if (!orderItem.Product.IsDownload)
                throw new ArgumentException("Product is not downloadable");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderItem(orderItem))
                return RedirectToAction("List");

            var model = new OrderModel.UploadLicenseModel
            {
                LicenseDownloadId = orderItem.LicenseDownloadId.HasValue ? orderItem.LicenseDownloadId.Value : 0,
                OrderId = order.Id,
                OrderItemId = orderItem.Id
            };

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public virtual IActionResult UploadLicenseFilePopup(OrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == model.OrderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderItem(orderItem))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                orderItem.LicenseDownloadId = model.LicenseDownloadId;
            else
                orderItem.LicenseDownloadId = null;
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public virtual IActionResult DeleteLicenseFilePopup(OrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderItem = order.OrderItems.FirstOrDefault(x => x.Id == model.OrderItemId);
            if (orderItem == null)
                throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderItem(orderItem))
                return RedirectToAction("List");

            //attach license
            orderItem.LicenseDownloadId = null;
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        public virtual IActionResult AddProductToOrder(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var model = new OrderModel.AddOrderProductModel
            {
                OrderId = orderId
            };
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddProductToOrder(DataSourceRequest command, OrderModel.AddOrderProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            var gridModel = new DataSourceResult();
            var products = _productService.SearchProducts(categoryIds: new List<int> {model.SearchCategoryId},
                manufacturerId: model.SearchManufacturerId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize,
                showHidden: true);
            gridModel.Data = products.Select(x =>
            {
                var productModel = new OrderModel.AddOrderProductModel.ProductModel
                {
                    Id = x.Id,
                    Name =  x.Name,
                    Sku = x.Sku,
                };

                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        public virtual IActionResult AddProductToOrderDetails(int orderId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var model = PrepareAddProductToOrderModel(orderId, productId);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddProductToOrderDetails(int orderId, int productId, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var order = _orderService.GetOrderById(orderId);
            var product = _productService.GetProductById(productId);
            //save order item

            //basic properties
            decimal.TryParse(form["UnitPriceInclTax"], out decimal unitPriceInclTax);
            decimal.TryParse(form["UnitPriceExclTax"], out decimal unitPriceExclTax);
            int.TryParse(form["Quantity"], out int quantity);
            decimal.TryParse(form["SubTotalInclTax"], out decimal priceInclTax);
            decimal.TryParse(form["SubTotalExclTax"], out decimal priceExclTax);

            //warnings
            var warnings = new List<string>();

            //attributes
            var attributesXml = ParseProductAttributes(product, form, warnings);

            #region Gift cards

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            if (product.IsGiftCard)
            {
                foreach (var formKey in form.Keys)
                {
                    if (formKey.Equals("giftcard.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.SenderName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals("giftcard.Message", StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            #region Rental product

            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                ParseRentalDates(form, out rentalStartDate, out rentalEndDate);
            }

            #endregion

            //warnings
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(order.Customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemGiftCardWarnings(ShoppingCartType.ShoppingCart, product, attributesXml));
            warnings.AddRange(_shoppingCartService.GetRentalProductWarnings(product, rentalStartDate, rentalEndDate));
            if (!warnings.Any())
            {
                //no errors

                //attributes
                var attributeDescription = _productAttributeFormatter.FormatAttributes(product, attributesXml, order.Customer);

                //weight
                var itemWeight = _shippingService.GetShoppingCartItemWeight(product, attributesXml);

                //save item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    Order = order,
                    ProductId = product.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    OriginalProductCost = _priceCalculationService.GetProductCost(product, attributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributesXml,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate
                };
                order.OrderItems.Add(orderItem);
                _orderService.UpdateOrder(order);

                //adjust inventory
                _productService.AdjustInventory(orderItem.Product, -orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters
                {
                    UpdatedOrder = order,
                    UpdatedOrderItem = orderItem,
                    PriceInclTax = unitPriceInclTax,
                    PriceExclTax = unitPriceExclTax,
                    SubTotalInclTax = priceInclTax,
                    SubTotalExclTax = priceExclTax,
                    Quantity = quantity
                };
                _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = "A new order item has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);

                //gift cards
                if (product.IsGiftCard)
                {
                    for (var i = 0; i < orderItem.Quantity; i++)
                    {
                        var gc = new GiftCard
                        {
                            GiftCardType = product.GiftCardType,
                            PurchasedWithOrderItem = orderItem,
                            Amount = unitPriceExclTax,
                            IsGiftCardActivated = false,
                            GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                            RecipientName = recipientName,
                            RecipientEmail = recipientEmail,
                            SenderName = senderName,
                            SenderEmail = senderEmail,
                            Message = giftCardMessage,
                            IsRecipientNotified = false,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        _giftCardService.InsertGiftCard(gc);
                    }
                }

                //redirect to order details page
                foreach (var warning in updateOrderParameters.Warnings)
                    WarningNotification(warning);

                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }
            
            //errors
            var model = PrepareAddProductToOrderModel(order.Id, product.Id);
            model.Warnings.AddRange(warnings);
            return View(model);
        }

        #endregion

        #endregion
        
        #region Addresses

        public virtual IActionResult AddressEdit(int addressId, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new OrderAddressModel
            {
                OrderId = orderId,
                Address = address.ToModel()
            };
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;

            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(OrderAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                throw new ArgumentException("No address found with the specified id");

            //custom address attributes
            var customAttributes = model.Form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
            }

            //If we got this far, something failed, redisplay form
            model.OrderId = order.Id;
            model.Address = address.ToModel();
            model.Address.FirstNameEnabled = true;
            model.Address.FirstNameRequired = true;
            model.Address.LastNameEnabled = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailEnabled = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.Address.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, showHidden: true).ToList() : new List<StateProvince>();
            if (states.Any())
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            //customer attribute services
            model.Address.PrepareCustomAddressAttributes(address, _addressAttributeService, _addressAttributeParser);

            return View(model);
        }
        
        #endregion
        
        #region Shipments

        public virtual IActionResult ShipmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new ShipmentListModel();
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            return View(model);
		}

		[HttpPost]
        public virtual IActionResult ShipmentListSelect(DataSourceRequest command, ShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId: vendorId,
                warehouseId: model.WarehouseId, 
                shippingCountryId: model.CountryId, 
                shippingStateId: model.StateProvinceId, 
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber, 
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue, 
                createdToUtc: endDateValue, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = shipments.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ShipmentsByOrder(int orderId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return Content("");

            //shipments
            var shipmentModels = new List<ShipmentModel>();
            var shipments = order.Shipments
                //a vendor should have access only to his products
                .Where(s => _workContext.CurrentVendor == null || HasAccessToShipment(s))
                .OrderBy(s => s.CreatedOnUtc)
                .ToList();
            foreach (var shipment in shipments)
                shipmentModels.Add(PrepareShipmentModel(shipment, false));

            var gridModel = new DataSourceResult
            {
                Data = shipmentModels,
                Total = shipmentModels.Count
            };
            
            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ShipmentsItemsByShipmentId(int shipmentId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                throw new ArgumentException("No shipment found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return Content("");

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return Content("");

            //shipments
            var shipmentModel = PrepareShipmentModel(shipment, true);
            var gridModel = new DataSourceResult
            {
                Data = shipmentModel.Items,
                Total = shipmentModel.Items.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult AddShipment(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var model = new ShipmentModel
            {
                OrderId = order.Id,
                CustomOrderNumber = order.CustomOrderNumber
            };

            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var orderItems = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderItems = orderItems.Where(HasAccessToOrderItem).ToList();
            }

            foreach (var orderItem in orderItems)
            {
                //we can ship only shippable products
                if (!orderItem.Product.IsShipEnabled)
                    continue;

                //quantities
                var qtyInThisShipment = 0;
                var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                var qtyOrdered = orderItem.Quantity;
                var qtyInAllShipments = orderItem.GetTotalNumberOfItemsInAllShipment();

                //ensure that this product can be added to a shipment
                if (maxQtyToAdd <= 0)
                    continue;

                var shipmentItemModel = new ShipmentModel.ShipmentItemModel
                {
                    OrderItemId = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                    AttributeInfo = orderItem.AttributeDescription,
                    ShipSeparately = orderItem.Product.ShipSeparately,
                    ItemWeight = orderItem.ItemWeight.HasValue ? $"{orderItem.ItemWeight:F2} [{baseWeightIn}]" : "",
                    ItemDimensions =$"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimensionIn}]",
                    QuantityOrdered = qtyOrdered,
                    QuantityInThisShipment = qtyInThisShipment,
                    QuantityInAllShipments = qtyInAllShipments,
                    QuantityToAdd = maxQtyToAdd,
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                if (orderItem.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    orderItem.Product.UseMultipleWarehouses)
                {
                    //multiple warehouses supported
                    shipmentItemModel.AllowToChooseWarehouse = true;
                    foreach (var pwi in orderItem.Product.ProductWarehouseInventory
                        .OrderBy(w => w.WarehouseId).ToList())
                    {
                        var warehouse = pwi.Warehouse;
                        if (warehouse != null)
                        {
                            shipmentItemModel.AvailableWarehouses.Add(new ShipmentModel.ShipmentItemModel.WarehouseInfo
                            {
                                WarehouseId = warehouse.Id,
                                WarehouseName = warehouse.Name,
                                StockQuantity = pwi.StockQuantity,
                                ReservedQuantity = pwi.ReservedQuantity,
                                PlannedQuantity = _shipmentService.GetQuantityInShipments(orderItem.Product, warehouse.Id, true, true)
                            });
                        }
                    }
                }
                else
                {
                    //multiple warehouses are not supported
                    var warehouse = _shippingService.GetWarehouseById(orderItem.Product.WarehouseId);
                    if (warehouse != null)
                    {
                        shipmentItemModel.AvailableWarehouses.Add(new ShipmentModel.ShipmentItemModel.WarehouseInfo
                        {
                            WarehouseId = warehouse.Id,
                            WarehouseName = warehouse.Name,
                            StockQuantity = orderItem.Product.StockQuantity
                        });
                    }
                }
                    
                model.Items.Add(shipmentItemModel);
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult AddShipment(int orderId, IFormCollection form, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var orderItems = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderItems = orderItems.Where(HasAccessToOrderItem).ToList();
            }

            Shipment shipment = null;
            decimal? totalWeight = null;
            foreach (var orderItem in orderItems)
            {
                //is shippable
                if (!orderItem.Product.IsShipEnabled)
                    continue;
                
                //ensure that this product can be shipped (have at least one item to ship)
                var maxQtyToAdd = orderItem.GetTotalNumberOfItemsCanBeAddedToShipment();
                if (maxQtyToAdd <= 0)
                    continue;

                var qtyToAdd = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out qtyToAdd);
                        break;
                    }

                var warehouseId = 0;
                if (orderItem.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    orderItem.Product.UseMultipleWarehouses)
                {
                    //multiple warehouses supported
                    //warehouse is chosen by a store owner
                    foreach (var formKey in form.Keys)
                        if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out warehouseId);
                            break;
                        }
                }
                else
                {
                    //multiple warehouses are not supported
                    warehouseId = orderItem.Product.WarehouseId;
                }

                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out qtyToAdd);
                        break;
                    }

                //validate quantity
                if (qtyToAdd <= 0)
                    continue;
                if (qtyToAdd > maxQtyToAdd)
                    qtyToAdd = maxQtyToAdd;
                
                //ok. we have at least one item. let's create a shipment (if it does not exist)

                var orderItemTotalWeight = orderItem.ItemWeight.HasValue ? orderItem.ItemWeight * qtyToAdd : null;
                if (orderItemTotalWeight.HasValue)
                {
                    if (!totalWeight.HasValue)
                        totalWeight = 0;
                    totalWeight += orderItemTotalWeight.Value;
                }
                if (shipment == null)
                {
                    var trackingNumber = form["TrackingNumber"];
                    var adminComment = form["AdminComment"];
                    shipment = new Shipment
                    {
                        OrderId = order.Id,
                        TrackingNumber = trackingNumber,
                        TotalWeight = null,
                        ShippedDateUtc = null,
                        DeliveryDateUtc = null,
                        AdminComment = adminComment,
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                }
                //create a shipment item
                var shipmentItem = new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = qtyToAdd,
                    WarehouseId = warehouseId
                };
                shipment.ShipmentItems.Add(shipmentItem);
            }

            //if we have at least one item in the shipment, then save it
            if (shipment != null && shipment.ShipmentItems.Any())
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);

                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
                return continueEditing
                           ? RedirectToAction("ShipmentDetails", new {id = shipment.Id})
                           : RedirectToAction("Edit", new { id = orderId });
            }
            
            ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoProductsSelected"));
            return RedirectToAction("AddShipment", new { orderId = orderId });
        }

        public virtual IActionResult ShipmentDetails(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            var model = PrepareShipmentModel(shipment, true, true);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteShipment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                _productService.ReverseBookedInventory(orderItem.Product, shipmentItem,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
            }

            var orderId = shipment.OrderId;
            _shipmentService.DeleteShipment(shipment);

            var order =_orderService.GetOrderById(orderId);
            //add a note
            order.OrderNotes.Add(new OrderNote
            {
                Note = "A shipment has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
            _orderService.UpdateOrder(order);
            LogEditOrder(order.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Deleted"));
            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public virtual IActionResult SetTrackingNumber(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.TrackingNumber = model.TrackingNumber;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setadmincomment")]
        public virtual IActionResult SetShipmentAdminComment(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.AdminComment = model.AdminComment;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasshipped")]
        public virtual IActionResult SetAsShipped(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Ship(shipment, true);
                LogEditOrder(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("saveshippeddate")]
        public virtual IActionResult EditShippedDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ShippedDateUtc.HasValue)
                {
                    throw new Exception("Enter shipped date");
                }
                shipment.ShippedDateUtc = model.ShippedDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasdelivered")]
        public virtual IActionResult SetAsDelivered(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Deliver(shipment, true);
                LogEditOrder(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }


        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savedeliverydate")]
        public virtual IActionResult EditDeliveryDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                //No shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.DeliveryDateUtc.HasValue)
                {
                    throw new Exception("Enter delivery date");
                }
                shipment.DeliveryDateUtc = model.DeliveryDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        public virtual IActionResult PdfPackagingSlip(int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                //no shipment found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            var shipments = new List<Shipment>();
            shipments.Add(shipment);
            
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
        }

        [HttpPost, ActionName("ShipmentList")]
        [FormValueRequired("exportpackagingslips-all")]
        public virtual IActionResult PdfPackagingSlipAll(ShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId: vendorId,
                warehouseId: model.WarehouseId,
                shippingCountryId: model.CountryId,
                shippingStateId: model.StateProvinceId,
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue);

            //ensure that we at least one shipment selected
            if (!shipments.Any())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
        }

        [HttpPost]
        public virtual IActionResult PdfPackagingSlipSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                shipments.AddRange(_shipmentService.GetShipmentsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            //ensure that we at least one shipment selected
            if (!shipments.Any())
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
        }

        [HttpPost]
        public virtual IActionResult SetAsShippedSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                shipments.AddRange(_shipmentService.GetShipmentsByIds(selectedIds.ToArray()));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }
            
            foreach (var shipment in shipments)
            {
                try
                {
                    _orderProcessingService.Ship(shipment, true);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult SetAsDeliveredSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                shipments.AddRange(_shipmentService.GetShipmentsByIds(selectedIds.ToArray()));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            foreach (var shipment in shipments)
            {
                try
                {
                    _orderProcessingService.Deliver(shipment, true);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return Json(new { Result = true });
        }
        
        #endregion
        
        #region Order notes
        
        [HttpPost]
        public virtual IActionResult OrderNotesSelect(int orderId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            //order notes
            var orderNoteModels = new List<OrderModel.OrderNote>();
            foreach (var orderNote in order.OrderNotes
                .OrderByDescending(on => on.CreatedOnUtc))
            {
                var download = _downloadService.GetDownloadById(orderNote.DownloadId);
                orderNoteModels.Add(new OrderModel.OrderNote
                {
                    Id = orderNote.Id,
                    OrderId = orderNote.OrderId,
                    DownloadId = orderNote.DownloadId,
                    DownloadGuid = download != null ? download.DownloadGuid : Guid.Empty,
                    DisplayToCustomer = orderNote.DisplayToCustomer,
                    Note = orderNote.FormatOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = orderNoteModels,
                Total = orderNoteModels.Count
            };

            return Json(gridModel);
        }
        
        public virtual IActionResult OrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return Json(new { Result = false });

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Json(new { Result = false });

            var orderNote = new OrderNote
            {
                DisplayToCustomer = displayToCustomer,
                Note = message,
                DownloadId = downloadId,
                CreatedOnUtc = DateTime.UtcNow,
            };
            order.OrderNotes.Add(orderNote);
            _orderService.UpdateOrder(order);

            //new order notification
            if (displayToCustomer)
            {
                //email
                _workflowMessageService.SendNewOrderNoteAddedCustomerNotification(
                    orderNote, _workContext.WorkingLanguage.Id);

            }

            return Json(new {Result = true});
        }

        [HttpPost]
        public virtual IActionResult OrderNoteDelete(int id, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var orderNote = order.OrderNotes.FirstOrDefault(on => on.Id == id);
            if (orderNote == null)
                throw new ArgumentException("No order note found with the specified id");
            _orderService.DeleteOrderNote(orderNote);

            return new NullJsonResult();
        }
        
        #endregion

        #region Reports

        [HttpPost]
        public virtual IActionResult BestsellersBriefReportByQuantityList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 1);

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult BestsellersBriefReportByAmountList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 2);

            return Json(gridModel);
        }

        public virtual IActionResult BestsellersReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new BestsellersReportModel
            {
                //vendor
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //order statuses
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersReportList(DataSourceRequest command, BestsellersReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            var paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            var items = _orderReportService.BestSellersReport(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: model.BillingCountryId,
                orderBy: 2,
                vendorId: model.VendorId,
                categoryId: model.CategoryId,
                manufacturerId: model.ManufacturerId,
                storeId: model.StoreId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel
                    {
                        ProductId = x.ProductId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var product = _productService.GetProductById(x.ProductId);
                    if (product!= null)
                        m.ProductName = product.Name;
                    return m;
                }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }

        public virtual IActionResult NeverSoldReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new NeverSoldReportModel
            {

                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NeverSoldReportList(DataSourceRequest command, NeverSoldReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            
            var items = _orderReportService.ProductsNeverSold(vendorId: model.SearchVendorId,
                storeId: model.SearchStoreId,
                categoryId: model.SearchCategoryId,
                manufacturerId: model.SearchManufacturerId,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex : command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                    new NeverSoldReportLineModel
                    {
                        ProductId = x.Id,
                        ProductName = x.Name,
                    }),
                Total = items.TotalCount
            };

            return Json(gridModel);
        }
        
        [HttpPost]
        public virtual IActionResult OrderAverageReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var report = new List<OrderAverageReportLineSummary>();
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Pending));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Processing));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Complete));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Cancelled));
            var model = report.Select(x => new OrderAverageReportLineSummaryModel
            {
                OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                SumTodayOrders = _priceFormatter.FormatPrice(x.SumTodayOrders, true, false),
                SumThisWeekOrders = _priceFormatter.FormatPrice(x.SumThisWeekOrders, true, false),
                SumThisMonthOrders = _priceFormatter.FormatPrice(x.SumThisMonthOrders, true, false),
                SumThisYearOrders = _priceFormatter.FormatPrice(x.SumThisYearOrders, true, false),
                SumAllTimeOrders = _priceFormatter.FormatPrice(x.SumAllTimeOrders, true, false),
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult OrderIncompleteReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = new List<OrderIncompleteReportLineModel>();
            //not paid
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().Where(os => os != (int)OrderStatus.Cancelled).ToList();
            var paymentStatuses = new List<int>() { (int)PaymentStatus.Pending };
            var psPending = _orderReportService.GetOrderAverageReportLine(psIds: paymentStatuses, osIds: orderStatuses);
            model.Add(new OrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
                Count = psPending.CountOrders,
                Total = _priceFormatter.FormatPrice(psPending.SumOrders, true, false),
                ViewLink = Url.Action("List", "Order", new {
                    orderStatusIds = string.Join(",", orderStatuses),
                    paymentStatusIds = string.Join(",", paymentStatuses) })
            });
            //not shipped
            var shippingStatuses = new List<int>() { (int)ShippingStatus.NotYetShipped };
            var ssPending = _orderReportService.GetOrderAverageReportLine(osIds: orderStatuses, ssIds: shippingStatuses);
            model.Add(new OrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
                Count = ssPending.CountOrders,
                Total = _priceFormatter.FormatPrice(ssPending.SumOrders, true, false),
                ViewLink = Url.Action("List", "Order", new {
                    orderStatusIds = string.Join(",", orderStatuses),
                    shippingStatusIds = string.Join(",", shippingStatuses) })
            });
            //pending
            orderStatuses = new List<int>() { (int)OrderStatus.Pending };
            var osPending = _orderReportService.GetOrderAverageReportLine(osIds: orderStatuses);
            model.Add(new OrderIncompleteReportLineModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalIncompleteOrders"),
                Count = osPending.CountOrders,
                Total = _priceFormatter.FormatPrice(osPending.SumOrders, true, false),
                ViewLink = Url.Action("List", "Order", new { orderStatusIds = string.Join(",", orderStatuses) })
            });

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult CountryReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            var model = new CountryReportModel
            {

                //order statuses
                AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList()
            };
            model.AvailableOrderStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CountryReportList(DataSourceRequest command, CountryReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedKendoGridJson();

            var startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            var paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            var items = _orderReportService.GetCountryReport(
                os: orderStatus,
                ps: paymentStatus,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue);
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var country = _countryService.GetCountryById(x.CountryId.HasValue ? x.CountryId.Value : 0);
                    var m = new CountryReportLineModel
                    {
                        CountryName = country != null ? country.Name : "Unknown",
                        SumOrders = _priceFormatter.FormatPrice(x.SumOrders, true, false),
                        TotalOrders = x.TotalOrders,
                    };
                    return m;
                }),
                Total = items.Count
            };

            return Json(gridModel);
        }
        
        public virtual IActionResult LoadOrderStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    if (!timeZone.IsInvalidTime(searchYearDateUser))
                    {
                        for (var i = 0; i <= 12; i++)
                        {
                            result.Add(new
                            {
                                date = searchYearDateUser.Date.ToString("Y", culture),
                                value = _orderService.SearchOrders(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchYearDateUser = searchYearDateUser.AddMonths(1);
                        }
                    }
                    break;

                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchMonthDateUser))
                    {
                        for (var i = 0; i <= 30; i++)
                        {
                            result.Add(new
                            {
                                date = searchMonthDateUser.Date.ToString("M", culture),
                                value = _orderService.SearchOrders(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchMonthDateUser = searchMonthDateUser.AddDays(1);
                        }
                    }
                    break;

                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchWeekDateUser))
                    {
                        for (var i = 0; i <= 7; i++)
                        {
                            result.Add(new
                            {
                                date = searchWeekDateUser.Date.ToString("d dddd", culture),
                                value = _orderService.SearchOrders(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                    pageIndex: 0,
                                    pageSize: 1).TotalCount.ToString()
                            });

                            searchWeekDateUser = searchWeekDateUser.AddDays(1);
                        }
                    }
                    break;
            }

            return Json(result);
        }

        #endregion
    }
}