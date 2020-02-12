namespace Nop.Core.Domain.Catalog
{
    public partial class ProductTagWithCount
    {
        /// <summary>
        /// Gets or sets the product tag ID
        /// </summary>
        public int ProductTagId { get; set; }

        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int ProductCount { get; set; }
    }
}