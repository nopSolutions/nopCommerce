using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public class DeliveryDateMap : NopEntityTypeConfiguration<DeliveryDate>
    {
        public override void Configure(EntityTypeBuilder<DeliveryDate> builder)
        {
            base.Configure(builder);
            builder.ToTable("DeliveryDate");
            builder.HasKey(dd => dd.Id);
            builder.Property(dd => dd.Name).IsRequired().HasMaxLength(400);
        }
    }
}
