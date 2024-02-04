using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Factories
{
    public class CustomerFactory : IFactory<Customer>
    {
        public Task<Customer> InitializeAsync()
        {
            var defaultCustomer = new Customer
                                  {
                                      CustomerGuid = Guid.NewGuid(),
                                      CreatedOnUtc = DateTime.UtcNow,
                                      LastActivityDateUtc = DateTime.UtcNow,
                                      Active = true
                                  };

            return Task.FromResult(defaultCustomer);
        }
    }
}
