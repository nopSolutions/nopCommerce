using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Services.Orders;
using Nop.Services.Shipping;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/shipments")]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;
        private readonly IOrderProcessingService _orderProcessingService;

        public ShipmentsController(IShipmentService shipmentService,
            IOrderProcessingService orderProcessingService)
        {
            _shipmentService = shipmentService;
            _orderProcessingService = orderProcessingService;
        }

        // GET api/rest/shipments/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShipmentDto>> GetById(int id)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null) return NotFound();

            var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(id);

            return Ok(shipment.ToDto(items));
        }

        // POST api/rest/shipments/{id}/ship
        [HttpPost("{id:int}/ship")]
        public async Task<ActionResult<ShipmentDto>> Ship(int id, [FromBody] ShipShipmentRequest request = null)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(id);
            if (shipment == null) return NotFound();

            if (shipment.ShippedDateUtc.HasValue)
                return BadRequest(new { error = $"Shipment {id} has already been shipped." });

            await _orderProcessingService.ShipAsync(shipment, request?.NotifyCustomer ?? false);

            var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(id);

            return Ok(shipment.ToDto(items));
        }

        // GET api/rest/shipments?orderId=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<ShipmentDto>>> GetAll([FromQuery] int? orderId = null)
        {
            if (!orderId.HasValue)
                return BadRequest(new { error = "An 'orderId' query value is required." });

            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderId.Value);

            var result = new List<ShipmentDto>();
            foreach (var shipment in shipments)
            {
                var items = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);
                result.Add(shipment.ToDto(items));
            }

            return Ok(PagingHelper.ToInMemoryPagedResult(result, 0, PagingHelper.MAX_PAGE_SIZE));
        }
    }
}
