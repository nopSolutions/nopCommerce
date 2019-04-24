using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
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

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the order model factory implementation
    /// </summary>
    public partial class OrderModelFactory : IOrderModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OrderModelFactory(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAffiliateService affiliateService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICurrencyService currencyService,
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
            IShipmentService shipmentService,
            IShippingService shippingService,
            IStoreService storeService,
            ITaxService taxService,
            IUrlHelperFactory urlHelperFactory,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            IUrlRecordService urlRecordService,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _affiliateService = affiliateService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _currencyService = currencyService;
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
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _storeService = storeService;
            _taxService = taxService;
            _urlHelperFactory = urlHelperFactory;
            _vendorService = vendorService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _orderSettings = orderSettings;
            _shippingSettings = shippingSettings;
            _urlRecordService = urlRecordService;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(address.CustomAttributes);

            //set some of address fields as enabled and required
            model.FirstNameEnabled = true;
            model.FirstNameRequired = true;
            model.LastNameEnabled = true;
            model.LastNameRequired = true;
            model.EmailEnabled = true;
            model.EmailRequired = true;
            model.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.CompanyRequired = _addressSettings.CompanyRequired;
            model.CountryEnabled = _addressSettings.CountryEnabled;
            model.CountryRequired = _addressSettings.CountryEnabled;
            model.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.CountyEnabled = _addressSettings.CountyEnabled;
            model.CountyRequired = _addressSettings.CountyRequired;
            model.CityEnabled = _addressSettings.CityEnabled;
            model.CityRequired = _addressSettings.CityRequired;
            model.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.PhoneRequired = _addressSettings.PhoneRequired;
            model.FaxEnabled = _addressSettings.FaxEnabled;
            model.FaxRequired = _addressSettings.FaxRequired;
        }

        /// <summary>
        /// Prepare order item models
        /// </summary>
        /// <param name="models">List of order item models</param>
        /// <param name="order">Order</param>
        protected virtual void PrepareOrderItemModels(IList<OrderItemModel> models, Order order)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            //get order items
            var orderItems = order.OrderItems;
            if (_workContext.CurrentVendor != null)
                orderItems = orderItems.Where(orderItem => orderItem.Product.VendorId == _workContext.CurrentVendor.Id).ToList();

            foreach (var orderItem in orderItems)
            {
                //fill in model values from the entity
                var orderItemModel = new OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Quantity = orderItem.Quantity,
                    IsDownload = orderItem.Product.IsDownload,
                    DownloadCount = orderItem.DownloadCount,
                    DownloadActivationType = orderItem.Product.DownloadActivationType,
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
                orderItemModel.Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml);
                orderItemModel.VendorName = _vendorService.GetVendorById(orderItem.Product.VendorId)?.Name;

                //picture
                var orderItemPicture = _pictureService.GetProductPicture(orderItem.Product, orderItem.AttributesXml);
                orderItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(orderItemPicture, 75);

                //license file
                if (orderItem.LicenseDownloadId.HasValue)
                {
                    orderItemModel.LicenseDownloadGuid = _downloadService
                        .GetDownloadById(orderItem.LicenseDownloadId.Value)?.DownloadGuid ?? Guid.Empty;
                }

                //unit price
                orderItemModel.UnitPriceInclTax = _priceFormatter
                    .FormatPrice(orderItem.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.UnitPriceExclTax = _priceFormatter
                    .FormatPrice(orderItem.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //discounts
                orderItemModel.DiscountInclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountInclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.DiscountExclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountExclTax, true,
                    primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                //subtotal
                orderItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(orderItem.PriceInclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, true, true);
                orderItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(orderItem.PriceExclTax, true, primaryStoreCurrency,
                    _workContext.WorkingLanguage, false, true);

                //recurring info
                if (orderItem.Product.IsRecurring)
                {
                    orderItemModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"),
                        orderItem.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(orderItem.Product.RecurringCyclePeriod));
                }

                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //prepare return request models
                PrepareReturnRequestBriefModels(orderItemModel.ReturnRequests, orderItem);

                //gift card identifiers
                orderItemModel.PurchasedGiftCardIds = _giftCardService
                    .GetGiftCardsByPurchasedWithOrderItemId(orderItem.Id).Select(card => card.Id).ToList();

                models.Add(orderItemModel);
            }
        }

        /// <summary>
        /// Prepare return request brief models
        /// </summary>
        /// <param name="models">List of return request brief models</param>
        /// <param name="orderItem">Order item</param>
        protected virtual void PrepareReturnRequestBriefModels(IList<OrderItemModel.ReturnRequestBriefModel> models, OrderItem orderItem)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var returnRequests = _returnRequestService.SearchReturnRequests(orderItemId: orderItem.Id);
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
        protected virtual void PrepareOrderModelTotals(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

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
            var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
            var displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
            var displayTax = !displayTaxRates;
            foreach (var tr in taxRates)
            {
                model.TaxRates.Add(new OrderModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = _priceFormatter.FormatPrice(tr.Value, true, false)
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
                    Amount = _priceFormatter.FormatPrice(-gcuh.UsedValue, true, false)
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
            if (_workContext.CurrentVendor != null)
                return;

            var profit = _orderReportService.ProfitReport(orderId: order.Id);
            model.Profit = _priceFormatter.FormatPrice(profit, true, false);
        }

        /// <summary>
        /// Prepare order model payment info
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        protected virtual void PrepareOrderModelPaymentInfo(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //prepare billing address
            model.BillingAddress = order.BillingAddress.ToModel(model.BillingAddress);
            PrepareAddressModel(model.BillingAddress, order.BillingAddress);

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
            var pm = _paymentPluginManager.LoadPluginBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
            model.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);

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

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId)?.CurrencyCode;
            model.MaxAmountToRefund = order.OrderTotal - order.RefundedAmount;

            //recurring payment record
            model.RecurringPaymentId = _orderService.SearchRecurringPayments(initialOrderId: order.Id, showHidden: true).FirstOrDefault()?.Id ?? 0;
        }

        /// <summary>
        /// Prepare order model shipping info
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        protected virtual void PrepareOrderModelShippingInfo(OrderModel model, Order order)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            model.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
            if (order.ShippingStatus == ShippingStatus.ShippingNotRequired)
                return;

            model.IsShippable = true;
            model.ShippingMethod = order.ShippingMethod;
            model.CanAddNewShipments = _orderService.HasItemsToAddToShipment(order);
            model.PickupInStore = order.PickupInStore;
            if (!order.PickupInStore)
            {
                model.ShippingAddress = order.ShippingAddress.ToModel(model.ShippingAddress);
                PrepareAddressModel(model.ShippingAddress, order.ShippingAddress);
                model.ShippingAddressGoogleMapsUrl = $"https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={WebUtility.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : ""))}";
            }
            else
            {
                if (order.PickupAddress == null)
                    return;

                model.PickupAddress = order.PickupAddress.ToModel(model.PickupAddress);
                model.PickupAddressGoogleMapsUrl = $"https://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={WebUtility.UrlEncode($"{order.PickupAddress.Address1} {order.PickupAddress.ZipPostalCode} {order.PickupAddress.City} {(order.PickupAddress.Country != null ? order.PickupAddress.Country.Name : string.Empty)}")}";
            }
        }

        /// <summary>
        /// Prepare product attribute models
        /// </summary>
        /// <param name="models">List of product attribute models</param>
        /// <param name="order">Order</param>
        /// <param name="product">Product</param>
        protected virtual void PrepareProductAttributeModels(IList<AddProductToOrderModel.ProductAttributeModel> models, Order order, Product product)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddProductToOrderModel.ProductAttributeModel
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
                            _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue, order.Customer), out _);

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
                                priceAdjustmentStr = priceAdjustment > 0 ? $"+{_priceFormatter.FormatPrice(priceAdjustment, false, false)}" : $"-{_priceFormatter.FormatPrice(-priceAdjustment, false, false)}";
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
        protected virtual void PrepareShipmentItemModel(ShipmentItemModel model, OrderItem orderItem)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //fill in additional values (not existing in the entity)
            model.OrderItemId = orderItem.Id;
            model.ProductId = orderItem.ProductId;
            model.ProductName = orderItem.Product.Name;
            model.Sku = _productService.FormatSku(orderItem.Product, orderItem.AttributesXml);
            model.AttributeInfo = orderItem.AttributeDescription;
            model.ShipSeparately = orderItem.Product.ShipSeparately;
            model.QuantityOrdered = orderItem.Quantity;
            model.QuantityInAllShipments = _orderService.GetTotalNumberOfItemsInAllShipment(orderItem);
            model.QuantityToAdd = _orderService.GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);

            var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name;
            var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId)?.Name;
            if (orderItem.ItemWeight.HasValue)
                model.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";
            model.ItemDimensions =
                $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimension}]";

            if (!orderItem.Product.IsRental)
                return;

            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalStartDateUtc.Value) : string.Empty;
            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                ? _productService.FormatRentalDate(orderItem.Product, orderItem.RentalEndDateUtc.Value) : string.Empty;
            model.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"), rentalStartDate, rentalEndDate);
        }

        /// <summary>
        /// Prepare shipment status event models
        /// </summary>
        /// <param name="models">List of shipment status event models</param>
        /// <param name="shipment">Shipment</param>
        protected virtual void PrepareShipmentStatusEventModels(IList<ShipmentStatusEventModel> models, Shipment shipment)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            var shipmentTracker = _shipmentService.GetShipmentTracker(shipment);
            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
            if (shipmentEvents == null)
                return;

            foreach (var shipmentEvent in shipmentEvents)
            {
                var shipmentStatusEventModel = new ShipmentStatusEventModel
                {
                    Date = shipmentEvent.Date,
                    EventName = shipmentEvent.EventName,
                    Location = shipmentEvent.Location
                };
                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                shipmentStatusEventModel.Country = shipmentEventCountry != null
                    ? _localizationService.GetLocalized(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

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
        /// <returns>Order search model</returns>
        public virtual OrderSearchModel PrepareOrderSearchModel(OrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;

            //prepare available order, payment and shipping statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            _baseAdminModelFactory.PrepareShippingStatuses(searchModel.AvailableShippingStatuses);
            if (searchModel.AvailableShippingStatuses.Any())
            {
                if (searchModel.ShippingStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                    searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available warehouses
            _baseAdminModelFactory.PrepareWarehouses(searchModel.AvailableWarehouses);

            //prepare available payment methods
            searchModel.AvailablePaymentMethods = _paymentPluginManager.LoadAllPlugins().Select(method =>
                new SelectListItem { Text = method.PluginDescriptor.FriendlyName, Value = method.PluginDescriptor.SystemName }).ToList();
            searchModel.AvailablePaymentMethods.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = string.Empty });

            //prepare available billing countries
            searchModel.AvailableCountries = _countryService.GetAllCountriesForBilling(showHidden: true)
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //prepare grid
            searchModel.SetGridPageSize();

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order list model</returns>
        public virtual OrderListModel PrepareOrderListModel(OrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            if (_workContext.CurrentVendor != null)
                searchModel.VendorId = _workContext.CurrentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var product = _productService.GetProductById(searchModel.ProductId);
            var filterByProductId = product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id)
                ? searchModel.ProductId : 0;

            //get orders
            var orders = _orderService.SearchOrders(storeId: searchModel.StoreId,
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
            var model = new OrderListModel().PrepareToGrid(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.Select(order =>
                {
                    //fill in model values from the entity
                    var orderModel = new OrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        ShippingStatusId = order.ShippingStatusId,
                        CustomerEmail = order.BillingAddress.Email,
                        CustomerFullName = $"{order.BillingAddress.FirstName} {order.BillingAddress.LastName}",
                        CustomerId = order.CustomerId,
                        CustomOrderNumber = order.CustomOrderNumber
                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Deleted";
                    orderModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    orderModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    orderModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);

                    return orderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare order aggregator model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>Order aggregator model</returns>
        public virtual OrderAggreratorModel PrepareOrderAggregatorModel(OrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            if (_workContext.CurrentVendor != null)
                searchModel.VendorId = _workContext.CurrentVendor.Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var product = _productService.GetProductById(searchModel.ProductId);
            var filterByProductId = product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id)
                ? searchModel.ProductId : 0;

            //prepare additional model data
            var reportSummary = _orderReportService.GetOrderAverageReportLine(storeId: searchModel.StoreId,
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

            var profit = _orderReportService.ProfitReport(storeId: searchModel.StoreId,
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

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            var shippingSum = _priceFormatter
                .FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            var taxSum = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false);
            var totalSum = _priceFormatter.FormatPrice(reportSummary.SumOrders, true, false);
            var profitSum = _priceFormatter.FormatPrice(profit, true, false);

            var model = new OrderAggreratorModel
            {
                aggregatorprofit = profitSum,
                aggregatorshipping = shippingSum,
                aggregatortax = taxSum,
                aggregatortotal = totalSum
            };

            return model;
        }

        /// <summary>
        /// Prepare order model
        /// </summary>
        /// <param name="model">Order model</param>
        /// <param name="order">Order</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Order model</returns>
        public virtual OrderModel PrepareOrderModel(OrderModel model, Order order, bool excludeProperties = false)
        {
            if (order != null)
            {
                //fill in model values from the entity
                model = model ?? new OrderModel
                {
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                    CustomerId = order.CustomerId,
                    VatNumber = order.VatNumber,
                    CheckoutAttributeInfo = order.CheckoutAttributeDescription
                };

                model.OrderGuid = order.OrderGuid;
                model.CustomOrderNumber = order.CustomOrderNumber;
                model.CustomerIp = order.CustomerIp;
                model.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                model.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Deleted";
                model.CustomerInfo = order.Customer.IsRegistered() ? order.Customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
                model.CustomValues = _paymentService.DeserializeCustomValues(order);

                var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);
                if (affiliate != null)
                {
                    model.AffiliateId = affiliate.Id;
                    model.AffiliateName = _affiliateService.GetAffiliateFullName(affiliate);
                }

                //prepare order totals
                PrepareOrderModelTotals(model, order);

                //prepare order items
                PrepareOrderItemModels(model.Items, order);
                model.HasDownloadableProducts = model.Items.Any(item => item.IsDownload);

                //prepare payment info
                PrepareOrderModelPaymentInfo(model, order);

                //prepare shipping info
                PrepareOrderModelShippingInfo(model, order);

                //prepare nested search model
                PrepareOrderShipmentSearchModel(model.OrderShipmentSearchModel, order);
                PrepareOrderNoteSearchModel(model.OrderNoteSearchModel, order);
            }

            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
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
        /// <returns>Upload license model</returns>
        public virtual UploadLicenseModel PrepareUploadLicenseModel(UploadLicenseModel model, Order order, OrderItem orderItem)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            model.LicenseDownloadId = orderItem.LicenseDownloadId ?? 0;
            model.OrderId = order.Id;
            model.OrderItemId = orderItem.Id;

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the order
        /// </summary>
        /// <param name="searchModel">Product search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>Product search model to add to the order</returns>
        public virtual AddProductToOrderSearchModel PrepareAddProductToOrderSearchModel(AddProductToOrderSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            searchModel.OrderId = order.Id;

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model to add to the order
        /// </summary>
        /// <param name="searchModel">Product search model to add to the order</param>
        /// <param name="order">Order</param>
        /// <returns>Product search model to add to the order</returns>
        public virtual AddProductToOrderListModel PrepareAddProductToOrderListModel(AddProductToOrderSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new AddProductToOrderListModel().PrepareToGrid(searchModel, products, () =>
            {
                //fill in model values from the entity
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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
        /// <returns>Product model to add to the order</returns>
        public virtual AddProductToOrderModel PrepareAddProductToOrderModel(AddProductToOrderModel model, Order order, Product product)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;
            model.OrderId = order.Id;
            model.Name = product.Name;
            model.IsRental = product.IsRental;
            model.ProductType = product.ProductType;
            model.AutoUpdateOrderTotals = _orderSettings.AutoUpdateOrderTotalsOnEditingOrder;

            var presetQty = 1;
            var presetPrice = _priceCalculationService.GetFinalPrice(product, order.Customer, decimal.Zero, true, presetQty);
            var presetPriceInclTax = _taxService.GetProductPrice(product, presetPrice, true, order.Customer, out _);
            var presetPriceExclTax = _taxService.GetProductPrice(product, presetPrice, false, order.Customer, out _);
            model.UnitPriceExclTax = presetPriceExclTax;
            model.UnitPriceInclTax = presetPriceInclTax;
            model.Quantity = presetQty;
            model.SubTotalExclTax = presetPriceExclTax;
            model.SubTotalInclTax = presetPriceInclTax;

            //attributes
            PrepareProductAttributeModels(model.ProductAttributes, order, product);
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
        /// <returns>Order address model</returns>
        public virtual OrderAddressModel PrepareOrderAddressModel(OrderAddressModel model, Order order, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (address == null)
                throw new ArgumentNullException(nameof(address));

            model.OrderId = order.Id;

            //prepare address model
            model.Address = address.ToModel(model.Address);
            PrepareAddressModel(model.Address, address);

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.Address.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.Address.AvailableStates, model.Address.CountryId);

            //prepare custom address attributes
            _addressAttributeModelFactory.PrepareCustomAddressAttributes(model.Address.CustomAddressAttributes, address);

            return model;
        }

        /// <summary>
        /// Prepare shipment search model
        /// </summary>
        /// <param name="searchModel">Shipment search model</param>
        /// <returns>Shipment search model</returns>
        public virtual ShipmentSearchModel PrepareShipmentSearchModel(ShipmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(searchModel.AvailableCountries);

            //prepare available states and provinces
            _baseAdminModelFactory.PrepareStatesAndProvinces(searchModel.AvailableStates, searchModel.CountryId);

            //prepare available warehouses
            _baseAdminModelFactory.PrepareWarehouses(searchModel.AvailableWarehouses);

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
        /// <returns>Shipment list model</returns>
        public virtual ShipmentListModel PrepareShipmentListModel(ShipmentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter shipments
            var vendorId = _workContext.CurrentVendor?.Id ?? 0;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get shipments
            var shipments = _shipmentService.GetAllShipments(vendorId,
                searchModel.WarehouseId,
                searchModel.CountryId,
                searchModel.StateProvinceId,
                searchModel.County,
                searchModel.City,
                searchModel.TrackingNumber,
                searchModel.LoadNotShipped,
                startDateValue,
                endDateValue,
                searchModel.Page - 1,
                searchModel.PageSize);

            //prepare list model
            var model = new ShipmentListModel().PrepareToGrid(searchModel, shipments, () =>
            {
                //fill in model values from the entity
                return shipments.Select(shipment =>
                {
                    //fill in model values from the entity
                    var shipmentModel = shipment.ToModel<ShipmentModel>();

                    //convert dates to the user time
                    shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString()
                        : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet");
                    shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString()
                        : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet");

                    //fill in additional values (not existing in the entity)
                    shipmentModel.CanShip = !shipment.ShippedDateUtc.HasValue;
                    shipmentModel.CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue;
                    shipmentModel.CustomOrderNumber = shipment.Order.CustomOrderNumber;

                    if (shipment.TotalWeight.HasValue)
                        shipmentModel.TotalWeight = $"{shipment.TotalWeight:F2} [{_measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name}]";

                    return shipmentModel;
                });
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
        /// <returns>Shipment model</returns>
        public virtual ShipmentModel PrepareShipmentModel(ShipmentModel model, Shipment shipment, Order order,
            bool excludeProperties = false)
        {
            if (shipment != null)
            {
                //fill in model values from the entity
                model = model ?? shipment.ToModel<ShipmentModel>();

                model.CanShip = !shipment.ShippedDateUtc.HasValue;
                model.CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue;
                model.CustomOrderNumber = shipment.Order.CustomOrderNumber;

                model.ShippedDate = shipment.ShippedDateUtc.HasValue
                    ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString()
                    : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet");
                model.DeliveryDate = shipment.DeliveryDateUtc.HasValue
                    ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString()
                    : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet");

                if (shipment.TotalWeight.HasValue)
                    model.TotalWeight =
                        $"{shipment.TotalWeight:F2} [{_measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name}]";

                //prepare shipment items
                foreach (var item in shipment.ShipmentItems)
                {
                    var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                    if (orderItem == null)
                        continue;

                    //fill in model values from the entity
                    var shipmentItemModel = new ShipmentItemModel
                    {
                        Id = item.Id,
                        QuantityInThisShipment = item.Quantity,
                        ShippedFromWarehouse = _shippingService.GetWarehouseById(item.WarehouseId)?.Name
                    };

                    PrepareShipmentItemModel(shipmentItemModel, orderItem);

                    model.Items.Add(shipmentItemModel);
                }

                //prepare shipment events
                if (!string.IsNullOrEmpty(shipment.TrackingNumber))
                {
                    var shipmentTracker = _shipmentService.GetShipmentTracker(shipment);
                    if (shipmentTracker != null)
                    {
                        model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                        if (_shippingSettings.DisplayShipmentEventsToStoreOwner)
                            PrepareShipmentStatusEventModels(model.ShipmentStatusEvents, shipment);
                    }
                }
            }

            if (shipment != null)
                return model;

            model.OrderId = order.Id;
            model.CustomOrderNumber = order.CustomOrderNumber;
            var orderItems = order.OrderItems.Where(item => item.Product.IsShipEnabled).ToList();
            if (_workContext.CurrentVendor != null)
                orderItems = orderItems.Where(item => item.Product.VendorId == _workContext.CurrentVendor.Id).ToList();

            foreach (var orderItem in orderItems)
            {
                var shipmentItemModel = new ShipmentItemModel();
                PrepareShipmentItemModel(shipmentItemModel, orderItem);

                //ensure that this product can be added to a shipment
                if (shipmentItemModel.QuantityToAdd <= 0)
                    continue;

                if (orderItem.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    orderItem.Product.UseMultipleWarehouses)
                {
                    //multiple warehouses supported
                    shipmentItemModel.AllowToChooseWarehouse = true;
                    foreach (var pwi in orderItem.Product.ProductWarehouseInventory.OrderBy(w => w.WarehouseId)
                        .ToList())
                    {
                        var warehouse = pwi.Warehouse;
                        if (warehouse != null)
                        {
                            shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                            {
                                WarehouseId = warehouse.Id,
                                WarehouseName = warehouse.Name,
                                StockQuantity = pwi.StockQuantity,
                                ReservedQuantity = pwi.ReservedQuantity,
                                PlannedQuantity =
                                    _shipmentService.GetQuantityInShipments(orderItem.Product, warehouse.Id, true, true)
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
                        shipmentItemModel.AvailableWarehouses.Add(new ShipmentItemModel.WarehouseInfo
                        {
                            WarehouseId = warehouse.Id,
                            WarehouseName = warehouse.Name,
                            StockQuantity = orderItem.Product.StockQuantity
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
        /// <returns>Order shipment list model</returns>
        public virtual OrderShipmentListModel PrepareOrderShipmentListModel(OrderShipmentSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //get shipments
            var shipments = order.Shipments.ToList();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(shipment => shipment.ShipmentItems.Any(item =>
                    _orderService.GetOrderItemById(item.OrderItemId)?.Product?.VendorId == _workContext.CurrentVendor.Id)).ToList();
            }

            shipments = shipments.OrderBy(shipment => shipment.CreatedOnUtc).ToList();

            var pagedShipments = shipments.ToPagedList(searchModel);

            //prepare list model
            var model = new OrderShipmentListModel().PrepareToGrid(searchModel, pagedShipments, () =>
            {
                //fill in model values from the entity
                return pagedShipments.Select(shipment =>
                {
                    //fill in model values from the entity
                    var shipmentModel = shipment.ToModel<ShipmentModel>();

                    //convert dates to the user time
                    shipmentModel.ShippedDate = shipment.ShippedDateUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc).ToString()
                        : _localizationService.GetResource("Admin.Orders.Shipments.ShippedDate.NotYet");
                    shipmentModel.DeliveryDate = shipment.DeliveryDateUtc.HasValue
                        ? _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString()
                        : _localizationService.GetResource("Admin.Orders.Shipments.DeliveryDate.NotYet");

                    //fill in additional values (not existing in the entity)
                    shipmentModel.CanShip = !shipment.ShippedDateUtc.HasValue;
                    shipmentModel.CanDeliver = shipment.ShippedDateUtc.HasValue && !shipment.DeliveryDateUtc.HasValue;
                    shipmentModel.CustomOrderNumber = shipment.Order.CustomOrderNumber;

                    var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name;
                    if (shipment.TotalWeight.HasValue)
                        shipmentModel.TotalWeight = $"{shipment.TotalWeight:F2} [{baseWeight}]";

                    return shipmentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged shipment item list model
        /// </summary>
        /// <param name="searchModel">Shipment item search model</param>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment item list model</returns>
        public virtual ShipmentItemListModel PrepareShipmentItemListModel(ShipmentItemSearchModel searchModel, Shipment shipment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            //get shipments
            var shipmentItems = shipment.ShipmentItems.ToList().ToPagedList(searchModel);

            //prepare list model
            var model = new ShipmentItemListModel().PrepareToGrid(searchModel, shipmentItems, () =>
            {
                //fill in model values from the entity
                return shipmentItems.Select(item =>
                {
                    //fill in model values from the entity
                    var shipmentItemModel = new ShipmentItemModel
                    {
                        Id = item.Id,
                        QuantityInThisShipment = item.Quantity
                    };

                    //fill in additional values (not existing in the entity)
                    var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                    if (orderItem == null)
                        return shipmentItemModel;

                    shipmentItemModel.OrderItemId = orderItem.Id;
                    shipmentItemModel.ProductId = orderItem.ProductId;
                    shipmentItemModel.ProductName = orderItem.Product.Name;
                    shipmentItemModel.ShippedFromWarehouse = _shippingService.GetWarehouseById(item.WarehouseId)?.Name;

                    var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)?.Name;
                    var baseDimension = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId)?.Name;
                    if (orderItem.ItemWeight.HasValue)
                        shipmentItemModel.ItemWeight = $"{orderItem.ItemWeight:F2} [{baseWeight}]";

                    shipmentItemModel.ItemDimensions =
                        $"{orderItem.Product.Length:F2} x {orderItem.Product.Width:F2} x {orderItem.Product.Height:F2} [{baseDimension}]";

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
        /// <returns>Order note list model</returns>
        public virtual OrderNoteListModel PrepareOrderNoteListModel(OrderNoteSearchModel searchModel, Order order)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //get notes
            var orderNotes = order.OrderNotes.OrderByDescending(on => on.CreatedOnUtc).ToList().ToPagedList(searchModel);

            //prepare list model
            var model = new OrderNoteListModel().PrepareToGrid(searchModel, orderNotes, () =>
            {
                //fill in model values from the entity
                return orderNotes.Select(orderNote =>
                {
                    //fill in model values from the entity
                    var orderNoteModel = orderNote.ToModel<OrderNoteModel>();

                    //convert dates to the user time
                    orderNoteModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderNoteModel.Note = _orderService.FormatOrderNoteText(orderNote);
                    orderNoteModel.DownloadGuid = _downloadService.GetDownloadById(orderNote.DownloadId)?.DownloadGuid ?? Guid.Empty;

                    return orderNoteModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare bestseller brief search model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>Bestseller brief search model</returns>
        public virtual BestsellerBriefSearchModel PrepareBestsellerBriefSearchModel(BestsellerBriefSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize(5);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged bestseller brief list model
        /// </summary>
        /// <param name="searchModel">Bestseller brief search model</param>
        /// <returns>Bestseller brief list model</returns>
        public virtual BestsellerBriefListModel PrepareBestsellerBriefListModel(BestsellerBriefSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get bestsellers
            var bestsellers = _orderReportService.BestSellersReport(showHidden: true,
                vendorId: _workContext.CurrentVendor?.Id ?? 0,
                orderBy: searchModel.OrderBy,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new BestsellerBriefListModel().PrepareToGrid(searchModel, bestsellers, () =>
            {
                //fill in model values from the entity
                return bestsellers.Select(bestseller =>
                {
                    //fill in model values from the entity
                    var bestsellerModel = new BestsellerModel
                    {
                        ProductId = bestseller.ProductId,
                        TotalQuantity = bestseller.TotalQuantity
                    };

                    //fill in additional values (not existing in the entity)
                    bestsellerModel.ProductName = _productService.GetProductById(bestseller.ProductId)?.Name;
                    bestsellerModel.TotalAmount = _priceFormatter.FormatPrice(bestseller.TotalAmount, true, false);

                    return bestsellerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare order average line summary report list model
        /// </summary>
        /// <param name="searchModel">Order average line summary report search model</param>
        /// <returns>Order average line summary report list model</returns>
        public virtual OrderAverageReportListModel PrepareOrderAverageReportListModel(OrderAverageReportSearchModel searchModel)
        {
            //get report
            var report = new List<OrderAverageReportLineSummary>
            {
                _orderReportService.OrderAverageReport(0, OrderStatus.Pending),
                _orderReportService.OrderAverageReport(0, OrderStatus.Processing),
                _orderReportService.OrderAverageReport(0, OrderStatus.Complete),
                _orderReportService.OrderAverageReport(0, OrderStatus.Cancelled)
            };

            var pagedList = new PagedList<OrderAverageReportLineSummary>(report, 0, int.MaxValue);

            //prepare list model
            var model = new OrderAverageReportListModel().PrepareToGrid(searchModel, pagedList, () =>
            {
                //fill in model values from the entity
                return pagedList.Select(reportItem => new OrderAverageReportModel
                {
                    OrderStatus = _localizationService.GetLocalizedEnum(reportItem.OrderStatus),
                    SumTodayOrders = _priceFormatter.FormatPrice(reportItem.SumTodayOrders, true, false),
                    SumThisWeekOrders = _priceFormatter.FormatPrice(reportItem.SumThisWeekOrders, true, false),
                    SumThisMonthOrders = _priceFormatter.FormatPrice(reportItem.SumThisMonthOrders, true, false),
                    SumThisYearOrders = _priceFormatter.FormatPrice(reportItem.SumThisYearOrders, true, false),
                    SumAllTimeOrders = _priceFormatter.FormatPrice(reportItem.SumAllTimeOrders, true, false)
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare incomplete order report list model
        /// </summary>
        /// <param name="searchModel">Incomplete order report search model</param>
        /// <returns>Incomplete order report list model</returns>
        public virtual OrderIncompleteReportListModel PrepareOrderIncompleteReportListModel(OrderIncompleteReportSearchModel searchModel)
        {
            var orderIncompleteReportModels = new List<OrderIncompleteReportModel>();

            //get URL helper
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            //not paid
            var orderStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<int>().Where(os => os != (int)OrderStatus.Cancelled).ToList();
            var paymentStatuses = new List<int> { (int)PaymentStatus.Pending };
            var psPending = _orderReportService.GetOrderAverageReportLine(psIds: paymentStatuses, osIds: orderStatuses);
            orderIncompleteReportModels.Add(new OrderIncompleteReportModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
                Count = psPending.CountOrders,
                Total = _priceFormatter.FormatPrice(psPending.SumOrders, true, false),
                ViewLink = urlHelper.Action("List", "Order", new
                {
                    orderStatuses = string.Join(",", orderStatuses),
                    paymentStatuses = string.Join(",", paymentStatuses)
                })
            });

            //not shipped
            var shippingStatuses = new List<int> { (int)ShippingStatus.NotYetShipped };
            var ssPending = _orderReportService.GetOrderAverageReportLine(osIds: orderStatuses, ssIds: shippingStatuses);
            orderIncompleteReportModels.Add(new OrderIncompleteReportModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
                Count = ssPending.CountOrders,
                Total = _priceFormatter.FormatPrice(ssPending.SumOrders, true, false),
                ViewLink = urlHelper.Action("List", "Order", new
                {
                    orderStatuses = string.Join(",", orderStatuses),
                    shippingStatuses = string.Join(",", shippingStatuses)
                })
            });

            //pending
            orderStatuses = new List<int> { (int)OrderStatus.Pending };
            var osPending = _orderReportService.GetOrderAverageReportLine(osIds: orderStatuses);
            orderIncompleteReportModels.Add(new OrderIncompleteReportModel
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalIncompleteOrders"),
                Count = osPending.CountOrders,
                Total = _priceFormatter.FormatPrice(osPending.SumOrders, true, false),
                ViewLink = urlHelper.Action("List", "Order", new { orderStatuses = string.Join(",", orderStatuses) })
            });

            var pagedList = new PagedList<OrderIncompleteReportModel>(orderIncompleteReportModels, 0, int.MaxValue);

            //prepare list model
            var model = new OrderIncompleteReportListModel().PrepareToGrid(searchModel, pagedList, () => pagedList);
            return model;
        }

        #endregion
    }
}