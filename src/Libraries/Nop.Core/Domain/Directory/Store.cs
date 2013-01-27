
namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents a store
    /// </summary>
    public partial class Store : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
    }
}
