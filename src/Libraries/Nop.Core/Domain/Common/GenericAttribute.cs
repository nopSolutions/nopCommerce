namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Represents a generic attribute
    /// </summary>
    public partial class GenericAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public virtual int EntityId { get; set; }
        
        /// <summary>
        /// Gets or sets the key group
        /// </summary>
        public virtual string KeyGroup { get; set; }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }
    }
}
