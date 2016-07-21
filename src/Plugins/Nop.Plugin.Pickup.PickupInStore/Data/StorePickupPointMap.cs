using Nop.Data.Mapping;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    public partial class StorePickupPointMap : NopEntityTypeConfiguration<StorePickupPoint>
    {
        public StorePickupPointMap()
        {
            this.ToTable("StorePickupPoint");
            this.HasKey(point => point.Id);
            this.Property(point => point.PickupFee).HasPrecision(18, 4);
        }
    }
}