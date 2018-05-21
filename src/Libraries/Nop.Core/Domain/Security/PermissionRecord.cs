using System.Collections.Generic;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record
    /// </summary>
    public partial class PermissionRecord : BaseEntity
    {
        private ICollection<PermissionRecordCustomerRoleMapping> _permissionRecordCustomerRoleMappings;

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
        /// Gets or sets the permission record-customer role mappings
        /// </summary>
        public virtual ICollection<PermissionRecordCustomerRoleMapping> PermissionRecordCustomerRoleMappings
        {
            get => _permissionRecordCustomerRoleMappings ?? (_permissionRecordCustomerRoleMappings = new List<PermissionRecordCustomerRoleMapping>());
            protected set => _permissionRecordCustomerRoleMappings = value;
        }   
    }
}