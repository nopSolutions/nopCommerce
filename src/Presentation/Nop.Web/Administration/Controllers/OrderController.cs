using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public partial class OrderController : BaseNopController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderProcessingService _orderProcessingService;
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
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly AddressSettings _addressSettings;
        
        #endregion

        #region Ctor

        public OrderController(IOrderService orderService, 
            IOrderReportService orderReportService, IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            IDiscountService discountService, ILocalizationService localizationService,
            IWorkContext workContext, ICurrencyService currencyService,
            IEncryptionService encryptionService, IPaymentService paymentService,
            IMeasureService measureService, IPdfService pdfService,
            IAddressService addressService, ICountryService countryService,
            IStateProvinceService stateProvinceService, IProductService productService,
            IExportManager exportManager, IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICategoryService categoryService, IManufacturerService manufacturerService,
            IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser,
            IProductAttributeFormatter productAttributeFormatter, IShoppingCartService shoppingCartService,
            IGiftCardService giftCardService, IDownloadService downloadService,
            IShipmentService shipmentService, IStoreService storeService, IVendorService vendorService,
            CatalogSettings catalogSettings, CurrencySettings currencySettings, TaxSettings taxSettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings, AddressSettings addressSettings)
		{
            this._orderService = orderService;
            this._orderReportService = orderReportService;
            this._orderProcessingService = orderProcessingService;
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
            this._storeService = storeService;
            this._vendorService = vendorService;

            this._catalogSettings = catalogSettings;
            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
            this._addressSettings = addressSettings;
		}
        
        #endregion

        #region Utilities

        [NonAction]
        protected bool HasAccessToOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = order.OrderProductVariants.Any(opv => opv.ProductVariant.Product.VendorId == vendorId);
            return hasVendorProducts;
        }

        [NonAction]
        protected bool HasAccessToOrderProductVariant(OrderProductVariant opv)
        {
            if (opv == null)
                throw new ArgumentNullException("opv");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return opv.ProductVariant.Product.VendorId == vendorId;
        }

        [NonAction]
        protected bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var hasVendorProducts = false;
            var vendorId = _workContext.CurrentVendor.Id;
            foreach (var sopv in shipment.ShipmentOrderProductVariants)
            {
                var opv = _orderService.GetOrderProductVariantById(sopv.OrderProductVariantId);
                if (opv != null)
                {
                    if (opv.ProductVariant.Product.VendorId == vendorId)
                    {
                        hasVendorProducts = true;
                        break;
                    }
                }
            }
            return hasVendorProducts;
        }

        [NonAction]
        protected void PrepareOrderDetailsModel(OrderModel model, Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = order.Id;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.OrderGuid = order.OrderGuid;
            var store = _storeService.GetStoreById(order.StoreId);
            model.StoreName = store != null ? store.Name : "Unknown";
            model.CustomerId = order.CustomerId;
            var customer = order.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CustomerIp = order.CustomerIp;
            model.VatNumber = order.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.DisplayPdfInvoice = _pdfSettings.Enabled;
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;
            model.AffiliateId = order.AffiliateId;
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            
            #region Order totals

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            //subtotal
            model.OrderSubtotalInclTax = _priceFormatter.FormatPrice(order.OrderSubtotalInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderSubtotalExclTax = _priceFormatter.FormatPrice(order.OrderSubtotalExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
            model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;
            //discount (applied to order subtotal)
            string orderSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            string orderSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
            model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
            model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

            //shipping
            model.OrderShippingInclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderShippingExclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
            model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }
            model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;


            //tax
            model.Tax = _priceFormatter.FormatPrice(order.OrderTax, true, false);
            SortedDictionary<decimal, decimal> taxRates = order.TaxRatesDictionary;
            bool displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
            bool displayTax = !displayTaxRates;
            foreach (var tr in order.TaxRatesDictionary)
            {
                model.TaxRates.Add(new OrderModel.TaxRate()
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
                model.GiftCards.Add(new OrderModel.GiftCard()
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-gcuh.UsedValue, true, false),
                });
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-order.RedeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
            model.OrderTotalValue = order.OrderTotal;

            //refunded amount
            if (order.RefundedAmount > decimal.Zero)
                model.RefundedAmount = _priceFormatter.FormatPrice(order.RefundedAmount, true, false);

            //used discounts
            var duh = _discountService.GetAllDiscountUsageHistory(null, null, order.Id, 0, int.MaxValue);
            foreach (var d in duh)
            {
                model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel()
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.Discount.Name
                });
            }

            #endregion

            #region Payment info

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
                string cardExpirationMonthDecrypted = _encryptionService.DecryptText(order.CardExpirationMonth);
                if (!String.IsNullOrEmpty(cardExpirationMonthDecrypted) && cardExpirationMonthDecrypted != "0")
                    model.CardExpirationMonth = cardExpirationMonthDecrypted;
                string cardExpirationYearDecrypted = _encryptionService.DecryptText(order.CardExpirationYear);
                if (!String.IsNullOrEmpty(cardExpirationYearDecrypted) && cardExpirationYearDecrypted != "0")
                    model.CardExpirationYear = cardExpirationYearDecrypted;

                model.AllowStoringCreditCardNumber = true;
            }
            else
            {
                string maskedCreditCardNumberDecrypted = _encryptionService.DecryptText(order.MaskedCreditCardNumber);
                if (!String.IsNullOrEmpty(maskedCreditCardNumberDecrypted))
                    model.CardNumber = maskedCreditCardNumberDecrypted;
            }


            //purchase order number (we have to find a better to inject this information because it's related to a certain plugin)
            var pm = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            if (pm != null && pm.PluginDescriptor.SystemName.Equals("Payments.PurchaseOrder", StringComparison.InvariantCultureIgnoreCase))
            {
                model.DisplayPurchaseOrderNumber = true;
                model.PurchaseOrderNumber = order.PurchaseOrderNumber;
            }

            //payment transaction info
            model.AuthorizationTransactionId = order.AuthorizationTransactionId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.SubscriptionTransactionId = order.SubscriptionTransactionId;

            //payment method info
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
            var recurringPayment = _orderService.SearchRecurringPayments(0, 0, order.Id, null, 0, int.MaxValue, true).FirstOrDefault();
            if (recurringPayment != null)
            {
                model.RecurringPaymentId = recurringPayment.Id;
            }
            #endregion

            #region Billing & shipping info

            model.BillingAddress = order.BillingAddress.ToModel();
            model.BillingAddress.FirstNameEnabled = true;
            model.BillingAddress.FirstNameRequired = true;
            model.BillingAddress.LastNameEnabled = true;
            model.BillingAddress.LastNameRequired = true;
            model.BillingAddress.EmailEnabled = true;
            model.BillingAddress.EmailRequired = true;
            model.BillingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.BillingAddress.CompanyRequired = _addressSettings.CompanyRequired;
            model.BillingAddress.CountryEnabled = _addressSettings.CountryEnabled;
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

            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext); ;
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;

                model.ShippingAddress = order.ShippingAddress.ToModel();
                model.ShippingAddress.FirstNameEnabled = true;
                model.ShippingAddress.FirstNameRequired = true;
                model.ShippingAddress.LastNameEnabled = true;
                model.ShippingAddress.LastNameRequired = true;
                model.ShippingAddress.EmailEnabled = true;
                model.ShippingAddress.EmailRequired = true;
                model.ShippingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
                model.ShippingAddress.CompanyRequired = _addressSettings.CompanyRequired;
                model.ShippingAddress.CountryEnabled = _addressSettings.CountryEnabled;
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

                model.ShippingMethod = order.ShippingMethod;

                model.ShippingAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}", Server.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "")));
                model.CanAddNewShipments = order.HasItemsToAddToShipment();
            }

            #endregion

            #region Products
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            bool hasDownloadableItems = false;
            var products = order.OrderProductVariants;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products
                    .Where(opv => opv.ProductVariant.Product.VendorId == _workContext.CurrentVendor.Id)
                    .ToList();
            }
            foreach (var opv in products)
            {
                if (opv.ProductVariant != null && opv.ProductVariant.IsDownload)
                    hasDownloadableItems = true;

                var opvModel = new OrderModel.OrderProductVariantModel()
                {
                    Id = opv.Id,
                    ProductVariantId = opv.ProductVariantId,
                    FullProductName = opv.ProductVariant.FullProductName,
                    Sku = opv.ProductVariant.FormatSku(opv.AttributesXml, _productAttributeParser),
                    Quantity = opv.Quantity,
                    IsDownload = opv.ProductVariant.IsDownload,
                    DownloadCount = opv.DownloadCount,
                    DownloadActivationType = opv.ProductVariant.DownloadActivationType,
                    IsDownloadActivated = opv.IsDownloadActivated,
                    LicenseDownloadId = opv.LicenseDownloadId
                };
                //vendor
                var vendor = _vendorService.GetVendorById(opv.ProductVariant.Product.VendorId);
                opvModel.VendorName = vendor != null ? vendor.Name : "";

                //unit price
                opvModel.UnitPriceInclTaxValue = opv.UnitPriceInclTax;
                opvModel.UnitPriceExclTaxValue = opv.UnitPriceExclTax;
                opvModel.UnitPriceInclTax = _priceFormatter.FormatPrice(opv.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                opvModel.UnitPriceExclTax = _priceFormatter.FormatPrice(opv.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //discounts
                opvModel.DiscountInclTaxValue = opv.DiscountAmountInclTax;
                opvModel.DiscountExclTaxValue = opv.DiscountAmountExclTax;
                opvModel.DiscountInclTax = _priceFormatter.FormatPrice(opv.DiscountAmountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                opvModel.DiscountExclTax = _priceFormatter.FormatPrice(opv.DiscountAmountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //subtotal
                opvModel.SubTotalInclTaxValue = opv.PriceInclTax;
                opvModel.SubTotalExclTaxValue = opv.PriceExclTax;
                opvModel.SubTotalInclTax = _priceFormatter.FormatPrice(opv.PriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                opvModel.SubTotalExclTax = _priceFormatter.FormatPrice(opv.PriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                opvModel.AttributeInfo = opv.AttributeDescription;
                if (opv.ProductVariant.IsRecurring)
                    opvModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"), opv.ProductVariant.RecurringCycleLength, opv.ProductVariant.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //return requests
                opvModel.ReturnRequestIds = _orderService.SearchReturnRequests(0, 0, opv.Id, null, 0, int.MaxValue)
                    .Select(rr => rr.Id).ToList();
                //gift cards
                opvModel.PurchasedGiftCardIds = _giftCardService.GetGiftCardsByPurchasedWithOrderProductVariantId(opv.Id)
                    .Select(gc => gc.Id).ToList();

                model.Items.Add(opvModel);
            }
            model.HasDownloadableProducts = hasDownloadableItems;
            #endregion
        }

        [NonAction]
        protected OrderModel.AddOrderProductModel.ProductDetailsModel PrepareAddProductToOrderModel(int orderId, int productVariantId)
        {

            var productVariant = _productService.GetProductVariantById(productVariantId);
            var model = new OrderModel.AddOrderProductModel.ProductDetailsModel()
            {
                ProductVariantId = productVariantId,
                OrderId = orderId,
                Name = productVariant.FullProductName,
                UnitPriceExclTax = decimal.Zero,
                UnitPriceInclTax = decimal.Zero,
                Quantity = 1,
                SubTotalExclTax = decimal.Zero,
                SubTotalInclTax = decimal.Zero
            };

            //attributes
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id);
            foreach (var attribute in productVariantAttributes)
            {
                var pvaModel = new OrderModel.AddOrderProductModel.ProductVariantAttributeModel()
                {
                    Id = attribute.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var pvaValues = _productAttributeService.GetProductVariantAttributeValues(attribute.Id);
                    foreach (var pvaValue in pvaValues)
                    {
                        var pvaValueModel = new OrderModel.AddOrderProductModel.ProductVariantAttributeValueModel()
                        {
                            Id = pvaValue.Id,
                            Name = pvaValue.Name,
                            IsPreSelected = pvaValue.IsPreSelected
                        };
                        pvaModel.Values.Add(pvaValueModel);
                    }
                }

                model.ProductVariantAttributes.Add(pvaModel);
            }
            //gift card
            model.GiftCard.IsGiftCard = productVariant.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = productVariant.GiftCardType;
            }
            return model;
        }

        [NonAction]
        protected ShipmentModel PrepareShipmentModel(Shipment shipment, bool prepareProducts)
        {
            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var model = new ShipmentModel()
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                TotalWeight = shipment.TotalWeight.HasValue ? string.Format("{0:F2} [{1}]", shipment.TotalWeight, baseWeightIn) : "",
                ShippedDate = shipment.ShippedDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet"),
                CanShip = !shipment.ShippedDateUtc.HasValue,
                DeliveryDate = shipment.DeliveryDateUtc.HasValue ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString() : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet"),
                CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue,
                DisplayPdfPackagingSlip = _pdfSettings.Enabled,
            };

            if (prepareProducts)
            {
                foreach (var sopv in shipment.ShipmentOrderProductVariants)
                {
                    var opv = _orderService.GetOrderProductVariantById(sopv.OrderProductVariantId);
                    if (opv == null)
                        continue;

                    //quantities
                    var qtyInThisShipment = sopv.Quantity;
                    var maxQtyToAdd = opv.GetTotalNumberOfItemsCanBeAddedToShipment();
                    var qtyOrdered = opv.Quantity;
                    var qtyInAllShipments = opv.GetTotalNumberOfItemsInAllShipment();

                    var sopvModel = new ShipmentModel.ShipmentOrderProductVariantModel()
                    {
                        Id = sopv.Id,
                        OrderProductVariantId = opv.Id,
                        ProductVariantId = opv.ProductVariantId,
                        FullProductName = opv.ProductVariant.FullProductName,
                        Sku = opv.ProductVariant.FormatSku(opv.AttributesXml, _productAttributeParser),
                        AttributeInfo = opv.AttributeDescription,
                        ItemWeight = opv.ItemWeight.HasValue ? string.Format("{0:F2} [{1}]", opv.ItemWeight, baseWeightIn) : "",
                        ItemDimensions = string.Format("{0:F2} x {1:F2} x {2:F2} [{3}]", opv.ProductVariant.Length, opv.ProductVariant.Width, opv.ProductVariant.Height, baseDimensionIn),
                        QuantityOrdered = qtyOrdered,
                        QuantityInThisShipment = qtyInThisShipment,
                        QuantityInAllShipments = qtyInAllShipments,
                        QuantityToAdd = maxQtyToAdd,
                    };

                    model.Products.Add(sopvModel);
                }
            }
            return model;
        }

        #endregion

        #region Order list

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //order statuses
            var model = new OrderListModel();
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //stores
            model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(0, int.MaxValue, true))
                model.AvailableVendors.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

            //a vendor should have access only to orders with his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            return View(model);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult OrderList(GridCommand command, OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;

            //load orders
            var orders = _orderService.SearchOrders(model.StoreId, model.VendorId, 0,
                startDateValue, endDateValue, orderStatus,
                paymentStatus, shippingStatus, model.CustomerEmail, model.OrderGuid,
                command.Page - 1, command.PageSize);
            var gridModel = new GridModel<OrderModel>
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new OrderModel()
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        CustomerEmail = x.BillingAddress.Email,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = orders.TotalCount
            };

            //summary report
            //implemented as a workaround described here: http://www.telerik.com/community/forums/aspnet-mvc/grid/gridmodel-aggregates-how-to-use.aspx
            var reportSummary = _orderReportService.GetOrderAverageReportLine(model.StoreId,
                model.VendorId, orderStatus,  paymentStatus, shippingStatus, 
                startDateValue, endDateValue, model.CustomerEmail);
            var profit = _orderReportService.ProfitReport(model.StoreId, 
                model.VendorId, orderStatus, paymentStatus, shippingStatus, 
                startDateValue, endDateValue, model.CustomerEmail);
		    var aggregator = new OrderModel()
		    {
                aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
		        aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumOrders, true, false)
		    };
		    gridModel.Aggregates = aggregator;
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public ActionResult GoToOrderId(OrderListModel model)
        {
            var order = _orderService.GetOrderById(model.GoDirectlyToNumber);
            if (order != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            else
                return List();
        }

        #endregion

        #region Export / Import

        public ActionResult ExportXmlAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                var orders = _orderService.SearchOrders(0, 0, 0, null, null, null,
                    null, null, null, null, 0, int.MaxValue);

                var xml = _exportManager.ExportOrdersToXml(orders);
                return new XmlDownloadResult(xml, "orders.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids));
            }

            var xml = _exportManager.ExportOrdersToXml(orders);
            return new XmlDownloadResult(xml, "orders.xml");
        }

	    public ActionResult ExportExcelAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                var orders = _orderService.SearchOrders(0, 0, 0, null, null, null,
                    null, null, null, null, 0, int.MaxValue);
                
                byte[] bytes = null;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportOrdersToXlsx(stream, orders);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "orders.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor cannot export orders
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids));
            }

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _exportManager.ExportOrdersToXlsx(stream, orders);
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "orders.xlsx");
        }

        #endregion

        #region Order details

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public ActionResult CancelOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.CancelOrder(order, true);
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
        public ActionResult CaptureOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Capture(order);
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
        public ActionResult MarkOrderAsPaid(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.MarkOrderAsPaid(order);
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
        public ActionResult RefundOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Refund(order);
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
        public ActionResult RefundOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.RefundOffline(order);
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
        public ActionResult VoidOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                var errors = _orderProcessingService.Void(order);
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
        public ActionResult VoidOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                _orderProcessingService.VoidOffline(order);
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
        
        public ActionResult PartiallyRefundOrderPopup(int id, bool online)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public ActionResult PartiallyRefundOrderPopup(string btnId, string formId, int id, bool online, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            try
            {
                decimal amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new NopException("Enter amount to refund");

                decimal maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = _orderProcessingService.PartiallyRefund(order, amountToRefund).ToList();
                else
                    _orderProcessingService.PartiallyRefundOffline(order, amountToRefund);

                if (errors.Count == 0)
                {
                    //success
                    ViewBag.RefreshPage = true;
                    ViewBag.btnId = btnId;
                    ViewBag.formId = formId;

                    PrepareOrderDetailsModel(model, order);
                    return View(model);
                }
                else
                {
                    //error
                    PrepareOrderDetailsModel(model, order);
                    foreach (var error in errors)
                        ErrorNotification(error, false);
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                //error
                PrepareOrderDetailsModel(model, order);
                ErrorNotification(exc, false);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            _orderProcessingService.DeleteOrder(order);
            return RedirectToAction("List");
        }

        public ActionResult PdfInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var order = _orderService.GetOrderById(orderId);
            var orders = new List<Order>();
            orders.Add(order);
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", string.Format("order_{0}.pdf", order.Id));
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public ActionResult EditCreditCardInfo(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            if (order.AllowStoringCreditCardNumber)
            {
                string cardType = model.CardType;
                string cardName = model.CardName;
                string cardNumber = model.CardNumber;
                string cardCvv2 = model.CardCvv2;
                string cardExpirationMonth = model.CardExpirationMonth;
                string cardExpirationYear = model.CardExpirationYear;

                order.CardType = _encryptionService.EncryptText(cardType);
                order.CardName = _encryptionService.EncryptText(cardName);
                order.CardNumber = _encryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                _orderService.UpdateOrder(order);
            }

            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public ActionResult EditOrderTotals(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
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

            PrepareOrderDetailsModel(model, order);
            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOpv")]
        [ValidateInput(false)]
        public ActionResult EditOrderProductVariant(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnSaveOpv", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnSaveOpv".Length));

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == opvId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");


            decimal unitPriceInclTax, unitPriceExclTax, discountInclTax, discountExclTax,priceInclTax,priceExclTax;
            int quantity;
            if (!decimal.TryParse(form["pvUnitPriceInclTax" + opvId], out unitPriceInclTax))
                unitPriceInclTax =orderProductVariant.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + opvId], out unitPriceExclTax))
                unitPriceExclTax = orderProductVariant.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + opvId], out quantity))
                quantity = orderProductVariant.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + opvId], out discountInclTax))
                discountInclTax = orderProductVariant.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + opvId], out discountExclTax))
                discountExclTax = orderProductVariant.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + opvId], out priceInclTax))
                priceInclTax = orderProductVariant.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + opvId], out priceExclTax))
                priceExclTax = orderProductVariant.PriceExclTax;

            if (quantity > 0)
            {
                orderProductVariant.UnitPriceInclTax = unitPriceInclTax;
                orderProductVariant.UnitPriceExclTax = unitPriceExclTax;
                orderProductVariant.Quantity = quantity;
                orderProductVariant.DiscountAmountInclTax = discountInclTax;
                orderProductVariant.DiscountAmountExclTax = discountExclTax;
                orderProductVariant.PriceInclTax = priceInclTax;
                orderProductVariant.PriceExclTax = priceExclTax;
                _orderService.UpdateOrder(order);
            }
            else
            {
                _orderService.DeleteOrderProductVariant(orderProductVariant);
            }

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOpv")]
        [ValidateInput(false)]
        public ActionResult DeleteOrderProductVariant(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = id });

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnDeleteOpv", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnDeleteOpv".Length));

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == opvId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            _orderService.DeleteOrderProductVariant(orderProductVariant);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        [ValidateInput(false)]
        public ActionResult ResetDownloadCount(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == opvId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderProductVariant(orderProductVariant))
                return RedirectToAction("List");

            orderProductVariant.DownloadCount = 0;
            _orderService.UpdateOrder(order);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        [ValidateInput(false)]
        public ActionResult ActivateDownloadOrderProductVariant(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == opvId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderProductVariant(orderProductVariant))
                return RedirectToAction("List");

            orderProductVariant.IsDownloadActivated = !orderProductVariant.IsDownloadActivated;
            _orderService.UpdateOrder(order);

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        public ActionResult UploadLicenseFilePopup(int id, int opvId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == opvId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");
            
            if (!orderProductVariant.ProductVariant.IsDownload)
                throw new ArgumentException("Product variant is not downloadable");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderProductVariant(orderProductVariant))
                return RedirectToAction("List");

            var model = new OrderModel.UploadLicenseModel()
            {
                LicenseDownloadId = orderProductVariant.LicenseDownloadId.HasValue ? orderProductVariant.LicenseDownloadId.Value : 0,
                OrderId = order.Id,
                OrderProductVariantId = orderProductVariant.Id
            };

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public ActionResult UploadLicenseFilePopup(string btnId, string formId, OrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == model.OrderProductVariantId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderProductVariant(orderProductVariant))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                orderProductVariant.LicenseDownloadId = model.LicenseDownloadId;
            else
                orderProductVariant.LicenseDownloadId = null;
            _orderService.UpdateOrder(order);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public ActionResult DeleteLicenseFilePopup(string btnId, string formId, OrderModel.UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            var orderProductVariant = order.OrderProductVariants.FirstOrDefault(x => x.Id == model.OrderProductVariantId);
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToOrderProductVariant(orderProductVariant))
                return RedirectToAction("List");

            //attach license
            orderProductVariant.LicenseDownloadId = null;
            _orderService.UpdateOrder(order);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }

        public ActionResult AddProductToOrder(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var model = new OrderModel.AddOrderProductModel();
            model.OrderId = orderId;
            //categories
            model.AvailableCategories.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var c in _categoryService.GetAllCategories(showHidden: true))
                model.AvailableCategories.Add(new SelectListItem() { Text = c.GetCategoryNameWithPrefix(_categoryService), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(true))
                model.AvailableManufacturers.Add(new SelectListItem() { Text = m.Name, Value = m.Id.ToString() });
            
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AddProductToOrder(GridCommand command, OrderModel.AddOrderProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            var gridModel = new GridModel();
            var productVariants = _productService.SearchProductVariants(model.SearchCategoryId,
                model.SearchManufacturerId, 0, model.SearchProductName, false,
                command.Page - 1, command.PageSize, true);
            gridModel.Data = productVariants.Select(x =>
            {
                var productVariantModel = new OrderModel.AddOrderProductModel.ProductVariantLineModel()
                {
                    Id = x.Id,
                    Name =  x.FullProductName,
                    Sku = x.Sku,
                };

                return productVariantModel;
            });
            gridModel.Total = productVariants.TotalCount;
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult AddProductToOrderDetails(int orderId, int productVariantId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var model = PrepareAddProductToOrderModel(orderId, productVariantId);
            return View(model);
        }

        [HttpPost]
        public ActionResult AddProductToOrderDetails(int orderId, int productVariantId, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var order = _orderService.GetOrderById(orderId);
            var productVariant = _productService.GetProductVariantById(productVariantId);
            //save order item

            //basic properties
            var unitPriceInclTax = decimal.Zero;
            decimal.TryParse(form["UnitPriceInclTax"], out unitPriceInclTax);
            var unitPriceExclTax = decimal.Zero;
            decimal.TryParse(form["UnitPriceExclTax"], out unitPriceExclTax);
            var quantity = 1;
            int.TryParse(form["Quantity"], out quantity);
            var priceInclTax = decimal.Zero;
            decimal.TryParse(form["SubTotalInclTax"], out priceInclTax);
            var priceExclTax = decimal.Zero;
            decimal.TryParse(form["SubTotalExclTax"], out priceExclTax);

            //attributes
            //warnings
            var warnings = new List<string>();
            string attributes = "";

            #region Product attributes
            string selectedAttributes = string.Empty;
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id);
            foreach (var attribute in productVariantAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}", attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
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
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var httpPostedFile = this.Request.Files[controlId];
                            if ((httpPostedFile != null) && (!String.IsNullOrEmpty(httpPostedFile.FileName)))
                            {
                                int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
                                if (httpPostedFile.ContentLength > fileMaxSize)
                                {
                                    warnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)));
                                }
                                else
                                {
                                    //save an uploaded file
                                    var download = new Download()
                                    {
                                        DownloadGuid = Guid.NewGuid(),
                                        UseDownloadUrl = false,
                                        DownloadUrl = "",
                                        DownloadBinary = httpPostedFile.GetDownloadBits(),
                                        ContentType = httpPostedFile.ContentType,
                                        Filename = System.IO.Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                        Extension = System.IO.Path.GetExtension(httpPostedFile.FileName),
                                        IsNew = true
                                    };
                                    _downloadService.InsertDownload(download);
                                    //save attribute
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, download.DownloadGuid.ToString());
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            attributes = selectedAttributes;

            #endregion

            #region Gift cards

            string recipientName = "";
            string recipientEmail = "";
            string senderName = "";
            string senderEmail = "";
            string giftCardMessage = "";
            if (productVariant.IsGiftCard)
            {
                foreach (string formKey in form.AllKeys)
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

                attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            //warnings
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(ShoppingCartType.ShoppingCart, productVariant, attributes));
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemGiftCardWarnings(ShoppingCartType.ShoppingCart, productVariant, attributes));
            if (warnings.Count == 0)
            {
                //no errors

                //attributes
                string attributeDescription = _productAttributeFormatter.FormatAttributes(productVariant, attributes, order.Customer);

                //save item
                var opv = new OrderProductVariant()
                {
                    OrderProductVariantGuid = Guid.NewGuid(),
                    Order = order,
                    ProductVariantId = productVariant.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributes,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0
                };
                order.OrderProductVariants.Add(opv);
                _orderService.UpdateOrder(order);

                //gift cards
                if (productVariant.IsGiftCard)
                {
                    for (int i = 0; i < opv.Quantity; i++)
                    {
                        var gc = new GiftCard()
                        {
                            GiftCardType = productVariant.GiftCardType,
                            PurchasedWithOrderProductVariant = opv,
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
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }
            else
            {
                //errors
                var model = PrepareAddProductToOrderModel(order.Id, productVariant.Id);
                model.Warnings.AddRange(warnings);
                return View(model);
            }
        }

        #endregion

        #endregion

        #region Addresses

        public ActionResult AddressEdit(int addressId, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new OrderAddressModel();
            model.OrderId = orderId;
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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(OrderAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                throw new ArgumentException("No address found with the specified id");

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        #endregion
        
        #region Shipments

        public ActionResult ShipmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new ShipmentListModel();
            model.DisplayPdfPackagingSlip = _pdfSettings.Enabled;
            return View(model);
		}

		[GridAction(EnableCustomBinding = true)]
        public ActionResult ShipmentListSelect(GridCommand command, ShipmentListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId, startDateValue, endDateValue, 
                command.Page - 1, command.PageSize);
            var gridModel = new GridModel<ShipmentModel>
            {
                Data = shipments.Select(shipment => PrepareShipmentModel(shipment, false)),
                Total = shipments.TotalCount
            };
			return new JsonResult
			{
				Data = gridModel
			};
		}
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult ShipmentsByOrder(int orderId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

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

            var model = new GridModel<ShipmentModel>
            {
                Data = shipmentModels,
                Total = shipmentModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        public ActionResult AddShipment(int orderId)
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

            var model = new ShipmentModel()
            {
                OrderId = order.Id,
            };

            //measures
            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
            var baseWeightIn = baseWeight != null ? baseWeight.Name : "";
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId);
            var baseDimensionIn = baseDimension != null ? baseDimension.Name : "";

            var orderProductVariants = order.OrderProductVariants;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderProductVariants = orderProductVariants.Where(opv => HasAccessToOrderProductVariant(opv)).ToList();
            }

            foreach (var opv in orderProductVariants)
            {
                //we can ship only shippable products
                if (!opv.ProductVariant.IsShipEnabled)
                    continue;

                //quantities
                var qtyInThisShipment = 0;
                var maxQtyToAdd = opv.GetTotalNumberOfItemsCanBeAddedToShipment();
                var qtyOrdered = opv.Quantity;
                var qtyInAllShipments = opv.GetTotalNumberOfItemsInAllShipment();

                //ensure that this product variant can be added to a shipment
                if (maxQtyToAdd <= 0)
                    continue;

                var sopvModel = new ShipmentModel.ShipmentOrderProductVariantModel()
                {
                    OrderProductVariantId = opv.Id,
                    ProductVariantId = opv.ProductVariantId,
                    FullProductName = opv.ProductVariant.FullProductName,
                    Sku = opv.ProductVariant.FormatSku(opv.AttributesXml, _productAttributeParser),
                    AttributeInfo = opv.AttributeDescription,
                    ItemWeight = opv.ItemWeight.HasValue ? string.Format("{0:F2} [{1}]", opv.ItemWeight, baseWeightIn) : "",
                    ItemDimensions = string.Format("{0:F2} x {1:F2} x {2:F2} [{3}]", opv.ProductVariant.Length, opv.ProductVariant.Width, opv.ProductVariant.Height, baseDimensionIn),
                    QuantityOrdered = qtyOrdered,
                    QuantityInThisShipment = qtyInThisShipment,
                    QuantityInAllShipments = qtyInAllShipments,
                    QuantityToAdd = maxQtyToAdd,
                };

                model.Products.Add(sopvModel);
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult AddShipment(int orderId, FormCollection form, bool continueEditing)
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

            var orderProductVariants = order.OrderProductVariants;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderProductVariants = orderProductVariants.Where(opv => HasAccessToOrderProductVariant(opv)).ToList();
            }

            Shipment shipment = null;
            decimal? totalWeight = null;
            foreach (var opv in orderProductVariants)
            {
                //is shippable
                if (!opv.ProductVariant.IsShipEnabled)
                    continue;
                
                //ensure that this product variant can be shipped (have at least one item to ship)
                var maxQtyToAdd = opv.GetTotalNumberOfItemsCanBeAddedToShipment();
                if (maxQtyToAdd <= 0)
                    continue;

                int qtyToAdd = 0; //parse quantity
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("qtyToAdd{0}", opv.Id), StringComparison.InvariantCultureIgnoreCase))
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

                var opvTotalWeight = opv.ItemWeight.HasValue ? opv.ItemWeight * qtyToAdd : null;
                if (opvTotalWeight.HasValue)
                {
                    if (!totalWeight.HasValue)
                        totalWeight = 0;
                    totalWeight += opvTotalWeight.Value;
                }
                if (shipment == null)
                {
                    var trackingNumber = form["TrackingNumber"];
                    shipment = new Shipment()
                    {
                        OrderId = order.Id,
                        TrackingNumber = trackingNumber,
                        TotalWeight = null,
                        ShippedDateUtc = null,
                        DeliveryDateUtc = null,
                        CreatedOnUtc = DateTime.UtcNow,
                    };
                }
                //create a shipment order product variant
                var sopv = new ShipmentOrderProductVariant()
                {
                    OrderProductVariantId = opv.Id,
                    Quantity = qtyToAdd,
                };
                shipment.ShipmentOrderProductVariants.Add(sopv);
            }

            //if we have at least one item in the shipment, then save it
            if (shipment != null && shipment.ShipmentOrderProductVariants.Count > 0)
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);

                SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
                return continueEditing
                           ? RedirectToAction("ShipmentDetails", new {id = shipment.Id})
                           : RedirectToAction("Edit", new { id = orderId });
            }
            else
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoProductsSelected"));
                return RedirectToAction("AddShipment", new { orderId = orderId });
            }
        }

        public ActionResult ShipmentDetails(int id)
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

            var model = PrepareShipmentModel(shipment, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteShipment(int id)
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

            var orderId = shipment.OrderId;
            _shipmentService.DeleteShipment(shipment);

            SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Deleted"));
            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public ActionResult SetTrackingNumber(ShipmentModel model)
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
        [FormValueRequired("setasshipped")]
        public ActionResult SetAsShipped(int id)
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
        public ActionResult SetAsDelivered(int id)
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
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                ErrorNotification(exc, true);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }
        
        public ActionResult PdfPackagingSlip(int shipmentId)
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
            
            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", string.Format("packagingslip_{0}.pdf", shipment.Id));
        }

        public ActionResult PdfPackagingSlipAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            var shipments = _shipmentService.GetAllShipments(vendorId, null, null, 0, int.MaxValue).ToList();

            //ensure that we at least one shipment selected
            if (shipments.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.PrintPackagingSlip.NoShipments"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "packagingslips.pdf");
        }

        public ActionResult PdfPackagingSlipSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                shipments.AddRange(_shipmentService.GetShipmentsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(s => HasAccessToShipment(s)).ToList();
            }

            //ensure that we at least one shipment selected
            if (shipments.Count == 0)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.PrintPackagingSlip.NoShipments"));
                return RedirectToAction("ShipmentList");
            }

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }
            return File(bytes, "application/pdf", "packagingslips.pdf");
        }

        #endregion

        #region Order notes
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult OrderNotesSelect(int orderId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return Content("");

            //order notes
            var orderNoteModels = new List<OrderModel.OrderNote>();
            foreach (var orderNote in order.OrderNotes
                .OrderByDescending(on => on.CreatedOnUtc))
            {
                orderNoteModels.Add(new OrderModel.OrderNote()
                {
                    Id = orderNote.Id,
                    OrderId = orderNote.OrderId,
                    DisplayToCustomer = orderNote.DisplayToCustomer,
                    Note = orderNote.FormatOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var model = new GridModel<OrderModel.OrderNote>
            {
                Data = orderNoteModels,
                Total = orderNoteModels.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }
        
        [ValidateInput(false)]
        public ActionResult OrderNoteAdd(int orderId, bool displayToCustomer, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var orderNote = new OrderNote()
            {
                DisplayToCustomer = displayToCustomer,
                Note = message,
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

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        
        [GridAction(EnableCustomBinding = true)]
        public ActionResult OrderNoteDelete(int orderId, int orderNoteId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            //a vendor does not have to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            var orderNote = order.OrderNotes.FirstOrDefault(on => on.Id == orderNoteId);
            if (orderNote == null)
                throw new ArgumentException("No order note found with the specified id");
            _orderService.DeleteOrderNote(orderNote);

            return OrderNotesSelect(orderId, command);
        }

        #endregion

        #region Reports

        [NonAction]
        protected GridModel<BestsellersReportLineModel> GetBestsellersBriefReportModel(int pageIndex,
            int pageSize, int orderBy)
        {
            //a vendor should have access only to his products
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //group by product variants (not products)
            var items = _orderReportService.BestSellersReport(
                vendorId : vendorId,
                orderBy: orderBy,
                pageIndex: pageIndex,
                pageSize: pageSize,
                showHidden: true);
            var gridModel = new GridModel<BestsellersReportLineModel>
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel()
                    {
                        ProductVariantId = x.EntityId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var productVariant = _productService.GetProductVariantById(x.EntityId);
                    if (productVariant != null)
                        m.ProductVariantFullName = productVariant.FullProductName;
                    return m;
                }),
                Total = items.TotalCount
            };
            return gridModel;
        }
        public ActionResult BestsellersBriefReportByQuantity()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            return PartialView();
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersBriefReportByQuantityList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 1);
            return new JsonResult
            {
                Data = gridModel
            };
        }
        public ActionResult BestsellersBriefReportByAmount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            return PartialView();
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersBriefReportByAmountList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            var gridModel = GetBestsellersBriefReportModel(command.Page - 1,
                command.PageSize, 2);
            return new JsonResult
            {
                Data = gridModel
            };
        }



        public ActionResult BestsellersReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new BestsellersReportModel();

            //order statuses
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling())
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });

            //vendor
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            return View(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersReportList(GridCommand command, BestsellersReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor should have access only to his products
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;
            
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;

            //group by product variants (not products)
            var items = _orderReportService.BestSellersReport(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                os: orderStatus,
                ps: paymentStatus,
                billingCountryId: model.BillingCountryId,
                orderBy: 2,
                vendorId: vendorId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            var gridModel = new GridModel<BestsellersReportLineModel>
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel()
                    {
                        ProductVariantId = x.EntityId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var productVariant = _productService.GetProductVariantById(x.EntityId);
                    if (productVariant != null)
                        m.ProductVariantFullName = productVariant.FullProductName;
                    return m;
                }),
                Total = items.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }



        public ActionResult NeverSoldReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var model = new NeverSoldReportModel();
            return View(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult NeverSoldReportList(GridCommand command, NeverSoldReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor should have access only to his products
            int vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            
            var items = _orderReportService.ProductsNeverSold(vendorId,
                startDateValue, endDateValue,
                command.Page - 1, command.PageSize, true);
            var gridModel = new GridModel<NeverSoldReportLineModel>
            {
                Data = items.Select(x =>
                    new NeverSoldReportLineModel()
                    {
                        ProductVariantId = x.Id,
                        ProductVariantFullName = x.Product.Name + " " + x.Name,
                    }),
                Total = items.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }


        [NonAction]
        protected virtual IList<OrderAverageReportLineSummaryModel> GetOrderAverageReportModel()
        {
            var report = new List<OrderAverageReportLineSummary>();
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Pending));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Processing));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Complete));
            report.Add(_orderReportService.OrderAverageReport(0, OrderStatus.Cancelled));
            var model = report.Select(x =>
            {
                return new OrderAverageReportLineSummaryModel()
                {
                    OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    SumTodayOrders = _priceFormatter.FormatPrice(x.SumTodayOrders, true, false),
                    SumThisWeekOrders = _priceFormatter.FormatPrice(x.SumThisWeekOrders, true, false),
                    SumThisMonthOrders = _priceFormatter.FormatPrice(x.SumThisMonthOrders, true, false),
                    SumThisYearOrders = _priceFormatter.FormatPrice(x.SumThisYearOrders, true, false),
                    SumAllTimeOrders = _priceFormatter.FormatPrice(x.SumAllTimeOrders, true, false),
                };
            }).ToList();

            return model;
        }
        [ChildActionOnly]
        public ActionResult OrderAverageReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            var model = GetOrderAverageReportModel();
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult OrderAverageReportList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor does have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = GetOrderAverageReportModel();
            var gridModel = new GridModel<OrderAverageReportLineSummaryModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected virtual IList<OrderIncompleteReportLineModel> GetOrderIncompleteReportModel()
        {
            var model = new List<OrderIncompleteReportLineModel>();
            //not paid
            var psPending = _orderReportService.GetOrderAverageReportLine(0, 0, null, PaymentStatus.Pending, null, null, null, null, true);
            model.Add(new OrderIncompleteReportLineModel()
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
                Count = psPending.CountOrders,
                Total = _priceFormatter.FormatPrice(psPending.SumOrders, true, false)
            });
            //not shipped
            var ssPending = _orderReportService.GetOrderAverageReportLine(0, 0, null, null, ShippingStatus.NotYetShipped, null, null, null, true);
            model.Add(new OrderIncompleteReportLineModel()
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
                Count = ssPending.CountOrders,
                Total = _priceFormatter.FormatPrice(ssPending.SumOrders, true, false)
            });
            //pending
            var osPending = _orderReportService.GetOrderAverageReportLine(0, 0, OrderStatus.Pending, null, null, null, null, null, true);
            model.Add(new OrderIncompleteReportLineModel()
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalIncompleteOrders"),
                Count = osPending.CountOrders,
                Total = _priceFormatter.FormatPrice(osPending.SumOrders, true, false)
            });
            return model;
        }
        [ChildActionOnly]
        public ActionResult OrderIncompleteReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");

            var model = GetOrderIncompleteReportModel();
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult OrderIncompleteReportList(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content("");
            
            //a vendor does have access to this report
            if (_workContext.CurrentVendor != null)
                return Content("");

            var model = GetOrderIncompleteReportModel();
            var gridModel = new GridModel<OrderIncompleteReportLineModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        #endregion
    }
}
