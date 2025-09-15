using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers.Api;

/// <summary>
/// Orders API controller
/// </summary>
public class OrdersController : BaseApiController
{
    #region Fields

    private readonly IOrderService _orderService;

    #endregion

    #region Ctor

    public OrdersController(
        IWorkContext workContext,
        IPermissionService permissionService,
        ICustomerService customerService,
        IOrderService orderService)
        : base(workContext, permissionService, customerService)
    {
        _orderService = orderService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all orders
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    public async Task<IActionResult> GetOrders(int pageIndex = 0, int pageSize = 10)
    {
        if (!await HasPermissionAsync(StandardPermission.Orders.ORDERS_VIEW))
            return Forbid();

        var orders = await _orderService.SearchOrdersAsync(
            pageIndex: pageIndex,
            pageSize: pageSize);

        var orderDtos = orders.Select(o => new OrderDto
        {
            Id = o.Id,
            OrderGuid = o.OrderGuid,
            CustomerId = o.CustomerId,
            OrderTotal = o.OrderTotal,
            OrderStatus = o.OrderStatus.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            ShippingStatus = o.ShippingStatus.ToString(),
            CreatedOnUtc = o.CreatedOnUtc
        });

        return Ok(orderDtos);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOrder(int id)
    {
        if (!await HasPermissionAsync(StandardPermission.Orders.ORDERS_VIEW))
            return Forbid();

        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        var orderDto = new OrderDto
        {
            Id = order.Id,
            OrderGuid = order.OrderGuid,
            CustomerId = order.CustomerId,
            OrderTotal = order.OrderTotal,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            ShippingStatus = order.ShippingStatus.ToString(),
            CreatedOnUtc = order.CreatedOnUtc
        };

        return Ok(orderDto);
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Order DTO
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Order ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Order GUID
        /// </summary>
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        public string OrderStatus { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        public string PaymentStatus { get; set; }

        /// <summary>
        /// Shipping status
        /// </summary>
        public string ShippingStatus { get; set; }

        /// <summary>
        /// Created on UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }

    #endregion
}