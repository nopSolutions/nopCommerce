using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Helpers
{
    public class CustomerRolesHelper : ICustomerRolesHelper
    {
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";
        private readonly IStaticCacheManager _cacheManager;

        private readonly ICustomerService _customerService;

        public CustomerRolesHelper(ICustomerService customerService, IStaticCacheManager cacheManager)
        {
            _customerService = customerService;
            _cacheManager = cacheManager;
        }

        public async Task<IList<CustomerRole>> GetValidCustomerRolesAsync(List<int> roleIds)
        {
            // This is needed because the caching messes up the entity framework context
            // and when you try to send something TO the database it throws an exception.
            await _cacheManager.RemoveByPrefixAsync(CUSTOMERROLES_ALL_KEY);

            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
            {
                if (roleIds != null && roleIds.Contains(customerRole.Id))
                {
                    newCustomerRoles.Add(customerRole);
                }
            }

            return newCustomerRoles;
        }

        public bool IsInGuestsRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
        }

        public bool IsInRegisteredRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
        }

    }
}
