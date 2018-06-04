using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipment item mapping configuration
    /// </summary>
    public partial class ShipmentItemMap : NopEntityTypeConfiguration<ShipmentItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ShipmentItem> builder)
        {
            builder.ToTable(nameof(ShipmentItem));
            builder.HasKey(item => item.Id);

            builder.HasOne(item => item.Shipment)
                .WithMany(shipment => shipment.ShipmentItems)
                .HasForeignKey(item => item.ShipmentId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}