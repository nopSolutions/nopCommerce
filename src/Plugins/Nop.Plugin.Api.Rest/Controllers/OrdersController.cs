using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using Nop.Services.Shipping;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IAddressService _addressService;

        public OrdersController(IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IProductService productService,
            IShipmentService shipmentService,
            IAddressService addressService)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _productService = productService;
            _shipmentService = shipmentService;
            _addressService = addressService;
        }

        // GET api/rest/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDetailDto>> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return Ok(await BuildDetailAsync(order));
        }

        // GET api/rest/orders?pageIndex=0&pageSize=20&customerId=&billingEmail=&orderStatusId=
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] int? customerId = null,
            [FromQuery] string? billingEmail = null,
            [FromQuery] string? billingPhone = null,
            [FromQuery] int? orderStatusId = null,
            [FromQuery] int? paymentStatusId = null,
            [FromQuery] int? shippingStatusId = null,
            [FromQuery] DateTime? createdFromUtc = null,
            [FromQuery] DateTime? createdToUtc = null)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            var orders = await SearchOrdersAsync(index, size, customerId, billingEmail, billingPhone,
                orderStatusId, paymentStatusId, shippingStatusId, createdFromUtc, createdToUtc);

            return Ok(orders.ToPagedResult().Map(o => o.ToDto()));
        }

        /// <summary>
        /// Looks an order up by the number a merchant assigned to it
        /// </summary>
        /// <remarks>
        /// Kept separate from the list endpoint because it answers with a single record and does not
        /// page, so that a caller holding an order number does not have to guess how deep the order sits
        /// </remarks>
        [HttpGet("search")]
        public async Task<ActionResult<OrderDetailDto>> SearchByOrderNumber([FromQuery] string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return BadRequest(new { error = "An 'orderNumber' query value is required." });

            var order = await _orderService.GetOrderByCustomOrderNumberAsync(orderNumber.Trim());
            if (order == null) return NotFound();

            return Ok(await BuildDetailAsync(order));
        }

        // GET api/rest/orders/{id}/shipments
        [HttpGet("{id:int}/shipments")]
        public async Task<ActionResult<PagedResult<ShipmentDto>>> GetShipments(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(id);

            var result = new List<ShipmentDto>();
            foreach (var shipment in shipments)
            {
                var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
                result.Add(shipment.ToDto(items));
            }

            return Ok(PagingHelper.ToInMemoryPagedResult(result, 0, PagingHelper.MAX_PAGE_SIZE));
        }

        // POST api/rest/orders/{id}/mark-paid
        [HttpPost("{id:int}/mark-paid")]
        public async Task<ActionResult<OrderDetailDto>> MarkAsPaid(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            if (!_orderProcessingService.CanMarkOrderAsPaid(order))
                return BadRequest(new { error = $"Order {id} is {order.OrderStatus} and cannot be marked as paid." });

            await _orderProcessingService.MarkOrderAsPaidAsync(order);

            return Ok(await BuildDetailAsync(order));
        }

        // POST api/rest/orders/{id}/cancel
        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<OrderDetailDto>> Cancel(int id, [FromBody] CancelOrderRequest request = null)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            if (!_orderProcessingService.CanCancelOrder(order))
                return BadRequest(new { error = $"Order {id} is {order.OrderStatus} and cannot be cancelled." });

            await _orderProcessingService.CancelOrderAsync(order, request?.NotifyCustomer ?? false);

            return Ok(await BuildDetailAsync(order));
        }

        // POST api/rest/orders/{id}/shipments
        [HttpPost("{id:int}/shipments")]
        public async Task<ActionResult<ShipmentDto>> CreateShipment(int id, [FromBody] CreateShipmentRequest request)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            if (!await _orderService.HasItemsToAddToShipmentAsync(order))
                return BadRequest(new { error = $"Order {id} has no items left that can be shipped." });

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            var requested = await ResolveShipmentQuantitiesAsync(order, orderItems, request);

            if (requested.Error != null)
                return BadRequest(new { error = requested.Error });

            if (requested.Quantities.Count == 0)
                return BadRequest(new { error = "No shippable items were requested." });

            var shipment = new Shipment
            {
                OrderId = order.Id,
                TrackingNumber = request?.TrackingNumber,
                AdminComment = request?.AdminComment,
                CreatedOnUtc = DateTime.UtcNow
            };

            //the shipment is written first because every item row carries the shipment identifier
            await _shipmentService.InsertShipmentAsync(shipment);

            foreach (var pair in requested.Quantities)
            {
                await _shipmentService.InsertShipmentItemAsync(new ShipmentItem
                {
                    ShipmentId = shipment.Id,
                    OrderItemId = pair.Key,
                    Quantity = pair.Value
                });
            }

            var savedItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

            return Ok(shipment.ToDto(savedItems));
        }

        #region Utilities

        /// <summary>
        /// Runs the shared order query. Zero on any status identifier means "no filter", matching the
        /// convention the rest of the query string uses.
        /// </summary>
        protected virtual async Task<IPagedList<Order>> SearchOrdersAsync(int pageIndex, int pageSize,
            int? customerId, string billingEmail, string billingPhone, int? orderStatusId,
            int? paymentStatusId, int? shippingStatusId, DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            return await _orderService.SearchOrdersAsync(
                customerId: customerId ?? 0,
                createdFromUtc: createdFromUtc,
                createdToUtc: createdToUtc,
                osIds: orderStatusId is > 0 ? new List<int> { orderStatusId.Value } : null,
                psIds: paymentStatusId is > 0 ? new List<int> { paymentStatusId.Value } : null,
                ssIds: shippingStatusId is > 0 ? new List<int> { shippingStatusId.Value } : null,
                billingPhone: billingPhone,
                billingEmail: billingEmail,
                //passed explicitly because the default is a non null empty string, and the service only
                //treats it as "load all records" when it is empty
                billingLastName: string.Empty,
                pageIndex: pageIndex,
                pageSize: pageSize);
        }

        /// <summary>
        /// Decides what each order item ships, either from the request or by taking everything pending
        /// </summary>
        /// <param name="order">The order being shipped</param>
        /// <param name="orderItems">Items of the order</param>
        /// <param name="request">The requested quantities, may be null or empty</param>
        /// <returns>
        /// The order item identifier to quantity map, plus a message when the request cannot be met
        /// </returns>
        /// <remarks>
        /// Reports a problem instead of throwing, because every failure here is a bad request rather
        /// than a server fault
        /// </remarks>
        protected virtual async Task<(Dictionary<int, int> Quantities, string Error)> ResolveShipmentQuantitiesAsync(
            Order order, IList<OrderItem> orderItems, CreateShipmentRequest request)
        {
            var remaining = new Dictionary<int, int>();
            foreach (var orderItem in orderItems)
            {
                remaining[orderItem.Id] =
                    await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
            }

            var quantities = new Dictionary<int, int>();

            if (request?.Items is not { Count: > 0 })
            {
                //no explicit selection, so take everything that is still pending
                foreach (var pair in remaining)
                {
                    if (pair.Value > 0)
                        quantities[pair.Key] = pair.Value;
                }

                return (quantities, null);
            }

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    return (null, "Shipment item quantities must be greater than zero.");

                if (!remaining.ContainsKey(item.OrderItemId))
                    return (null, $"Order item {item.OrderItemId} does not belong to order {order.Id}.");

                if (quantities.ContainsKey(item.OrderItemId))
                    return (null, $"Order item {item.OrderItemId} is listed more than once.");

                if (item.Quantity > remaining[item.OrderItemId])
                    return (null,
                        $"Order item {item.OrderItemId} only has {remaining[item.OrderItemId]} item(s) left to ship, {item.Quantity} requested.");

                quantities[item.OrderItemId] = item.Quantity;
            }

            return (quantities, null);
        }

        /// <summary>
        /// Builds the order detail projection, resolving the product names in a single query
        /// </summary>
        protected virtual async Task<OrderDetailDto> BuildDetailAsync(Order order)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            var addressIds = new List<int> { order.BillingAddressId };
            if (order.ShippingAddressId.HasValue)
                addressIds.Add(order.ShippingAddressId.Value);
            if (order.PickupAddressId.HasValue)
                addressIds.Add(order.PickupAddressId.Value);

            //the address service resolves one record at a time, so this is at most three lookups
            var addresses = new Dictionary<int, Address>();
            foreach (var addressId in addressIds.Distinct().Where(id => id > 0))
            {
                var address = await _addressService.GetAddressByIdAsync(addressId);
                if (address != null)
                    addresses[addressId] = address;
            }

            addresses.TryGetValue(order.BillingAddressId, out var billingAddress);

            Address shippingAddress = null;
            if (order.ShippingAddressId.HasValue)
                addresses.TryGetValue(order.ShippingAddressId.Value, out shippingAddress);
            if (shippingAddress == null && order.PickupAddressId.HasValue)
                addresses.TryGetValue(order.PickupAddressId.Value, out shippingAddress);

            var productIds = orderItems.Select(i => i.ProductId).Distinct().ToArray();
            var products = await _productService.GetProductsByIdsAsync(productIds);
            var productsById = products.ToDictionary(p => p.Id);

            return order.ToDetailDto(orderItems, billingAddress, shippingAddress, productsById);
        }

        #endregion
    }
}
