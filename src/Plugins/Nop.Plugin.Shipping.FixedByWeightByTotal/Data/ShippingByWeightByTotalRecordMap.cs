using Nop.Data.Mapping;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    public partial class ShippingByWeightByTotalRecordMap : NopEntityTypeConfiguration<ShippingByWeightByTotalRecord>
    {
        public ShippingByWeightByTotalRecordMap()
        {
            this.ToTable("ShippingByWeightByTotal");
            this.HasKey(x => x.Id);

            this.Property(x => x.Zip).HasMaxLength(400);
        }
    }
}