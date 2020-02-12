using LinqToDB.Mapping;
using Nop.Data;
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
        public override void Configure(EntityMappingBuilder<ShippingByWeightByTotalRecord> builder)
        {
            builder.HasTableName(nameof(ShippingByWeightByTotalRecord));
            builder.Property(record => record.StoreId);
            builder.Property(record => record.WarehouseId);
            builder.Property(record => record.CountryId);
            builder.Property(record => record.StateProvinceId);
            builder.Property(record => record.ShippingMethodId);
            builder.Property(record => record.WeightFrom).HasPrecision(18).HasScale(2);
            builder.Property(record => record.WeightTo).HasPrecision(18).HasScale(2);
            builder.Property(record => record.OrderSubtotalFrom).HasPrecision(18).HasScale(2);
            builder.Property(record => record.OrderSubtotalTo).HasPrecision(18).HasScale(2);
            builder.Property(record => record.AdditionalFixedCost).HasPrecision(18).HasScale(2);
            builder.Property(record => record.PercentageRateOfSubtotal).HasPrecision(18).HasScale(2);
            builder.Property(record => record.RatePerWeightUnit).HasPrecision(18).HasScale(2);
            builder.Property(record => record.LowerWeightLimit).HasPrecision(18).HasScale(2);
            builder.Property(record => record.Zip).HasLength(400);
        }

        #endregion
    }
}