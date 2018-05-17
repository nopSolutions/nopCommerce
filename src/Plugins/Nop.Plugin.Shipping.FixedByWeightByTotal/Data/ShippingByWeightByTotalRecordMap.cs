using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    /// <summary>
    /// Represents a shipping by weight or by total record mapping configuration
    /// </summary>
    public partial class ShippingByWeightByTotalRecordMap : NopEntityTypeConfiguration<ShippingByWeightByTotalRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ShippingByWeightByTotalRecord> builder)
        {
            builder.ToTable(nameof(ShippingByWeightByTotalRecord));
            builder.HasKey(record => record.Id);

            builder.Property(record => record.Zip).HasMaxLength(400);
        }

        #endregion
    }
}