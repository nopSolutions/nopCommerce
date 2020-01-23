using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial class OrderModelFactory : IOrderModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public OrderModelFactory(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PdfSettings pdfSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _pdfSettings = pdfSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListModel PrepareCustomerOrderListModel()
        {
            var model = new CustomerOrderListModel();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id);
            foreach (var order in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                    PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus),
                    ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                    CustomOrderNumber = order.CustomOrderNumber
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.Orders.Add(orderModel);
            }

            var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);

                var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = $"{recurringPayment.CycleLength} {_localizationService.GetLocalizedEnum(recurringPayment.CyclePeriod)}",
                    NextPayment = _orderProcessingService.GetNextPaymentDate(recurringPayment) is DateTime nextPaymentDate ? _dateTimeHelper.ConvertToUserTime(nextPaymentDate, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = _orderProcessingService.GetCyclesRemaining(recurringPayment),
                    InitialOrderId = order.Id,
                    InitialOrderNumber = order.CustomOrderNumber,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                    CanRetryLastPayment = _orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        public virtual OrderDetailsModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            var model = new OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
                CustomOrderNumber = order.CustomOrderNumber,

                //shipping info
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus)
            };
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickupInStore = order.PickupInStore;
                if (!order.PickupInStore)
                {
                    var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId ?? 0);

                    _addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                        address: shippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
                else if (order.PickupAddressId.HasValue && _addressService.GetAddressById(order.PickupAddressId.Value) is Address pickupAddress)
                {
                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupAddress.Address1,
                        City = pickupAddress.City,
                        County = pickupAddress.County,
                        CountryName = _countryService.GetCountryByAddress(pickupAddress)?.Name ?? string.Empty,
                        ZipPostalCode = pickupAddress.ZipPostalCode
                    };
                }

                model.ShippingMethod = order.ShippingMethod;

                //shipments (only already shipped)
                var shipments = _shipmentService.GetShipmentsByOrderId(order.Id, true).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsModel.ShipmentBriefModel
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

            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

            //billing info
            _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                address: billingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentPluginManager.LoadPluginBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            var displayTax = true;
            var displayTaxRates = true;
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
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new OrderDetailsModel.TaxRate
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
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //gift cards
            foreach (var gcuh in _giftCardService.GetGiftCardUsageHistory(order))
            {
                model.GiftCards.Add(new OrderDetailsModel.GiftCard
                {
                    CouponCode = _giftCardService.GetGiftCardById(gcuh.GiftCardId).GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntryId.HasValue && _rewardPointService.GetRewardPointsHistoryEntryById(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in _orderService.GetOrderNotesByOrderId(order.Id, true)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;

            var orderItems = _orderService.GetOrderItems(order.Id);

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);

                var orderItemModel = new OrderDetailsModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = _productService.FormatSku(product, orderItem.AttributesXml),
                    VendorName = _vendorService.GetVendorById(product.VendorId)?.Name ?? string.Empty,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                };
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(orderItemModel);

                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

                //downloadable products
                if (_orderService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = product.DownloadId;
                if (_orderService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        public virtual ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);

            if (order == null)
                throw new Exception("order cannot be loaded");
            var model = new ShipmentDetailsModel
            {
                Id = shipment.Id
            };
            if (shipment.ShippedDateUtc.HasValue)
                model.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
            if (shipment.DeliveryDateUtc.HasValue)
                model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

            //tracking number and shipment information
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                model.TrackingNumber = shipment.TrackingNumber;
                var shipmentTracker = _shipmentService.GetShipmentTracker(shipment);
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
                                    ? _localizationService.GetLocalized(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
                                shipmentStatusEventModel.Date = shipmentEvent.Date;
                                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                shipmentStatusEventModel.Location = shipmentEvent.Location;
                                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                            }
                    }
                }
            }

            //products in this shipment
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            foreach (var shipmentItem in _shipmentService.GetShipmentItemsByShipmentId(shipment.Id))
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = _productService.GetProductById(orderItem.ProductId);

                var shipmentItemModel = new ShipmentDetailsModel.ShipmentItemModel
                {
                    Id = shipmentItem.Id,
                    Sku = _productService.FormatSku(product, orderItem.AttributesXml),
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    AttributeInfo = orderItem.AttributeDescription,
                    QuantityOrdered = orderItem.Quantity,
                    QuantityShipped = shipmentItem.Quantity,
                };
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(shipmentItemModel);
            }

            //order details model
            model.Order = PrepareOrderDetailsModel(order);

            return model;
        }

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        public virtual CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page)
        {
            //get reward points history
            var customer = _workContext.CurrentCustomer;
            var store = _storeContext.CurrentStore;
            var pageSize = _rewardPointsSettings.PageSize;
            var rewardPoints = _rewardPointService.GetRewardPointsHistory(customer.Id, store.Id, true, pageIndex: --page ?? 0, pageSize: pageSize);

            //prepare model
            var model = new CustomerRewardPointsModel
            {
                RewardPoints = rewardPoints.Select(historyEntry =>
                {
                    var activatingDate = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                    return new CustomerRewardPointsModel.RewardPointsHistoryModel
                    {
                        Points = historyEntry.Points,
                        PointsBalance = historyEntry.PointsBalance.HasValue ? historyEntry.PointsBalance.ToString()
                            : string.Format(_localizationService.GetResource("RewardPoints.ActivatedLater"), activatingDate),
                        Message = historyEntry.Message,
                        CreatedOn = activatingDate,
                        EndDate = !historyEntry.EndDateUtc.HasValue ? null :
                            (DateTime?)_dateTimeHelper.ConvertToUserTime(historyEntry.EndDateUtc.Value, DateTimeKind.Utc)
                    };
                }).ToList(),

                PagerModel = new PagerModel
                {
                    PageSize = rewardPoints.PageSize,
                    TotalRecords = rewardPoints.TotalCount,
                    PageIndex = rewardPoints.PageIndex,
                    ShowTotalSummary = true,
                    RouteActionName = "CustomerRewardPointsPaged",
                    UseRouteLinks = true,
                    RouteValues = new RewardPointsRouteValues { pageNumber = page ?? 0 }
                }
            };

            //current amount/balance
            var rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            var rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            var rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);

            //minimum amount/balance
            var minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            var minimumRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            var minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);

            return model;
        }

        #endregion
    }
}