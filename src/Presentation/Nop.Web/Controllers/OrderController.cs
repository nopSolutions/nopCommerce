using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers
{
    public partial class OrderController : BaseNopController
    {
		#region Fields

        private readonly IOrderService _orderService;
        private readonly IShipmentService _shipmentService;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly ILocalizationService _localizationService;
        private readonly IPdfService _pdfService;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;

        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

		#region Constructors

        public OrderController(IOrderService orderService, 
            IShipmentService shipmentService, IWorkContext workContext,
            ICurrencyService currencyService, IPriceFormatter priceFormatter,
            IOrderProcessingService orderProcessingService, IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService, ILocalizationService localizationService,
            IPdfService pdfService, IShippingService shippingService,
            ICountryService countryService, IProductAttributeParser productAttributeParser,
            IWebHelper webHelper, 
            CatalogSettings catalogSettings, OrderSettings orderSettings,
            TaxSettings taxSettings, PdfSettings pdfSettings,
            ShippingSettings shippingSettings, AddressSettings addressSettings)
        {
            this._orderService = orderService;
            this._shipmentService = shipmentService;
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._orderProcessingService = orderProcessingService;
            this._dateTimeHelper = dateTimeHelper;
            this._paymentService = paymentService;
            this._localizationService = localizationService;
            this._pdfService = pdfService;
            this._shippingService = shippingService;
            this._countryService = countryService;
            this._productAttributeParser = productAttributeParser;
            this._webHelper = webHelper;

            this._catalogSettings = catalogSettings;
            this._orderSettings = orderSettings;
            this._taxSettings = taxSettings;
            this._pdfSettings = pdfSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected OrderDetailsModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new OrderDetailsModel();

            model.Id = order.Id;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.IsReOrderAllowed = _orderSettings.IsReOrderAllowed;
            model.IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order);
            model.DisplayPdfInvoice = _pdfSettings.Enabled;

            //shipping info
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.ShippingAddress.PrepareModel(order.ShippingAddress, false, _addressSettings);
                model.ShippingMethod = order.ShippingMethod;
   

                //shipments (only already shipped)
                var shipments = order.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsModel.ShipmentBriefModel()
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                    model.Shipments.Add(shipmentModel);
                }
            }


            //billing info
            model.BillingAddress.PrepareModel(order.BillingAddress, false, _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);

            //purchase order number (we have to find a better to inject this information because it's related to a certain plugin)
            if (paymentMethod != null && paymentMethod.PluginDescriptor.SystemName.Equals("Payments.PurchaseOrder", StringComparison.InvariantCultureIgnoreCase))
            {
                model.DisplayPurchaseOrderNumber = true;
                model.PurchaseOrderNumber = order.PurchaseOrderNumber;
            }


            //totals
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    {
                        //order subtotal
                        var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                        model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        //discount (applied to order subtotal)
                        var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                        if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                            model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        //order shipping
                        var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                        model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        //payment method additional fee
                        var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                        if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                            model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                    }
                    break;
                case TaxDisplayType.IncludingTax:
                    {
                        //order subtotal
                        var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                        model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        //discount (applied to order subtotal)
                        var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                        if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                            model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        //order shipping
                        var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                        model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        //payment method additional fee
                        var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                        if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                            model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                    }
                    break;
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
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
                    displayTaxRates = _taxSettings.DisplayTaxRates && order.TaxRatesDictionary.Count > 0;
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in order.TaxRatesDictionary)
                    {
                        model.TaxRates.Add(new OrderDetailsModel.TaxRate()
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;


            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);


            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderDetailsModel.GiftCard()
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.OrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote()
                {
                    Note = orderNote.FormatOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }


            //purchased products
            model.ShowSku = _catalogSettings.ShowProductSku;
            var orderProductVariants = _orderService.GetAllOrderProductVariants(order.Id, null, null, null, null, null, null);
            foreach (var opv in orderProductVariants)
            {
                var opvModel = new OrderDetailsModel.OrderProductVariantModel()
                {
                    Id = opv.Id,
                    Sku = opv.ProductVariant.FormatSku(opv.AttributesXml, _productAttributeParser),
                    ProductId = opv.ProductVariant.ProductId,
                    ProductSeName = opv.ProductVariant.Product.GetSeName(),
                    Quantity = opv.Quantity,
                    AttributeInfo = opv.AttributeDescription,
                };

                //product name
                if (!String.IsNullOrEmpty(opv.ProductVariant.GetLocalized(x => x.Name)))
                    opvModel.ProductName = string.Format("{0} ({1})", opv.ProductVariant.Product.GetLocalized(x => x.Name), opv.ProductVariant.GetLocalized(x => x.Name));
                else
                    opvModel.ProductName = opv.ProductVariant.Product.GetLocalized(x => x.Name);
                model.Items.Add(opvModel);

                //unit price, subtotal
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var opvUnitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.UnitPriceExclTax, order.CurrencyRate);
                            opvModel.UnitPrice = _priceFormatter.FormatPrice(opvUnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                            var opvPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.PriceExclTax, order.CurrencyRate);
                            opvModel.SubTotal = _priceFormatter.FormatPrice(opvPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var opvUnitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.UnitPriceInclTax, order.CurrencyRate);
                            opvModel.UnitPrice = _priceFormatter.FormatPrice(opvUnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                            var opvPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.PriceInclTax, order.CurrencyRate);
                            opvModel.SubTotal = _priceFormatter.FormatPrice(opvPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                        }
                        break;
                }
            }

            return model;
        }

        [NonAction]
        protected ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("order cannot be loaded");
            var model = new ShipmentDetailsModel();
            
            model.Id = shipment.Id;
            if (shipment.ShippedDateUtc.HasValue)
                model.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
            if (shipment.DeliveryDateUtc.HasValue)
                model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
            
            //tracking number and shipment information
            model.TrackingNumber = shipment.TrackingNumber;
            var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(order.ShippingRateComputationMethodSystemName);
            if (srcm != null &&
                srcm.PluginDescriptor.Installed &&
                srcm.IsShippingRateComputationMethodActive(_shippingSettings))
            {
                var shipmentTracker = srcm.ShipmentTracker;
                if (shipmentTracker != null)
                {
                    model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                    if (_shippingSettings.DisplayShipmentEventsToCustomers)
                    {
                        var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
                        if (shipmentEvents != null)
                            foreach (var shipmentEvent in shipmentEvents)
                            {
                                var shipmentStatusEventModel = new ShipmentDetailsModel.ShipmentStatusEventModel();
                                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                                shipmentStatusEventModel.Country = shipmentEventCountry != null
                                                                       ? shipmentEventCountry.GetLocalized(x => x.Name)
                                                                       : shipmentEvent.CountryCode;
                                shipmentStatusEventModel.Date = shipmentEvent.Date;
                                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                shipmentStatusEventModel.Location = shipmentEvent.Location;
                                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                            }
                    }
                }
            }
            
            //products in this shipment
            model.ShowSku = _catalogSettings.ShowProductSku;
            foreach (var sopv in shipment.ShipmentOrderProductVariants)
            {
                var opv = _orderService.GetOrderProductVariantById(sopv.OrderProductVariantId);
                if (opv == null)
                    continue;

                var sopvModel = new ShipmentDetailsModel.ShipmentOrderProductVariantModel()
                {
                    Id = sopv.Id,
                    Sku = opv.ProductVariant.FormatSku(opv.AttributesXml, _productAttributeParser),
                    ProductId = opv.ProductVariant.ProductId,
                    ProductSeName = opv.ProductVariant.Product.GetSeName(),
                    AttributeInfo = opv.AttributeDescription,
                    QuantityOrdered = opv.Quantity,
                    QuantityShipped = sopv.Quantity,
                };

                //product name//product name
                if (!String.IsNullOrEmpty(opv.ProductVariant.GetLocalized(x => x.Name)))
                    sopvModel.ProductName = string.Format("{0} ({1})", opv.ProductVariant.Product.GetLocalized(x => x.Name), opv.ProductVariant.GetLocalized(x => x.Name));
                else
                    sopvModel.ProductName = opv.ProductVariant.Product.GetLocalized(x => x.Name);
                model.Items.Add(sopvModel);
            }

            //order details model
            model.Order = PrepareOrderDetailsModel(order);
            
            return model;
        }

        #endregion

        #region Order details

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Details(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareOrderDetailsModel(order);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult PrintOrderDetails(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareOrderDetailsModel(order);
            model.PrintMode = true;

            return View("Details", model);
        }

        public ActionResult GetPdfInvoice(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

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

        public ActionResult ReOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            _orderProcessingService.ReOrder(order);
            return RedirectToRoute("ShoppingCart");
        }

        [HttpPost, ActionName("Details")]
        [FormValueRequired("repost-payment")]
        public ActionResult RePostPayment(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            if (!_paymentService.CanRePostProcessPayment(order))
                return RedirectToRoute("OrderDetails", new { orderId = orderId });

            var postProcessPaymentRequest = new PostProcessPaymentRequest()
            {
                Order = order
            };
            _paymentService.PostProcessPayment(postProcessPaymentRequest);

            if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
            {
                //redirection or POST has been done in PostProcessPayment
                return Content("Redirected");
            }
            else
            {
                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("OrderDetails", new { orderId = orderId });
            }
        }

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult ShipmentDetails(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return new HttpUnauthorizedResult();

            var order = shipment.Order;
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareShipmentDetailsModel(shipment);

            return View(model);
        }
        #endregion
    }
}
