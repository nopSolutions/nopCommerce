using System.Collections.Generic;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents a default permission record
    /// </summary>
    public class DefaultPermissionRecord
    {
        public DefaultPermissionRecord()
        {
            PermissionRecords = new List<PermissionRecord>();
        }

        /// <summary>
        /// Gets or sets the customer role system name
        /// </summary>
        public string CustomerRoleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the permissions
        /// </summary>
        public IEnumerable<PermissionRecord> PermissionRecords { get; set; }
    }
}