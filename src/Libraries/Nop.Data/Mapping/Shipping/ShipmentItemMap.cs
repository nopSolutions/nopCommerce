using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ShipmentItemMap : NopEntityTypeConfiguration<ShipmentItem>
    {
        public override void Configure(EntityTypeBuilder<ShipmentItem> builder)
        {
            base.Configure(builder);
            builder.ToTable("ShipmentItem");
            builder.HasKey(si => si.Id);

            builder.HasOne(si => si.Shipment)
                .WithMany(s => s.ShipmentItems)
                .IsRequired(true)
                .HasForeignKey(si => si.ShipmentId);
        }
    }
}