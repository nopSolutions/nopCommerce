using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission record-customer role mapping class
    /// </summary>
    [Table(NopMappingDefaults.PermissionRecordRoleTable)]
    public partial class PermissionRecordCustomerRoleMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [NotColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the permission record identifier
        /// </summary>
        [Column("PermissionRecord_Id")]
        public int PermissionRecordId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        [Column("CustomerRole_Id")]
        public int CustomerRoleId { get; set; }
    }
}