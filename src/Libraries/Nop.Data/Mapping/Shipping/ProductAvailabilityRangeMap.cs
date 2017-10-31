using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public class ProductAvailabilityRangeMap : NopEntityTypeConfiguration<ProductAvailabilityRange>
    {
        public override void Configure(EntityTypeBuilder<ProductAvailabilityRange> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductAvailabilityRange");
            builder.HasKey(range => range.Id);
            builder.Property(range => range.Name).IsRequired().HasMaxLength(400);
        }
    }
}
