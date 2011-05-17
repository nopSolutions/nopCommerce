using System;
using System.Linq;
using System.Web.Mvc;

using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

using Telerik.Web.Mvc;
using System.Collections.Generic;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
	public class OrderController : BaseNopController
    {
        #region Fields

		private readonly IOrderService _orderService;
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

        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        
        #endregion

        #region Ctor

        public OrderController(IOrderService orderService, IOrderProcessingService orderProcessingService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter, ILocalizationService localizationService,
            IWorkContext workContext, ICurrencyService currencyService,
            IEncryptionService encryptionService, IPaymentService paymentService,
            IMeasureService measureService, IPdfService pdfService,
            IAddressService addressService, ICountryService countryService,
            IStateProvinceService stateProvinceService,
            CurrencySettings currencySettings, TaxSettings taxSettings,
            MeasureSettings measureSettings, PdfSettings pdfSettings)
		{
            this._orderService = orderService;
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

            this._currencySettings = currencySettings;
            this._taxSettings = taxSettings;
            this._measureSettings = measureSettings;
            this._pdfSettings = pdfSettings;
		}
        
        #endregion

        #region Utilities

        private OrderModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            var model = new OrderModel()
            {
                Id = order.Id,
                OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                OrderGuid = order.OrderGuid,
                CustomerId = order.CustomerId,
                CustomerIp = order.CustomerIp,
                VatNumber = order.VatNumber,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                DisplayPdfInvoice = _pdfSettings.Enabled,
                AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType,
                TaxDisplayType = _taxSettings.TaxDisplayType
            };
            
            #region Order totals

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            //subtotal
            model.OrderSubtotalInclTax = _priceFormatter.FormatPrice(order.OrderSubtotalInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderSubtotalExclTax = _priceFormatter.FormatPrice(order.OrderSubtotalExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            //discount (applied to order subtotal)
            string orderSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            string orderSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;


            //shipping
            model.OrderShippingInclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderShippingExclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }


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

            //discount
            if (order.OrderDiscount > 0)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-order.OrderDiscount, true, false);

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
            if (pm != null && pm.SystemName == "PURCHASEORDER")
            {
                model.DisplayPurchaseOrderNumber = true;
                model.PurchaseOrderNumber = order.PurchaseOrderNumber;
            }

            //payment transaction info
            model.AuthorizationTransactionId = order.AuthorizationTransactionId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.SubscriptionTransactionId = order.SubscriptionTransactionId;

            //payment method info
            model.PaymentMethod = pm != null ? pm.FriendlyName : order.PaymentMethodSystemName;
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

            #region Product info

            //UNDONE Product info

            #endregion

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
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null 
                            :(DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone);

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
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString()
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

        #region Order details

        public ActionResult Edit(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id", "id");

            var model = PrepareOrderDetailsModel(order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public ActionResult CancelOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");
            
            try
            {
                _orderProcessingService.CancelOrder(order, true);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public ActionResult CaptureOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");
            
            try
            {
                var errors = _orderProcessingService.Capture(order);
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors = errors.ToList();
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }

        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public ActionResult MarkOrderAsPaid(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");
            
            try
            {
                _orderProcessingService.MarkOrderAsPaid(order);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public ActionResult RefundOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            try
            {
                var errors = _orderProcessingService.Refund(order);
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors = errors.ToList();
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public ActionResult RefundOrderOffline(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            try
            {
                _orderProcessingService.RefundOffline(order);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public ActionResult VoidOrder(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            try
            {
                var errors = _orderProcessingService.Void(order);
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors = errors.ToList();
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public ActionResult VoidOrderOffline(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            try
            {
                _orderProcessingService.VoidOffline(order);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var order = _orderService.GetOrderById(id);
            _orderService.DeleteOrder(order);
            return RedirectToAction("List");
        }

        public ActionResult PdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            string fileName = string.Format("order_{0}_{1}.pdf", order.OrderGuid, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = string.Format("{0}content\\files\\ExportImport\\{1}", this.Request.PhysicalApplicationPath, fileName);
            _pdfService.PrintOrderToPdf(order, _workContext.WorkingLanguage, filePath);
            var pdfBytes = System.IO.File.ReadAllBytes(filePath);
            return File(pdfBytes, "application/pdf", fileName);
        }

        public ActionResult PdfPackagingSlip(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var orders = new List<Order>();
            orders.Add(order);
            string fileName = string.Format("packagingslip_{0}_{1}.pdf", order.OrderGuid, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = string.Format("{0}content\\files\\ExportImport\\{1}", this.Request.PhysicalApplicationPath, fileName);
            _pdfService.PrintPackagingSlipsToPdf(orders, filePath);
            var pdfBytes = System.IO.File.ReadAllBytes(filePath);
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("settrackingnumber")]
        public ActionResult SetTrackingNumber(OrderModel model)
        {
            var order = _orderService.GetOrderById(model.Id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            order.TrackingNumber = model.TrackingNumber;
            _orderService.UpdateOrder(order);

            ViewData["selectedTab"] = "shippinginfo";
            model = PrepareOrderDetailsModel(order);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("setasshipped")]
        public ActionResult SetAsShipped(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "shippinginfo";

            try
            {
                _orderProcessingService.Ship(order, true);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("setasdelivered")]
        public ActionResult SetAsDelivered(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            ViewData["selectedTab"] = "shippinginfo";

            try
            {
                _orderProcessingService.Deliver(order, true);
                var model = PrepareOrderDetailsModel(order);
                return View(model);
            }
            catch (Exception exc)
            {
                //error
                var model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }


        //edit
        public ActionResult PartiallyRefundOrderPopup(int id, bool online)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                throw new ArgumentException("No order found with the specified id");

            var model = PrepareOrderDetailsModel(order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public ActionResult PartiallyRefundOrderPopup(string btnId, int id, bool online, OrderModel model)
        {
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

                    model = PrepareOrderDetailsModel(order);
                    return View(model);
                }
                else
                {
                    //error
                    model = PrepareOrderDetailsModel(order);
                    model.ChangePaymentStatusErrors = errors.ToList();
                    return View(model);
                }
            }
            catch (Exception exc)
            {
                //error
                model = PrepareOrderDetailsModel(order);
                model.ChangePaymentStatusErrors.Add(exc.Message);
                return View(model);
            }
        }


        #endregion

        #region Addresses
        
        public ActionResult AddressEdit(int addressId, int orderId)
        {
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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(OrderAddressModel model)
        {
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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        #endregion

        #region Order notes


        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult OrderNotesSelect(int orderId, GridCommand command)
        {
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
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc).ToString()
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
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

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
    }
}
