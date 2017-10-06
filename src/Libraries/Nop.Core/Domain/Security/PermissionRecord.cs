using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public partial class PermissionRecord : BaseEntity
    {
        private ICollection<CustomerRole_PermissionRecord> _customerRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<CustomerRole_PermissionRecord> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole_PermissionRecord>()); }
            protected set { _customerRoles = value; }
        }

        public void CustomerRolesAdd(CustomerRole customerRole)
        {
            CustomerRole_PermissionRecord cr = new CustomerRole_PermissionRecord()
            {
                CustomerRole = customerRole,
                CustomerRoleId = customerRole.Id,
                PermissionRecord = this,
                PermissionRecordId = this.Id
            };
            CustomerRoles.Add(cr);
        }

        public void CustomerRolesRemove(CustomerRole customerRole)
        {
            var item = ((List<CustomerRole_PermissionRecord>)CustomerRoles).Find(p => p.CustomerRoleId == customerRole.Id && p.PermissionRecordId == this.Id);
            CustomerRoles.Remove(item);
        }
    }
}
