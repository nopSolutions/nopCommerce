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
            builder.Property(record => record.WeightFrom);
            builder.Property(record => record.WeightTo);
            builder.Property(record => record.OrderSubtotalFrom);
            builder.Property(record => record.OrderSubtotalTo);
            builder.Property(record => record.AdditionalFixedCost);
            builder.Property(record => record.PercentageRateOfSubtotal);
            builder.Property(record => record.RatePerWeightUnit);
            builder.Property(record => record.LowerWeightLimit);
            builder.Property(record => record.Zip).HasLength(400);
        }

        #endregion
    }
}