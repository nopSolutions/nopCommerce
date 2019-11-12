using LinqToDB.Mapping;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Represents a shipment mapping configuration
    /// </summary>
    public partial class ShipmentMap : NopEntityTypeConfiguration<Shipment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Shipment> builder)
        {
            builder.HasTableName(nameof(Shipment));

            builder.Property(shipment => shipment.TotalWeight).HasDbType("decimal(18, 4)");

            builder.Property(shipment => shipment.OrderId);
            builder.Property(shipment => shipment.TrackingNumber);
            builder.Property(shipment => shipment.ShippedDateUtc);
            builder.Property(shipment => shipment.DeliveryDateUtc);
            builder.Property(shipment => shipment.AdminComment);
            builder.Property(shipment => shipment.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(shipment => shipment.Order)
            //    .WithMany(order => order.Shipments)
            //    .HasForeignKey(shipment => shipment.OrderId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}