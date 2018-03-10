using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public class ProductAvailabilityRangeMap : NopEntityTypeConfiguration<ProductAvailabilityRange>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductAvailabilityRangeMap()
        {
            this.ToTable("ProductAvailabilityRange");
            this.HasKey(range => range.Id);
            this.Property(range => range.Name).IsRequired().HasMaxLength(400);
        }
    }
}
