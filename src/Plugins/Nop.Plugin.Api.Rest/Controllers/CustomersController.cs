using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService) => _customerService = customerService;

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer.ToDto());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var customers = await _customerService.GetAllCustomersAsync(pageIndex: page - 1, pageSize: pageSize);
            var dto = customers.Select(c => c.ToDto());
            return Ok(dto);
        }
    }
}
