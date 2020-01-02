using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrderItemsParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme,
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderItemsController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderItemApiService _orderItemApiService;
        private readonly IOrderService _orderService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductApiService _productApiService;
        private readonly ITaxService _taxService;

        public OrderItemsController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IOrderItemApiService orderItemApiService,
            IOrderApiService orderApiService,
            IOrderService orderService,
            IProductApiService productApiService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IPictureService pictureService, IDTOHelper dtoHelper)
            : base(jsonFieldsSerializer,
                aclService,
                customerService,
                storeMappingService,
                storeService,
                discountService,
                customerActivityService,
                localizationService,
                pictureService)
        {
            _orderItemApiService = orderItemApiService;
            _orderApiService = orderApiService;
            _orderService = orderService;
            _productApiService = productApiService;
            _priceCalculationService = priceCalculationService;
            _taxService = taxService;
            _dtoHelper = dtoHelper;
        }

        [HttpGet]
        [Route("/api/orders/{orderId}/items")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderItems(int orderId, OrderItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var allOrderItemsForOrder =
                _orderItemApiService.GetOrderItemsForOrder(order, parameters.Limit, parameters.Page,
                    parameters.SinceId);

            var orderItemsRootObject = new OrderItemsRootObject
            {
                OrderItems = allOrderItemsForOrder.Select(item => _dtoHelper.PrepareOrderItemDTO(item)).ToList()
            };

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [Route("/api/orders/{orderId}/items/count")]
        [ProducesResponseType(typeof(OrderItemsCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderItemsCount(int orderId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItemsCountForOrder = _orderItemApiService.GetOrderItemsCount(order);

            var orderItemsCountRootObject = new OrderItemsCountRootObject
            {
                Count = orderItemsCountForOrder
            };

            return Ok(orderItemsCountRootObject);
        }

        [HttpGet]
        [Route("/api/orders/{orderId}/items/{orderItemId}")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderItemByIdForOrder(int orderId, int orderItemId, string fields = "")
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItem = _orderService.GetOrderItemById(orderItemId);

            if (orderItem == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            var orderItemDtos = new List<OrderItemDto> {_dtoHelper.PrepareOrderItemDTO(orderItem)};

            var orderItemsRootObject = new OrderItemsRootObject
            {
                OrderItems = orderItemDtos
            };

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/orders/{orderId}/items")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult CreateOrderItem(int orderId,
            [ModelBinder(typeof(JsonModelBinder<OrderItemDto>))]
            Delta<OrderItemDto> orderItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var product = GetProduct(orderItemDelta.Dto.ProductId);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            if (product.IsRental)
            {
                if (orderItemDelta.Dto.RentalStartDateUtc == null)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc", "required");
                }

                if (orderItemDelta.Dto.RentalEndDateUtc == null)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_end_date_utc", "required");
                }

                if (orderItemDelta.Dto.RentalStartDateUtc > orderItemDelta.Dto.RentalEndDateUtc)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc",
                        "should be before rental_end_date_utc");
                }

                if (orderItemDelta.Dto.RentalStartDateUtc < DateTime.UtcNow)
                {
                    return Error(HttpStatusCode.BadRequest, "rental_start_date_utc", "should be a future date");
                }
            }

            var newOrderItem = PrepareDefaultOrderItemFromProduct(order, product);
            orderItemDelta.Merge(newOrderItem);

            order.OrderItems.Add(newOrderItem);

            _orderService.UpdateOrder(order);

            CustomerActivityService.InsertActivity("AddNewOrderItem",
                LocalizationService.GetResource("ActivityLog.AddNewOrderItem"), newOrderItem);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(newOrderItem.ToDto());

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/orders/{orderId}/items/{orderItemId}")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult UpdateOrderItem(int orderId, int orderItemId,
            [ModelBinder(typeof(JsonModelBinder<OrderItemDto>))]
            Delta<OrderItemDto> orderItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var orderItemToUpdate = _orderService.GetOrderItemById(orderItemId);

            if (orderItemToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            // This is needed because those fields shouldn't be updatable. That is why we save them and after the merge set them back.
            int? productId = orderItemToUpdate.ProductId;
            var rentalStartDate = orderItemToUpdate.RentalStartDateUtc;
            var rentalEndDate = orderItemToUpdate.RentalEndDateUtc;

            orderItemDelta.Merge(orderItemToUpdate);

            orderItemToUpdate.ProductId = (int) productId;
            orderItemToUpdate.RentalStartDateUtc = rentalStartDate;
            orderItemToUpdate.RentalEndDateUtc = rentalEndDate;

            _orderService.UpdateOrder(order);

            CustomerActivityService.InsertActivity("UpdateOrderItem",
                LocalizationService.GetResource("ActivityLog.UpdateOrderItem"), orderItemToUpdate);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(orderItemToUpdate.ToDto());

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{orderId}/items/{orderItemId}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteOrderItemById(int orderId, int orderItemId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItem = _orderService.GetOrderItemById(orderItemId);
            _orderService.DeleteOrderItem(orderItem);

            return new RawJsonActionResult("{}");
        }

        [HttpDelete]
        [Route("/api/orders/{orderId}/items")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteAllOrderItemsForOrder(int orderId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItemsList = order.OrderItems.ToList();

            foreach (var t in orderItemsList)
            {
                _orderService.DeleteOrderItem(t);
            }

            return new RawJsonActionResult("{}");
        }

        private Product GetProduct(int? productId)
        {
            Product product = null;

            if (productId.HasValue)
            {
                var id = productId.Value;

                product = _productApiService.GetProductById(id);
            }

            return product;
        }

        private OrderItem PrepareDefaultOrderItemFromProduct(Order order, Product product)
        {
            var presetQty = 1;
            var presetPrice =
                _priceCalculationService.GetFinalPrice(product, order.Customer, decimal.Zero, true, presetQty);

            var presetPriceInclTax =
                _taxService.GetProductPrice(product, presetPrice, true, order.Customer, out _);
            var presetPriceExclTax =
                _taxService.GetProductPrice(product, presetPrice, false, order.Customer, out _);

            var orderItem = new OrderItem
            {
                OrderItemGuid = new Guid(),
                UnitPriceExclTax = presetPriceExclTax,
                UnitPriceInclTax = presetPriceInclTax,
                PriceInclTax = presetPriceInclTax,
                PriceExclTax = presetPriceExclTax,
                OriginalProductCost = _priceCalculationService.GetProductCost(product, null),
                Quantity = presetQty,
                Product = product,
                Order = order
            };

            return orderItem;
        }
    }
}