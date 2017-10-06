using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Security;

namespace Nop.Core.Domain.Customers
{
    public class CustomerRole_PermissionRecord
    {
        public int CustomerRoleId { get; set; }
        public virtual CustomerRole CustomerRole { get; set; }

        public int PermissionRecordId { get; set; }
        public virtual PermissionRecord PermissionRecord { get; set; }
    }
}
