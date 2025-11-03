using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.Manufacturers;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
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
    [AuthorizePermission("ManageOrders")]
    public class OrderItemsController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderItemApiService _orderItemApiService;
        private readonly IOrderService _orderService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductApiService _productApiService;
        private readonly ITaxService _taxService;

        public OrderItemsController(
            IJsonFieldsSerializer jsonFieldsSerializer,
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
        [Route("/api/orders/{orderId}/items", Name = "GetOrderItems")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderItems([FromRoute] int orderId, [FromQuery] OrderItemsParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var allOrderItemsForOrder = await _orderItemApiService.GetOrderItemsForOrderAsync(order, parameters.Limit, parameters.Page,
                                                           parameters.SinceId);

            var orderItemsRootObject = new OrderItemsRootObject
            {
                OrderItems = await allOrderItemsForOrder.SelectAwait(async item => await _dtoHelper.PrepareOrderItemDTOAsync(item)).ToListAsync()
            };

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [Route("/api/orders/{orderId}/items/count", Name = "GetOrderItemsCount")]
        [ProducesResponseType(typeof(OrderItemsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderItemsCount([FromRoute] int orderId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItemsCountForOrder = await _orderItemApiService.GetOrderItemsCountAsync(order);

            var orderItemsCountRootObject = new OrderItemsCountRootObject
            {
                Count = orderItemsCountForOrder
            };

            return Ok(orderItemsCountRootObject);
        }

        [HttpGet]
        [Route("/api/orders/{orderId}/items/{orderItemId}", Name = "GetOrderItemByIdForOrder")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetOrderItemByIdForOrder([FromRoute] int orderId, [FromRoute] int orderItemId, [FromQuery] string fields = "")
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId);

            if (orderItem == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            var orderItemDtos = new List<OrderItemDto>
                                {
                                    await _dtoHelper.PrepareOrderItemDTOAsync(orderItem)
                                };

            var orderItemsRootObject = new OrderItemsRootObject
            {
                OrderItems = orderItemDtos
            };

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/orders/{orderId}/items", Name = "CreateOrderItem")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateOrderItem(
            [FromRoute]
            int orderId,
            [FromBody]
            OrderItemDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = new Dictionary<string, object>();
            
            foreach (var jsonProperty in jsonProperties)
            {
                objectPropertyNameValuePairs.Add(jsonProperty.Name, jsonProperty.Value);
            }
            
            var orderItemDelta = new Delta<OrderItemDto>(objectPropertyNameValuePairs);
            orderItemDelta.Dto = dto;
            
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

            var newOrderItem = await PrepareDefaultOrderItemFromProductAsync(order, product);
            orderItemDelta.Merge(newOrderItem);
            await _orderService.InsertOrderItemAsync(newOrderItem);

            await _orderService.UpdateOrderAsync(order);

            await CustomerActivityService.InsertActivityAsync("AddNewOrderItem", await LocalizationService.GetResourceAsync("ActivityLog.AddNewOrderItem"), newOrderItem);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(newOrderItem.ToDto());

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/orders/{orderId}/items/{orderItemId}", Name = "UpdateOrderItem")]
        [ProducesResponseType(typeof(OrderItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateOrderItem([FromRoute] int orderId, [FromRoute] int orderItemId,
            [FromBody]
            
            OrderItemDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = new Dictionary<string, object>();
            
            foreach (var jsonProperty in jsonProperties)
            {
                objectPropertyNameValuePairs.Add(jsonProperty.Name, jsonProperty.Value);
            }
            
            var orderItemDelta = new Delta<OrderItemDto>(objectPropertyNameValuePairs);
            orderItemDelta.Dto = dto;


            
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var orderItemToUpdate = await _orderService.GetOrderItemByIdAsync(orderItemId);

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

            orderItemToUpdate.ProductId = (int)productId;
            orderItemToUpdate.RentalStartDateUtc = rentalStartDate;
            orderItemToUpdate.RentalEndDateUtc = rentalEndDate;

            await _orderService.UpdateOrderAsync(order);

            await CustomerActivityService.InsertActivityAsync("UpdateOrderItem", await LocalizationService.GetResourceAsync("ActivityLog.UpdateOrderItem"), orderItemToUpdate);

            var orderItemsRootObject = new OrderItemsRootObject();

            orderItemsRootObject.OrderItems.Add(orderItemToUpdate.ToDto());

            var json = JsonFieldsSerializer.Serialize(orderItemsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{orderId}/items/{orderItemId}", Name = "DeleteOrderItemById")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteOrderItemById([FromRoute] int orderId, [FromRoute] int orderItemId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId);
            await _orderService.DeleteOrderItemAsync(orderItem);

            return new RawJsonActionResult("{}");
        }

        [HttpDelete]
        [Route("/api/orders/{orderId}/items", Name = "DeleteAllOrderItemsForOrder")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteAllOrderItemsForOrder([FromRoute] int orderId)
        {
            var order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            foreach (var item in orderItems)
            {
                await _orderService.DeleteOrderItemAsync(item);
            }

            return new RawJsonActionResult("{}");
        }

		#region Private methods

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

        private async Task<OrderItem> PrepareDefaultOrderItemFromProductAsync(Order order, Product product)
        {
            var customer = await CustomerService.GetCustomerByIdAsync(order.CustomerId);
            var store = await StoreService.GetStoreByIdAsync(order.StoreId);
            var presetQty = 1;
            var price = await _priceCalculationService.GetFinalPriceAsync(product, customer,  store,decimal.Zero, true, presetQty);

            var (priceInclTax, _) = await _taxService.GetProductPriceAsync(product, price.finalPrice, includingTax: true, customer);
            var (priceExclTax, _) = await _taxService.GetProductPriceAsync(product, price.finalPrice, includingTax: false, customer);

            var orderItem = new OrderItem
            {
                OrderItemGuid = new Guid(),
                UnitPriceExclTax = priceExclTax,
                UnitPriceInclTax = priceInclTax,
                PriceInclTax = priceInclTax,
                PriceExclTax = priceExclTax,
                OriginalProductCost = await _priceCalculationService.GetProductCostAsync(product, null),
                Quantity = presetQty,
                ProductId = product.Id,
                OrderId = order.Id
            };

            return orderItem;
        }

		#endregion
	}
}
