using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public class ProductAvailabilityRangeMap : NopEntityTypeConfiguration<ProductAvailabilityRange>
    {
        public ProductAvailabilityRangeMap()
        {
            this.ToTable("ProductAvailabilityRange");
            this.HasKey(range => range.Id);
            this.Property(range => range.Name).IsRequired().HasMaxLength(400);
        }
    }
}
