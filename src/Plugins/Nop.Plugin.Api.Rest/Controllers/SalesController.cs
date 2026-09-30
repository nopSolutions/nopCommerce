using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Plugin.Api.Rest.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/sales")]
    public class SalesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public SalesController(IOrderService orderService) => _orderService = orderService;

        // GET api/rest/sales/totals?from=2023-01-01&to=2023-12-31
        [HttpGet("totals")]
        public async Task<IActionResult> Totals([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            // Use service search to avoid loading unrelated orders
            var orders = await _orderService.SearchOrdersAsync(createdFromUtc: from, createdToUtc: to);

            var total = orders.Sum(o => o.OrderTotal);
            return Ok(new SalesDto { Total = total, Count = orders.Count });
        }
    }
}
