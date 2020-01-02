using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Api.Helpers
{
    public class CustomerRolesHelper : ICustomerRolesHelper
    {
        private const string CUSTOMERROLES_ALL_BASE_KEY = "Nop.customerrole.all";
        private const string CUSTOMERROLES_ALL_KEY = CUSTOMERROLES_ALL_BASE_KEY + "-{0}";

        private readonly ICustomerService _customerService;
        private readonly ICacheManager _cacheManager;

        public CustomerRolesHelper(ICustomerService customerService, ICacheManager cacheManager)
        {
            _customerService = customerService;
            _cacheManager = cacheManager;
        }

        public IList<CustomerRole> GetValidCustomerRoles(List<int> roleIds)
        {
            // This is needed because the caching messeup the entity framework context
            // and when you try to send something TO the database it throws an exeption.
            _cacheManager.RemoveByPrefix(CUSTOMERROLES_ALL_BASE_KEY);

            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
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