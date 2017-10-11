using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    public partial class StorePickupPointMap : NopEntityTypeConfiguration<StorePickupPoint>
    {
        public override void Configure(EntityTypeBuilder<StorePickupPoint> builder)
        {
            builder.ToTable("StorePickupPoint");
            builder.HasKey(point => point.Id);
            builder.Property(point => point.PickupFee);
        }
    }
}