using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            var dto = order.ToDto();
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var orders = await _orderService.SearchOrdersAsync(pageIndex: page - 1, pageSize: pageSize);
            var dto = orders.Select(o => o.ToDto());
            return Ok(dto);
        }
    }
}
