using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public partial class ShipmentOrderProductVariantMap : EntityTypeConfiguration<ShipmentOrderProductVariant>
    {
        public ShipmentOrderProductVariantMap()
        {
            this.ToTable("Shipment_OrderProductVariant");
            this.HasKey(sopv => sopv.Id);

            this.HasRequired(sopv => sopv.Shipment)
                .WithMany(s => s.ShipmentOrderProductVariants)
                .HasForeignKey(sopv => sopv.ShipmentId);
        }
    }
}