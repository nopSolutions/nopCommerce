using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Expo.Server.Client;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Companies;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Api.Security;
using Nop.Web.Models.Order;
using TimeZoneConverter;

namespace Nop.Web.Controllers.Api.Security
{
    [Produces("application/json")]
    [Route("api/order")]
    [AuthorizeAttribute]
    public class OrderApiController : BaseApiController
    {
        #region Fields

        private readonly ICustomerService _customerService;
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
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICompanyService _companyService;

        #endregion

        #region Ctor

        public OrderApiController(ICurrencyService currencyService,
            ICustomerService customerService,
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
            IOrderTotalCalculationService orderTotalCalculationService,
            ICustomerActivityService customerActivityService,
            ICompanyService companyService)
        {
            _orderService = orderService;
            _customerService = customerService;
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
            _customerActivityService = customerActivityService;
            _companyService = companyService;
        }

        #endregion

        #region Utility

        protected virtual async Task<CartErrorModel> AddProductToCart(int productId = 0, int quantity = 0)
        {
            if (quantity <= 0)
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = "Quantity should be > 0" });

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = "invalid customer" });

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = "No product found" });

            var cartType = (ShoppingCartType)1;
            if (product.OrderMinimumQuantity > quantity)
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = "Quantity should be > 0" });

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
                    return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = "The maximum number of distinct products allowed in the cart is 10." });
                //cannot be added to the cart
                //let's display standard warnings
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = string.Join(" , ", addToCartWarnings).ToString() });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: cartType,
                storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                return await Task.FromResult(new CartErrorModel { Success = false, Id = productId, Message = string.Join(" , ", addToCartWarnings).ToString() });
            }

            return await Task.FromResult(new CartErrorModel { Success = true, Id = productId, Message = await _localizationService.GetResourceAsync("Product.Added.Successfully.To.Cart") });
        }

        protected virtual async Task LogEditOrderAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            await _customerActivityService.InsertActivityAsync("EditOrder",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
        }

        #endregion

        #region Order
        public class ReOrderModel
        {
            public bool Success { get; set; }
            public int Id { get; set; }
            public int Quantity { get; set; }
            public string Message { get; set; }
        }
        public class CartErrorModel
        {
            public bool Success { get; set; }
            public int Id { get; set; }
            public string Message { get; set; }
        }

        public class OrderRatingModel
        {
            public int OrderId { get; set; }
            public int Rating { get; set; }
            public string RatingText { get; set; }
        }
       
        [HttpPost("cancel-order/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.Cancelled.Failed") });

            //try to get an customer with the order id
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("customer.NotFound") });

            await _orderProcessingService.CancelOrderAsync(order, true);
            await LogEditOrderAsync(order.Id);

            if (customer.OrderStatusNotification)
            {
                var expoSDKClient = new PushApiClient();
                var pushTicketReq = new PushTicketRequest()
                {
                    PushTo = new List<string>() { customer.PushToken },
                    PushTitle = await _localizationService.GetResourceAsync("PushNotification.OrderCancelTitle"),
                    PushBody = await _localizationService.GetResourceAsync("PushNotification.OrderCancelBody")
                };
                var result = await expoSDKClient.PushSendAsync(pushTicketReq);
            }

            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.Cancelled.Successfully") });
        }

        [HttpPost("delete-cart/{ids}")]
        public async Task<IActionResult> DeleteCart(string ids)
        {
            int[] cartIds = null;
            if (!string.IsNullOrEmpty(ids))
                cartIds = Array.ConvertAll(ids.Split(","), s => int.Parse(s));

            var carts = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);
            foreach (var sci in carts)
            {
                if (cartIds.Contains(sci.Id))
                    await _shoppingCartService.DeleteShoppingCartItemAsync(sci);
            }
            return Ok(new { success = false, message = "Cart deleted successfully" });
        }

        [HttpGet("check-products/{productids}/{quantities}")]
        public async Task<IActionResult> CheckProducts(string productids, string quantities)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var carts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);
            foreach (var item in carts)
                await _shoppingCartService.DeleteShoppingCartItemAsync(item.Id);

            var errorList = new List<CartErrorModel>();
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
                    errorList.Add(new CartErrorModel { Success = false, Id = product.Id, Message = product.Name + " is not valid" });

                errorList.Add(await AddProductToCart(product.Id, qtys[counter]));

                counter++;
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, _storeContext.GetCurrentStore().Id);

            var cartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, false);
            if (errorList.Any() && errorList.Where(x => !x.Success).Count() > 0)
                return Ok(new { success = false, errorList = errorList.Where(x => !x.Success), cartTotal = cartTotal.shoppingCartTotal });

            return Ok(new { success = true, message = "All products are fine", cartTotal = cartTotal.shoppingCartTotal });
        }

        [HttpPost("order-confirmation/{scheduleDate}")]
        public async Task<IActionResult> OrderConfirmation(string scheduleDate)
        {
            var processPaymentRequest = new ProcessPaymentRequest();
            var customer = await _workContext.GetCurrentCustomerAsync();
            _paymentService.GenerateOrderGuid(processPaymentRequest);
            processPaymentRequest.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
            processPaymentRequest.CustomerId = customer.Id;
            
            var company = await _companyService.GetCompanyByCustomerIdAsync(customer.Id);
            var timezoneInfo = TZConvert.GetTimeZoneInfo(company.TimeZone);
            processPaymentRequest.ScheduleDate = _dateTimeHelper.ConvertToUtcTime(
                Convert.ToDateTime(scheduleDate), 
                timezoneInfo)
                .ToString("MM/dd/yyyy HH:mm:ss");
            
            processPaymentRequest.PaymentMethodSystemName = "Payments.CheckMoneyOrder";
            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
            
            return Ok(new { success = placeOrderResult.Success, 
                message = placeOrderResult.Success ? 
                    await _localizationService.GetResourceAsync("Order.Placed.Successfully") :
                    string.Join(", ", placeOrderResult.Errors)
            });
        }
        [HttpPost("reorder/{orderId}")]
        public virtual async Task<IActionResult> ReOrderAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.Deleted || (await _workContext.GetCurrentCustomerAsync()).Id != order.CustomerId)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.NoOrderFound") });

            var productsList = new List<ReOrderModel>();
            //move shopping cart items (if possible)
            foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                if (!product.Published || product.Deleted)
                    productsList.Add(new ReOrderModel { Success = false, Id = product.Id, Message = product.Name + " is not valid", Quantity = orderItem.Quantity });
                else
                {
                    productsList.Add(new ReOrderModel { Success = true, Id = product.Id, Quantity = orderItem.Quantity });
                }
            }
            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.ReOrdered"), productsList });
        }
        [HttpPost("order-rating")]
        public async Task<IActionResult> OrderRatingAsync([FromBody] OrderRatingModel model)
        {
            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Order.Rating.Failed") });

            order.Rating = model.Rating;
            order.RatingText = model.RatingText;
            await _orderService.UpdateOrderAsync(order);
            return Ok(new { success = true, message = await _localizationService.GetResourceAsync("Order.Rating.Added") });
        }

        [HttpGet("get-todays-orders")]
        public async Task<IActionResult> GetTodaysOrdersAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.ScheduleDate.Date == DateTime.Now.Date).ToList();
            if (perviousOrders.Any())
            {
                var languageId = _workContext.GetWorkingLanguageAsync().Id;
                var model = new CustomerOrderListModel();
                foreach (var order in perviousOrders)
                {
                    var orderModel = new CustomerOrderListModel.OrderDetailsModel
                    {
                        Id = order.Id,
                        ScheduleDate = await _dateTimeHelper.ConvertToUserTimeAsync(order.ScheduleDate, DateTimeKind.Utc),
                        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                        ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                        IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                        CustomOrderNumber = order.CustomOrderNumber,
                        Rating = order.Rating,
                        RatingText = order.RatingText
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);

                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var productPicture = await _pictureService.GetPicturesByProductIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor != null ? vendor.Name : string.Empty,
                            ProductId = product.Id,
                            ProductPictureUrl = productPicture.Any() ? await _pictureService.GetPictureUrlAsync(productPicture.FirstOrDefault().Id) : await _pictureService.GetDefaultPictureUrlAsync(),
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
        public async Task<IActionResult> GetPreviousOrdersAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.ScheduleDate.Date < DateTime.Now.Date);
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
                        ScheduleDate = await _dateTimeHelper.ConvertToUserTimeAsync(order.ScheduleDate, DateTimeKind.Utc),
                        OrderStatusEnum = order.OrderStatus,
                        OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                        PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                        ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                        IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                        CustomOrderNumber = order.CustomOrderNumber,
                        Rating = order.Rating,
                        RatingText = order.RatingText
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);

                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var productPicture = await _pictureService.GetPicturesByProductIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor != null ? vendor.Name : string.Empty,
                            ProductId = product.Id,
                            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            ProductPictureUrl = productPicture.Any() ? await _pictureService.GetPictureUrlAsync(productPicture.FirstOrDefault().Id) : await _pictureService.GetDefaultPictureUrlAsync(),
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
        public async Task<IActionResult> GetUpcomingOrdersAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);
            var perviousOrders = orders.Where(x => x.ScheduleDate.Date > DateTime.Now.Date);
            if (perviousOrders.Any())
            {
                var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
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
                        ScheduleDate = await _dateTimeHelper.ConvertToUserTimeAsync(order.ScheduleDate, DateTimeKind.Utc),
                        Rating = order.Rating,
                        RatingText = order.RatingText
                    };
                    var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.GetWorkingLanguageAsync().Id);
                    var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

                    foreach (var orderItem in orderItems)
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        var productPicture = await _pictureService.GetPicturesByProductIdAsync(orderItem.ProductId);
                        var vendor = await _vendorService.GetVendorByProductIdAsync(product.Id);
                        var orderItemModel = new OrderDetailsModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = await _productService.FormatSkuAsync(product, orderItem.AttributesXml),
                            VendorName = vendor != null ? vendor.Name : string.Empty,
                            ProductId = product.Id,
                            ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                            ProductPictureUrl = productPicture.Any() ? await _pictureService.GetPictureUrlAsync(productPicture.FirstOrDefault().Id) : await _pictureService.GetDefaultPictureUrlAsync(),
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
