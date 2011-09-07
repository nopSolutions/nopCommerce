
namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a manufacturer template
    /// </summary>
    public partial class ManufacturerTemplate : BaseEntity
    {
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the view path
        /// </summary>
        public virtual string ViewPath { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
    }
}
