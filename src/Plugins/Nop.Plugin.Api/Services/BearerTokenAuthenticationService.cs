using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Services
{
	public class BearerTokenAuthenticationService : Nop.Services.Authentication.IAuthenticationService
	{
		// TODO:

		#region Fields

		private readonly CustomerSettings customerSettings;
		private readonly ICustomerService customerService;
		private readonly IHttpContextAccessor httpContextAccessor;

		#endregion

		#region Ctor

		public BearerTokenAuthenticationService(CustomerSettings customerSettings,
			ICustomerService customerService,
			IHttpContextAccessor httpContextAccessor)
		{
			this.customerSettings = customerSettings;
			this.customerService = customerService;
			this.httpContextAccessor = httpContextAccessor;
		}

		#endregion

		#region Methods

		public Task SignInAsync(Customer customer, bool isPersistent)
		{
			return Task.CompletedTask; // bearer token does not support signing in, so it is no-op
		}

		public Task SignOutAsync()
		{
			return Task.CompletedTask; // bearer token does not support signing out, so it is no-op
		}

		public async Task<Customer> GetAuthenticatedCustomerAsync()
		{
			//try to get authenticated user identity
			var authenticateResult = await httpContextAccessor.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
			if (!authenticateResult.Succeeded)
				return null;

			Customer customer = null;
			var identifierClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier);
			if (identifierClaim != null && Guid.TryParse(identifierClaim.Value, out Guid customerGuid))
			{
				customer = await customerService.GetCustomerByGuidAsync(customerGuid);
			}

			//whether the found customer is available
			if (customer == null || !customer.Active || customer.RequireReLogin || customer.Deleted)
				return null;

			return customer;
		}

		#endregion
	}
}
