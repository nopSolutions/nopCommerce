using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public partial class ShipmentMap : NopEntityTypeConfiguration<Shipment>
    {
        public override void Configure(EntityTypeBuilder<Shipment> builder)
        {
            base.Configure(builder);
            builder.ToTable("Shipment");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TotalWeight);

            builder.HasOne(s => s.Order)
                .WithMany(o => o.Shipments)
                .IsRequired(true)
                .HasForeignKey(s => s.OrderId);
        }
    }
}