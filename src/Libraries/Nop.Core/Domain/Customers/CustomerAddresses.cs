using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Customers
{
    public class CustomerAddresses
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; }
    }
}
