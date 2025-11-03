using System;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Api.Factories
{
    public class AddressFactory : IFactory<Address>
    {
        public Task<Address> InitializeAsync()
        {
            var address = new Address
                          {
                              CreatedOnUtc = DateTime.UtcNow
                          };

            return Task.FromResult(address);
        }
    }
}
