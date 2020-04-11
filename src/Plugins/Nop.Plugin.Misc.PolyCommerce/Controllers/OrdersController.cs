using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOrderModelFactory _orderModelFactory;

        public OrdersController(IRepository<Order> orderRepository, IOrderModelFactory orderModelFactory)
        {
            _orderRepository = orderRepository;
            _orderModelFactory = orderModelFactory;
        }

        [HttpGet]
        [Route("api/polycommerce/get_new_orders")]
        public async Task<IActionResult> GetNewOrders(int page, int pageSize, DateTime minCreatedDate)
        {
            var skipRecords = (page - 1) * pageSize;

            var orders = await _orderRepository.Table
                .Where(x => x.CreatedOnUtc >= minCreatedDate)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet]
        [Route("api/polycommerce/get_orders_by_id")]
        public async Task<IActionResult> GetOrdersById(string commaSeparatedOrderIds)
        {
            commaSeparatedOrderIds = commaSeparatedOrderIds.Trim().Replace(" ", string.Empty);

            var orderIds = commaSeparatedOrderIds.Split(',').Select(x => int.Parse(x));

            var orders = await _orderRepository.Table
                .Where(x => orderIds.Any(y => y == x.Id))
                .ToListAsync();

            var mappedOrders = orders.ConvertAll(x => _orderModelFactory.PrepareOrderModel(null, x));

            return Ok(mappedOrders);
        }

    }
}
