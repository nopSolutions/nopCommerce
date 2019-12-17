using System;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Factories
{
    public class CustomerFactory : IFactory<Customer>
    {
        public Customer Initialize()
        {
            var defaultCustomer = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                Active = true
            };

            return defaultCustomer;
        }
    }
}