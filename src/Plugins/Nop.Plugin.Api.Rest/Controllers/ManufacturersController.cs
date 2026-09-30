using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/manufacturers")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        // GET api/rest/manufacturers?pageIndex=0&pageSize=20&name=
        [HttpGet]
        public async Task<ActionResult<PagedResult<ManufacturerDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] string? name = null)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(name ?? string.Empty,
                pageIndex: index, pageSize: size);

            return Ok(manufacturers.ToPagedResult().Map(m => m.ToDto()));
        }

        // GET api/rest/manufacturers/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ManufacturerDto>> GetById(int id)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null) return NotFound();

            return Ok(manufacturer.ToDto());
        }
    }
}
