namespace Nop.Core.Domain.Stores
{
    /// <summary>
    /// Represents a store mapping record
    /// </summary>
    public partial class StoreMapping : BaseEntity
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
        /// Gets or sets the store identifier
        /// </summary>
        public virtual int StoreId { get; set; }
    }
}
