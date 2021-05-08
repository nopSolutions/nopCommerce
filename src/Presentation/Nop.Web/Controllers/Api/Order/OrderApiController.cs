using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/order")]
    [Authorize]
    public class OrderApiController : BaseApiController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        #endregion

        #region Ctor

        public OrderApiController(ICurrencyService currencyService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IProductService productService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IVendorService vendorService,
            IDateTimeHelper dateTimeHelper,
            IPaymentService paymentService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService)
        {
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _currencyService = currencyService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
            _vendorService = vendorService;
            _dateTimeHelper = dateTimeHelper;
            _paymentService = paymentService;
            _orderProcessingService = orderProcessingService;
            _orderTotalCalculationService = orderTotalCalculationService;
        }

        #endregion

        #region Order


        [HttpGet("addtocart/productId/{productId}/quantity/{quantity}")]
        public virtual IActionResult AddProductToCart(int productId = 0, int quantity = 0)
        {
            if (quantity <= 0)
                return Ok(new { success = false, message = "Quantity should be > 0" });

            var customer = _workContext.CurrentCustomer;
            if (customer == null)
                return Ok(new { success = false, message = "invalid customer" });

            var product = _productService.GetProductById(productId);
            if (product == null)
                return Ok(new { success = false, message = "No product found" });

            var cartType = (ShoppingCartType)1;
            if (product.OrderMinimumQuantity > quantity)
                return NotFound("Quantity should be > 0");

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, cartType, _storeContext.CurrentStore.Id);
            var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = _shoppingCartService
                .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
                product, _storeContext.CurrentStore.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                if (addToCartWarnings.Contains("The maximum number of distinct products allowed in the cart is 10."))
                    return Ok("The maximum number of distinct products allowed in the cart is 10.");

                //cannot be added to the cart
                //let's display standard warnings
                return Ok(string.Join(" , ", addToCartWarnings.ToArray().ToString()));
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = _shoppingCartService.AddToCart(customer: _workContext.CurrentCustomer,
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.CurrentStore.Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Ok(string.Join(" , ", addToCartWarnings.ToArray().ToString()));
            }

            return Ok(_localizationService.GetResource("Product.Added.Successfully.To.Cart"));
        }

        [HttpGet("check-products/{productids}")]
        public IActionResult CheckProducts(string productids)
        {
            var errorList = new List<object>();
            int[] ids = null;
            if (!string.IsNullOrEmpty(productids))
                ids = Array.ConvertAll(productids.Split(","), s => int.Parse(s));

            var products = _productService.GetProductsByIds(ids);
            foreach (var product in products)
            {
                if (!product.Published || product.Deleted)
                    errorList.Add(new { Id = product, message = product.Name + " is not published" });

                if (!errorList.Any())
                    AddProductToCart(product.Id, 1);
            }

            var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);
            var cartTotal = _orderTotalCalculationService.GetShoppingCartTotal(cart, false);
            if (errorList.Any())
                return Ok(new { success = false, errorList, cartTotal });

            return Ok(new { success = true, message = "All products are fine", cartTotal });
        }

        [HttpPost("order-confirmation")]
        public IActionResult OrderConfirmation(string scheduleDate)
        {
            var processPaymentRequest = new ProcessPaymentRequest();

            _paymentService.GenerateOrderGuid(processPaymentRequest);
            processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
            processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
            processPaymentRequest.PaymentMethodSystemName = "Payments.CheckMoneyOrder";
            var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
            if (placeOrderResult.Success)
            {
                placeOrderResult.PlacedOrder.ScheduleDate = Convert.ToDateTime(scheduleDate);
                _orderService.UpdateOrder(placeOrderResult.PlacedOrder);

                return Ok(new { success = true, message = "Order Placed Successfully" });
            }
            return Ok(new { success = false, message = "Error while placing order" });
        }

        [HttpGet("get-todays-orders")]
        public IActionResult GetTodaysOrders()
        {
            var perviousOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id).Where(x => x.CreatedOnUtc == DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.WorkingLanguage.Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
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
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage.Id);

                    var orderItems = _orderService.GetOrderItems(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        var vendor = _vendorService.GetVendorByProductId(product.Id);
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
                            VendorLogoPictureUrl = _pictureService.GetPictureUrl(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
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
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (_orderService.IsDownloadAllowed(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (_orderService.IsLicenseDownloadAllowed(orderItem))
                            orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
                    }
                    model.Orders.Add(orderModel);
                }
                return Ok(new { success = true, model });
            }
            return Ok(new { success = false, message = "No previous order found" });
        }

        [HttpGet("get-previous-orders")]
        public IActionResult GetPreviousOrders()
        {
            var perviousOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id).Where(x => x.CreatedOnUtc <= DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.WorkingLanguage.Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
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
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage.Id);

                    var orderItems = _orderService.GetOrderItems(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        var vendor = _vendorService.GetVendorByProductId(product.Id);
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
                            VendorLogoPictureUrl = _pictureService.GetPictureUrl(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
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
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (_orderService.IsDownloadAllowed(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (_orderService.IsLicenseDownloadAllowed(orderItem))
                            orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
                    }
                    model.Orders.Add(orderModel);
                }
                return Ok(new { success = true, model });
            }
            return Ok(new { success = false, message = "No previous order found" });
        }

        [HttpGet("get-upcoming-orders")]
        public IActionResult GetUpcomingOrders()
        {
            var perviousOrders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id).Where(x => x.ScheduleDate >= DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.WorkingLanguage.Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
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
                        CustomOrderNumber = order.CustomOrderNumber,
                        ScheduleDate = order.ScheduleDate.ToString()
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage.Id);
                    var orderItems = _orderService.GetOrderItems(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        var vendor = _vendorService.GetVendorByProductId(product.Id);
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
                            VendorLogoPictureUrl = _pictureService.GetPictureUrl(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
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
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (_orderService.IsDownloadAllowed(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (_orderService.IsLicenseDownloadAllowed(orderItem))
                            orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
                    }
                    model.Orders.Add(orderModel);
                }
                return Ok(new { success = true, model });
            }
            return Ok(new { success = false, message = "No upcoming order found" });
        }

        #endregion

    }
}
