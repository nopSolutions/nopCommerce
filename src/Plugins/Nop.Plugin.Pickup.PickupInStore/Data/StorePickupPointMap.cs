using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
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
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<StorePickupPoint> builder)
        {
            builder.ToTable(nameof(StorePickupPoint));
            builder.HasKey(point => point.Id);

            builder.Property(point => point.PickupFee).HasColumnType("decimal(18, 4)");
        }

        #endregion
    }
}