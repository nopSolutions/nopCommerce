using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class OrderController : BaseNopController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
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

        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        
        #endregion

        #region Ctor

        public OrderController(IOrderService orderService, 
            IOrderReportService orderReportService, IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter, ILocalizationService localizationService,
            IWorkContext workContext, ICurrencyService currencyService,
            IEncryptionService encryptionService, IPaymentService paymentService,
            IMeasureService measureService, IPdfService pdfService,
            IAddressService addressService, ICountryService countryService,
            IStateProvinceService stateProvinceService, IProductService productService,
            IExportManager exportManager, IPermissionService permissionService,
            CurrencySettings currencySettings, TaxSettings taxSettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings)
		{
            this._orderService = orderService;
            this._orderReportService = orderReportService;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._priceFormatter = priceFormatter;
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

            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
		}
        
        #endregion

        #region Utilities

        private void PrepareOrderDetailsModel(OrderModel model, Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = order.Id;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.OrderGuid = order.OrderGuid;
            model.CustomerId = order.CustomerId;
            model.CustomerIp = order.CustomerIp;
            model.VatNumber = order.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.DisplayPdfInvoice = _pdfSettings.Enabled;
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;
            model.AffiliateId = order.AffiliateId;
            
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


            //purchase order number
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

            #endregion

            #region Billing & shipping info

            model.BillingAddress = order.BillingAddress.ToModel();
            if (order.BillingAddress.Country != null)
                model.BillingAddress.CountryName = order.BillingAddress.Country.Name;
            if (order.BillingAddress.StateProvince != null)
                model.BillingAddress.StateProvinceName = order.BillingAddress.StateProvince.Name;


            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.DisplayPdfPackagingSlip = _pdfSettings.Enabled;

                model.ShippingAddress = order.ShippingAddress.ToModel();
                if (order.ShippingAddress.Country != null)
                    model.ShippingAddress.CountryName = order.ShippingAddress.Country.Name;
                if (order.ShippingAddress.StateProvince != null)
                    model.ShippingAddress.StateProvinceName = order.ShippingAddress.StateProvince.Name;


                model.ShippingMethod = order.ShippingMethod;
                model.TrackingNumber = order.TrackingNumber;

                model.CanShip = _orderProcessingService.CanShip(order);
                if (order.ShippedDateUtc.HasValue)
                    model.ShippedDate = _dateTimeHelper.ConvertToUserTime(order.ShippedDateUtc.Value, DateTimeKind.Utc).ToString();

                model.CanDeliver = _orderProcessingService.CanDeliver(order);
                if (order.DeliveryDateUtc.HasValue)
                    model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(order.DeliveryDateUtc.Value, DateTimeKind.Utc).ToString();


                model.OrderWeight = order.OrderWeight;
                var baseWeight = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId);
                if (baseWeight != null)
                    model.BaseWeightIn = baseWeight.Name;
                model.ShippingAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}", Server.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "")));
            }

            #endregion

            #region Products
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            bool hasDownloadableItems = false;
            foreach (var opv in order.OrderProductVariants)
            {
                if (opv.ProductVariant != null && opv.ProductVariant.IsDownload)
                    hasDownloadableItems = true;

                var opvModel = new OrderModel.OrderProductVariantModel()
                {
                    Id = opv.Id,
                    ProductVariantId = opv.ProductVariantId,
                    Quantity = opv.Quantity,
                    IsDownload = opv.ProductVariant.IsDownload,
                    DownloadCount = opv.DownloadCount,
                    DownloadActivationType = opv.ProductVariant.DownloadActivationType,
                    IsDownloadActivated = opv.IsDownloadActivated,
                    LicenseDownloadId = opv.LicenseDownloadId
                };

                //product name
                if (!String.IsNullOrEmpty(opv.ProductVariant.Name))
                    opvModel.FullProductName = string.Format("{0} ({1})", opv.ProductVariant.Product.Name, opv.ProductVariant.Name);
                else
                    opvModel.FullProductName = opv.ProductVariant.Product.Name;

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
                opvModel.ReturnRequestIds = _orderService.SearchReturnRequests(0, opv.Id, null)
                    .Select(rr=> rr.Id).ToList();

                model.Items.Add(opvModel);
            }
            model.HasDownloadableProducts = hasDownloadableItems;
            #endregion
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

            var model = new OrderListModel();
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
		}

		[GridAction(EnableCustomBinding = true)]
		public ActionResult OrderList(GridCommand command, OrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


            var orders = _orderService.SearchOrders(startDateValue, endDateValue, orderStatus,
                paymentStatus, shippingStatus, model.CustomerEmail, model.OrderGuid, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<OrderModel>
            {
                Data = orders.Select(x =>
                {
                    return new OrderModel()
                    {
                        Id = x.Id,
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

        public ActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            try
            {
                var orders = _orderService.SearchOrders(null, null, null,
                    null, null, null, null, 0, int.MaxValue);

                var fileName = string.Format("orders_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                var xml = _exportManager.ExportOrdersToXml(orders);
                return new XmlDownloadResult(xml, fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportExcel()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            try
            {
                var orders = _orderService.SearchOrders(null, null, null,
                    null, null, null, null, 0, int.MaxValue);

                string fileName = string.Format("orders_{0}_{1}.xls", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), CommonHelper.GenerateRandomDigitCode(4));
                string filePath = string.Format("{0}content\\files\\ExportImport\\{1}", Request.PhysicalApplicationPath, fileName);

                _exportManager.ExportOrdersToXls(filePath, orders);

                var bytes = System.IO.File.ReadAllBytes(filePath);
                return File(bytes, "text/xls", fileName);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Order details

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                throw new ArgumentException("No order found with the specified id", "id");

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public ActionResult CancelOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");
            
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
                throw new ArgumentException("No order found with the specified id");
            
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
                throw new ArgumentException("No order found with the specified id");
            
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
                throw new ArgumentException("No order found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

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

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public ActionResult EditCreditCardInfo(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

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
        
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            _orderService.DeleteOrder(order);
            return RedirectToAction("List");
        }

        public ActionResult PdfInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);

            string fileName = string.Format("order_{0}_{1}.pdf", order.OrderGuid, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = string.Format("{0}content\\files\\ExportImport\\{1}", this.Request.PhysicalApplicationPath, fileName);
            _pdfService.PrintOrderToPdf(order, _workContext.WorkingLanguage, filePath);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf", fileName);
        }

        public ActionResult PdfPackagingSlip(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            var orders = new List<Order>();
            orders.Add(order);
            string fileName = string.Format("packagingslip_{0}_{1}.pdf", order.OrderGuid, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = string.Format("{0}content\\files\\ExportImport\\{1}", this.Request.PhysicalApplicationPath, fileName);
            _pdfService.PrintPackagingSlipsToPdf(orders, filePath);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf", fileName);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("settrackingnumber")]
        public ActionResult SetTrackingNumber(OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(model.Id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            order.TrackingNumber = model.TrackingNumber;
            _orderService.UpdateOrder(order);

            ViewData["selectedTab"] = "shippinginfo";
            PrepareOrderDetailsModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("setasshipped")]
        public ActionResult SetAsShipped(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "shippinginfo";

            try
            {
                _orderProcessingService.Ship(order, true);
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
        [FormValueRequired("setasdelivered")]
        public ActionResult SetAsDelivered(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "shippinginfo";

            try
            {
                _orderProcessingService.Deliver(order, true);
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
                throw new ArgumentException("No order found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

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

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOpv")]
        [ValidateInput(false)]
        public ActionResult EditOrderProductVariant(int id, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnSaveOpv", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnSaveOpv".Length));

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == opvId).FirstOrDefault();
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
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnDeleteOpv", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnDeleteOpv".Length));

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == opvId).FirstOrDefault();
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
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == opvId).FirstOrDefault();
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "products";

            //get order product variant identifier
            int opvId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    opvId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == opvId).FirstOrDefault();
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == opvId).FirstOrDefault();
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");
            
            if (!orderProductVariant.ProductVariant.IsDownload)
                throw new ArgumentException("Product variant is not downloadable");

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
                throw new ArgumentException("No order found with the specified id");

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == model.OrderProductVariantId).FirstOrDefault();
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

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
                throw new ArgumentException("No order found with the specified id");

            var orderProductVariant = order.OrderProductVariants.Where(x => x.Id == model.OrderProductVariantId).FirstOrDefault();
            if (orderProductVariant == null)
                throw new ArgumentException("No order product variant found with the specified id");

            //attach license
            orderProductVariant.LicenseDownloadId = null;
            _orderService.UpdateOrder(order);

            //success
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View(model);
        }
        #endregion

        #region Addresses
        
        public ActionResult AddressEdit(int addressId, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new OrderAddressModel();
            model.OrderId = orderId;
            model.Address = address.ToModel();
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
                throw new ArgumentException("No order found with the specified id");

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

        #region Order notes
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult OrderNotesSelect(int orderId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

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
                    Note = Core.Html.HtmlHelper.FormatText(orderNote.Note, false, true, false, false, false, false),
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
                throw new ArgumentException("No order found with the specified id");

            order.OrderNotes.Add(new OrderNote()
            {
                DisplayToCustomer = displayToCustomer,
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
            });
            _orderService.UpdateOrder(order);

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

            var orderNote = order.OrderNotes.Where(on => on.Id == orderNoteId).FirstOrDefault();
            if (orderNote == null)
                throw new ArgumentException("No order note found with the specified id");
            _orderService.DeleteOrderNote(orderNote);

            return OrderNotesSelect(orderId, command);
        }


        #endregion

        #region Reports

        [NonAction]
        protected IList<BestsellersReportLineModel> GetBestsellersBriefReportModel(int recordsToReturn, int orderBy)
        {
            var report = _orderReportService.BestSellersReport(null, null,
                null, null, null, recordsToReturn, orderBy, true);

            var model = report.Select(x =>
            {
                var m = new BestsellersReportLineModel()
                {
                    ProductVariantId = x.ProductVariantId,
                    TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                    TotalQuantity = x.TotalQuantity,
                };
                var productVariant = _productService.GetProductVariantById(x.ProductVariantId);
                if (productVariant != null)
                    m.ProductVariantFullName = productVariant.Product.Name + " " + productVariant.Name;
                return m;
            }).ToList();

            return model;
        }
        public ActionResult BestsellersBriefReportByQuantity()
        {
            var model = GetBestsellersBriefReportModel(5, 1);
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersBriefReportByQuantityList(GridCommand command)
        {
            var model = GetBestsellersBriefReportModel(5, 1);
            var gridModel = new GridModel<BestsellersReportLineModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        public ActionResult BestsellersBriefReportByAmount()
        {
            var model = GetBestsellersBriefReportModel(5, 2);
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersBriefReportByAmountList(GridCommand command)
        {
            var model = GetBestsellersBriefReportModel(5, 2);
            var gridModel = new GridModel<BestsellersReportLineModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }




        public ActionResult BestsellersReport()
        {
            var model = new BestsellersReportModel();
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult BestsellersReportList(GridCommand command, BestsellersReportModel model)
        {
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;


            //return first 100 records
            var items = _orderReportService.BestSellersReport(startDateValue, endDateValue,
                orderStatus, paymentStatus, null, 100, 2, true);
            var gridModel = new GridModel<BestsellersReportLineModel>
            {
                Data = items.Select(x =>
                {
                    var m = new BestsellersReportLineModel()
                    {
                        ProductVariantId = x.ProductVariantId,
                        TotalAmount = _priceFormatter.FormatPrice(x.TotalAmount, true, false),
                        TotalQuantity = x.TotalQuantity,
                    };
                    var productVariant = _productService.GetProductVariantById(x.ProductVariantId);
                    if (productVariant != null)
                        m.ProductVariantFullName = productVariant.Product.Name + " " + productVariant.Name;
                    return m;
                }),
                Total = items.Count
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
            report.Add(_orderReportService.OrderAverageReport(OrderStatus.Pending));
            report.Add(_orderReportService.OrderAverageReport(OrderStatus.Processing));
            report.Add(_orderReportService.OrderAverageReport(OrderStatus.Complete));
            report.Add(_orderReportService.OrderAverageReport(OrderStatus.Cancelled));
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
            var model = GetOrderAverageReportModel();
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult OrderAverageReportList(GridCommand command)
        {
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
            var psPending = _orderReportService.GetOrderAverageReportLine(null, PaymentStatus.Pending, null, null, null);
            model.Add(new OrderIncompleteReportLineModel()
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalUnpaidOrders"),
                Count = psPending.CountOrders,
                Total = _priceFormatter.FormatPrice(psPending.SumOrders, true, false)
            });
            //not shipped
            var ssPending = _orderReportService.GetOrderAverageReportLine(null, null, ShippingStatus.NotYetShipped, null, null);
            model.Add(new OrderIncompleteReportLineModel()
            {
                Item = _localizationService.GetResource("Admin.SalesReport.Incomplete.TotalNotShippedOrders"),
                Count = ssPending.CountOrders,
                Total = _priceFormatter.FormatPrice(ssPending.SumOrders, true, false)
            });
            //pending
            var osPending = _orderReportService.GetOrderAverageReportLine(OrderStatus.Pending, null, null, null, null);
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
            var model = GetOrderIncompleteReportModel();
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult OrderIncompleteReportList(GridCommand command)
        {
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
