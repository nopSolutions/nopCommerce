using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/order")]
    [AuthorizeAttribute]
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
        public virtual async Task<IActionResult> AddProductToCart(int productId = 0, int quantity = 0)
        {
            if (quantity <= 0)
                return Ok(new { success = false, message = "Quantity should be > 0" });

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Ok(new { success = false, message = "invalid customer" });

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return Ok(new { success = false, message = "No product found" });

            var cartType = (ShoppingCartType)1;
            if (product.OrderMinimumQuantity > quantity)
                return NotFound("Quantity should be > 0");

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, cartType, _storeContext.GetCurrentStore().Id);
            var shoppingCartItem = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = await _shoppingCartService
                .GetShoppingCartItemWarningsAsync(customer, cartType,
                product, _storeContext.GetCurrentStore().Id, string.Empty,
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
            addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: cartType,
                storeId: _storeContext.GetCurrentStore().Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return Ok(string.Join(" , ", addToCartWarnings.ToArray().ToString()));
            }

            return Ok(await _localizationService.GetResourceAsync("Product.Added.Successfully.To.Cart"));
        }

        [HttpGet("check-products/{productids}/{quantities}")]
        public async Task<IActionResult> CheckProducts(string productids, string quantities)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var carts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);
            foreach (var item in carts)
                await _shoppingCartService.DeleteShoppingCartItemAsync(item.Id);

            var errorList = new List<object>();
            int[] ids = null;
            if (!string.IsNullOrEmpty(productids))
                ids = Array.ConvertAll(productids.Split(","), s => int.Parse(s));

            int[] qtys = null;
            if (!string.IsNullOrEmpty(quantities))
                qtys = Array.ConvertAll(quantities.Split(","), s => int.Parse(s));

            var counter = 0;
            var products = await _productService.GetProductsByIdsAsync(ids);
            foreach (var product in products)
            {
                if (!product.Published || product.Deleted)
                    errorList.Add(new { Id = product, message = product.Name + " is not published" });

                if (!errorList.Any())
                    await AddProductToCart(product.Id, qtys[counter]);

                counter++;
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);

            var cartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false);
            if (errorList.Any())
                return Ok(new { success = false, errorList, cartTotal });

            return Ok(new { success = true, message = "All products are fine", cartTotal });
        }

        [HttpPost("order-confirmation")]
        public async Task<IActionResult> OrderConfirmation(string scheduleDate)
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var customer = await _workContext.GetCurrentCustomerAsync();
            _paymentService.GenerateOrderGuid(processPaymentRequest);
            processPaymentRequest.StoreId = _storeContext.GetCurrentStore().Id;
            processPaymentRequest.CustomerId = customer.Id;
            processPaymentRequest.PaymentMethodSystemName = "Payments.CheckMoneyOrder";
            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
            if (placeOrderResult.Success)
            {
                placeOrderResult.PlacedOrder.ScheduleDate = Convert.ToDateTime(scheduleDate);
                await _orderService.UpdateOrderAsync(placeOrderResult.PlacedOrder);

                return Ok(new { success = true, message = "Order Placed Successfully" });
            }
            return Ok(new { success = false, message = "Error while placing order" });
        }

        [HttpGet("get-todays-orders")]
        public async Task<IActionResult> GetTodaysOrders()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.CreatedOnUtc == DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.GetWorkingLanguageAsync().Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
                {
                    var orderModel = new CustomerOrderListModel.OrderDetailsModel
                    {
                        Id = order.Id,
                        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                        ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                        IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                        CustomOrderNumber = order.CustomOrderNumber
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);

                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor?.Name ?? string.Empty,
                            ProductId = product.Id,
                            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            Quantity = orderItem.Quantity,
                            AttributeInfo = orderItem.AttributeDescription,
                            VendorLogoPictureUrl = await _pictureService.GetPictureUrlAsync(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
                        };
                        //rental info
                        if (product.IsRental)
                        {
                            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                            orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                                rentalStartDate, rentalEndDate);
                        }
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (await _orderService.IsDownloadAllowedAsync(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                            orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
                    }
                    model.Orders.Add(orderModel);
                }
                return Ok(new { success = true, model });
            }
            return Ok(new { success = false, message = "No previous order found" });
        }

        [HttpGet("get-previous-orders")]
        public async Task<IActionResult> GetPreviousOrders()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.CreatedOnUtc <= DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.GetWorkingLanguageAsync().Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
                {
                    var orderModel = new CustomerOrderListModel.OrderDetailsModel
                    {
                        Id = order.Id,
                        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                        ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                        IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                        CustomOrderNumber = order.CustomOrderNumber
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);

                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor.Name ?? string.Empty,
                            ProductId = product.Id,
                            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            Quantity = orderItem.Quantity,
                            AttributeInfo = orderItem.AttributeDescription,
                            VendorLogoPictureUrl = await _pictureService.GetPictureUrlAsync(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
                        };
                        //rental info
                        if (product.IsRental)
                        {
                            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                            orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                                rentalStartDate, rentalEndDate);
                        }
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (await _orderService.IsDownloadAllowedAsync(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                            orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
                    }
                    model.Orders.Add(orderModel);
                }
                return Ok(new { success = true, model });
            }
            return Ok(new { success = false, message = "No previous order found" });
        }

        [HttpGet("get-upcoming-orders")]
        public async Task<IActionResult> GetUpcomingOrders()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.ScheduleDate >= DateTime.UtcNow);
            if (perviousOrders.Any())
            {
                var languageId = _workContext.GetWorkingLanguageAsync().Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
                {
                    var orderModel = new CustomerOrderListModel.OrderDetailsModel
                    {
                        Id = order.Id,
                        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                        ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                        IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                        CustomOrderNumber = order.CustomOrderNumber,
                        ScheduleDate = order.ScheduleDate.ToString()
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor.Name ?? string.Empty,
                            ProductId = product.Id,
                            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            Quantity = orderItem.Quantity,
                            AttributeInfo = orderItem.AttributeDescription,
                            VendorLogoPictureUrl = await _pictureService.GetPictureUrlAsync(vendor != null ? vendor.PictureId : 0, showDefaultPicture: true)
                        };
                        //rental info
                        if (product.IsRental)
                        {
                            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                                ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                            orderItemModel.RentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                                rentalStartDate, rentalEndDate);
                        }
                        orderModel.Items.Add(orderItemModel);

                        //unit price, subtotal
                        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                        {
                            //including tax
                            var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                            var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                        }
                        else
                        {
                            //excluding tax
                            var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                            orderItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                            var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                            orderItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                        }

                        //downloadable products
                        if (await _orderService.IsDownloadAllowedAsync(orderItem))
                            orderItemModel.DownloadId = product.DownloadId;
                        if (await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
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
