using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET api/rest/customers/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer.ToDto());
        }

        // GET api/rest/customers?pageIndex=0&pageSize=20&email=&firstName=&customerRoleId=&isActive=
        [HttpGet]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] string? email = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? phone = null,
            [FromQuery] int? customerRoleId = null,
            [FromQuery] DateTime? createdFromUtc = null,
            [FromQuery] DateTime? createdToUtc = null,
            [FromQuery] bool? isActive = null)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            var customers = await _customerService.GetAllCustomersAsync(
                createdFromUtc: createdFromUtc,
                createdToUtc: createdToUtc,
                //zero means "no filter", matching the convention the rest of the query string uses
                customerRoleIds: customerRoleId is > 0 ? new[] { customerRoleId.Value } : null,
                email: email,
                firstName: firstName,
                lastName: lastName,
                phone: phone,
                isActive: isActive,
                pageIndex: index,
                pageSize: size);

            return Ok(customers.ToPagedResult().Map(c => c.ToDto()));
        }

        // GET api/rest/customers/{id}/addresses
        [HttpGet("{id:int}/addresses")]
        public async Task<ActionResult<List<AddressDto>>> GetAddresses(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            var addresses = await _customerService.GetAddressesByCustomerIdAsync(id);

            return Ok(addresses.Select(a => a.ToDto()).ToList());
        }
    }
}
