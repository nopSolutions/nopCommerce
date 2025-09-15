using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers.Api;

/// <summary>
/// Customers API controller
/// </summary>
public class CustomersController : BaseApiController
{
    #region Ctor

    public CustomersController(
        IWorkContext workContext,
        IPermissionService permissionService,
        ICustomerService customerService)
        : base(workContext, permissionService, customerService)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all customers
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of customers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), 200)]
    public async Task<IActionResult> GetCustomers(int pageIndex = 0, int pageSize = 10)
    {
        if (!await HasPermissionAsync(StandardPermission.Customers.CUSTOMERS_VIEW))
            return Forbid();

        var customers = await _customerService.GetAllCustomersAsync(
            pageIndex: pageIndex,
            pageSize: pageSize);

        var customerDtos = customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Email = c.Email,
            Username = c.Username,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Phone = c.Phone,
            CreatedOnUtc = c.CreatedOnUtc,
            Active = c.Active
        });

        return Ok(customerDtos);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(int id)
    {
        if (!await HasPermissionAsync(StandardPermission.Customers.CUSTOMERS_VIEW))
            return Forbid();

        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
            return NotFound();

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            Email = customer.Email,
            Username = customer.Username,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            CreatedOnUtc = customer.CreatedOnUtc,
            Active = customer.Active
        };

        return Ok(customerDto);
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Customer DTO
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// Customer ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Created on UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Active status
        /// </summary>
        public bool Active { get; set; }
    }

    #endregion
}