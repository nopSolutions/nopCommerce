using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    public partial class ShippingByWeightByTotalRecordMap : NopEntityTypeConfiguration<ShippingByWeightByTotalRecord>
    {
        public override void Configure(EntityTypeBuilder<ShippingByWeightByTotalRecord> builder)
        {
            builder.ToTable("ShippingByWeightByTotal");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Zip).HasMaxLength(400);
            base.Configure(builder);
        }
    }
}