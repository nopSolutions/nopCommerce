using LinqToDB;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a predefined product attribute value mapping configuration
    /// </summary>
    public partial class PredefinedProductAttributeValueMap : NopEntityTypeConfiguration<PredefinedProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PredefinedProductAttributeValue> builder)
        {
            builder.HasTableName(nameof(PredefinedProductAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(value => value.PriceAdjustment).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(value => value.WeightAdjustment).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(value => value.Cost).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);

            builder.Property(value => value.ProductAttributeId);
            builder.Property(value => value.PriceAdjustmentUsePercentage);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
        }

        #endregion
    }
}