using LinqToDB.Mapping;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    /// <summary>
    /// Represents a store pickup point mapping configuration
    /// </summary>
    public partial class StorePickupPointMap : NopEntityTypeConfiguration<StorePickupPoint>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        public override void Configure(EntityMappingBuilder<StorePickupPoint> builder)
        {
            builder.HasTableName(nameof(StorePickupPoint));
            builder.Property(storePickupPoint => storePickupPoint.Name);
            builder.Property(storePickupPoint => storePickupPoint.Description);
            builder.Property(storePickupPoint => storePickupPoint.AddressId);
            builder.Property(point => point.PickupFee).HasDbType("decimal(18, 4)");
            builder.Property(storePickupPoint => storePickupPoint.OpeningHours);
            builder.Property(storePickupPoint => storePickupPoint.DisplayOrder);
            builder.Property(storePickupPoint => storePickupPoint.StoreId);
        }

        #endregion
    }
}