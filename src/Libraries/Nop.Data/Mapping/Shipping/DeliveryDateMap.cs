using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public class DeliveryDateMap : EntityTypeConfiguration<DeliveryDate>
    {
        public DeliveryDateMap()
        {
            this.ToTable("DeliveryDate");
            this.HasKey(dd => dd.Id);
            this.Property(dd => dd.Name).IsRequired().HasMaxLength(400);
        }
    }
}
