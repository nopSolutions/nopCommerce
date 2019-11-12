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

            builder.Property(value => value.Name).HasLength(400);
            builder.HasColumn(value => value.Name).IsColumnRequired();
            builder.Property(value => value.PriceAdjustment).HasDbType("decimal(18, 4)");
            builder.Property(value => value.WeightAdjustment).HasDbType("decimal(18, 4)");
            builder.Property(value => value.Cost).HasDbType("decimal(18, 4)");

            builder.Property(value => value.ProductAttributeId);
            builder.Property(value => value.PriceAdjustmentUsePercentage);
            builder.Property(value => value.IsPreSelected);
            builder.Property(value => value.DisplayOrder);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(value => value.ProductAttribute)
            //    .WithMany()
            //    .HasForeignKey(value => value.ProductAttributeId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}