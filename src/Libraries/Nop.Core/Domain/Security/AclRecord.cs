using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// 权限记录表
    /// Represents an ACL record
    /// </summary>
    public partial class AclRecord : BaseEntity
    {
        /// <summary>
        /// 实体编号
        /// Gets or sets the entity identifier
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// 实体名称
        /// Gets or sets the entity name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 客户角色编号
        /// Gets or sets the customer role identifier
        /// </summary>
        public int CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the customer role
        /// </summary>
        public virtual CustomerRole CustomerRole { get; set; }
    }
}
