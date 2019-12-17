using System;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Api.Factories
{
    public class AddressFactory : IFactory<Address>
    {
        public Address Initialize()
        {
            var address = new Address()
            {
                CreatedOnUtc = DateTime.UtcNow
            };

            return address;
        }
    }
}