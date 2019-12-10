using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

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
            builder.Property(value => value.PriceAdjustment).HasDecimal();
            builder.Property(value => value.WeightAdjustment).HasDecimal();
            builder.Property(value => value.Cost).HasDecimal();
            builder.Property(value => value.ProductAttributeId);
            builder.Property(value => value.PriceAdjustmentUsePercentage);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);
        }

        #endregion
    }
}