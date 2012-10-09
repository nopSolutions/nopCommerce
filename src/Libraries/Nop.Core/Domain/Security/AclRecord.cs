namespace Nop.Core.Domain.Security
{
    /// <summary>
    /// Represents an ACL record
    /// </summary>
    public partial class AclRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity name
        /// </summary>
        public virtual string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public virtual int CustomerRoleId { get; set; }
    }
}
