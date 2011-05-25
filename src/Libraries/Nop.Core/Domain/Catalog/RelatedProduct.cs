namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a related product
    /// </summary>
    public partial class RelatedProduct : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first product identifier
        /// </summary>
        public virtual int ProductId1 { get; set; }

        /// <summary>
        /// Gets or sets the second product identifier
        /// </summary>
        public virtual int ProductId2 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
    }

}
