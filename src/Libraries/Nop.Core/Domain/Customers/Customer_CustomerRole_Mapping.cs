using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Customers
{
    public class Customer_CustomerRole_Mapping
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int CustomerRoleId { get; set; }
        public virtual CustomerRole CustomerRole { get; set; }
    }
}
