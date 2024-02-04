using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Infrastructure;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Authorization.Requirements
{
    public class CustomerRoleRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsCustomerInRoleAsync()
        {
            try
            {
                var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

                var customerIdClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);

                if (customerIdClaim != null && Guid.TryParse(customerIdClaim.Value, out var customerGuid))
                {
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();

                    var customer = await customerService.GetCustomerByGuidAsync(customerGuid);
                   
                    if (customer != null)
                    {
                        var customerRoles = await customerService.GetCustomerRolesAsync(customer);
                        return IsInApiRole(customerRoles);
                    }
                }
            }
            catch
            {
                // best effort
            }

            return false;
        }

        private static bool IsInApiRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == Constants.Roles.ApiRoleSystemName) != null;
        }
    }
}
